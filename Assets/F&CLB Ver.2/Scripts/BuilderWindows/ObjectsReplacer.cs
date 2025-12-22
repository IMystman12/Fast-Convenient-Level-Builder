using UnityEditor;
using UnityEngine;

public class ObjectsReplacer : BuilderWindow
{
    [SerializeField]
    protected GameObject[] objects;
    protected bool snap;
    [MenuItem("Window/Level Builders/Object Re-placer")]
    private static void ShowWindow()
    {
        GetWindow<ObjectsReplacer>().InitializeWindow<ObjectsReplacer>();
    }
    protected override void OnGUI()
    {
        if (objects == null)
        {
            objects = new GameObject[]{
        Resources.Load<GameObject>("BigDesk_Mesh"),
        Resources.Load<GameObject >("Table_Test_Mesh"),
        Resources.Load<GameObject >("Chair_Test_Mesh")
            };
        }
        SerializedObject so = new SerializedObject(this);
        EditorGUILayout.PropertyField(so.FindProperty("objects"));
        so.ApplyModifiedProperties();
        snap = EditorGUILayout.Toggle("Snap!", snap);
        base.OnGUI();
    }
    protected override void Build()
    {
        foreach (var item in objects)
        {
            RandomPlace(item, BuilderHelper.GetGridPosition(item.transform.position).y);
        }
    }
    protected void RandomPlace(GameObject itm, int height)
    {
        Vector3 position = ((Vector3)buildPosition * 10f) + new Vector3(UnityEngine.Random.Range(0, size.x * 10), height * 10, UnityEngine.Random.Range(0, size.z * 10));
        position = snap ? BuilderHelper.GetGridPosition(position) * 10 : position;
        itm.transform.position = position;
    }
}
