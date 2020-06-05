using UnityEngine;
using System.Collections;
using System;

namespace uFlex
{
    //[Serializable]
    //public struct FlexShape
    //{
    //    public int indicesCount;
    //    public int[] indices;
    //    public int offset;
    //    public float stiffness;
    //    public Vector3 centre;
    //}

    public class FlexShapeMatching : MonoBehaviour
    {
        public bool m_initialized = false;

        public int m_shapesCount = 0;                 //!< The number of shape matching constraints

        public int m_shapeIndicesCount = 0;           //!< Total number of indices for shape constraints	

        public int m_shapesIndex = -1;

        public int m_shapesIndicesIndex = -1;

     //   public FlexShape[] m_shapes;

        [HideInInspector]
        public int[] m_shapeIndices;             //!< The indices of the shape matching constraints

        [HideInInspector]
        public int[] m_shapeOffsets;             //!< Each entry stores the end of the shape's indices in the indices array (exclusive prefix sum of shape lengths)

        //[HideInInspector]
        public float[] m_shapeCoefficients;      //!< The stiffness coefficient for each shape

        [HideInInspector]
        public Vector3[] m_shapeCenters;           //!< The position of the center of mass of each shape, an array of vec3s mNumShapes in length

        [HideInInspector]
        public Vector3[] m_shapeTranslations;

        [HideInInspector]
        public Quaternion[] m_shapeRotations;

        [HideInInspector]
        public Vector3[] m_shapeRestPositions;


        // Use this for initialization
        void Start()
        {

        }

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}