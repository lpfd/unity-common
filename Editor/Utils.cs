using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Leap.Forward
{
    internal class Utils
    {
        public static void SaveAsset<T>(T mesh, string assetPath) where T : Object
        {
            T existingAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existingAsset == null)
            {

                AssetDatabase.CreateAsset(mesh, assetPath);
            }
            else
            {
                if (existingAsset is Mesh asset)
                    asset.Clear();
                EditorUtility.CopySerialized(mesh, existingAsset);
            }
        }

        public static void SetIsReadable(Texture2D mainTex, bool isReadable)
        {
            if (mainTex == null)
                return;
            if (mainTex.isReadable == isReadable)
                return;
            var textureAssetPath = AssetDatabase.GetAssetPath(mainTex);
            var tImporter = AssetImporter.GetAtPath(textureAssetPath) as TextureImporter;
            if (tImporter == null)
                return;
            if (tImporter.isReadable != isReadable)
            {
                tImporter.isReadable = isReadable;
                AssetDatabase.ImportAsset(textureAssetPath);
                AssetDatabase.Refresh();
            }
        }

        public static GameObject SavePrefab(string assetPath, GameObject root, params Object[] extraAssets)
        {
            var dirName = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            var mainType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            GameObject tmpPrefab = null;
            if (mainType != null)
            {
                if (typeof(GameObject).IsAssignableFrom(mainType))
                {
                    foreach (var o in AssetDatabase.LoadAllAssetsAtPath(assetPath))
                    {
                        if (o is GameObject gameObject && tmpPrefab == null)
                        {
                            tmpPrefab = gameObject;
                            continue;
                        }
                        AssetDatabase.RemoveObjectFromAsset(o);
                    }
                }
                else
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }

            if (tmpPrefab == null)
            {
                var tmp = new GameObject();
                try
                {
                    tmpPrefab = PrefabUtility.SaveAsPrefabAsset(tmp, assetPath);
                }
                finally
                {
                    GameObject.DestroyImmediate(tmp);
                }
            }

            HashSet<string> names = new HashSet<string>();
            foreach (var extraAsset in extraAssets.Where(_ => _ != null).Distinct())
            {
                if (!names.Add(extraAsset.name))
                {
                    for (int i = 2; ; ++i)
                    {
                        var n = extraAsset.name + "_" + i;
                        if (names.Add(n))
                        {
                            extraAsset.name = n;
                            break;
                        }
                    }
                }
                AssetDatabase.AddObjectToAsset(extraAsset, tmpPrefab);
            }

            GameObject prefab = PrefabUtility.ReplacePrefab(root, tmpPrefab);
            PrefabUtility.SavePrefabAsset(prefab);
            return prefab;
        }
    }
}