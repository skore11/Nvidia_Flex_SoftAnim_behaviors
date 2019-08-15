using UnityEngine;
using System;
using System.Collections.Generic;

namespace uFlex
{
    /// <summary>
    /// An example of Flex solver driving the simulation
    /// </summary>
    [RequireComponent(typeof(FlexContainer), typeof(FlexParameters))]
    public class FlexSolver : MonoBehaviour
    {

        /// <summary>
        /// Unity's FixedTimeStep multiplier for FlexSolver
        /// </summary>
        [Tooltip("Unity's FixedTimeStep multiplier for FlexSolver")]
        public float m_fixedTimeStepMult = 1.0f;

        /// <summary>
        /// Maximum number of simulation particles possible for this solver
        /// </summary>
        //[Tooltip("Maximum number of simulation particles possible for this solver")]
        //public int m_maxParticlesCount = 1024;

        /// <summary>
        /// Maximum number of diffuse (non-simulation) particles possible for this solver
        /// </summary>
        //[Tooltip("Maximum number of diffuse (non-simulation) particles possible for this solver")]
        //[HideInInspector]
        //public int m_maxDiffuseParticlesCount = 0;

        /// <summary>
        /// Maximum number of neighbors per particle possible for this solver
        /// </summary>
        [Tooltip("Maximum number of neighbors per particle possible for this solver")]
        public byte m_maxNeighboursCount = 128;

        /// <summary>
        /// The time dt will be divided into the number of sub-steps given by this parameter
        /// </summary>
        [Tooltip("The time dt will be divided into the number of sub-steps given by this parameter")]
        public int m_solverSubSteps = 1;

        /// <summary>
        /// Check if you want to make runtime modifications (eg. emitters, adding/removing flex objects, changing parameters, moving colliders etc) to the container at the cost of performance
        /// (you can still manipulate particles postions and/or velocities without this checked)
        /// </summary>
        [Tooltip("Check if you want to make runtime modifications (eg. emitters, adding/removing flex objects, changing parameters, moving colliders etc) to the container at the cost of performance ")]
        public bool m_dynamic = true;

        /// <summary>
        /// A pointer to native structure
        /// </summary>
        [HideInInspector]
        public IntPtr m_solverPtr;

        [HideInInspector]
        public FlexContainer m_cntr;

        [HideInInspector]
        public FlexDiffuseParticles m_diffuseParticles;

        [HideInInspector]
        public FlexColliders m_colliders;

        [HideInInspector]
        public FlexParameters m_parameters;

        [HideInInspector]
        protected Flex.Params m_params;

        protected Flex.ErrorCallback m_errorCallback;

        protected Flex.Timers m_timers;

        protected Vector3 m_minBounds;
        protected Vector3 m_maxBounds;


        protected FlexProcessor[] m_processors;

        void Start()
        {

            if (m_cntr == null)
                m_cntr = GetComponent<FlexContainer>();

            if (m_diffuseParticles == null)
                m_diffuseParticles = GetComponent<FlexDiffuseParticles>();

            if (m_parameters == null)
                m_parameters = GetComponent<FlexParameters>();

            if (m_colliders == null)
                m_colliders = GetComponent<FlexColliders>();

            m_errorCallback = new Flex.ErrorCallback(this.ErrorCallback);

            m_timers = new Flex.Timers();

            Flex.Error flexErr = Flex.Init(100, m_errorCallback, -1);

            Debug.Log("NVidia FleX v" + Flex.GetVersion());
            if (flexErr != Flex.Error.eFlexErrorNone)
                Debug.LogError("FlexInit: "+flexErr);

            if(m_diffuseParticles)
                m_solverPtr = Flex.CreateSolver(m_cntr.m_maxParticlesCount, m_diffuseParticles.m_maxDiffuseParticlesCount, m_maxNeighboursCount);
            else
                m_solverPtr = Flex.CreateSolver(m_cntr.m_maxParticlesCount, 0, m_maxNeighboursCount);

            m_parameters.GetParams(ref m_params);
            Flex.SetParams(m_solverPtr, ref m_params);

            m_cntr.UpdateContainer();

            m_processors = FindObjectsOfType<FlexProcessor>();
            foreach (FlexProcessor fp in m_processors)
            {
                fp.FlexStart(this, m_cntr, m_parameters);
            }

            PushParticlesToGPU(m_solverPtr, m_cntr, Flex.Memory.eFlexMemoryHost);

            PushConstraintsToGPU(m_solverPtr, m_cntr, Flex.Memory.eFlexMemoryHost);

            if (m_colliders)
                m_colliders.ProcessColliders(m_solverPtr, Flex.Memory.eFlexMemoryHost);

            //Flex.SetShapes(m_solverPtr, m_cnt.shapeGeometry, shapeGeometry.Length, shapeAabbMin, shapeAabbMax, shapeStarts, shapePositions, shapeRotations,
            //          shapePrevPositions, shapePrevRotations, shapeFlags, shapeStarts.Length, Flex.Memory.eFlexMemoryHost);


  
        }



