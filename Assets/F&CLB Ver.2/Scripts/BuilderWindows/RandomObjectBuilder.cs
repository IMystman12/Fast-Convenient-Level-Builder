using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomObjectBuilder : ObjectsReplacer
{
    int rotationLerps = 8, min = 5, max = 10;
    bool snapToGrid;
    [MenuItem("Window/Level Builders/Random Object")]
    protected override void VirtualVariablesOnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        EditorGUILayout.PropertyField(so.FindProperty("objects"));
        so.ApplyModifiedProperties();
        rotationLerps = EditorGUILayout.IntField("Rotation Lerps:", rotationLerps);
        min = EditorGUILayout.IntField("Min Spawns:", min);
        max = EditorGUILayout.IntField("Max Spawns:", max);
        snapToGrid = EditorGUILayout.Toggle("Snap Grid:", snapToGrid);
        EditorGUILayout.HelpBox("the spawn for every level not totaly![PLACEHOLDER]", MessageType.Info);
    }
    protected override void Build()
    {
        Transform t;
        for (int i = 0; i < size.y; i++)
        {
            t = new GameObject("Objects").transform;
            int c = UnityEngine.Random.Range(min, max);
            for (int j = 0; j < c; j++)
            {
                RandomPlace(Instantiate(objects[UnityEngine.Random.Range(0, objects.Length)], buildPosition * 10, Quaternion.Euler(0, (360 / rotationLerps) * UnityEngine.Random.Range(0, rotationLerps), 0), t), i);
            }
        }
    }
}
