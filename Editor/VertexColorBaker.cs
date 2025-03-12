using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Leap.Forward
{
    public class VertexColorBaker : IDisposable
    {
        private readonly string _exportFolder;
        private readonly Material _materialOverride;
        private readonly Material _transparentMaterialOverride;
        private readonly Cache<Object> _cache = new Cache<Object>();
        private readonly Cache<VisitedMesh> _meshCache = new Cache<VisitedMesh>();
        private readonly HashSet<Texture2D> _texturesToRollback = new HashSet<Texture2D>();
        private readonly Queue<Action> _postActions = new Queue<Action>();
        private HashSet<System.Type> _unknownTypes = new HashSet<System.Type>();
        private readonly bool _splitTransparent;

        public VertexColorBaker(string exportFolder, Material materialOverride, Material transparentMaterialOverride)
        {
            _exportFolder = exportFolder;
            _materialOverride = materialOverride;
            _transparentMaterialOverride = transparentMaterialOverride;
            _splitTransparent = _transparentMaterialOverride != null;
        }

        public bool FlatFaceColor { get; set; }
        public bool ApplyScale { get; set; }
        public bool ApplyRotation { get; set; }

        public void BakePrefabAtPath(string path, string relPath)
        {
            if (path == relPath)
            {
                Debug.LogError($"It isn't safe to override the asset. Skipping the {path}.");
            }
            _cache.Push();
            _meshCache.Push();
            try
            {
                var prefabRoot = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (prefabRoot == null)
                    return;


                if (_cache.TryGetObject(prefabRoot, out _))
                    return;
                var extraAssets = new HashSet<UnityEngine.Object>();

                var newPrefabRoot = new GameObject();
                try
                {
                    CloneGameObject(prefabRoot, newPrefabRoot, extraAssets);
                    while (_postActions.Count > 0)
                    {
                        var a = _postActions.Dequeue();
                        a();
                    }

                    RegisterGlobal(prefabRoot, Utils.SavePrefab(relPath, newPrefabRoot, extraAssets.ToArray()));
                }
                finally
                {
                    Object.DestroyImmediate(newPrefabRoot);
                }
            }
            finally
            {
                _cache.Pop();
                _meshCache.Pop();
            }
        }

        public void RegisterGlobal(GameObject prefabRoot, GameObject savePrefab)
        {
            _cache.AddGlobal(prefabRoot, savePrefab);
        }

        private void CloneGameObject(GameObject prefabRoot, GameObject newPrefabRoot, HashSet<Object> extraAssets)
        {
            newPrefabRoot.transform.localPosition = prefabRoot.transform.localPosition;
            newPrefabRoot.transform.localRotation = prefabRoot.transform.localRotation;
            newPrefabRoot.transform.localScale = prefabRoot.transform.localScale;
            if (ApplyScale)
                newPrefabRoot.transform.localScale = Vector3.one;
            if (ApplyRotation)
                newPrefabRoot.transform.localRotation = Quaternion.identity;
            newPrefabRoot.name = prefabRoot.name;
            var components = new List<Component>();
            prefabRoot.GetComponents(components);
            foreach (var component in components.Where(_ => _ != null))
            {
                if (component is MeshFilter meshFilter)
                {
                    CloneMeshFilter(meshFilter, newPrefabRoot.AddComponent<MeshFilter>(), extraAssets);
                }
                else if (component is MeshRenderer meshRenderer)
                {
                    CloneMeshRenderer(meshRenderer, newPrefabRoot.AddComponent<MeshRenderer>(), extraAssets);
                }
                else if (component is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    CloneSkinnedMeshRenderer(skinnedMeshRenderer, newPrefabRoot.AddComponent<SkinnedMeshRenderer>(), extraAssets);
                }
                else if (component is Transform)
                {
                }
                else if (component is Collider collider)
                {
                    CloneCollider(collider, newPrefabRoot, extraAssets);
                }
                else if (component is Light light)
                {
                    CloneLight(light, newPrefabRoot, extraAssets);
                }
                else if (component is LODGroup lodGroup)
                {
                    CloneLODGroup(lodGroup, newPrefabRoot, extraAssets);
                }
                else
                {
                    if (_unknownTypes.Add(component.GetType()))
                    {
                        Debug.LogWarning($"Unsupported component type: {component.GetType().Name}");
                    }

                }
            }
            for (int i = 0; i < prefabRoot.transform.childCount; ++i)
            {
                var gameObject = prefabRoot.transform.GetChild(i).gameObject;
                if (gameObject.activeSelf)
                {
                    var child = new GameObject(gameObject.name);
                    child.transform.parent = newPrefabRoot.transform;
                    CloneGameObject(gameObject, child, extraAssets);
                }
            }
        }

        private T CloneComponent<T>(T component, GameObject gameObject) where T : Component
        {
            T newComponent = gameObject.AddComponent<T>();
            newComponent.name = component.name;
            return newComponent;
        }

        private void CloneLODGroup(LODGroup lodGroup, GameObject gameObject, HashSet<Object> extraAssets)
        {
            var newLodGroup = CloneComponent(lodGroup, gameObject);
            _postActions.Enqueue(() =>
            {
                FixLodGroup(lodGroup, newLodGroup);
            });

        }

        private void FixLodGroup(LODGroup lodGroup, LODGroup newLodGroup)
        {
            var newLods = new LOD[lodGroup.lodCount];
            for (var lodIndex = 0; lodIndex < newLods.Length; lodIndex++)
            {
                var lod = lodGroup.GetLODs()[lodIndex];
                var lodRenderers = lod.renderers;
                var renderers = new Renderer[lod.renderers.Length];
                for (var index = 0; index < lodRenderers.Length; index++)
                {
                    renderers[index] = newLodGroup.transform.FindRecursive(lodRenderers[index].name)
                        ?.GetComponent<Renderer>();
                }

                newLods[lodIndex].renderers = renderers;
                newLods[lodIndex].fadeTransitionWidth = lod.fadeTransitionWidth;
                newLods[lodIndex].screenRelativeTransitionHeight = lod.screenRelativeTransitionHeight;
            }
            newLodGroup.SetLODs(newLods);
        }

        private void CloneLight(Light light, GameObject gameObject, HashSet<Object> extraAssets)
        {
            var newLight = CloneComponent(light, gameObject);
            newLight.type = light.type;
            newLight.color = light.color;
            newLight.intensity = light.intensity;
            newLight.areaSize = light.areaSize;
            newLight.innerSpotAngle = light.innerSpotAngle;
        }

        private void CloneCollider(Collider collider, GameObject gameObject, HashSet<Object> extraAssets)
        {
            Collider newCollider = null;
            if (collider is MeshCollider meshCollider)
            {
                var newMeshCollider = CloneComponent(meshCollider, gameObject);
                GetTransform(meshCollider, out var scale, out var rotation);
                newMeshCollider.sharedMesh = CloneAndBakeMesh(meshCollider.sharedMesh, null, scale, rotation).Mesh;
                if (newMeshCollider.sharedMesh != null)
                    extraAssets.Add(newMeshCollider.sharedMesh);
                newCollider = newMeshCollider;
            }
            else if (collider is BoxCollider boxCollider)
            {
                var newBoxCollider = CloneComponent(boxCollider, gameObject);
                newBoxCollider.size = boxCollider.size;
                newBoxCollider.center = boxCollider.center;
                newCollider = newBoxCollider;
            }
            else if (collider is SphereCollider sphereCollider)
            {
                var newSphereCollider = CloneComponent(sphereCollider, gameObject);
                newSphereCollider.radius = sphereCollider.radius;
                newSphereCollider.center = sphereCollider.center;
                newCollider = newSphereCollider;
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                var newCapsuleCollider = CloneComponent(capsuleCollider, gameObject);
                newCapsuleCollider.radius = capsuleCollider.radius;
                newCapsuleCollider.direction = capsuleCollider.direction;
                newCapsuleCollider.height = capsuleCollider.height;
                newCapsuleCollider.center = capsuleCollider.center;
                newCollider = newCapsuleCollider;
            }
            else
            {
                if (_unknownTypes.Add(collider.GetType()))
                {
                    Debug.LogWarning($"Unsupported collider type: {collider.GetType().Name}");
                }
            }

            if (newCollider != null)
            {
                newCollider.enabled = collider.enabled;
                newCollider.sharedMaterial = collider.sharedMaterial;
            }
        }

        private void CloneMeshFilter(MeshFilter meshFilter, MeshFilter newMeshFilter, HashSet<Object> extraAssets)
        {
            newMeshFilter.name = meshFilter.name;
            var renderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
            GetTransform(meshFilter, out var scale, out var rotation);
            var meshAndMaterials = CloneAndBakeMesh(meshFilter.sharedMesh, renderer?.sharedMaterials ?? Array.Empty<Material>(), scale, rotation);
            newMeshFilter.sharedMesh = meshAndMaterials.Mesh;
            extraAssets.Add(newMeshFilter.sharedMesh);
        }

        private void GetTransform(Component meshFilter, out Vector3 scale, out Quaternion rotation)
        {
            scale = ApplyScale ? meshFilter.transform.localToWorldMatrix.lossyScale : Vector3.one;
            rotation = ApplyRotation ? meshFilter.transform.localToWorldMatrix.rotation : Quaternion.identity;
        }

        private void CloneSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer, SkinnedMeshRenderer newSkinnedMeshRenderer, HashSet<Object> extraAssets)
        {
            newSkinnedMeshRenderer.name = skinnedMeshRenderer.name;
            if (skinnedMeshRenderer.rootBone)
                skinnedMeshRenderer.rootBone.gameObject.SetActive(true);
            var scale = ApplyScale ? skinnedMeshRenderer.transform.localToWorldMatrix.lossyScale : Vector3.one;
            var rotation = ApplyRotation ? skinnedMeshRenderer.transform.localToWorldMatrix.rotation : Quaternion.identity;
            var meshAndMaterials =
                CloneAndBakeMesh(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials, scale, rotation);
            newSkinnedMeshRenderer.sharedMesh = meshAndMaterials.Mesh;
            newSkinnedMeshRenderer.sharedMaterials = meshAndMaterials.Materials ?? skinnedMeshRenderer.sharedMaterials;
            extraAssets.Add(newSkinnedMeshRenderer.sharedMesh);

            var nodeScale = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);
            _postActions.Enqueue(() =>
            {
                FixBonesAndScale(skinnedMeshRenderer, newSkinnedMeshRenderer, nodeScale);
            });
        }

        private Material[] GetMaterialOverrides(Mesh mesh)
        {
            if (_splitTransparent && mesh != null && mesh.subMeshCount > 1)
                return new[] { _materialOverride, _transparentMaterialOverride };
            return new[] { _materialOverride };
        }

        private void FixBonesAndScale(SkinnedMeshRenderer skinnedMeshRenderer, SkinnedMeshRenderer newSkinnedMeshRenderer, Vector3 scale)
        {
            var root = newSkinnedMeshRenderer.gameObject.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

            newSkinnedMeshRenderer.rootBone = root.FindRecursive(skinnedMeshRenderer.rootBone.name);
            if (newSkinnedMeshRenderer.rootBone != null)
            {
                var bones = new Transform[skinnedMeshRenderer.bones.Length];
                for (var index = 0; index < bones.Length; index++)
                {
                    bones[index] = root.FindRecursive(skinnedMeshRenderer.bones[index].name);
                }

                newSkinnedMeshRenderer.bones = bones;
                if (scale != Vector3.one)
                {
                    var invScale = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);
                    newSkinnedMeshRenderer.transform.localScale =
                        Vector3.Scale(newSkinnedMeshRenderer.transform.localScale, scale);
                    newSkinnedMeshRenderer.rootBone.localScale =
                        Vector3.Scale(newSkinnedMeshRenderer.rootBone.localScale, scale);

                    foreach (var bone in newSkinnedMeshRenderer.rootBone.GetChildrenRecursive())
                    {
                        bone.localPosition = Vector3.Scale(bone.localPosition, invScale);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Something is wrong with bones at " + AssetDatabase.GetAssetPath(skinnedMeshRenderer));
            }
        }

        private Transform FixBone(string actualName, string expectedName, SkinnedMeshRenderer renderer)
        {
            foreach (var bone in renderer.bones)
            {
                if (bone != null && bone.name == actualName)
                {
                    bone.name = expectedName;
                    return bone;
                }
            }

            return null;
        }

        private VisitedMesh CloneAndBakeMesh(Mesh mesh, Material[] materials, Vector3 scale, Quaternion rotation)
        {
            if (mesh == null)
                return default;

            if (_meshCache.TryGetObject(mesh, out var existingMesh))
            {
                return existingMesh;
            }
            materials ??= Array.Empty<Material>();
            var inverseScale = new Vector3(1.0f / scale.x, 1.0f / scale.y, 1.0f / scale.z);
            var inverseRotation = Quaternion.Inverse(rotation);
            var builder = new MeshBuilder();
            var vertices = mesh.GetVertices();
            for (var index = 0; index < vertices.Length; index++)
            {
                vertices[index].vertex = rotation * Vector3.Scale(vertices[index].vertex, scale);
                vertices[index].normal = (rotation * Vector3.Scale(vertices[index].normal, scale)).normalized;
                var t = vertices[index].tangent;
                var tt = rotation * Vector3.Scale(new Vector3(t.x, t.y, t.z), scale);
                vertices[index].tangent = new Vector4(tt.x, tt.y, tt.z, t.w);
            }

            var submeshTranslatedIndices = new List<List<int>>();
            for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; ++submeshIndex)
            {
                var material = (materials.Length > submeshIndex) ? materials[submeshIndex] : null;
                var translatedIndices = new List<int>();
                submeshTranslatedIndices.Add(translatedIndices);
                var indices = new List<int>();
                mesh.GetIndices(indices, submeshIndex);
                Color colorMultiplier = Color.white;
                if (material != null)
                {
                    var mainTexture = material.GetTexture("_MainTex") as Texture2D;
                    if (material.HasColor("_Color"))
                    {
                        colorMultiplier = material.GetColor("_Color");
                    }
                    if (mainTexture != null && !mainTexture.isReadable)
                    {
                        _texturesToRollback.Add(mainTexture);
                        Utils.SetIsReadable(mainTexture, true);
                    }

                    var canReadTexture = mainTexture != null && mainTexture.isReadable;
                    if (canReadTexture && FlatFaceColor)
                    {
                        foreach (var face in indices.ToTriplets())
                        {
                            var vertex0 = vertices[face[0]];
                            var vertex1 = vertices[face[1]];
                            var vertex2 = vertices[face[2]];
                            var color0 = mainTexture.GetPixelBilinear(vertex0.uv.x, vertex0.uv.y) *
                                         colorMultiplier;
                            var color1 = mainTexture.GetPixelBilinear(vertex1.uv.x, vertex1.uv.y) *
                                         colorMultiplier;
                            var color2 = mainTexture.GetPixelBilinear(vertex2.uv.x, vertex2.uv.y) *
                                         colorMultiplier;

                            var flatColor = (color0 + color1 + color2) / 3.0f;

                            vertex0.color = flatColor;
                            vertex1.color = flatColor;
                            vertex2.color = flatColor;
                            translatedIndices.Add(builder.Add(vertex0));
                            translatedIndices.Add(builder.Add(vertex1));
                            translatedIndices.Add(builder.Add(vertex2));
                        }
                    }
                    else
                    {
                        foreach (var index in indices)
                        {
                            var vertex = vertices[index];
                            var uv = vertex.uv;
                            if (canReadTexture)
                            {
                                vertex.color = mainTexture.GetPixelBilinear(uv.x, uv.y).linear * colorMultiplier;
                            }
                            else
                            {
                                vertex.color = colorMultiplier;
                            }

                            translatedIndices.Add(builder.Add(vertex));
                        }
                    }
                }
                else
                {
                    foreach (var index in indices)
                    {
                        var vertex = vertices[index];
                        vertex.color = colorMultiplier;
                        translatedIndices.Add(builder.Add(vertex));
                    }
                }
            }

            var newMesh = new Mesh
            {
                name = mesh.name,
                bindposes = ScaleBindPoses(mesh.bindposes, scale)
            };

            builder.Build(newMesh);

            if (_materialOverride != null)
            {
                if (_transparentMaterialOverride != null)
                {

                    var opaque = new List<int>();
                    var transparent = new List<int>();

                    foreach (var face in submeshTranslatedIndices.SelectMany(_ => _.ToTriplets()))
                    {
                        var a = (builder[face[0]].color.a)
                                * (builder[face[1]].color.a)
                                * (builder[face[2]].color.a);
                        if (a < 0.999f)
                        {
                            transparent.AddRange(face);
                        }
                        else
                        {
                            opaque.AddRange(face);
                        }
                    }

                    if (transparent.Count > 0 && opaque.Count > 0)
                    {
                        newMesh.subMeshCount = 2;
                        newMesh.SetIndices(opaque.ToArray(), MeshTopology.Triangles, 0);
                        newMesh.SetIndices(transparent.ToArray(), MeshTopology.Triangles, 1);
                        return _meshCache.AddLocal(mesh, new VisitedMesh(newMesh, new[] { _materialOverride, _transparentMaterialOverride }, scale));
                    }
                    if (transparent.Count > 0)
                    {
                        newMesh.subMeshCount = 1;
                        newMesh.SetIndices(transparent.ToArray(), MeshTopology.Triangles, 0);
                        return _meshCache.AddLocal(mesh, new VisitedMesh(newMesh, new[] { _transparentMaterialOverride }, scale));
                    }
                    if (opaque.Count > 0)
                    {
                        newMesh.subMeshCount = 1;
                        newMesh.SetIndices(opaque.ToArray(), MeshTopology.Triangles, 0);
                        return _meshCache.AddLocal(mesh, new VisitedMesh(newMesh, new[] { _materialOverride }, scale));
                    }
                }
                newMesh.subMeshCount = 1;
                newMesh.SetIndices(submeshTranslatedIndices.SelectMany(_ => _).ToArray(), MeshTopology.Triangles, 0);
                return _meshCache.AddLocal(mesh, new VisitedMesh(newMesh, new[] { _materialOverride }, scale));
            }
            else
            {
                for (int submeshIndex = 0; submeshIndex < submeshTranslatedIndices.Count; ++submeshIndex)
                {
                    newMesh.SetIndices(submeshTranslatedIndices[submeshIndex], mesh.GetSubMesh(submeshIndex).topology, submeshIndex);
                }
                return _meshCache.AddLocal(mesh, new VisitedMesh(newMesh, null, scale));
            }
        }

        private Matrix4x4[] ScaleBindPoses(Matrix4x4[] meshBindposes, Vector3 scale)
        {
            if (scale == Vector3.one)
                return meshBindposes;

            var res = new Matrix4x4[meshBindposes.Length];
            for (var index = 0; index < res.Length; index++)
            {
                var bindPose = meshBindposes[index];
                var p1 = bindPose.GetColumn(3);
                bindPose.SetColumn(3, new Vector4(p1.x * scale.x, p1.y * scale.y, p1.z * scale.z, p1.w));
                res[index] = bindPose;
            }

            return res;
        }


        private void CloneMeshRenderer(MeshRenderer renderer, MeshRenderer newRenderer, HashSet<Object> extraAssets)
        {
            newRenderer.shadowCastingMode = renderer.shadowCastingMode;
            newRenderer.receiveShadows = renderer.receiveShadows;
            newRenderer.receiveGI = renderer.receiveGI;
            GetTransform(renderer, out var scale, out var rotation);

            var meshAndMaterials = CloneAndBakeMesh(renderer.GetComponent<MeshFilter>().sharedMesh, renderer?.sharedMaterials, scale, rotation);
            newRenderer.sharedMaterials = meshAndMaterials.Materials ?? renderer.sharedMaterials;
        }

        public void Dispose()
        {
            foreach (var texture2D in _texturesToRollback)
            {
                Utils.SetIsReadable(texture2D, false);
            }
        }

        public class Cache<T>
        {
            private readonly Dictionary<Object, T> _globalMap = new Dictionary<Object, T>();
            private Stack<Dictionary<Object, T>> _localMap = new Stack<Dictionary<Object, T>>();

            public Cache()
            {
                _localMap.Push(new Dictionary<Object, T>());
            }
            public bool TryGetObject(Object key, out T value)
            {
                if (key == null)
                {
                    value = default;
                    return false;
                }
                if (_globalMap.TryGetValue(key, out value))
                    return true;
                if (_localMap.Peek().TryGetValue(key, out value))
                    return true;
                return false;
            }

            public void Push()
            {
                _localMap.Push(new Dictionary<Object, T>());
            }

            public void Pop()
            {
                _localMap.Pop();
            }

            public void AddGlobal(Object key, T value)
            {
                _globalMap.Add(key, value);
            }

            public T AddLocal(Object key, T value)
            {
                _localMap.Peek().Add(key, value);
                return value;
            }
        }

        struct VisitedMesh
        {
            public VisitedMesh(Mesh mesh, Material[] sharedMaterials, Vector3 scale)
            {
                Mesh = mesh;
                Materials = sharedMaterials;
                Scale = scale;
            }

            public Mesh Mesh;
            public Material[] Materials;
            public Vector3 Scale;
        }
    }

}