        void Update()
        {

            //get the data from the GPU
            PullParticlesFromGPU(m_solverPtr, m_cntr, Flex.Memory.eFlexMemoryHostAsync);

            if(m_diffuseParticles && m_diffuseParticles.m_maxDiffuseParticlesCount > 0)
                m_diffuseParticles.m_diffuseParticlesCount = Flex.GetDiffuseParticles(m_solverPtr, m_diffuseParticles.m_diffuseParticlesHndl.AddrOfPinnedObject(), m_diffuseParticles.m_diffuseVelocitiesHndl.AddrOfPinnedObject(), m_diffuseParticles.m_sortedDepthHndl.AddrOfPinnedObject(), Flex.Memory.eFlexMemoryHostAsync);

            //update solver parameters
            //TODO check impact on performance
            m_parameters.GetParams(ref m_params);
            Flex.SetParams(m_solverPtr, ref m_params);

            //ensure memory transfers have finished
            Flex.SetFence();
            Flex.WaitFence();

            if(Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("uFlex Scene Reset");
                for (int i = 0; i < m_cntr.m_particlesCount; i++)
                {
                    m_cntr.m_particles[i] = m_cntr.m_restParticles[i];
                    m_cntr.m_velocities[i] = new Vector3();
                }
            }

            //copy data to instances
            UpdateGameObjects();

            //do the custom processing on flex gameobjects
            m_processors = FindObjectsOfType<FlexProcessor>();
            foreach (FlexProcessor fp in m_processors)
            {
                fp.PreContainerUpdate(this, m_cntr, m_parameters);
            }

            //update container
            if(m_dynamic)
                m_cntr.UpdateContainer();

            //do the custom processing on the container
            foreach (FlexProcessor fp in m_processors)
            {
                fp.PostContainerUpdate(this, m_cntr, m_parameters);
            }

            //copy the processed data back to the GPU
            PushParticlesToGPU(m_solverPtr, m_cntr, Flex.Memory.eFlexMemoryHostAsync);

         
            if (m_dynamic)
            {
                //Async transfers on unpinned arrays!?
                //PushConstraintsToGPU(m_solverPtr, m_cntr, Flex.Memory.eFlexMemoryHostAsync);
                //m_colliders.UpdateColliders(m_solverPtr, Flex.Memory.eFlexMemoryHostAsync);

                PushConstraintsToGPU(m_solverPtr, m_cntr, Flex.Memory.eFlexMemoryHost);
                m_colliders.UpdateColliders(m_solverPtr, Flex.Memory.eFlexMemoryHost);
            }

            //do not wait here for memory transfers to finish
            //Flex.SetFence();
            //Flex.WaitFence();


            //Debug.Log("Total Time: "+m_timers.mTotal);
        }

        void FixedUpdate()
        {
            Flex.UpdateSolver(m_solverPtr, Time.fixedDeltaTime * m_fixedTimeStepMult, m_solverSubSteps, IntPtr.Zero);
        }

        private void PushConstraintsToGPU(IntPtr solverPtr, FlexContainer cnt, Flex.Memory memory)
        {

            if (cnt.m_springsCount > 0)
                Flex.SetSprings(solverPtr, cnt.m_springIndices, cnt.m_springRestLengths, cnt.m_springCoefficients, cnt.m_springsCount, memory);
            else
                Flex.SetSprings(solverPtr, null, null, null, 0, memory);

            if (cnt.m_shapesCount > 0)
                Flex.SetRigids(solverPtr, cnt.m_shapeOffsets, cnt.m_shapeIndices, cnt.m_shapeRestPositions, null, cnt.m_shapeCoefficients, cnt.m_shapeRotations, cnt.m_shapeTranslations, cnt.m_shapeOffsets.Length - 1, memory);
            else
                Flex.SetRigids(solverPtr, null, null, null, null, null, null, null, 0, memory);

            if (cnt.m_trianglesCount > 0)
                Flex.SetDynamicTriangles(solverPtr, cnt.m_triangleIndices, cnt.m_triangleNormals, cnt.m_trianglesCount, memory);
            else
                Flex.SetDynamicTriangles(solverPtr, null, null, 0, memory);

            if (cnt.m_inflatablesCount > 0)
                Flex.SetInflatables(solverPtr, cnt.m_inflatableStarts, cnt.m_inflatableCounts, cnt.m_inflatableVolumes, cnt.m_inflatablePressures, cnt.m_inflatableStiffness, cnt.m_inflatablesCount, memory);
            else
                Flex.SetInflatables(solverPtr, null, null, null, null, null, 0, memory);

        }

