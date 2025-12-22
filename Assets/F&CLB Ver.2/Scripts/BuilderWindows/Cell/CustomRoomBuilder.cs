using UnityEditor;
using UnityEngine;

public class CustomRoomBuilder : CellBuilder
{
    bool x, y = true, z, xF, yF, zF, pastOn, pastOff;
    bool[,,] blocks;
    GUILayoutOption[] standardOptions => new GUILayoutOption[] { GUILayout.Height(10), GUILayout.Width(10) };
    [MenuItem("Window/Level Builders/Custom Room")]
    public static void ShowWindow()
    {
        GetWindow<CustomRoomBuilder>().InitializeWindow<CustomRoomBuilder>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        base.VirtualVariablesOnGUI();
        Event e = Event.current;
        if (e.type == EventType.MouseDown)
        {
        }
        bool flag = false;
        for (int i = 0; i < 3; i++)
        {
            if (blocks == null || (blocks.GetLength(i) != size[i] && !flag))
            {
                flag = true;
                blocks = new bool[size.x, size.y, size.z];
            }
        }
        EditorGUILayout.BeginHorizontal();
        x = EditorGUILayout.Toggle("X ", x);
        y = EditorGUILayout.Toggle("Y ", y);
        z = EditorGUILayout.Toggle("Z ", z);
        EditorGUILayout.EndHorizontal();
        bool flagModified = false;
        bool org = false;
        if (x)
        {
            EditorGUILayout.Space(10);
            xF = EditorGUILayout.Foldout(xF, "X:");
            if (xF)
            {
                if (GUILayout.Button("All On!"))
                {
                    flagModified = true;
                    flag = true;
                }
                if (GUILayout.Button("All Off!"))
                {
                    flagModified = true;
                    flag = false;
                }
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Blocks:");
                for (int y = blocks.GetLength(1) - 1; y > -1; y--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        org = blocks[0, y, z];
                        if (flagModified)
                        {
                            blocks[0, y, z] = flag;
                        }
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
            }
        }
        if (y)
        {
            EditorGUILayout.Space(10);
            yF = EditorGUILayout.Foldout(yF, "Y:");
            if (yF)
            {
                if (GUILayout.Button("All On!"))
                {
                    flagModified = true;
                    flag = true;
                }
                if (GUILayout.Button("All Off!"))
                {
                    flagModified = true;
                    flag = false;
                }
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Blocks:");
                for (int x = blocks.GetLength(0) - 1; x > -1; x--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        org = blocks[x, 0, z];
                        if (flagModified)
                        {
                            blocks[x, 0, z] = flag;
                        }
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
            }
        }
        if (z)
        {
            EditorGUILayout.Space(10);
            zF = EditorGUILayout.Foldout(zF, "Z:");
            if (zF)
            {
                if (GUILayout.Button("All On!"))
                {
                    flagModified = true;
                    flag = true;
                }
                if (GUILayout.Button("All Off!"))
                {
                    flagModified = true;
                    flag = false;
                }
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Blocks:");
                for (int y = blocks.GetLength(1) - 1; y > -1; y--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int x = blocks.GetLength(0) - 1; x > -1; x--)
                    {
                        org = blocks[x, y, 0];
                        if (flagModified)
                        {
                            blocks[x, y, 0] = flag;
                        }
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
            }
        }
    }
    protected override void Build()
    {
        Transform room = new GameObject("Room").transform;
        room.position = buildPosition * 10;
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
