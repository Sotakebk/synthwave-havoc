using UnityEditor;
using UnityEngine;

namespace TopDownShooter
{
    public static class DebugSettings
    {
        public static bool DrawDebugLines { get; set; }
    }

#if UNITY_EDITOR
    public class DebugEditorExtensions : Editor
    {
        [MenuItem("Custom/Toggle drawing debug lines")]
        public static void ToggleDrawDebugLines()
        {
            DebugSettings.DrawDebugLines = !DebugSettings.DrawDebugLines;
            Debug.Log($"DrawDebugLines set to {DebugSettings.DrawDebugLines}");
        }
    }
#endif
}
