using UnityEngine;
using System.Collections;

namespace uFlex
{

    public class FlexInflatable : MonoBehaviour
    {

        public int m_inflatableId = -1;
        /// <summary>
        /// offset into the solver's dynamic triangles for each inflatable
        /// </summary>
        public int m_trisOffset = -1;
        
        /// <summary>
        /// triangle counts for each inflatable
        /// </summary>
        public int m_trisCount = -1;

        /// <summary>
        /// Rest volume for the inflatables
        /// </summary>
        public float m_restVolume;

        /// <summary>
        /// Pressure, a value of 1.0 means the rest volume, larger than 1.0 means over-inflated, and smaller than 1.0 means under-inflated
        /// </summary>
        public float m_pressure = 1.0f;

        /// <summary>
        /// Scaling factors for the constraint, this is roughly equivalent to stiffness but includes a constraint scaling factor from position-based dynamics, see helper code for details,
        /// </summary>
        public float m_stiffness = 1.0f;

        public int m_inflatableIndex = -1;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}