#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR;

namespace Leap.Forward
{
    [CreateAssetMenu(fileName = "MergeSkinnedMeshesRecipe", menuName = "Scriptable Objects/Merge Skinned Meshes Recipe")]
    public class MergeSkinnedMeshesRecipe : ScriptableObject
    {
        [SerializeField]
        public GameObject[] PrefabsToMerge;

        [SerializeField]
        public string OutputPrefabAssetName;

#if UNITY_EDITOR

        public bool Browse()
        {
            // Ask the user where to save it
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Generated Prefab",
                "NewPrefab",           // Default file name
                "prefab",              // Extension
                "Choose where to save the prefab"
            );

            if (!string.IsNullOrEmpty(path))
            {
                OutputPrefabAssetName = path;
                return true;
            }

            return false;
        }

        public void Merge()
        {
            if (string.IsNullOrEmpty(OutputPrefabAssetName))
            {
                if (!Browse())
                {
                    return;
                }
            }

            var uniqueBones = new Dictionary<string, Transform>();
            var allSMRs = new Dictionary<Mesh, SkinnedMeshRenderer>();

            GameObject newRoot = new GameObject(Path.GetFileNameWithoutExtension(OutputPrefabAssetName));

            Transform AddBone(Transform bone, Transform rootBone)
            {
                if (bone == null)
                {
                    return bone;
                }

                if (uniqueBones.TryGetValue(bone.name, out var transform))
                {
                    return transform;
                }

                var newBone = new GameObject(bone.name);
                uniqueBones.Add(bone.name, newBone.transform);
                if (bone == rootBone || bone.parent == null)
                {
                    newBone.transform.parent = newRoot.transform;
                }
                else
                {
                    newBone.transform.parent = AddBone(bone.parent, rootBone);
                }

                newBone.transform.localPosition = bone.localPosition;
                newBone.transform.localRotation = bone.localRotation;
                newBone.transform.localScale = bone.localScale;
                return newBone.transform;
            }

            foreach (var go in PrefabsToMerge)
            {
                if (go == null)
                    continue;

                var smrList = go.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var smr in smrList)
                {
                    if (smr.sharedMesh == null)
                        continue;

                    allSMRs.Add(smr.sharedMesh, smr);

                    var newRootBone = AddBone(smr.rootBone, smr.rootBone);

                    foreach (var bone in smr.bones)
                    {
                        if (bone == null) continue;

                        AddBone(bone, smr.rootBone);
                    }

                    GameObject newSmrObject = new GameObject(smr.gameObject.name);
                    newSmrObject.transform.parent = newRoot.transform;

                    var newSmr = newSmrObject.AddComponent<SkinnedMeshRenderer>();
                    newSmr.sharedMesh = smr.sharedMesh;
                    newSmr.rootBone = uniqueBones[smr.rootBone.name];
                    newSmr.bones = smr.bones.Select(_ => uniqueBones[_.name]).ToArray();
                    newSmr.sharedMaterials = smr.sharedMaterials;
                    newSmr.enabled = smr.enabled;
                    newSmr.shadowCastingMode = smr.shadowCastingMode;
                    newSmr.staticShadowCaster = smr.staticShadowCaster;
                    newSmr.receiveShadows = smr.receiveShadows;
                }
            }
            PrefabUtility.SaveAsPrefabAsset(newRoot, OutputPrefabAssetName);
            DestroyImmediate(newRoot);
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MergeSkinnedMeshesRecipe))]
    public class MergeSkinnedMeshesRecipeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw default fields
            DrawDefaultInspector();

            // Reference to the actual target
            MergeSkinnedMeshesRecipe myTarget = (MergeSkinnedMeshesRecipe)target;

            if (GUILayout.Button("Browse"))
            {
                myTarget.Browse();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Merge"))
            {
                myTarget.Merge();
            }
        }
    }
#endif
}
