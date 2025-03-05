using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace Leap.Forward
{
    public class BuildScript
    {
        public static string GetCommandLineArg(string name)
        {
            var args = ((IEnumerable<string>)System.Environment.GetCommandLineArgs()).GetEnumerator();
            while (args.MoveNext())
            {
                if (args.Current == name)
                {
                    if (args.MoveNext())
                        return args.Current;
                    return null;
                }
            }
            return null;
        }

        public static IEnumerable<string> GetCommandLineArgs(string name)
        {
            var args = ((IEnumerable<string>)System.Environment.GetCommandLineArgs()).GetEnumerator();
            while (args.MoveNext())
            {
                if (args.Current == name)
                {
                    if (args.MoveNext())
                        yield return args.Current;
                    else
                        yield break;
                    
                }
            }
        }

        [MenuItem("Tools/Build Android")]
        public static void BuildAndroid()
        {
            PerformBuildTarget("Android", null);
        }

        [MenuItem("Tools/Build WebGL")]
        public static void BuildWebGL()
        {
            PerformBuildTarget("WebGL", null);
        }

        [MenuItem("Tools/Build Windows")]
        public static void BuildWindows()
        {
            PerformBuildTarget("StandaloneWindows64", null);
        }

        public static void PerformBuild()
        {
            PerformBuildTarget(GetCommandLineArg("-buildTarget"), GetCommandLineArg("-buildTargetName"));
        }

        public static void PerformBuildTarget(string buildTarget, string buildTargetName)
        {
            buildTargetName = buildTargetName ?? buildTarget;
            var namedBuildTarget = NamedBuildTarget.WebGL;
            var target = BuildTarget.StandaloneWindows64;
            switch (buildTarget)
            {
                case "StandaloneWindows64":
                    namedBuildTarget = NamedBuildTarget.Standalone;
                    target = BuildTarget.StandaloneWindows64;
                    break;
                case "StandaloneLinux64":
                    namedBuildTarget = NamedBuildTarget.Standalone;
                    target = BuildTarget.StandaloneLinux64;
                    break;
                case "StandaloneOSX":
                    namedBuildTarget = NamedBuildTarget.Standalone;
                    target = BuildTarget.StandaloneOSX;
                    break;
                case "Android":
                    namedBuildTarget = NamedBuildTarget.Android;
                    target = BuildTarget.Android;
                    break;
                case "WebGL":
                    namedBuildTarget = NamedBuildTarget.WebGL;
                    target = BuildTarget.WebGL;
                    break;
                default:
                    throw new Exception($"Unknown -buildTarget $buildTarget");
            }

            string buildPath = "Builds/" + buildTargetName;

            // Get all asset paths in the project
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

            // Filter out paths that end with ".unity" (scene files)
            var scenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            string[] scenePaths = scenes.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            var defines = new List<string>();
            defines.Add("ENABLE_" + buildTargetName.ToUpper());
            foreach (var define in GetCommandLineArgs("-define"))
            {
                defines.Add(define);
            }

            var options = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = buildPath,
                target = target,
                options = BuildOptions.None,
                extraScriptingDefines = defines.ToArray()
            };

            // Build process
            BuildPipeline.BuildPlayer(options);
        }
    }

}