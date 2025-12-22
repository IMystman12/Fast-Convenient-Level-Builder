using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MazeGenerator : CellBuilder
{
    int patchCount = 1, minPatchSize = 5, maxPatchSize = 10, minPatchConnections = 2, maxPatchConnections = 3, perPatchAttempts = 100;
    [MenuItem("Window/Level Builders/Maze Generator")]
    private static void ShowWindow()
    {
        GetWindow<MazeGenerator>().InitializeWindow<MazeGenerator>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        base.VirtualVariablesOnGUI();
        patchCount = EditorGUILayout.IntField("Patch Count:", patchCount);
        minPatchSize = EditorGUILayout.IntField("Min Patch Size:", minPatchSize);
        maxPatchSize = EditorGUILayout.IntField("Max Patch Size:", maxPatchSize);
        minPatchConnections = EditorGUILayout.IntField("Min Patch Connections:", minPatchConnections);
        maxPatchConnections = EditorGUILayout.IntField("Max Patch Connections:", maxPatchConnections);
        perPatchAttempts = EditorGUILayout.IntField("Patch Attempts:", perPatchAttempts);
    }
    protected override void Build()
    {
    IL_0001:
        Transform transform = new GameObject("Room").transform;
        transform.position = buildPosition * 10;
        Cell[,,] cells = new Cell[size.x, size.y, size.z];
        Vector3Int sizePatch = Vector3Int.zero, positionPatch = Vector3Int.zero, positionCur, start, end;
        int attempts;
        bool flag, flag0;
        List<Vector3Int> patchConnections = new List<Vector3Int>();
        List<Vector3Int> patchCells = new List<Vector3Int>();
        for (int i = 0; i < patchCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                sizePatch[j] = Mathf.Min(size[j], Random.Range(minPatchSize, maxPatchSize + 1));
            }
            attempts = 0;
            flag0 = false;
            while (attempts < perPatchAttempts && !flag0)
            {
                attempts++;
                for (int j = 0; j < 3; j++)
                {
                    positionPatch[j] = Random.Range(0, size[j] - sizePatch[j] + 1);
                }

                //check patch
                flag = false;
                start = positionPatch;
                end = sizePatch + positionPatch;
                for (int x = start.x; x < end.x; x++)
                {
                    for (int y = start.y; y < end.y; y++)
                    {
                        for (int z = start.z; z < end.z; z++)
                        {
                            if (cells[x, y, z])
                            {
                                flag = true;
                            }
                        }
                    }
                }

                if (!flag)
                {
                    List<Vector3Int> portentialPatchConnections = new List<Vector3Int>();
                    for (int x = start.x; x < end.x; x++)
                    {
                        for (int y = start.y; y < end.y; y++)
                        {
                            for (int z = start.z; z < end.z; z++)
                            {
                                positionCur = new Vector3Int(x, y, z);
                                cells[x, y, z] = BuilderHelper.CreateCell(positionCur, transform);
                                BuilderHelper.ConnectAround(cells, positionCur);
                                patchCells.Add(positionCur);
                            }
                        }
                    }

                    for (int x = start.x; x < end.x; x++)
                    {
                        for (int y = start.y; y < end.y; y++)
                        {
                            for (int z = start.z; z < end.z; z++)
                            {
                                positionCur = new Vector3Int(x, y, z);
                                if (BuilderHelper.GetFreeNeighbors(cells, positionCur).Count > 0)
                                {
                                    portentialPatchConnections.Add(positionCur);
                                }
                            }
                        }
                    }

                    int countConnections = Random.Range(minPatchConnections, maxPatchConnections);
                    for (int a = 0; a < countConnections; a++)
                    {
                        if (portentialPatchConnections.Count > 0)
                        {
                            patchConnections.Add(portentialPatchConnections.Random());
                        }
                    }
                    flag0 = true;
                }
            }
        }

        //paths
        start = Vector3Int.zero;
        attempts = 0;
        while (BuilderHelper.CellFromPosition(cells, start) && attempts < 1000)
        {
            attempts++;
            for (int i = 0; i < 3; i++)
            {
                start[i] = Random.Range(0, size[i]);
            }
        }
        List<Vector3Int> activeCells = new List<Vector3Int>()
        {
            start
        };
        cells[start.x, start.y, start.z] = BuilderHelper.CreateCell(start, transform);
        List<Vector3Int> activeDirs;
        Vector3Int pos, pos0, dir;
        attempts = 0;
        while (activeCells.Count > 0)
        {
            attempts++;
            //sample dirs
            pos = activeCells.Random();
            activeDirs = BuilderHelper.GetFreeNeighbors(cells, pos);

            //connect
            if (activeDirs.Count > 0)
            {
                dir = activeDirs.Random();
                pos0 = pos + dir;
                activeCells.Add(pos0);
                cells[pos0.x, pos0.y, pos0.z] = BuilderHelper.CreateCell(pos0 , transform);
                if (!BuilderHelper.CellFromPosition(cells, pos) || !BuilderHelper.CellFromPosition(cells, pos0))
                {
                    Debug.Log("error");
                }
                BuilderHelper.ConnectCell(BuilderHelper.CellFromPosition(cells, pos), BuilderHelper.CellFromPosition(cells, pos0));
            }
            else
            {
                activeCells.Remove(pos);
            }
        }

        foreach (var item in patchConnections)
        {
            activeCells.Clear();
            for (int i = 0; i < 6; i++)
            {
                pos = item + BuilderHelper.AllDirections[i];
                if (!BuilderHelper.ContainsOutOfRange(cells, pos) && !patchCells.Contains(pos))
                {
                    activeCells.Add(pos);
                }
            }

            if (activeCells.Count > 0)
            {
                pos0 = activeCells.Random();
                BuilderHelper.ConnectCell(BuilderHelper.CellFromPosition(cells, pos0), BuilderHelper.CellFromPosition(cells, item));
            }
        }

        BuilderHelper.ClearCellScripts(cells);
    }
}
