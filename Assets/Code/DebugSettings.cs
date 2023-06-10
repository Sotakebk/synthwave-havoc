using UnityEditor;

namespace TopDownShooter
{
    public static class DebugSettings
    {
        public static bool DrawCameraLines { get; set; }
        public static bool DrawPlayerMovementLines { get; set; }
        public static bool DrawEnemyMovementLines { get; set; }
        public static bool DrawEnemyPathLines { get; set; }
        public static bool DrawEnemyAvoidanceLines { get; set; }
        public static bool DrawEnemyVisionLines { get; set; }
        public static bool DrawAvoidCollisionWithStaticObjectLines { get; set; }
    }

#if UNITY_EDITOR

    public class DebugEditorExtensions : Editor
    {
        [MenuItem("Custom/Turn all debug lines off")]
        public static void TurnAllDebugLinesOff()
        {
            DebugSettings.DrawCameraLines = false;
            DebugSettings.DrawPlayerMovementLines = false;
            DebugSettings.DrawEnemyMovementLines = false;
            DebugSettings.DrawEnemyPathLines = false;
            DebugSettings.DrawEnemyAvoidanceLines = false;
            DebugSettings.DrawEnemyVisionLines = false;
            DebugSettings.DrawAvoidCollisionWithStaticObjectLines = false;
        }

        [MenuItem("Custom/Turn all debug lines on")]
        public static void TurnAllDebugLinesOn()
        {
            DebugSettings.DrawCameraLines = true;
            DebugSettings.DrawPlayerMovementLines = true;
            DebugSettings.DrawEnemyMovementLines = true;
            DebugSettings.DrawEnemyPathLines = true;
            DebugSettings.DrawEnemyAvoidanceLines = true;
            DebugSettings.DrawEnemyVisionLines = true;
            DebugSettings.DrawAvoidCollisionWithStaticObjectLines = true;
        }

        [MenuItem("Custom/Toggle camera lines")]
        public static void ToggleCameraDebugLines()
        {
            DebugSettings.DrawCameraLines = !DebugSettings.DrawCameraLines;
        }

        [MenuItem("Custom/Toggle player movement lines")]
        public static void DrawPlayerMovementDebugLines()
        {
            DebugSettings.DrawPlayerMovementLines = !DebugSettings.DrawPlayerMovementLines;
        }

        [MenuItem("Custom/Toggle enemy movement lines")]
        public static void DrawEnemyMovementDebugLines()
        {
            DebugSettings.DrawEnemyMovementLines = !DebugSettings.DrawEnemyMovementLines;
        }

        [MenuItem("Custom/Toggle enemy pathing lines")]
        public static void ToggleEnemyPathingDebugLines()
        {
            DebugSettings.DrawEnemyPathLines = !DebugSettings.DrawEnemyPathLines;
        }

        [MenuItem("Custom/Toggle enemy avoidance lines")]
        public static void ToggleEnemyAvoidanceDebugLines()
        {
            DebugSettings.DrawEnemyAvoidanceLines = !DebugSettings.DrawEnemyAvoidanceLines;
        }

        [MenuItem("Custom/Toggle enemy vision lines")]
        public static void ToggleEnemyVisionLines()
        {
            DebugSettings.DrawEnemyVisionLines = !DebugSettings.DrawEnemyVisionLines;
        }
        [MenuItem("Custom/Toggle avoid collision lines")]
        public static void ToggleAvoidCollisionLines()
        {
            DebugSettings.DrawAvoidCollisionWithStaticObjectLines = !DebugSettings.DrawAvoidCollisionWithStaticObjectLines;
        }
    }

#endif
}