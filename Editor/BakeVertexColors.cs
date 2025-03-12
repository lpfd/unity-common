using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Leap.Forward
{
    public class BakeVertexColor
    {
        [MenuItem("Assets/Leap Forward/Bake Texture To Vertex Color")]
        public static void BakeTextureOnSelectedAsset()
        {
            EditorCoroutineUtility.StartCoroutine(new CoroutineProgress("Bake vertex colors", BakeTextureOnSelectedAssetImpl().GetEnumerator()), SceneManager.GetActiveScene());
        }

        public static IEnumerable<CoroutineProgress.ProgressReport> BakeTextureOnSelectedAssetImpl()
        {
            // Retrieve the selected asset GUIDs
            string[] assetGUIDs = Selection.assetGUIDs;
            float progressScale = 1.0f / assetGUIDs.Length;
            for (int assetIndex = 0; assetIndex < assetGUIDs.Length; assetIndex++)
            {
                string selectedGuid = assetGUIDs[assetIndex];
                // Convert GUID to asset path
                string assetPath = AssetDatabase.GUIDToAssetPath(selectedGuid);

                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    foreach (var report in BakeFolder(assetPath))
                        yield return new CoroutineProgress.ProgressReport(report.Info, (assetIndex+report.Progress)*progressScale);
                }
                else
                {
                    var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                    if (assetType == typeof(GameObject))
                    {
                        yield return new CoroutineProgress.ProgressReport(assetPath, assetIndex * progressScale);
                        BakePrefab(assetPath);
                    }
                    //else if (assetType == typeof(Mesh))
                    //{
                    //    yield return new CoroutineProgress.ProgressReport(assetPath, assetIndex * progressScale);
                    //    BakeMesh(assetPath);
                    //}
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void BakePrefab(string assetPath)
        {
            if (assetPath.EndsWith(".baked.prefab"))
                return;
            var outputAssetFolder = Path.GetDirectoryName(assetPath);
            var outputAssetPath = Path.Combine(outputAssetFolder, Path.GetFileNameWithoutExtension(assetPath) + ".baked.prefab");

            var baker = new VertexColorBaker(outputAssetFolder, AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath("1996ef8b0e944f7449224996be82b7f6")), AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath("d303451c420378a46ac2515a1e9984b9")));
            baker.BakePrefabAtPath(assetPath, outputAssetPath);
        }

        private static IEnumerable<CoroutineProgress.ProgressReport> BakeFolder(string assetPath)
        {
            // Find all prefab asset GUIDs in the specified folder and its subfolders
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { assetPath });
            // Find all mesh asset GUIDs in the specified folder and its subfolders
            //string[] meshGuids = AssetDatabase.FindAssets("t:Mesh", new[] { assetPath });

            var total = prefabGuids.Length;// + meshGuids.Length;

            int counter = 0;
            // Iterate through the GUIDs and load the corresponding prefabs
            foreach (string guid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                yield return new CoroutineProgress.ProgressReport(path, counter / (float)total);
                BakePrefab(path);
                ++counter;
            }


            // Iterate through the GUIDs and load the corresponding prefabs
            //foreach (string guid in meshGuids)
            //{
            //    var path = AssetDatabase.GUIDToAssetPath(guid);
            //    yield return new CoroutineProgress.ProgressReport(path, counter / (float)total);
            //    BakeMesh(path);
            //    ++counter;
            //}
        }

        [MenuItem("Assets/Leap Forward/Bake Texture To Vertex Color", true)]
        public static bool ValidateBakeTextureOnSelectedAsset()
        {
            // Retrieve the selected asset GUIDs
            foreach (var selectedGuid in Selection.assetGUIDs)
            {
                // Convert GUID to asset path
                string assetPath = AssetDatabase.GUIDToAssetPath(selectedGuid);

                if (AssetDatabase.IsValidFolder(assetPath))
                    return true;

                var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                if (assetType == typeof(Mesh) || assetType == typeof(GameObject))
                    return true;
            }

            return false;
        }
    }
}