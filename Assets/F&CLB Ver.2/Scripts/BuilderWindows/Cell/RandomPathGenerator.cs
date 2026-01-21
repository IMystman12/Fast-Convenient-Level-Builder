#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RandomPathGenerator : CellBuilder
{
    public float turnChance = 50;
    public int checkDistance = 1;
    [MenuItem("Window/Level Builders/Rooms/Random Path Generator")]
    private static void ShowWindow()
    {
        GetWindow<RandomPathGenerator>().InitializeWindow<RandomPathGenerator>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        base.VirtualVariablesOnGUI();
        turnChance = EditorGUILayout.Slider("Turn Chance:", turnChance, 0, 100);
    }
    protected override void Build()
    {
        Transform roomBase = CreateParentFromPosition("Room");
        Vector3Int start = Vector3Int.zero, end = size - Vector3Int.one, direction = Vector3Int.forward, pos0;
        Cell[,,] cells = new Cell[size.x, size.y, size.z];
        for (int i = 0; i < 2; i++)
        {
            start[i] = size[i] / 2;
            end[i] = size[i] / 2;
        }
        cells[start.x, start.y, start.z] = BuilderHelper.CreateCell(start, roomBase);
        List<Vector3Int> open = new List<Vector3Int>();
        while (start != end)
        {
            List<Vector3Int> list = BuilderHelper.GetFreeNeighbors(cells, start);
            if (Random(0, 100) <= turnChance)
            {
                list.Remove(direction);
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
            }
            else
            {
                open.Remove(start);
                start = open.Random();
            }
        }
        BuilderHelper.ClearCellScripts(cells);
    }
}
#endif