#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class RoomBuilder : CellBuilder
{
    [MenuItem("Window/Level Builders/Rooms/Base")]
    private static void ShowWindow()
    {
        GetWindow<RoomBuilder>().InitializeWindow<RoomBuilder>();
    }
    protected override void Build()
    {
        Transform transform = CreateParentFromPosition("Room");
        Cell[,,] cells = new Cell[size.x, size.y, size.z];
        Vector3Int position;
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                for (int z = 0; z < cells.GetLength(2); z++)
                {
                    position = new Vector3Int(x, y, z);
                    cells[x, y, z] = BuilderHelper.CreateCell(position, transform);
                    BuilderHelper.ConnectAround(cells, position);
                }
            }
        }
        BuilderHelper.ClearCellScripts(cells);
    }
}
#endif