#if UNITY_EDITOR
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HallGenerator : CellBuilder
{
    int minPlotCount = 5, maxPlotCount = 6, minPlotSize = 9;
    FillType fillType;
    const int pathWidth = 1;
    public enum FillType
    {
        Outline,
        Completely
    }
    [MenuItem("Window/Level Builders/Rooms/Hall Generator")]
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
        Transform roomBase = CreateParentFromPosition("Room");
        Plot[,,] plotMap = new Plot[size.x, size.y, size.z];
        List<Plot> plots = new List<Plot>();

        {
            int pc = Random(minPlotCount, maxPlotCount);
            List<Vector3Int> list = new List<Vector3Int>();
            Plot p;
            for (int i = 0; i < pc; i++)
            {
                p = new Plot();
                for (int j = 0; j < 3; j++)
                {
                    p.start[j] = Random(0, size[j]);
                }
                while (list.Contains(p.start))
                {
                    for (int j = 0; j < 3; j++)
                    {
                        p.start[j] = Random(0, size[j]);
                    }
                }
                p.end = p.start;
                list.Add(p.start);
                plots.Add(p);
                p.Fill(plotMap);
            }
        }

        {
            int c;
            List<int> dirs = new List<int>();
            while (plots.Count > 0)
            {
                for (int j = 0; j < plots.Count; j++)
                {
                    dirs.Clear();
                    for (int i = 0; i < 6; i++)
                    {
                        if (plots[j].Check(plotMap, i, pathWidth + 1))
                        {
                            dirs.Add(i);
                        }
                    }
                    if (dirs.Count == 0)
                    {
                        plots.RemoveAt(j);
                        j--;
                    }
                    else
                    {
                        c = Random(0, dirs.Count);
                        plots[j].Resize(dirs[c], pathWidth);
                        plots[j].Fill(plotMap);
                    }
                }
            }
        }

        {
            Cell[,,] cells = new Cell[size.x, size.y, size.z];
            Vector3Int v3;
            switch (fillType)
            {
                case FillType.Outline:
                    Plot p = new Plot();
                    bool flag;
                    for (int a = 0; a < size.x; a++)
                    {
                        for (int b = 0; b < size.y; b++)
                        {
                            for (int c = 0; c < size.z; c++)
                            {
                                if (plotMap[a, b, c] == null)
                                {
                                    flag = false;
                                    v3 = new Vector3Int(a, b, c);
                                    for (int i = 0; i < 3; i++)
                                    {
                                        p.start[i] = Mathf.Max(0, v3[i] - 1);
                                        p.end[i] = Mathf.Min(size[i] - 1, v3[i] + 1);
                                    }

                                    for (int d = p.start.x; d < p.end.x + 1; d++)
                                    {
                                        for (int e = p.start.y; e < p.end.y + 1; e++)
                                        {
                                            for (int f = p.start.z; f < p.end.z + 1; f++)
                                            {
                                                if (plotMap[d, e, f] != null)
                                                {
                                                    flag = true;
                                                }
                                            }
                                        }
                                    }

                                    if (flag)
                                    {
                                        cells[a, b, c] = BuilderHelper.CreateCell(v3, roomBase);
                                        BuilderHelper.ConnectAround(cells, v3);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case FillType.Completely:
                    for (int a = 0; a < size.x; a++)
                    {
                        for (int b = 0; b < size.y; b++)
                        {
                            for (int c = 0; c < size.z; c++)
                            {
                                v3 = new Vector3Int(a, b, c);
                                if (plotMap[a, b, c] == null)
                                {
                                    cells[a, b, c] = BuilderHelper.CreateCell(v3, roomBase);
                                    BuilderHelper.ConnectAround(cells, v3);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
    public class Plot
    {
        public Vector3Int start, end;
        public void Resize(int dir, int expand)
        {
            Vector3Int d = BuilderHelper.GetVector3IntFromDirection(dir) * expand;
            for (int i = 0; i < 3; i++)
            {
                start[i] += Mathf.Min(d[i], 0);
                end[i] += Mathf.Max(d[i], 0);
            }
        }
        public void Fill(Plot[,,] map)
        {
            for (int a = start.x; a < end.x + 1; a++)
            {
                for (int b = start.y; b < end.y + 1; b++)
                {
                    for (int c = start.z; c < end.z + 1; c++)
                    {
                        map[a, b, c] = this;
                    }
                }
            }
        }
        public bool Check(Plot[,,] map, int dir, int expand)
        {
            Plot p = new Plot()
            {
                start = start,
                end = end
            };

            p.Resize(dir, expand);

            for (int i = 0; i < 3; i++)
            {
                if (p.start[i] < 0)
                {
                    return false;
                }
                if (p.end[i] > map.GetLength(i) - 1)
                {
                    return false;
                }
            }

            for (int a = p.start.x; a < p.end.x + 1; a++)
            {
                for (int b = p.start.y; b < p.end.y + 1; b++)
                {
                    for (int c = p.start.z; c < p.end.z + 1; c++)
                    {
                        if (map[a, b, c] != null && map[a, b, c] != this)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
#endif