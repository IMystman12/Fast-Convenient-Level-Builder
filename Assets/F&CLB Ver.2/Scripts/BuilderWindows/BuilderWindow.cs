#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class BuilderWindow : EditorWindow
{
    public static Vector3Int buildPosition => BuilderHelper.position;
    protected static Vector3Int size => BuilderRenderer.Instance.size;
    protected Vector2 scroll;
    public static bool navigationSurface = true;
    protected static bool useSeed;
    protected static int seed, previousSeed;
    protected static System.Random random;
    protected virtual void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        Vector3Int v = EditorGUILayout.Vector3IntField("Position:", buildPosition);
        if (v != buildPosition)
        {
            BuilderRenderer.Instance.transform.position = v * 10;
        }
        BuilderRenderer.Instance.size = EditorGUILayout.Vector3IntField("Size:", size);
        navigationSurface = GUILayout.Toggle(navigationSurface, "Navigation Surface");
        useSeed = GUILayout.Toggle(useSeed, "Use Seed");
        if (useSeed)
        {
            previousSeed = EditorGUILayout.IntField("Seed", seed);
            if (random == null || previousSeed != seed || GUILayout.Button("New Random!"))
            {
                random = new System.Random(seed);
            }
            seed = previousSeed;
        }
        VirtualVariablesOnGUI();
        if (GUILayout.Button("Build!"))
        {
            Build();
        }
        EditorGUILayout.EndScrollView();
    }
    protected virtual void InitializeWindow<T>() where T : EditorWindow
    {
        GetWindow<T>().Show();
    }
    protected virtual void VirtualVariablesOnGUI()
    {

    }
    protected virtual void Build()
    {

    }
    public static int Random(int minInclude, int maxExclude)
    {
        if (useSeed)
        {
            return random.Next(minInclude, maxExclude);
        }
        return UnityEngine.Random.Range(minInclude, maxExclude);
    }
    protected Transform CreateParentFromPosition(string name, bool undo)
    {
        Transform t = new GameObject(name).transform;
        t.position = buildPosition * 10;
        if (undo)
        {
            Undo.RegisterCreatedObjectUndo(t.gameObject, $"Build {name}");
        }
        return t;
    }
}
public class CellBuilder : BuilderWindow
{
    public static CellBuilder Instance { get; protected set; }
    public Material ceiling;
    public Material wall;
    public Material floor;
    void Awake()
    {
        ceiling = BuilderHelper.ceiling;
        wall = BuilderHelper.wall;
        floor = BuilderHelper.floor;
    }
    void OnFocus() => Instance = this;
    protected override void VirtualVariablesOnGUI()
    {
        ceiling = EditorGUILayout.ObjectField("Celing:", ceiling, typeof(Material), false) as Material;
        wall = EditorGUILayout.ObjectField("Wall:", wall, typeof(Material), false) as Material;
        floor = EditorGUILayout.ObjectField("Floor:", floor, typeof(Material), false) as Material;
    }
    protected void SizeCheck(int val)
    {
        for (int i = 0; i < 3; i++)
        {
            if (size[i] < val)
            {
                BuilderRenderer.Instance.size = new Vector3Int(Mathf.Max(size.x, val), Mathf.Max(size.y, val), Mathf.Max(size.z, val));
                break;
            }
        }
    }
}
#endif