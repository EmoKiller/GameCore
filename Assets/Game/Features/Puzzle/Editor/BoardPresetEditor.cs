using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardPreset))]
public sealed class BoardPresetEditor : Editor
{
    private TileVisualDatabase _tileVisualDatabase;

    private SpecialResolverDatabase _database;

    private Dictionary<string, TileSpecialData>
        _specialLookup;

    private const int CellSize = 50;

    private const int BrushSize = 50;

    private const int Padding = 4;

    private const int Spacing = 6;

    private BoardPreset _preset;

    private Vector2 _scroll;

    private ETileType _selectedTileType;

    private TileSpecialData _selectedSpecial;

    private void OnEnable()
    {
        _preset = (BoardPreset)target;

        EnsureTileArray();

        _database =
            FindDatabase<SpecialResolverDatabase>();

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

        foreach (TileVisualEntry entry in _tileVisualDatabase.Entries)
        {
            if (entry.Type == ETileType.None)
            {
                continue;
            }

            bool selected =
                _selectedSpecial == null &&
                _selectedTileType == entry.Type;

            if (DrawTileButton(
                    entry.Sprite,
                    BrushSize,
                    selected))
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
                    (BrushSize + Spacing)));

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

        bool selected =
            _selectedSpecial == special;

        if (DrawTileButton(
                special.Icon,
                BrushSize,
                selected))
        {
            _selectedTileType =
                entry.TileType;

            _selectedSpecial =
                special;

            Repaint();
        }
    }

    private void DrawGrid()
    {
        EditorGUILayout.LabelField(
            "Board",
            EditorStyles.boldLabel);
        _scroll =
            EditorGUILayout.BeginScrollView(
                _scroll);

        for (int y = _preset.Height - 1;  y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0;
                 x < _preset.Width;
                 x++)
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

        Sprite sprite = null;

        if (tile.Special != null)
        {
            sprite =
                tile.Special.Icon;
        }
        else if (tile.TileType != ETileType.None)
        {
            sprite =
                _tileVisualDatabase.GetSprite(
                    tile.TileType);
        }

        Rect rect =
            GUILayoutUtility.GetRect(
                CellSize,
                CellSize,
                GUILayout.Width(CellSize),
                GUILayout.Height(CellSize));

        DrawTileVisual(
            rect,
            sprite,
            false);

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

    private bool DrawTileButton(
        Sprite sprite,
        int size,
        bool selected)
    {
        Rect rect =
            GUILayoutUtility.GetRect(
                size,
                size,
                GUILayout.Width(size),
                GUILayout.Height(size));

        GUIStyle style =
            GUI.skin.button;

        GUI.Box(
            rect,
            GUIContent.none,
            style);

        if (selected)
        {
            EditorGUI.DrawRect(
                new Rect(
                    rect.x,
                    rect.y,
                    rect.width,
                    2),
                Color.yellow);

            EditorGUI.DrawRect(
                new Rect(
                    rect.x,
                    rect.yMax - 2,
                    rect.width,
                    2),
                Color.yellow);

            EditorGUI.DrawRect(
                new Rect(
                    rect.x,
                    rect.y,
                    2,
                    rect.height),
                Color.yellow);

            EditorGUI.DrawRect(
                new Rect(
                    rect.xMax - 2,
                    rect.y,
                    2,
                    rect.height),
                Color.yellow);
        }

        if (sprite != null)
        {
            Rect iconRect =
                new Rect(
                    rect.x + Padding,
                    rect.y + Padding,
                    rect.width - Padding * 2,
                    rect.height - Padding * 2);

            DrawSprite(
                iconRect,
                sprite);
        }

        return GUI.Button(
            rect,
            GUIContent.none,
            GUIStyle.none);
    }

    private void DrawSprite(
        Rect rect,
        Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        Rect uv =
            new Rect(
                sprite.textureRect.x /
                    sprite.texture.width,

                sprite.textureRect.y /
                    sprite.texture.height,

                sprite.textureRect.width /
                    sprite.texture.width,

                sprite.textureRect.height /
                    sprite.texture.height);

        GUI.DrawTextureWithTexCoords(
            rect,
            sprite.texture,
            uv);
    }

    private void PaintCell(
        int x,
        int y)
    {
        ref TilePresetData tile =
            ref GetTile(x, y);

        tile.TileType =
            _selectedTileType;

        tile.Special =
            _selectedSpecial;

        EditorUtility.SetDirty(_preset);

        Repaint();
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

    private int GetIndex(
        int x,
        int y)
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
        TilePresetData[] tiles =
            new TilePresetData[
                _preset.Width *
                _preset.Height];

        for (int y = 0;
             y < _preset.Height;
             y++)
        {
            for (int x = 0;
                 x < _preset.Width;
                 x++)
            {
                int index =
                    y * _preset.Width + x;

                tiles[index] =
                    new TilePresetData
                    {
                        Position =
                            new Vector2Int(
                                x,
                                y),

                        TileType =
                            ETileType.None,

                        Special = null
                    };
            }
        }

        _preset.Tiles = tiles;

        EditorUtility.SetDirty(
            _preset);
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
        string[] guids =
            AssetDatabase.FindAssets(
                $"t:{typeof(T).Name}");

        if (guids.Length == 0)
        {
            Debug.LogError(
                $"Missing database: {typeof(T).Name}");

            return null;
        }

        string path =
            AssetDatabase.GUIDToAssetPath(
                guids[0]);

        return AssetDatabase
            .LoadAssetAtPath<T>(path);
    }

    private void BuildLookup()
    {
        _specialLookup =
            new Dictionary<string, TileSpecialData>();

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

            _specialLookup[
                special.Id] = special;
        }
    }
    private void DrawTileVisual(
        Rect rect,
        Sprite sprite,
        bool selected)
    {
        GUIStyle style =
            GUI.skin.button;

        GUI.Box(
            rect,
            GUIContent.none,
            style);

        if (selected)
        {
            EditorGUI.DrawRect(
                new Rect(
                    rect.x,
                    rect.y,
                    rect.width,
                    2),
                Color.yellow);
        }

        if (sprite == null)
        {
            return;
        }

        Rect iconRect =
            new Rect(
                rect.x + Padding,
                rect.y + Padding,
                rect.width - Padding * 2,
                rect.height - Padding * 2);

        DrawSprite(
            iconRect,
            sprite);
    }
}