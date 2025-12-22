using UnityEngine;

public class Cell : MonoBehaviour
{
    public MeshRenderer ceiling;
    public MeshRenderer[] walls = new MeshRenderer[4];
    public MeshRenderer floor;
    public MeshRenderer _NavMesh;
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
        ceiling.material = CellBuilder.ceiling;
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].material = CellBuilder.wall;
        }
        floor.material = CellBuilder.floor;
    }
    public void DestroyWall(int dir)
    {
        Vector3 vector = _NavMesh ? _NavMesh.transform.localPosition : Vector3.zero;
        MeshRenderer _destroy = null;
        switch (dir)
        {
            case 0:
                _destroy = walls[0];
                if (_NavMesh)
                {
                    vector.z = 2;
                    _NavMesh.transform.localScale += Vector3.up * 4;
                }
                break;
            case 1:
                _destroy = ceiling;
                break;
            case 2:
                _destroy = walls[1];
                if (_NavMesh)
                {
                    vector.x = 2;
                    _NavMesh.transform.localScale += Vector3.left * -4;
                }
                break;
            case 3:
                _destroy = walls[2];
                if (_NavMesh)
                {
                    vector.z = -2;
                    _NavMesh.transform.localScale += Vector3.up * 4;
                }
                break;
            case 4:
                _destroy = floor;
                if (_NavMesh)
                {
                    DestroyImmediate(_NavMesh.gameObject);
                }
                break;
            case 5:
                _destroy = walls[3];
                if (_NavMesh)
                {
                    vector.x = -2;
                    _NavMesh.transform.localScale += Vector3.left * -4;
                }
                break;
        }
        if (_destroy != null)
        {
            DestroyImmediate(_destroy.gameObject);
            if (_NavMesh)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (_NavMesh.transform.localScale[i] == 10)
                    {
                        vector[i == 1 ? 2 : 0] = 0;
                    }
                }
                _NavMesh.transform.localPosition = vector;
            }
        }
    }
}
