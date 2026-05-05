using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardPreset))]
public sealed class BoardPresetEditor : Editor
{
    private TileVisualDatabase _tileVisualDatabase;
    private SpecialResolverDatabase _database;
    private Dictionary<string, TileSpecialData> _specialLookup;
    private const int CellSize = 50;
    const int Spacing = 6;

    private BoardPreset _preset;

    private Vector2 _scroll;

    private void OnEnable()
    {
        _preset = (BoardPreset)target;

        EnsureTileArray();
        _database = FindDatabase<SpecialResolverDatabase>();
        _tileVisualDatabase =
        FindDatabase<TileVisualDatabase>();
        BuildLookup();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawBoardSize();

        GUILayout.Space(10);

        DrawBrushToolbar();

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

    private void DrawCell(
        int x,
        int y)
    {
        ref TilePresetData tile =
            ref GetTile(x, y);

        Rect rect =
            GUILayoutUtility.GetRect(
                CellSize,
                CellSize);

        GUI.Box(
            rect,
            GUIContent.none);

        Texture preview = null;

        if (tile.Special != null &&
            tile.Special.Icon != null)
        {
            preview =
                AssetPreview.GetAssetPreview(
                    tile.Special.Icon);

            if (preview == null)
            {
                preview =
                    AssetPreview.GetMiniThumbnail(
                        tile.Special.Icon);
            }
        }
        else
        {
            Sprite sprite =
                _tileVisualDatabase.GetSprite(
                    tile.TileType);

            if (sprite != null)
            {
                preview =
                    AssetPreview.GetAssetPreview(
                        sprite);

                if (preview == null)
                {
                    preview =
                        AssetPreview.GetMiniThumbnail(
                            sprite);
                }
            }
        }

        if (preview != null)
        {
            GUI.DrawTexture(
                rect,
                preview,
                ScaleMode.ScaleToFit);
        }

        Event e =
            Event.current;

        bool isInside =
            rect.Contains(
                e.mousePosition);

        if (isInside == false)
        {
            return;
        }

        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown ||
                e.type == EventType.MouseDrag)
            {
                PaintCell(x, y);

                e.Use();
            }
        }

        if (e.button == 1)
        {
            if (e.type == EventType.MouseDown ||
                e.type == EventType.MouseDrag)
            {
                ClearCell(x, y);

                e.Use();
            }
        }
    }

    private void ClearCell(
        int x,
        int y)
    {
        ref TilePresetData tile =
            ref GetTile(x, y);

        tile.TileType =
            ETileType.None;

        tile.Special = null;

        EditorUtility.SetDirty(
            _preset);

        Repaint();
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

    private T FindDatabase<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        if (guids.Length == 0)
        {
            Debug.LogError($"Missing database: {typeof(T).Name}");

            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);

        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    private void BuildLookup()
    {
        _specialLookup = new Dictionary<string, TileSpecialData>();

        if (_database == null)
        {
            return;
        }

        foreach (SpecialResolverEntry entry
                in _database.Entries)
        {
            TileSpecialData special =
                entry.Result;

            if (special == null)
            {
                continue;
            }

            _specialLookup[special.Id] =
                special;
        }
    }


    private ETileType _selectedTileType;

    private TileSpecialData _selectedSpecial;

    private void DrawBrushToolbar()
    {
        EditorGUILayout.LabelField(
            "Brushes",
            EditorStyles.boldLabel);

        DrawNormalTileBrushes();

        EditorGUILayout.Space();

        DrawSpecialBrushes();
    }
    private void DrawNormalTileBrushes()
    {
        if (_tileVisualDatabase == null)
        {
            return;
        }

        EditorGUILayout.LabelField(
            "Tiles",
            EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        foreach (TileVisualEntry entry
                in _tileVisualDatabase.Entries)
        {
            if (entry.Type == ETileType.None)
            {
                continue;
            }

            GUIStyle style =
                new GUIStyle(GUI.skin.button);

            if (_selectedSpecial == null &&
                _selectedTileType == entry.Type)
            {
                style.normal.background =
                    style.active.background;
            }

            Texture preview = AssetPreview.GetAssetPreview( entry.Sprite);

            if (preview == null)
            {
                preview = AssetPreview.GetMiniThumbnail(entry.Sprite);
            }

            GUIContent content = new GUIContent(preview);

            if (GUILayout.Button(
                    content,
                    style,
                    GUILayout.Width(48),
                    GUILayout.Height(48)))
            {
                _selectedTileType =
                    entry.Type;

                _selectedSpecial =
                    null;

                Repaint();
            }
        }

        EditorGUILayout.EndHorizontal();
    }
    private void DrawSpecialBrushes()
    {
        if (_database == null)
        {
            return;
        }

        float inspectorWidth =
            EditorGUIUtility.currentViewWidth;

        

        int columnCount =
            Mathf.Max(
                1,
                Mathf.FloorToInt(
                    (inspectorWidth - 20) /
                    (CellSize + Spacing)));

        int currentColumn = 0;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        foreach (SpecialResolverEntry entry
                in _database.Entries)
        {
            if (entry.Result == null)
            {
                continue;
            }

            if (currentColumn >= columnCount)
            {
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                currentColumn = 0;
            }

            DrawSpecialBrush(entry);

            currentColumn++;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
    private void DrawSpecialBrush(
        SpecialResolverEntry entry)
    {
        TileSpecialData special =
            entry.Result;

        if (special == null)
        {
            return;
        }

        GUIContent content =
            new GUIContent(
                AssetPreview.GetAssetPreview(
                    special.Icon));

        if (GUILayout.Button(
                content,
                GUILayout.Width(CellSize),
                GUILayout.Height(CellSize),
                GUILayout.ExpandWidth(false)))
        {
            _selectedTileType =
                entry.TileType;

            _selectedSpecial =
                special;

            Repaint();
        }
    }
    private void PaintCell(
        int x,
        int y)
    {
        ref TilePresetData tile =
            ref GetTile(x, y);

        if (_selectedSpecial != null)
        {
            tile.TileType =
                _selectedTileType;

            tile.Special =
                _selectedSpecial;
        }
        else
        {
            tile.TileType =
                _selectedTileType;

            tile.Special = null;
        }

        EditorUtility.SetDirty(
            _preset);

        Repaint();
    }
}