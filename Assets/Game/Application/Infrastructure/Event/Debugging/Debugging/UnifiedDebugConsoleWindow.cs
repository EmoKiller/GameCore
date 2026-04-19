#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Application.Debugging.Editor
{
    public sealed class UnifiedDebugConsoleWindow : EditorWindow
    {
        private int _tabIndex;

        [MenuItem("Tools/Debug/Unified Console")]
        public static void Open()
        {
            GetWindow<UnifiedDebugConsoleWindow>("Unified Debug Console");
        }

        private void OnGUI()
        {
            DrawTabs();
            DrawActivePanel();
        }

        private void DrawTabs()
        {
            _tabIndex = GUILayout.Toolbar(_tabIndex, new[]
            {
                "Events",
                "Flow",
                "Loading",
                "Metrics"
            });
        }

        private void DrawActivePanel()
        {
            switch (_tabIndex)
            {
                case 0: DrawEvents(); break;
                case 1: DrawFlow(); break;
                case 2: DrawLoading(); break;
                case 3: DrawMetrics(); break;
            }
        }

        private void DrawEvents()
        {
            GUILayout.Label("Event Timeline");
        }

        private void DrawFlow()
        {
            GUILayout.Label("Flow Execution");
        }

        private void DrawLoading()
        {
            GUILayout.Label("Loading Pipeline");
        }

        private void DrawMetrics()
        {
            GUILayout.Label("System Metrics");
        }
    }
}
#endif