using UnityEditor;

namespace Leap.Forward
{
    public class ToolsMenu
    {
        [MenuItem("Tools/Leap Forward/Build Android")]
        public static void BuildAndroid()
        {
            BuildScript.PerformBuildTarget("Android", null);
        }

        [MenuItem("Tools/Leap Forward/Build WebGL")]
        public static void BuildWebGL()
        {
            BuildScript.PerformBuildTarget("WebGL", null);
        }

        [MenuItem("Tools/Leap Forward/Build Windows")]
        public static void BuildWindows()
        {
            BuildScript.PerformBuildTarget("StandaloneWindows64", null);
        }
    }
}