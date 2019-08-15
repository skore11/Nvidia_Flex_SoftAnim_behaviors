using UnityEngine;
using System.Collections;

namespace uFlex
{
    /// <summary>
    /// Updates the transform of flex rigid bodies
    /// </summary>
    public class FlexRigidTransform : MonoBehaviour
    {
        private FlexShapeMatching m_shape;
     //   private FlexContainer m_container;
 
        // Use this for initialization
        void Start()
        {
      //      m_container = FindObjectOfType<FlexContainer>();
            m_shape = GetComponent<FlexShapeMatching>();
            if (m_shape.m_shapesCount > 1)
                Debug.Log("ShapesCount  > 1. Make sure that this Component is attached to Flex Rigid Bodies (i.e. with a single shape matching constraint)");
        }

        // Update is called once per frame
        void Update()
        {
            //transform.position = m_container.m_shapeTranslations[m_shape.m_shapesIndex] - m_container.m_shapeRotations[m_shape.m_shapesIndex] * m_shape.m_shapeCenters[0];
            //transform.rotation = m_container.m_shapeRotations[m_shape.m_shapesIndex];

            transform.position = m_shape.m_shapeTranslations[0] - m_shape.m_shapeRotations[0] * m_shape.m_shapeCenters[0];
            transform.rotation = m_shape.m_shapeRotations[0];


        }

        //public virtual void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.green;
        //    for (int i = 0; i < m_body.m_shapesCount; i++)
        //    {
        //     //   Gizmos.DrawSphere(m_body.m_shapeCenters[i], 1.0f);
        //    }
        //}
    }
}