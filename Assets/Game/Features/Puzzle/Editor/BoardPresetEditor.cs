using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardPreset))]
public sealed class BoardPresetEditor : Editor
{
    private const int CellSize = 70;

    private BoardPreset _preset;

    private Vector2 _scroll;

    private void OnEnable()
    {
        _preset = (BoardPreset)target;

        EnsureTileArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawBoardSize();

        GUILayout.Space(10);

        DrawGrid();

        GUILayout.Space(20);

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Reload Runtime Board"))
            {
                ReloadRuntimeBoard();
            }
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_preset);
        }
    }

    private void DrawBoardSize()
    {
        int width =
            EditorGUILayout.IntField(
                "Width",
                _preset.Width);

        int height =
            EditorGUILayout.IntField(
                "Height",
                _preset.Height);

        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);

        if (width != _preset.Width ||
            height != _preset.Height)
        {
            _preset.Width = width;
            _preset.Height = height;

            ResizeBoard();
        }
    }

    private void DrawGrid()
    {
        _scroll =
            EditorGUILayout.BeginScrollView(
                _scroll);

        for (int y = _preset.Height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < _preset.Width; x++)
            {
                DrawCell(x, y);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawCell(int x, int y)
    {
        ref TilePresetData tile =
            ref GetTile(x, y);

        string special =
            tile.Special != null
                ? tile.Special.Id
                : "-";

        string label =
            $"{tile.TileType}\n{special}";

        if (GUILayout.Button(
                label,
                GUILayout.Width(CellSize),
                GUILayout.Height(CellSize)))
        {
            ShowCellMenu(x, y);
        }
    }

    private void ShowCellMenu(int x, int y)
    {
        GenericMenu menu =
            new GenericMenu();

        AddTileMenuItem(
            menu,
            x,
            y,
            ETileType.None);

        menu.AddSeparator("Tile/");

        foreach (ETileType tileType
                 in System.Enum.GetValues(
                     typeof(ETileType)))
        {
            if (tileType == ETileType.None)
            {
                continue;
            }

            AddTileMenuItem(
                menu,
                x,
                y,
                tileType);
        }

        menu.AddSeparator("");

        menu.AddItem(
            new GUIContent(
                "Special/None"),
            false,
            () =>
            {
                ref TilePresetData tile =
                    ref GetTile(x, y);

                tile.Special = null;

                EditorUtility.SetDirty(
                    _preset);
            });

        AddSpecialMenuItem(
            menu,
            x,
            y,
            "Special/HorizontalLine",
            "HorizontalLine");

        AddSpecialMenuItem(
            menu,
            x,
            y,
            "Special/VerticalLine",
            "VerticalLine");

        AddSpecialMenuItem(
            menu,
            x,
            y,
            "Special/Bomb",
            "Bomb");

        AddSpecialMenuItem(
            menu,
            x,
            y,
            "Special/ColorBomb",
            "ColorBomb");;

        menu.AddSeparator("");

        menu.AddItem(
            new GUIContent("Clear"),
            false,
            () =>
            {
                ref TilePresetData tile =
                    ref GetTile(x, y);

                tile.TileType =
                    ETileType.None;

                tile.Special = null;

                EditorUtility.SetDirty(
                    _preset);
            });

        menu.ShowAsContext();
    }

    private void AddTileMenuItem(
        GenericMenu menu,
        int x,
        int y,
        ETileType tileType)
    {
        string path =
            $"Tile/{tileType}";

        menu.AddItem(
            new GUIContent(path),
            false,
            () =>
            {
                ref TilePresetData tile =
                    ref GetTile(x, y);

                tile.TileType =
                    tileType;

                EditorUtility.SetDirty(
                    _preset);
            });
    }

    private void AddSpecialMenuItem(
        GenericMenu menu,
        int x,
        int y,
        string menuPath,
        string specialId)
    {
        menu.AddItem(
            new GUIContent(menuPath),
            false,
            () =>
            {
                TileSpecialData special =
                    FindSpecial(
                        specialId);

                if (special == null)
                {
                    Debug.LogError(
                        $"Missing TileSpecialData: {specialId}");

                    return;
                }

                ref TilePresetData tile =
                    ref GetTile(x, y);

                tile.Special = special;

                EditorUtility.SetDirty(
                    _preset);
            });
    }
    private TileSpecialData FindSpecial(
        string id)
    {
        string[] guids =
            AssetDatabase.FindAssets(
                $"t:{nameof(TileSpecialData)}");

        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(
                    guid);

            TileSpecialData asset =
                AssetDatabase.LoadAssetAtPath
                    <TileSpecialData>(path);

            if (asset.Id == id)
            {
                return asset;
            }
        }

        return null;
    }

    private T FindBehaviour<T>()
        where T : SpecialTileBehaviour
    {
        string[] guids =
            AssetDatabase.FindAssets(
                $"t:{typeof(T).Name}");

        if (guids.Length == 0)
        {
            return null;
        }

        string path =
            AssetDatabase.GUIDToAssetPath(
                guids[0]);

        return AssetDatabase.LoadAssetAtPath<T>(
            path);
    }

    private int GetIndex(int x, int y)
    {
        return y * _preset.Width + x;
    }

    private ref TilePresetData GetTile(
        int x,
        int y)
    {
        int index =
            GetIndex(x, y);

        return ref _preset.Tiles[index];
    }

    private void ResizeBoard()
    {
        TilePresetData[] tiles = new TilePresetData[
                _preset.Width *
                _preset.Height];

        for (int y = 0; y < _preset.Height; y++)
        {
            for (int x = 0; x < _preset.Width; x++)
            {
                int index =
                    y * _preset.Width + x;

                tiles[index] = new TilePresetData
                    {
                        Position =
                            new Vector2Int(x, y),

                        TileType =
                            ETileType.None,

                        Special = null
                    };
            }
        }

        _preset.Tiles = tiles;

        EditorUtility.SetDirty(_preset);
    }

    private void EnsureTileArray()
    {
        if (_preset.Tiles == null ||
            _preset.Tiles.Length !=
            _preset.Width *
            _preset.Height)
        {
            ResizeBoard();
        }
    }

    private void ReloadRuntimeBoard()
    {
        PuzzleGameplayService gameplay =
            FindFirstObjectByType
                <PuzzleGameplayService>();

        if (gameplay == null)
        {
            Debug.LogError(
                "Missing PuzzleGameplayService");

            return;
        }

        gameplay.ReloadBoard(
            _preset);
    }
}