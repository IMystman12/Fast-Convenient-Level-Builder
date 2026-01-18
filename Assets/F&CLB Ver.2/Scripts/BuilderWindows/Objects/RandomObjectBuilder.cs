#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class RandomObjectBuilder : ObjectsReplacer
{
    int rotationLerps = 8, min = 5, max = 10;
    [MenuItem("Window/Level Builders/Objects/Random Generator")]
    private static void ShowWindow()
    {
        GetWindow<RandomObjectBuilder>().InitializeWindow<RandomObjectBuilder>();
    }
    protected override void VirtualVariablesOnGUI()
    {
        rotationLerps = EditorGUILayout.IntField("Rotation Lerps:", rotationLerps);
        min = EditorGUILayout.IntField("Min Spawns:", min);
        max = EditorGUILayout.IntField("Max Spawns:", max);
        EditorGUILayout.HelpBox("the spawn for every level not totaly![PLACEHOLDER]", MessageType.Info);
    }
    protected override void Build()
    {
        Transform t, a = CreateParentFromPosition("Objects");
        for (int i = 0; i < size.y; i++)
        {
            t = CreateParentFromPosition("Objects");
            t.SetParent(a);
            int c = Random(min, max);
            for (int j = 0; j < c; j++)
            {
                RandomPlace(Instantiate(objects[Random(0, objects.Length)], buildPosition * 10, Quaternion.Euler(0, (360 / rotationLerps) * Random(0, rotationLerps), 0), t), i);
            }
        }
    }
}
#endif