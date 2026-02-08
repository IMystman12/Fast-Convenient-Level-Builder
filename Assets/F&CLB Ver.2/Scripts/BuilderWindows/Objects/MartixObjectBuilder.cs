#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class MartixObjectBuilder : BuilderWindow
{
    [Serializable]
    public class ObjectVolumn
    {
        public GameObject gameObject;
        public Vector3 offset => renderer ? renderer.bounds.center + renderer.transform.position : Vector3.zero;
        public Vector3 volumn => renderer ? renderer.bounds.size : Vector3.one * 10;
        Renderer renderer
        {
            get
            {
                gameObject.transform.position = Vector3.zero;
                Renderer renderer = gameObject.GetComponent<Renderer>();
                if (!renderer)
                {
                    renderer = gameObject.GetComponentInChildren<Renderer>();
                }
                return renderer;
            }
        }
        public int weight = 100;
    }
    [SerializeField] private ObjectVolumn[] objects;
    public static ObjectVolumn[,,] objectVolumn;
    public static Vector3 margin = Vector3.one * 10;
    bool flag;
    public Vector3 rotation;
    [MenuItem("Window/Level Builders/Objects/Martix")]
    private static void ShowWindow()
    {
        GetWindow<MartixObjectBuilder>().InitializeWindow<MartixObjectBuilder>();
    }
    protected override void InitializeWindow<T>()
    {
        base.InitializeWindow<T>();
        Regenerate();
    }
    protected override void VirtualVariablesOnGUI()
    {
        CheckList();
        SerializedObject so = new SerializedObject(this);
        flag = EditorGUILayout.PropertyField(so.FindProperty("objects"));
        so.ApplyModifiedProperties();
        rotation = EditorGUILayout.Vector3Field("Rotation:", rotation);
        margin = EditorGUILayout.Vector3Field("Margin:", margin);
        if (GUILayout.Button("Regenerate!") || flag)
        {
            Regenerate();
        }
    }
    void CheckList()
    {
        if (objects == null || objects.Length == 0)
        {
            objects = new ObjectVolumn[]{
                new ObjectVolumn()
                {
                     gameObject= Resources.Load<GameObject>("BigDesk_Mesh"),
                },
                new ObjectVolumn()
                {
                     gameObject=  Resources.Load<GameObject >("Table_Test_Mesh"),
                },
                new ObjectVolumn()
                {
                     gameObject=     Resources.Load<GameObject >("Chair_Test_Mesh"),
                },
            };
        }
    }
    public void Regenerate()
    {
        CheckList();
        int num = 0, num0;
        for (int i = 0; i < objects.Length; i++)
        {
            num += objects[i].weight;
        }
        objectVolumn = new ObjectVolumn[size.x, size.y, size.z];
        for (int a = 0; a < size.x; a++)
        {
            for (int b = 0; b < size.y; b++)
            {
                for (int c = 0; c < size.z; c++)
                {
                    num0 = Random(0, num);
                    for (int i = 0; objectVolumn[a, b, c] == null; i++)
                    {
                        if (i == objects.Length - 1 || objects[i].weight > num0)
                        {
                            objectVolumn[a, b, c] = objects[i];
                        }
                        num0 -= objects[i].weight;
                    }
                }
            }
        }
    }
    protected override void Build()
    {
        Transform roomBase = CreateParentFromPosition("Objects");
        Quaternion quaternion = Quaternion.Euler(rotation);
        for (int a = 0; a < size.x; a++)
        {
            for (int b = 0; b < size.y; b++)
            {
                for (int c = 0; c < size.z; c++)
                {
                    Instantiate(objectVolumn[a, b, c].gameObject, BuilderHelper.position * 10 + Vector3.Scale(new Vector3(a, b, c), margin), quaternion, roomBase);
                }
            }
        }
    }
}
#endif