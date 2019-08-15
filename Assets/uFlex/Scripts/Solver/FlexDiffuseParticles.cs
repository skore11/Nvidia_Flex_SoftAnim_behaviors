using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace uFlex
{
    public class FlexDiffuseParticles : MonoBehaviour
    {
        public int m_diffuseParticlesCount = 0;

        public int m_maxDiffuseParticlesCount = 0;

        [HideInInspector]
        public Vector4[] m_diffuseParticles;

        [HideInInspector]
        public Vector4[] m_diffuseVelocities;

        [HideInInspector]
        public int[] m_sortedDepth;

        /*
        /// <summary>
        /// Particles with kinetic energy + divergence above this threshold will spawn new diffuse particles.
        /// </summary>
        [Tooltip("Particles with kinetic energy + divergence above this threshold will spawn new diffuse particles.")]
        public float m_DiffuseThreshold = 100.0f;

        /// <summary>
        /// Scales force opposing gravity that diffuse particles receive.
        /// </summary>
        [Tooltip("Scales force opposing gravity that diffuse particles receive.")]
        public float m_DiffuseBuoyancy = 0.0f;

        /// <summary>
        /// Scales force diffuse particles receive in direction of neighbor fluid particles.
        /// </summary>
        [Tooltip("Scales force diffuse particles receive in direction of neighbor fluid particles.")]
        public float m_DiffuseDrag = 0.0f;

        /// <summary>
        /// The number of neighbors below which a diffuse particle is considered ballistic.
        /// </summary>
        [Tooltip("The number of neighbors below which a diffuse particle is considered ballistic.")]
        public int m_DiffuseBallistic = 16;

        /// <summary>
        /// Diffuse particles will be sorted by depth along this axis if non-zero.
        /// </summary>
        [Tooltip("Diffuse particles will be sorted by depth along this axis if non-zero.")]
        public Vector3 m_DiffuseSortAxis = new Vector3(0, 0, 0);

        /// <summary>
        /// Time in seconds that a diffuse particle will live for after being spawned, particles will be spawned with a random lifetime in the range [0, mDiffuseLifetime].
        /// </summary>
        [Tooltip("Time in seconds that a diffuse particle will live for after being spawned, particles will be spawned with a random lifetime in the range [0, mDiffuseLifetime].")]
        public float m_DiffuseLifetime = 2.0f;
        */
        
        public GCHandle m_diffuseParticlesHndl;
        public GCHandle m_diffuseVelocitiesHndl;
        public GCHandle m_sortedDepthHndl;

        void Awake()
        {
            m_diffuseParticles = new Vector4[m_maxDiffuseParticlesCount];
            m_diffuseVelocities = new Vector4[m_maxDiffuseParticlesCount];
            m_sortedDepth = new int[m_maxDiffuseParticlesCount];

            m_diffuseParticlesHndl = GCHandle.Alloc(m_diffuseParticles, GCHandleType.Pinned);
            m_diffuseVelocitiesHndl = GCHandle.Alloc(m_diffuseVelocities, GCHandleType.Pinned);
            m_sortedDepthHndl = GCHandle.Alloc(m_sortedDepth, GCHandleType.Pinned);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        void OnApplicationQuit()
        {
            //free pinned arrays
            m_diffuseParticlesHndl.Free();
            m_diffuseVelocitiesHndl.Free();
            m_sortedDepthHndl.Free();
        }
    }

}