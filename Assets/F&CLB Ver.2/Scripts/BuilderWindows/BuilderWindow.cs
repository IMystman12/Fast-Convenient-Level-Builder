using UnityEditor;
using UnityEngine;
public class BuilderWindow : EditorWindow
{
    public static Vector3Int buildPosition => BuilderHelper.position;
    protected static Vector3Int size => BuilderRenderer.Instance.size;
    protected virtual void OnGUI()
    {
        InitializeRenderer();
        Vector3Int v = EditorGUILayout.Vector3IntField("Position:", buildPosition);
        if (v != buildPosition)
        {
            BuilderRenderer.Instance.transform.position = v * 10;
        }
        BuilderRenderer.Instance.size = EditorGUILayout.Vector3IntField("Size:", size);
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
    }
    protected virtual void VirtualVariablesOnGUI()
    {

    }
    protected virtual void Build()
    {

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
