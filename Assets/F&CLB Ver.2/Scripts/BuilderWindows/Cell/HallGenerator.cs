using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HallGenerator : CellBuilder
{
    int minPlotCount = 5, maxPlotCount = 6, minPlotSize = 9;
    FillType fillType;
    public enum FillType
    {
        Outline,
        Completely
    }
    [MenuItem("Window/Level Builders/Hall Generator")]
    private static void ShowWindow()
    {
        GetWindow<HallGenerator>().InitializeWindow<HallGenerator>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        base.VirtualVariablesOnGUI();
        minPlotCount = EditorGUILayout.IntField("Min Plot Count:", minPlotCount);
        maxPlotCount = EditorGUILayout.IntField("Max Plot Count:", maxPlotCount);
        minPlotSize = EditorGUILayout.IntField("Min Plot Size:", minPlotSize);
        fillType = (FillType)EditorGUILayout.EnumPopup("Fill Type:", fillType);
    }
    protected override void Build()
    {
    IL_0001:
        Transform roomBase = new GameObject("Room").transform;
        roomBase.transform.position = buildPosition * 10;
        bool[,,] filled = new bool[size.x, size.y, size.z];
        List<Plot> plots = new List<Plot>();
        Plot p;
        int count = Random.Range(minPlotCount, maxPlotCount);
        for (int i = 0; i < count; i++)
        {
            p = new Plot();
            for (int j = 0; j < 3; j++)
            {
                p.position[j] = Random.Range(0, size[j]);
            }
            while (Index(filled, p.position))
            {
                for (int j = 0; j < 3; j++)
                {
                    p.position[j] = Random.Range(0, size[j]);
                }
            }
            filled[p.position.x, p.position.y, p.position.z] = true;
            plots.Add(p);
        }

        List<int> list;
        int index = 0, dir, attempts = 0;
        while (plots.Count > 0 && attempts < 100)
        {
            attempts++;
            if (index == plots.Count)
            {
                index = 0;
            }

            list = plots[index].GetPossibleDirections(filled, 2);
            if (list.Count > 0)
            {
                dir = list.Random();
                Fill(filled, plots[index].GetOutline(dir));
                plots[index].UpdateSize(dir);

                index++;
            }
            else
            {
                Debug.Log($"No dir found! Remove {index}");
                plots.RemoveAt(index);
            }
        }

        Cell[,,] cells = new Cell[size.x, size.y, size.z];
        bool flag;
        Vector3Int vector, vector0;
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    vector = new Vector3Int(x, y, z);
                    if (Index(filled, vector))
                    {
                        cells[x, y, z] = BuilderHelper.CreateCell(vector, roomBase);
                        BuilderHelper.ConnectAround(cells, vector);
                    }
                    if (false)
                    {
                        flag = fillType is FillType.Completely && !Index(filled, vector);
                        if (fillType is FillType.Outline)
                        {
                            flag = false;
                            for (int i = 0; i < 6; i++)
                            {
                                vector0 = vector + BuilderHelper.AllDirections[i];
                                if (!OutOfRange(filled, vector0) && Index(filled, vector0))
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (flag)
                        {
                            cells[x, y, z] = BuilderHelper.CreateCell(vector, roomBase);
                            BuilderHelper.ConnectAround(cells, vector);
                        }
                    }
                }
            }
        }

        BuilderHelper.ClearCellScripts(cells);
        return;
    }
    static bool Index(bool[,,] bools, Vector3Int pos)
    {
        return bools[pos.x, pos.y, pos.z];
    }
    public void Fill(bool[,,] cells, List<Vector3Int> poses)
    {
        foreach (var item in poses)
        {
            cells[item.x, item.y, item.z] = true;
        }
    }
    static bool OutOfRange(bool[,,] bools, Vector3Int pos)
    {
        for (int i = 0; i < 3; i++)
        {
            if (pos[i] < 0 || pos[i] >= bools.GetLength(i))
            {
                return true;
            }
        }
        return false;
    }
    [Serializable]
    public class Plot
    {
        public Vector3Int position, size;
        public void UpdateSize(int dir)
        {
            size += BuilderHelper.AllDirections[dir];
            position += Vector3Int.Min(BuilderHelper.AllDirections[dir], Vector3Int.zero);
        }
        public bool Contains(bool[,,] cells, List<Vector3Int> poses)
        {
            foreach (var item in poses)
            {
                if (OutOfRange(cells, item) || Index(cells, item))
                {
                    return true;
                }
            }
            return false;
        }
        public List<int> GetPossibleDirections(bool[,,] cells, int length = 1)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                if (!Contains(cells, GetOutline(i, length)))
                {
                    result.Add(i);
                }
            }
            return result;
        }
        public List<Vector3Int> GetOutline(int dir, int length = 1)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            Vector3Int start = position, end = position + size;
            switch (dir)
            {
                case 0:
                    start.z += size.z;
                    end.z += length;
                    break;
                case 1:
                    start.y += size.y;
                    end.y += length;
                    break;
                case 2:
                    start.x += size.x;
                    end.x += length;
                    break;
                case 3:
                    start.z -= length;
                    end.z = position.z;
                    break;
                case 4:
                    start.y -= length;
                    end.y = position.y;
                    break;
                case 5:
                    start.x -= length;
                    end.x = position.x;
                    break;
            }
            for (int x = start.x; x < end.x; x++)
            {
                for (int y = start.y; y < end.y; y++)
                {
                    for (int z = start.z; z < end.z; z++)
                    {
                        result.Add(new Vector3Int(x, y, z));
                    }
                }
            }
            return result;
        }
    }
}
