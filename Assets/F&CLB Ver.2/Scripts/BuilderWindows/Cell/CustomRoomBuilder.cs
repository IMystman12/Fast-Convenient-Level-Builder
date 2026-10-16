#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CustomRoomBuilder : CellBuilder
{
    bool x, y = true, z, xF, yF, zF;
    bool[,,] blocks;
    GUILayoutOption[] standardOptions => new GUILayoutOption[] { GUILayout.Height(25), GUILayout.Width(25) };
    [MenuItem("Window/Level Builders/Rooms/Custom Room")]
    public static void ShowWindow()
    {
        GetWindow<CustomRoomBuilder>().InitializeWindow<CustomRoomBuilder>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        base.VirtualVariablesOnGUI();

        for (int i = 0; i < 3; i++)
        {
            if (blocks == null || (blocks.GetLength(i) != size[i]))
            {
                blocks = new bool[size.x, size.y, size.z];
                break;
            }
        }

        EditorGUILayout.Space(25);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("All On!")) SetAll(a => true);
        if (GUILayout.Button("All Off!")) SetAll(a => false);
        if (GUILayout.Button("Invert!")) SetAll(a => !a);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Open demo")) Application.OpenURL("file:///" + Path.GetFullPath(AssetDatabase.GetAssetPath(Resources.Load("Custom Room DEMO"))));
        EditorGUILayout.Space(25);

        EditorGUILayout.BeginHorizontal();
        x = EditorGUILayout.Toggle("X ", x, standardOptions);
        y = EditorGUILayout.Toggle("Y ", y, standardOptions);
        z = EditorGUILayout.Toggle("Z ", z, standardOptions);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Don't spawn:");        
        EditorGUILayout.Space(25);
        bool org;
        if (x)
        {
            xF = EditorGUILayout.Foldout(xF, "View at X+");
            if (xF)
            {
                for (int y = blocks.GetLength(1) - 1; y > -1; y--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        org = blocks[0, y, z];
                        blocks[0, y, z] = EditorGUILayout.Toggle(blocks[0, y, z], standardOptions);
                        if (org != blocks[0, y, z])
                        {
                            org = blocks[0, y, z];
                            for (int x = 0; x < blocks.GetLength(0); x++)
                            {
                                blocks[x, y, z] = org;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.LabelField("y: 0, z: 0");
                EditorGUILayout.Space(25);
            }
        }
        if (y)
        {
            yF = EditorGUILayout.Foldout(yF, "View at Y+");
            if (yF)
            {
                for (int z = blocks.GetLength(2) - 1; z > -1; z--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int x = 0; x < blocks.GetLength(0); x++)
                    {
                        org = blocks[x, 0, z];
                        blocks[x, 0, z] = EditorGUILayout.Toggle(blocks[x, 0, z], standardOptions);
                        if (org != blocks[x, 0, z])
                        {
                            org = blocks[x, 0, z];
                            for (int y = 0; y < blocks.GetLength(1); y++)
                            {
                                blocks[x, y, z] = org;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.LabelField("z: 0, x: 0");
                EditorGUILayout.Space(25);
            }
        }
        if (z)
        {
            zF = EditorGUILayout.Foldout(zF, "View at Z+");
            if (zF)
            {
                for (int y = blocks.GetLength(1) - 1; y > -1; y--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int x = blocks.GetLength(0) - 1; x > -1; x--)
                    {
                        org = blocks[x, y, 0];
                        blocks[x, y, 0] = EditorGUILayout.Toggle(blocks[x, y, 0], standardOptions);
                        if (org != blocks[x, y, 0])
                        {
                            org = blocks[x, y, 0];
                            for (int z = 0; z < blocks.GetLength(2); z++)
                            {
                                blocks[x, y, z] = org;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.LabelField($"x: {blocks.GetLength(0)}, y: 0");
                EditorGUILayout.Space(25);
            }
        }

        EditorGUILayout.Space(25);

    }
    delegate bool SetAllAction(bool val);
    void SetAll(SetAllAction action)
    {
        for (int a = 0; a < blocks.GetLength(0); a++)
        {
            for (int b = 0; b < blocks.GetLength(1); b++)
            {
                for (int c = 0; c < blocks.GetLength(2); c++)
                {
                    blocks[a, b, c] = action.Invoke(blocks[a, b, c]);
                }
            }
        }
    }
    protected override void Build()
    {
        Transform room = CreateParentFromPosition("Custom Room", true);
        Cell[,,] cells = new Cell[size.x, size.y, size.z];
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int z = 0; z < cells.GetLength(2); z++)
                {
                    if (!blocks[x, y, z])
                    {
                        Vector3Int position = new Vector3Int(x, y, z);
                        cells[x, y, z] = BuilderHelper.CreateCell(position, room);
                        BuilderHelper.ConnectAround(cells, position);
                    }
                }
            }
        }
        BuilderHelper.ClearCellScripts(cells);
    }
}
#endif