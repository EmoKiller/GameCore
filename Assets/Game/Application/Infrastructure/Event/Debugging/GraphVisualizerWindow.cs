#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Application.Debugging.Editor
{
    public sealed class GraphVisualizerWindow : EditorWindow
    {
        private int _tab;

        [MenuItem("Tools/Debug/Graph Visualizer")]
        public static void Open()
        {
            GetWindow<GraphVisualizerWindow>("Graph Visualizer");
        }

        private void OnGUI()
        {
            _tab = GUILayout.Toolbar(_tab, new[]
            {
                "Flow Graph",
                "Event Graph"
            });

            switch (_tab)
            {
                case 0: DrawFlow(); break;
                case 1: DrawEvent(); break;
            }
        }

        private void DrawFlow()
        {
            GUILayout.Label("Flow Execution Graph");
        }

        private void DrawEvent()
        {
            GUILayout.Label("Event Dependency Graph");
        }
    }
}
#endif