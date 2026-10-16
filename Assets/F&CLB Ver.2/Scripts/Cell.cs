#if UNITY_EDITOR
using UnityEngine;

public class Cell : MonoBehaviour
{
    public MeshRenderer ceiling;
    public MeshRenderer[] walls = new MeshRenderer[4];
    public MeshRenderer floor;
    public MeshRenderer _navMesh;
    public Vector3Int localPosition => BuilderHelper.GetGridPosition(transform.localPosition);
    public bool Null
    {
        get
        {
            bool flag = false;
            for (int i = 0; i < walls.Length; i++)
            {
                if (walls[i] != null && !flag)
                {
                    flag = true;
                }
            }
            return !(floor || ceiling || flag);
        }
    }
    public void Initialize()
    {
        ceiling.material = CellBuilder.Instance.ceiling;
        if (CellBuilder.Instance.ceiling == null)
        {
            DestroyImmediate(ceiling.gameObject);
        }
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].material = CellBuilder.Instance.wall;
            if (CellBuilder.Instance.wall == null)
            {
                DestroyImmediate(walls[i].gameObject);
            }
        }
        floor.material = CellBuilder.Instance.floor;
        if (CellBuilder.Instance.floor == null)
        {
            DestroyImmediate(floor.gameObject);
        }
        if (!CellBuilder.navigationSurface && _navMesh)
        {
            DestroyImmediate(_navMesh.gameObject);
        }
    }
    public void DestroyWall(int dir)
    {
        Vector3 vector = _navMesh ? _navMesh.transform.localPosition : Vector3.zero;
        MeshRenderer _destroy = null;
        switch (dir)
        {
            case 0:
                _destroy = walls[0];
                if (_navMesh)
                {
                    vector.z = 2;
                    _navMesh.transform.localScale += Vector3.up * 4;
                }
                break;
            case 1:
                _destroy = ceiling;
                break;
            case 2:
                _destroy = walls[1];
                if (_navMesh)
                {
                    vector.x = 2;
                    _navMesh.transform.localScale += Vector3.left * -4;
                }
                break;
            case 3:
                _destroy = walls[2];
                if (_navMesh)
                {
                    vector.z = -2;
                    _navMesh.transform.localScale += Vector3.up * 4;
                }
                break;
            case 4:
                _destroy = floor;
                if (_navMesh)
                {
                    DestroyImmediate(_navMesh.gameObject);
                }
                break;
            case 5:
                _destroy = walls[3];
                if (_navMesh)
                {
                    vector.x = -2;
                    _navMesh.transform.localScale += Vector3.left * -4;
                }
                break;
        }
        if (_destroy != null)
        {
            DestroyImmediate(_destroy.gameObject);
            if (_navMesh)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (_navMesh.transform.localScale[i] == 10)
                    {
                        vector[i == 1 ? 2 : 0] = 0;
                    }
                }
                _navMesh.transform.localPosition = vector;
            }
        }
    }
}
#endif