        private void PushParticlesToGPU(IntPtr solverPtr, FlexContainer cnt, Flex.Memory memory)
        {

            Flex.SetActive(solverPtr, cnt.m_activeSetHndl.AddrOfPinnedObject(), cnt.m_activeParticlesCount, memory);

            Flex.SetParticles(solverPtr, cnt.m_particlesHndl.AddrOfPinnedObject(), cnt.m_particlesCount, memory);
            Flex.SetRestParticles(solverPtr, cnt.m_restParticlesHndl.AddrOfPinnedObject(), cnt.m_particlesCount, memory);
            Flex.SetVelocities(solverPtr, cnt.m_velocitiesHndl.AddrOfPinnedObject(), cnt.m_particlesCount, memory);
            Flex.SetNormals(solverPtr, cnt.m_normalsHndl.AddrOfPinnedObject(), cnt.m_particlesCount, memory);
            Flex.SetPhases(solverPtr, cnt.m_phases, cnt.m_particlesCount, memory);

        }

        private void PullParticlesFromGPU(IntPtr solverPtr, FlexContainer cnt, Flex.Memory memory)
        {

            Flex.GetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, memory);
            Flex.GetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, memory);
            Flex.GetPhases(m_solverPtr, m_cntr.m_phasesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, memory);
            
            Flex.GetSmoothParticles(m_solverPtr, m_cntr.m_smoothedParticlesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, memory);
            Flex.GetNormals(m_solverPtr, m_cntr.m_normalsHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, memory);
            Flex.GetDensities(m_solverPtr, m_cntr.m_densitiesHndl.AddrOfPinnedObject(), memory);
            Flex.GetBounds(m_solverPtr, ref m_minBounds, ref m_maxBounds, memory);

            if (m_cntr.m_shapeCoefficients.Length > 0)
                Flex.GetRigidTransforms(m_solverPtr, m_cntr.m_shapeRotationHndl.AddrOfPinnedObject(), m_cntr.m_shapeTranslationsHndl.AddrOfPinnedObject(), memory);

        }



        private void UpdateGameObjects()
        {

            for (int iId = 0; iId < m_cntr.m_flexGameObjects.Count && iId < m_cntr.m_activeInstacesCount; iId++)
            {
                FlexParticles flexInstance = m_cntr.m_flexGameObjects[iId];
                if (flexInstance != null)
                {
                    Array.Copy(m_cntr.m_particles, flexInstance.m_particlesIndex, flexInstance.m_particles, 0, flexInstance.m_particlesCount);
                    Array.Copy(m_cntr.m_velocities, flexInstance.m_particlesIndex, flexInstance.m_velocities, 0, flexInstance.m_particlesCount);
                    Array.Copy(m_cntr.m_densities, flexInstance.m_particlesIndex, flexInstance.m_densities, 0, flexInstance.m_particlesCount);

                    FlexShapeMatching shapes = m_cntr.m_flexGameObjects[iId].GetComponent<FlexShapeMatching>();
                    if (shapes)
                    {
                        if (shapes.m_shapesIndex == -1)
                            continue;

                        Array.Copy(m_cntr.m_shapeTranslations, shapes.m_shapesIndex, shapes.m_shapeTranslations, 0, shapes.m_shapesCount);
                        Array.Copy(m_cntr.m_shapeRotations, shapes.m_shapesIndex, shapes.m_shapeRotations, 0, shapes.m_shapesCount);
                    }
                }
            }

        }

   
        public virtual void OnDrawGizmos()
        {
            Bounds b = new Bounds();
            b.SetMinMax(m_minBounds, m_maxBounds);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(b.center, b.size);


        }

        private void ErrorCallback(Flex.ErrorSeverity severity, String msg, String file, int line)
        {
            Debug.LogError(severity + ": " + msg + "\t[FILE: " + file + ": " + line + "]");
        }

        void OnApplicationQuit()
        {
            foreach (FlexProcessor fp in m_processors)
            {
                fp.FlexClose(this, m_cntr, m_parameters);
            }

            Flex.DestroySolver(m_solverPtr);
            Flex.Shutdown();
        }

    }

}