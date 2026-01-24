#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RandomPathGenerator : CellBuilder
{
    public int wayLengthMin = 2, wayLengthMax = 5, wayCountMin = 2, wayCountMax = 5;
    public int checkDistance = 1;
    [MenuItem("Window/Level Builders/Rooms/Random Path Generator")]
    private static void ShowWindow()
    {
        GetWindow<RandomPathGenerator>().InitializeWindow<RandomPathGenerator>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        base.VirtualVariablesOnGUI();
        wayLengthMin = EditorGUILayout.IntField("Way Length Min:", wayLengthMin);
        wayLengthMax = EditorGUILayout.IntField("Way Length Max:", wayLengthMax);
        wayCountMin = EditorGUILayout.IntField("Way Count Min:", wayCountMin);
        wayCountMax = EditorGUILayout.IntField("Way Count Max:", wayCountMax);
    }
    protected override void Build()
    {
        Transform roomBase = CreateParentFromPosition("Room");
        Vector3Int start = Vector3Int.zero, direction = Vector3Int.forward, pos0;
        Cell[,,] cells = new Cell[size.x, size.y, size.z];
        for (int i = 0; i < 2; i++)
        {
            start[i] = size[i] / 2;
        }
        cells[start.x, start.y, start.z] = BuilderHelper.CreateCell(start, roomBase);
        List<Vector3Int> open = new List<Vector3Int>();
        int a = Random(wayLengthMin, wayLengthMax), b = 1, c = Random(wayCountMin, wayCountMax);
        while (c > 0)
        {
            List<Vector3Int> list = BuilderHelper.GetFreeNeighbors(cells, start);
            if (a <= b)
            {
                list.Remove(direction);
                a = Random(wayLengthMin, wayLengthMax);
                c--;
            }
            if (list.Count > 0)
            {
                if (!list.Contains(direction))
                {
                    direction = list.Random();
                }
                start += direction;
                cells[start.x, start.y, start.z] = BuilderHelper.CreateCell(start, roomBase);
                BuilderHelper.ConnectAround(cells, start);
                open.Add(start);
                b++;
            }
            else
            {
                open.Remove(start);
                start = open.Random();
                a = Random(wayLengthMin, wayLengthMax);
                c--;
            }
        }
        BuilderHelper.ClearCellScripts(cells);
    }
}
#endif