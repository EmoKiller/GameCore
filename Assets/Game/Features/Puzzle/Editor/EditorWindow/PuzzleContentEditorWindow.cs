using UnityEditor;
using UnityEngine;

public sealed class PuzzleContentEditorWindow
    : EditorWindow
{
    [MenuItem("Tools/Puzzle/Content Editor")]
    private static void Open()
    {
        GetWindow<PuzzleContentEditorWindow>("Puzzle Content");
    }

    private void OnGUI()
    {
        GUILayout.Label("Puzzle Content Editor", EditorStyles.boldLabel);

        GUILayout.Space(10);

        GUILayout.Label(
            "Databases",
            EditorStyles.boldLabel);

        DrawConfigButton(
            "Tile Visual Database",
            _tileVisualDatabase);

        DrawConfigButton(
            "Special Resolver Database",
            _specialResolverDatabase);
    }


    private TileVisualDatabase _tileVisualDatabase;

    private SpecialResolverDatabase _specialResolverDatabase;

    private void OnEnable()
    {
        _tileVisualDatabase =
            FindAsset<TileVisualDatabase>();

        _specialResolverDatabase =
            FindAsset<SpecialResolverDatabase>();
    }
    private T FindAsset<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        if (guids.Length == 0)
        {
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);

        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    private void DrawConfigButton( string label, Object target)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(label);

        if (GUILayout.Button("Select"))
        {
            Selection.activeObject =
                target;
        }

        if (GUILayout.Button("Ping"))
        {
            EditorGUIUtility.PingObject(
                target);
        }

        GUILayout.EndHorizontal();
    }
}