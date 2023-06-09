using UnityEditor;

namespace TopDownShooter
{
    public static class DebugSettings
    {
        public static bool DrawCameraDebugLines { get; set; }
        public static bool DrawPlayerMovementDebugLines { get; set; }
        public static bool DrawEnemyMovementDebugLines { get; set; }
        public static bool DrawEnemyPathingDebugLines { get; set; }
        public static bool DrawEnemyAvoidanceDebugLines { get; set; }
    }

#if UNITY_EDITOR
    public class DebugEditorExtensions : Editor
    {
        [MenuItem("Custom/Turn all debug lines off")]
        public static void TurnAllDebugLinesOff()
        {
            DebugSettings.DrawCameraDebugLines = false;
            DebugSettings.DrawPlayerMovementDebugLines = false;
            DebugSettings.DrawEnemyMovementDebugLines = false;
            DebugSettings.DrawEnemyPathingDebugLines = false;
            DebugSettings.DrawEnemyAvoidanceDebugLines = false;
        }

        [MenuItem("Custom/Turn all debug lines on")]
        public static void TurnAllDebugLinesOn()
        {
            DebugSettings.DrawCameraDebugLines = true;
            DebugSettings.DrawPlayerMovementDebugLines = true;
            DebugSettings.DrawEnemyMovementDebugLines = true;
            DebugSettings.DrawEnemyPathingDebugLines = true;
            DebugSettings.DrawEnemyAvoidanceDebugLines = true;
        }

        [MenuItem("Custom/Toggle camera debug lines")]
        public static void ToggleCameraDebugLines()
        {
            DebugSettings.DrawCameraDebugLines = !DebugSettings.DrawCameraDebugLines;
        }

        [MenuItem("Custom/Toggle player movement debug lines")]
        public static void DrawPlayerMovementDebugLines()
        {
            DebugSettings.DrawPlayerMovementDebugLines = !DebugSettings.DrawPlayerMovementDebugLines;
        }

        [MenuItem("Custom/Toggle enemy movement debug lines")]
        public static void DrawEnemyMovementDebugLines()
        {
            DebugSettings.DrawEnemyMovementDebugLines = !DebugSettings.DrawEnemyMovementDebugLines;
        }

        [MenuItem("Custom/Toggle enemy pathing debug lines")]
        public static void ToggleEnemyPathingDebugLines()
        {
            DebugSettings.DrawEnemyPathingDebugLines = !DebugSettings.DrawEnemyPathingDebugLines;
        }
        [MenuItem("Custom/Toggle enemy avoidance debug lines")]
        public static void ToggleEnemyAvoidanceDebugLines()
        {
            DebugSettings.DrawEnemyAvoidanceDebugLines = !DebugSettings.DrawEnemyAvoidanceDebugLines;
        }
    }
#endif
}
