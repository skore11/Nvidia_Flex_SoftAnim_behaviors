using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace uFlex
{
    /// <summary>
    /// 
    /// </summary>
    public class FlexContainer : MonoBehaviour
    {
        public List<FlexParticles> m_flexGameObjects = new List<FlexParticles>();

        public int m_maxParticlesCount = 1024;

        public int m_activeInstacesCount = 0;

        //public Vector3 m_lowerBounds = new Vector3(-100, 0, -100);

        //public Vector3 m_upperBounds = new Vector3(100, 100, 100);

        public int m_particlesCount = 0;
        public int m_activeParticlesCount = 0;
        public int m_springsCount = 0;
        public int m_shapesCount = 0;
        public int m_shapeIndicesCount = 0;
        public int m_trianglesCount = 0;
        public int m_inflatablesCount = 0;

        [HideInInspector]
        public Particle[] m_smoothedParticles;

        [HideInInspector]
        public Particle[] m_particles;

        [HideInInspector]
        public Particle[] m_restParticles;

        [HideInInspector]
        public Vector3[] m_velocities;

        [HideInInspector]
        public Vector4[] m_normals;

        [HideInInspector]
        public Color[] m_colors;

        [HideInInspector]
        public int[] m_phases;

        [HideInInspector]
        public float[] m_densities;

        [HideInInspector]
        public int[] m_activeSet;

        [HideInInspector]
        public bool[] m_particlesActivity;

        [HideInInspector]
        public int[] m_springIndices;

        [HideInInspector]
        public float[] m_springRestLengths;

        [HideInInspector]
        public float[] m_springCoefficients;


        [HideInInspector]
        public int[] m_shapeIndices;

        [HideInInspector]
        public int[] m_shapeOffsets;

        [HideInInspector]
        public Vector3[] m_shapeCenters;

        [HideInInspector]
        public Vector3[] m_shapeRestPositions;

        [HideInInspector]
        public float[] m_shapeCoefficients;

        [HideInInspector]
        public Vector3[] m_shapeTranslations;

        [HideInInspector]
        public Vector3[] m_shapeTranslationsShift;

        [HideInInspector]
        public Quaternion[] m_shapeRotations;


        //Handles to pinned arrays
        public GCHandle m_smoothedParticlesHndl;
        public GCHandle m_particlesHndl;
        public GCHandle m_restParticlesHndl;
        public GCHandle m_velocitiesHndl;
        public GCHandle m_phasesHndl;
        public GCHandle m_densitiesHndl;
        public GCHandle m_normalsHndl;
        public GCHandle m_activeSetHndl;
        public GCHandle m_shapeTranslationsHndl;
        public GCHandle m_shapeRotationHndl;

        [HideInInspector]
        public int[] m_triangleIndices;

        [HideInInspector]
        public Vector3[] m_triangleNormals;

        [HideInInspector]
        public int[] m_inflatableStarts;

        [HideInInspector]
        public int[] m_inflatableCounts;

        [HideInInspector]
        public float[] m_inflatableVolumes;

        [HideInInspector]
        public float[] m_inflatableStiffness;

        [HideInInspector]
        public float[] m_inflatablePressures;



        void Awake()
        {

            InitParticleArrays();

        }

        void Start()
        {

        }

        void Update()
        {

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    Debug.Log("Updating Container...");
            //    ProcessFlexInstances(false);
            //}
        }

        int UpdateActiveSet()
        {
            int count = 0;

            //Bitmap inactive(c->mMaxParticles);

            //// create bitmap
            //for (size_t i = 0; i < c->mFreeList.size(); ++i)
            //{
            //    // if this fires then somehow a duplicate has ended up in the free list (double delete)
            //    assert(!inactive.IsSet(c->mFreeList[i]));

            //    inactive.Set(c->mFreeList[i]);
            //}

            //// iterate bitmap to find active elements
            //for (int i = 0; i < c->mMaxParticles; ++i)
            //    if (inactive.IsSet(i) == false)
            //        indices[count++] = i;

            // iterate bitmap to find active elements
            for (int i = 0; i < m_particlesCount; i++)
            {
                if (m_particlesActivity[i])
                    m_activeSet[count++] = i;
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitParticleArrays()
        {
            m_particles = new Particle[m_maxParticlesCount];
            m_restParticles = new Particle[m_maxParticlesCount];
            m_smoothedParticles = new Particle[m_maxParticlesCount];
            m_velocities = new Vector3[m_maxParticlesCount];
            m_normals = new Vector4[m_maxParticlesCount];
            m_colors = new Color[m_maxParticlesCount];

            m_phases = new int[m_maxParticlesCount];
            m_activeSet = new int[m_maxParticlesCount];
            m_densities = new float[m_maxParticlesCount];
            m_particlesActivity = new bool[m_maxParticlesCount];

            m_particlesHndl = GCHandle.Alloc(m_particles, GCHandleType.Pinned);
            m_restParticlesHndl = GCHandle.Alloc(m_restParticles, GCHandleType.Pinned);
            m_smoothedParticlesHndl = GCHandle.Alloc(m_smoothedParticles, GCHandleType.Pinned);
            m_velocitiesHndl = GCHandle.Alloc(m_velocities, GCHandleType.Pinned);
            m_phasesHndl = GCHandle.Alloc(m_phases, GCHandleType.Pinned);
            m_densitiesHndl = GCHandle.Alloc(m_densities, GCHandleType.Pinned);
            m_activeSetHndl = GCHandle.Alloc(m_activeSet, GCHandleType.Pinned);
            m_normalsHndl = GCHandle.Alloc(m_normals, GCHandleType.Pinned);

        }

        public void UpdateContainer()
        {

            //keep the order of objects already present in the container
            FlexParticles[] flexGOs = FindObjectsOfType<FlexParticles>();
            bool countChanged = m_flexGameObjects.Count != flexGOs.Length;

            //handle deletions
            m_flexGameObjects.RemoveAll(x => x == null);


            //handle deactivations
            m_flexGameObjects.RemoveAll(x => !x.gameObject.activeInHierarchy);

            //handle additions
            foreach (var ci in flexGOs)
            {
                if (!m_flexGameObjects.Contains(ci))
                {
                    if (ci.gameObject.activeInHierarchy)
                        m_flexGameObjects.Add(ci);
                }
            }

            //m_flexGameObjects.Clear();
            //m_flexGameObjects.AddRange(FindObjectsOfType<FlexParticles>());
            //bool countChanged = true;

            if (countChanged)
                Debug.Log("flexGameObjects count changed: "+ m_flexGameObjects.Count);

            UpdateCounts();

            ProcessParticles(countChanged);

            ProcessShapeTranslations(countChanged);

            ProcessTriangles(countChanged);

            ProcessSprings(countChanged);

            ProcessShapes(countChanged);

            ProcessInflatables(countChanged);


            ProcessExtraSprings();
        }

        private void UpdateCounts()
        {

            m_particlesCount = 0;
            m_springsCount = 0;
            m_shapesCount = 0;
            m_shapeIndicesCount = 0;
            m_trianglesCount = 0;
            m_inflatablesCount = 0;
            m_activeInstacesCount = 0;

            for (int iId = 0; iId < m_flexGameObjects.Count; iId++)
            {
                m_flexGameObjects[iId].m_instanceId = iId;

                FlexParticles  particles = m_flexGameObjects[iId].GetComponent<FlexParticles>();
                FlexTriangles triangles = m_flexGameObjects[iId].GetComponent<FlexTriangles>();
                FlexSprings springs = m_flexGameObjects[iId].GetComponent<FlexSprings>();
                FlexShapeMatching shapes = m_flexGameObjects[iId].GetComponent<FlexShapeMatching>();
                FlexInflatable inflatable = m_flexGameObjects[iId].GetComponent<FlexInflatable>();

                if (particles && particles.enabled)
                {
                    if(m_particlesCount + particles.m_particlesCount <= m_maxParticlesCount)
                    {
                        particles.m_particlesIndex = m_particlesCount;
                        m_particlesCount += particles.m_particlesCount;

                        m_activeInstacesCount++;
                    }
                    else
                    {
                        Debug.Log("Current particles count exceeded their maximum number for this container");
                    }

                }

                if (springs && springs.enabled)
                {
                    springs.m_springsIndex = m_springsCount;
                    m_springsCount += springs.m_springsCount;
                }

                if (shapes && shapes.enabled)
                {
                    shapes.m_shapesIndex = m_shapesCount;
                    shapes.m_shapesIndicesIndex = m_shapeIndicesCount;

                    m_shapesCount += shapes.m_shapesCount;
                    m_shapeIndicesCount += shapes.m_shapeIndicesCount;
                }

                if (triangles && triangles.enabled)
                {
                    triangles.m_trianglesIndex = m_trianglesCount;
                    m_trianglesCount += triangles.m_trianglesCount;
                }

                if (inflatable && inflatable.enabled)
                {
                    inflatable.m_inflatableIndex = m_inflatablesCount;
                    m_inflatablesCount++;
                }

            }

        }

        private void ProcessParticles(bool countChanged)
        {
            //print("processing particles");
            for (int iId = 0; iId < m_flexGameObjects.Count && iId < m_activeInstacesCount; iId++)
            {
        
                Transform tr = m_flexGameObjects[iId].transform;

                FlexParticles particles = m_flexGameObjects[iId];
                if (particles && particles.enabled)
                {
                    if (m_flexGameObjects[iId].m_initialized) //if initialized just update some arrays
                    {

                        Array.Copy(particles.m_particles, 0, m_particles, particles.m_particlesIndex, particles.m_particlesCount);
                        Array.Copy(particles.m_restParticles, 0, m_restParticles, particles.m_particlesIndex, particles.m_particlesCount);
                        Array.Copy(particles.m_velocities, 0, m_velocities, particles.m_particlesIndex, particles.m_particlesCount);
                        Array.Copy(particles.m_particlesActivity, 0, m_particlesActivity, particles.m_particlesIndex, particles.m_particlesCount);

                        particles.m_collisionGroup = particles.m_collisionGroup == -1 ? iId : particles.m_collisionGroup;
                        int phase = Flex.MakePhase(particles.m_collisionGroup, (int)particles.m_interactionType);

                        for (int pId = 0; pId < particles.m_particlesCount; pId++)
                        {
                            m_phases[pId + particles.m_particlesIndex] = phase;
                        }
                        //needs to update this if the flex game object is changing position in the containcer (i.e. iId)
                        //particles.m_collisionGroup = particles.m_collisionGroup == -1 ? iId : particles.m_collisionGroup;
                        //int phase = Flex.MakePhase(particles.m_collisionGroup, (int)particles.m_interactionType);
                        //for (int pId = 0; pId < particles.m_particlesCount; pId++)
                        //{
                        //    m_phases[pId + particles.m_particlesIndex] = phase;
                        //}
                    }   
                    else // do the full init
                    {
                        print("mass" + particles.m_mass);
                        //if group is -1 set the consecutive bodyId, else use a user defined number
                        particles.m_collisionGroup = particles.m_collisionGroup == -1 ? iId : particles.m_collisionGroup;
                        int phase = Flex.MakePhase(particles.m_collisionGroup, (int)particles.m_interactionType);
                        float invMass = particles.m_mass == 0 ? 0.0f : 1.0f / particles.m_mass;
                        //print("init" + particles.m_mass);
                        for (int pId = 0; pId < particles.m_particlesCount; pId++)
                        {

                            m_particles[pId + particles.m_particlesIndex].pos = tr.TransformPoint(particles.m_particles[pId].pos);
                            m_particles[pId + particles.m_particlesIndex].invMass = particles.m_overrideMass ? invMass : particles.m_particles[pId].invMass;
                            m_restParticles[pId + particles.m_particlesIndex] = particles.m_restParticles[pId];

                            //m_velocities[pId + particles.m_particlesIndex] = particles.m_velocities[pId];
                            m_velocities[pId + particles.m_particlesIndex] = particles.m_initialVelocity;
                            //print("velocities during process particles init of container: " + m_velocities[pId + particles.m_particlesIndex]);
                            m_colors[pId + particles.m_particlesIndex] = particles.m_colours[pId];
                            m_phases[pId + particles.m_particlesIndex] = phase;
                            m_particlesActivity[pId + particles.m_particlesIndex] = particles.m_particlesActivity[pId];
                        }

                        m_flexGameObjects[iId].m_initialized = true;

                    }
                }
            }

            //m_particlesCount = UpdateActiveSet();

            m_activeParticlesCount = UpdateActiveSet();

        }

        private void ProcessShapeTranslations(bool countChanged)
        {

            for (int iId = 0; iId < m_flexGameObjects.Count && iId < m_activeInstacesCount; iId++)
            {
            //    Transform tr = m_flexGameObjects[iId].transform;
                FlexShapeMatching shapes = m_flexGameObjects[iId].GetComponent<FlexShapeMatching>();

                if (shapes && shapes.enabled && !shapes.m_initialized)
                {

                    for (int sId = 0; sId < shapes.m_shapesCount; sId++)
                    {
                        shapes.m_shapeTranslations[sId] = shapes.transform.TransformPoint(shapes.m_shapeCenters[sId]);
                        shapes.m_shapeRotations[sId] = shapes.transform.rotation;
                    }

                    shapes.m_initialized = true;
                }
                
            }
            
        }

        private void ProcessTriangles(bool sizeChanged)
        {
            if (sizeChanged)
            {
                m_triangleIndices = new int[m_trianglesCount * 3];
                m_triangleNormals = new Vector3[m_trianglesCount];
            }

            for (int iId = 0; iId < m_flexGameObjects.Count && iId < m_activeInstacesCount; iId++)
            {
                FlexParticles particles = m_flexGameObjects[iId].GetComponent<FlexParticles>();
                FlexTriangles triangles = m_flexGameObjects[iId].GetComponent<FlexTriangles>();

                if (triangles && triangles.enabled)
                {
                    for (int tId = 0; tId < triangles.m_trianglesCount; tId++)
                    {
                        m_triangleIndices[tId * 3 + 0 + triangles.m_trianglesIndex * 3] = triangles.m_triangleIndices[tId * 3 + 0] + particles.m_particlesIndex;
                        m_triangleIndices[tId * 3 + 1 + triangles.m_trianglesIndex * 3] = triangles.m_triangleIndices[tId * 3 + 1] + particles.m_particlesIndex;
                        m_triangleIndices[tId * 3 + 2 + triangles.m_trianglesIndex * 3] = triangles.m_triangleIndices[tId * 3 + 2] + particles.m_particlesIndex;
                    }
                }
            }
        }

        private void ProcessSprings(bool sizeChanged)
        {
            if (sizeChanged)
            {
                m_springIndices = new int[m_springsCount * 2];
                m_springRestLengths = new float[m_springsCount];
                m_springCoefficients = new float[m_springsCount];
            }

            for (int iId = 0; iId < m_flexGameObjects.Count && iId < m_activeInstacesCount; iId++)
            {
                FlexParticles particles = m_flexGameObjects[iId].GetComponent<FlexParticles>();
                FlexSprings springs = m_flexGameObjects[iId].GetComponent<FlexSprings>();

                if (springs && springs.enabled)
                {
                    for (int sId = 0; sId < springs.m_springsCount; sId++)
                    {
                        m_springIndices[sId * 2 + 0 + springs.m_springsIndex * 2] = springs.m_springIndices[sId * 2 + 0] + particles.m_particlesIndex;
                        m_springIndices[sId * 2 + 1 + springs.m_springsIndex * 2] = springs.m_springIndices[sId * 2 + 1] + particles.m_particlesIndex;

                        m_springRestLengths[sId + springs.m_springsIndex] = springs.m_springRestLengths[sId];
                        m_springCoefficients[sId + springs.m_springsIndex] = springs.m_springCoefficients[sId];
                    }
                }
            }

        }

        private void ProcessShapes(bool sizeChanged)
        {
            if (sizeChanged)
            {
                m_shapeIndices = new int[m_shapeIndicesCount];
                m_shapeRestPositions = new Vector3[m_shapeIndicesCount];
                m_shapeOffsets = new int[m_shapesCount + 1];
                m_shapeCenters = new Vector3[m_shapesCount];
                m_shapeCoefficients = new float[m_shapesCount];

                m_shapeTranslations = new Vector3[m_shapesCount];
                m_shapeRotations = new Quaternion[m_shapesCount];

                m_shapeTranslationsHndl = GCHandle.Alloc(m_shapeTranslations, GCHandleType.Pinned);
                m_shapeRotationHndl = GCHandle.Alloc(m_shapeRotations, GCHandleType.Pinned);
            }

            int shapeIndex = 0;
            int shapeIndexOffset = 0;


            for (int iId = 0; iId < m_flexGameObjects.Count && iId < m_activeInstacesCount; iId++)
            {
                FlexParticles particles = m_flexGameObjects[iId].GetComponent<FlexParticles>();
                FlexShapeMatching shapes = m_flexGameObjects[iId].GetComponent<FlexShapeMatching>();

                if (shapes && shapes.enabled)
                {
                    int indexOffset = shapeIndexOffset;

                    int shapeStart = 0;
                    for (int s = 0; s < shapes.m_shapesCount; s++)
                    {
                        m_shapeOffsets[shapeIndex + 1] = shapes.m_shapeOffsets[s] + indexOffset;
                        m_shapeCoefficients[shapeIndex] = shapes.m_shapeCoefficients[s];
                        m_shapeTranslations[shapeIndex] = shapes.m_shapeTranslations[s];
                        m_shapeRotations[shapeIndex] = shapes.m_shapeRotations[s];

                        shapeIndex++;

                        int shapeEnd = shapes.m_shapeOffsets[s];

                        for (int i = shapeStart; i < shapeEnd; ++i)
                        {
                         //   int p = shapes.m_shapeIndices[i];

                            // remap indices and create local space positions for each shape
                            //Vector3 pos = particles.m_particles[p].pos;
                            //if(m_initialization)
                            //    m_shapeRestPositions[shapeIndexOffset] = pos - shapes.m_shapeCenters[s];

                            m_shapeRestPositions[shapeIndexOffset] = shapes.m_shapeRestPositions[i];
                            m_shapeIndices[shapeIndexOffset] = shapes.m_shapeIndices[i] + particles.m_particlesIndex;

                            shapeIndexOffset++;
                        }

                        shapeStart = shapeEnd;
                    }
                }
            }
        }

        private void ProcessInflatables(bool sizeChanged)
        {
            if (sizeChanged)
            {
                m_inflatableStarts = new int[m_inflatablesCount];
                m_inflatableCounts = new int[m_inflatablesCount];
                m_inflatableVolumes = new float[m_inflatablesCount];
                m_inflatablePressures = new float[m_inflatablesCount];
                m_inflatableStiffness = new float[m_inflatablesCount];
            }

            int inflatableId = 0;
            for (int iId = 0; iId < m_flexGameObjects.Count && iId < m_activeInstacesCount; iId++)
            {
                FlexTriangles triangles = m_flexGameObjects[iId].GetComponent<FlexTriangles>();
                FlexInflatable inflatable = m_flexGameObjects[iId].GetComponent<FlexInflatable>();

                if (inflatable && inflatable.enabled)
                {
                    m_inflatableStarts[inflatableId] = triangles.m_trianglesIndex;
                    m_inflatableCounts[inflatableId] = triangles.m_trianglesCount;
                    m_inflatableVolumes[inflatableId] = inflatable.m_restVolume;
                    m_inflatablePressures[inflatableId] = inflatable.m_pressure;
                    m_inflatableStiffness[inflatableId] = inflatable.m_stiffness;
                    inflatableId++;
                }
            }
        }



        private void ProcessExtraSprings()
        {
            FlexExtraSpring[] bs = FindObjectsOfType<FlexExtraSpring>();


            int sId = m_springsCount;
            m_springsCount += bs.Length;
            Array.Resize<int>(ref m_springIndices, m_springsCount * 2);
            Array.Resize<float>(ref m_springRestLengths, m_springsCount);
            Array.Resize<float>(ref m_springCoefficients, m_springsCount);


            foreach (FlexExtraSpring b in bs)
            {
                
                if (b.m_idB == -1)
                {
                    Vector3 posA = m_particles[b.m_idA + b.m_bodyA.m_particlesIndex].pos;
                    float minDist = float.MaxValue;
                    bool setLength = b.m_restLength < 0;
                    for (int i = 0; i < b.m_bodyB.m_particlesCount; i++)
                    {
                        float dist = Vector3.Distance(posA, m_particles[i + b.m_bodyB.m_particlesIndex].pos);
                        if(dist < minDist)
                        {
                            minDist = dist;
                            b.m_idB = i;
                            if(setLength)
                                b.m_restLength = dist;
                        }
                    }
                }


                m_springIndices[sId * 2 + 0] = b.m_idA + b.m_bodyA.m_particlesIndex;
                m_springIndices[sId * 2 + 1] = b.m_idB + b.m_bodyB.m_particlesIndex;

                m_springCoefficients[sId] = b.m_stiffness;
                m_springRestLengths[sId] = b.m_restLength;

                sId++;
            }

        }


        void OnApplicationQuit()
        {
            //free pinned arrays
            m_particlesHndl.Free();
            m_restParticlesHndl.Free();
            m_smoothedParticlesHndl.Free();
            m_velocitiesHndl.Free();
            m_normalsHndl.Free();
            m_phasesHndl.Free();
            m_densitiesHndl.Free();
            m_activeSetHndl.Free();


            m_shapeTranslationsHndl.Free();
            m_shapeRotationHndl.Free();
        }
    }
}