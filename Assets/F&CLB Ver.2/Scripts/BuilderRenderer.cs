#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuilderRenderer : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = areaColor;
        if (MartixObjectBuilder.objectVolumn == null)
        {
            Gizmos.DrawCube(BuilderHelper.position * 10 + new Vector3Int(size.x * 5 - 5, size.y * 5, size.z * 5 - 5), size * 10);
        }
        else
        {
            for (int a = 0; a < MartixObjectBuilder.objectVolumn.GetLength(0); a++)
            {
                for (int b = 0; b < MartixObjectBuilder.objectVolumn.GetLength(1); b++)
                {
                    for (int c = 0; c < MartixObjectBuilder.objectVolumn.GetLength(2); c++)
                    {
                        if (MartixObjectBuilder.objectVolumn[a, b, c] == null)
                        {
                            EditorWindow.GetWindow<MartixObjectBuilder>().Regenerate();
                        }
                        Gizmos.DrawCube(MartixObjectBuilder.objectVolumn[a, b, c].offset + BuilderHelper.position * 10 + Vector3.Scale(new Vector3(a, b, c), MartixObjectBuilder.margin), MartixObjectBuilder.objectVolumn[a, b, c].volumn);
                    }
                }
            }
        }
    }
    public Color areaColor = new Color(0, 1, 0, 0.5f);
    public Vector3Int size = Vector3Int.one;
    public static BuilderRenderer Instance => instance;
    private static BuilderRenderer instance => FindFirstObjectByType<BuilderRenderer>();
}
public static class BuilderHelper
{
    public static Material ceiling => Resources.Load("Placeholder_Celing") as Material;
    public static Material wall => Resources.Load("Placeholder_Wall") as Material;
    public static Material floor => Resources.Load("Placeholder_Floor") as Material;
    public static Vector3Int position => GetGridPosition(BuilderRenderer.Instance.transform.position);
    private static Vector3Int[] directions => new Vector3Int[]
     {
    new Vector3Int(0,0,1),
    new Vector3Int(0,1,0),
    new Vector3Int(1,0,0),
    new Vector3Int(0,0,-1),
    new Vector3Int(0,-1,0),
    new Vector3Int(-1,0,0)
     };
    public static Vector3Int[] AllDirections => directions;
    public static T Random<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            return list[list.Count > 1 ? BuilderWindow.Random(0, list.Count) : 0];
        }
        return default;
    }
    public static List<Vector3Int> GetFreeNeighbors(Cell[,,] cells, Vector3Int pos) => GetFreeNeighbors(cells, pos, 1);
    public static List<Vector3Int> GetFreeNeighbors(Cell[,,] cells, Vector3Int pos, int range)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        Vector3Int pos0;
        bool flag;
        for (int i = 0; i < 6; i++)
        {
            flag = true;
            for (int j = 0; j < range; j++)
            {
                pos0 = pos + AllDirections[i] * range;
                if (ContainsOutOfRange(cells, pos0) || ContainsCell(cells, pos0))
                {
                    flag = false;
                }
            }
            if (flag)
            {
                result.Add(AllDirections[i]);
            }
        }
        return result;
    }
    public static Vector3Int GetGridPosition(Vector3 position)
    {
        position /= 10;
        return new Vector3Int(Mathf.RoundToInt(position.x), Mathf.FloorToInt(position.y), Mathf.RoundToInt(position.z));
    }
    public static Cell CreateCell(Vector3Int _position, Transform room = null)
    {
        Cell val;
        if (room)
        {
            val = Object.Instantiate<Cell>(Resources.Load<Cell>("Cell"), (_position + position) * 10, Quaternion.identity, room);
        }
        else
        {
            val = Object.Instantiate<Cell>(Resources.Load<Cell>("Cell"), (_position + position) * 10, Quaternion.identity);
        }
        val.Initialize();
        return val;
    }
    public static bool ContainsOutOfRange(Cell[,,] cells, Vector3Int position)
    {
        for (int i = 0; i < 3; i++)
        {
            if (position[i] < 0 || cells.GetLength(i) <= position[i])
            {
                return true;
            }
        }
        return false;
    }
    public static bool ContainsCell(Cell[,,] cells, Vector3Int position) => CellFromPosition(cells, position);
    public static Cell CellFromPosition(Cell[,,] cells, Vector3Int position)
    {
        if (!ContainsOutOfRange(cells, position))
        {
            return cells[position.x, position.y, position.z];
        }
        return null;
    }
    public static void ConnectCell(Cell cellA, Cell cellB)
    {
        if (!cellA || !cellB)
        {
            Debug.LogWarning("ERROR!");
            return;
        }
        cellA.DestroyWall(GetDirectionFromVector3Int(cellB.localPosition - cellA.localPosition));
        cellB.DestroyWall(GetDirectionFromVector3Int(cellA.localPosition - cellB.localPosition));
    }
    public static void ConnectAround(Cell[,,] cells, Vector3Int position)
    {
        Cell cell = CellFromPosition(cells, position);
        if (!cell)
        {
            Debug.LogWarning("ERROR!");
            return;
        }
        Vector3Int newPosition;
        Cell newCell;
        for (int i = 0; i < directions.Length; i++)
        {
            newPosition = position + GetVector3IntFromDirection(i);
            newCell = CellFromPosition(cells, newPosition);
            if (newCell != null)
            {
                ConnectCell(cell, newCell);
            }
        }
    }
    public static Vector3Int GetVector3IntFromDirection(int direction)
    {
        if (direction < directions.Length)
        {
            return directions[direction];
        }
        return Vector3Int.zero;
    }
    public static int GetDirectionFromVector3Int(Vector3Int deltaPositions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i] == deltaPositions)
            {
                return i;
            }
        }
        return directions.Length;
    }
    public static void RandomlyPlaceObject()
    {

    }
    public static void ClearCellScripts(Cell[,,] cells)
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int z = 0; z < cells.GetLength(2); z++)
                {
                    if (cells[x, y, z])
                    {
                        if (cells[x, y, z].Null)
                        {
                            Object.DestroyImmediate(cells[x, y, z].gameObject);
                        }
                        else
                        {
                            Object.DestroyImmediate(cells[x, y, z]);
                        }
                    }
                }
            }
        }
    }
}
#endif