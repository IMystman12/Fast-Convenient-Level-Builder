#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class BuilderWindow : EditorWindow
{
    public static Vector3Int buildPosition => BuilderHelper.position;
    protected static Vector3Int size => BuilderRenderer.Instance.size;
    protected static bool useSeed;
    protected static int seed, previousSeed;
    protected static System.Random random;
    protected virtual void OnGUI()
    {
        InitializeRenderer();
        Vector3Int v = EditorGUILayout.Vector3IntField("Position:", buildPosition);
        if (v != buildPosition)
        {
            BuilderRenderer.Instance.transform.position = v * 10;
        }
        BuilderRenderer.Instance.size = EditorGUILayout.Vector3IntField("Size:", size);
        useSeed = GUILayout.Toggle(useSeed, "Use Seed:");
        if (useSeed)
        {
            previousSeed = EditorGUILayout.IntField("Seed:", seed);
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
    }
    private static void InitializeRenderer()
    {
        if (!BuilderRenderer.Instance)
        {
            new GameObject("Builder Renderer").AddComponent<BuilderRenderer>();
        }
    }
    protected virtual void InitializeWindow<T>() where T : EditorWindow
    {
        GetWindow<T>().Show();
        BuilderRenderer.Instance.SetMartixMode(false);
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
    protected Transform CreateParentFromPosition(string name)
    {
        Transform t = new GameObject(name).transform;
        t.position = buildPosition * 10;
        return t;
    }
}
public class CellBuilder : BuilderWindow
{
    public static Material ceiling;
    public static Material wall;
    public static Material floor;
    protected override void InitializeWindow<T>()
    {
        base.InitializeWindow<T>();
        ceiling = BuilderHelper.ceiling;
        wall = BuilderHelper.wall;
        floor = BuilderHelper.floor;
    }
    protected override void VirtualVariablesOnGUI()
    {
        ceiling = EditorGUILayout.ObjectField("Ceiling:", ceiling, typeof(Material), false) as Material;
        wall = EditorGUILayout.ObjectField("Wall:", wall, typeof(Material), false) as Material;
        floor = EditorGUILayout.ObjectField("Floor:", floor, typeof(Material), false) as Material;
    }
}
#endif