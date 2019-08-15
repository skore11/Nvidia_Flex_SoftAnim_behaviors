using UnityEngine;
using System.Collections;

namespace uFlex
{
    public class FlexSkinnedMesh : MonoBehaviour
    {

        [HideInInspector]
        public BoneWeight[] m_boneWeights;

        [HideInInspector]
        public Matrix4x4[] m_bindPoses;

        [HideInInspector]
        public Transform[] m_bones;

        private FlexShapeMatching m_shapes;
        
        // Use this for initialization
        void Start()
        {

            m_shapes = GetComponent<FlexShapeMatching>();
         
        }

        // Update is called once per frame
        void Update()
        {

            for (int i = 0; i < m_shapes.m_shapesCount; i++)
            {
                m_bones[i].localPosition = transform.InverseTransformPoint(m_shapes.m_shapeTranslations[i]);
                m_bones[i].localRotation = Quaternion.Inverse(transform.rotation) * m_shapes.m_shapeRotations[i];
            }


        }
    }
}