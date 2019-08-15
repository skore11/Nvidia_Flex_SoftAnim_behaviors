using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class VertexWeight
{
    public int index;
    public Vector3 localPosition;
    public float weight;

    public VertexWeight()
    {

    }

    public VertexWeight(int i, Vector3 p, float w)
    {
        index = i;
        localPosition = p;
        weight = w;
    }
}

[System.Serializable]
public class WeightList
{
    private Transform _temp; // cached on use, not serialized
    public Transform transform
    {
        get
        {
            if (_temp == null)
            {
                _temp = new GameObject().transform;
                _temp.position = pos;
                _temp.rotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);
                _temp.localScale = scale;
            }
            return _temp;
        }
        set
        {
            pos = value.position;
            rot = new Vector4(value.rotation.x, value.rotation.y, value.rotation.z, value.rotation.w);
            scale = value.localScale;
        }
    }
    public int boneIndex; // for transform
    public Vector3 pos;
    public Vector4 rot;
    public Vector3 scale;

    public List<VertexWeight> weights = new List<VertexWeight>();
}

[System.Serializable]
public class ShapeIndex
{
    public int shapeStart;
    public int shapeMid;
    public int shapeEnd;
    public bool valid; // false if particle count < 3
}

public class VertMapAsset : ScriptableObject
{
    // index = soft body particle index. Value = vertex index.
    public List<int> vertexParticleMap;
    public List<Vector3> particleRestPositions;
    public List<int> nearestVertIndex;
    public List<int> uniqueIndex;
    public WeightList[] particleNodeWeights; // one per node (vert). Weights of standard mesh
    public List<ShapeIndex> shapeIndex;

   
}
