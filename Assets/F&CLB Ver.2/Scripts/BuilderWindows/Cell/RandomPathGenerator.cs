#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RandomPathGenerator : CellBuilder
{
    public float turnChance = 0.5f;
 //   [MenuItem("Window/Level Builders/Rooms/Random Path Generator")]
    private static void ShowWindow()
    {
        GetWindow<RandomPathGenerator>().InitializeWindow<RandomPathGenerator>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        turnChance = EditorGUILayout.FloatField("Turn Chance:", turnChance);
    }
    protected override void Build()
    {
        bool flag = true;
        Vector3Int start = Vector3Int.zero;
        Cell[,,] cells = new Cell[size.x, size.y, size.z];
        for (int i = 0; i < 3; i++)
        {
            start[i] = buildPosition[i] + Random(0, size[i]);
        }
        while (flag)
        {

        }
        BuilderHelper.ClearCellScripts(cells);
    }
}
#endif