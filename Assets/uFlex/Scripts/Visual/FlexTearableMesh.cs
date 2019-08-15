using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace uFlex
{
    /// <summary>
    /// A component updating the visual mesh according to the underlying particles.
    /// The vertices of the visual mesh are mapped at start to the nodes of physics model. Their number can also be different.
    /// Due to uv data some visual meshes often have vertices duplicated (e.g. on texture seams).
    /// </summary>
    /// 
  //  [RequireComponent(typeof(MeshFilter), typeof(FlexParticles))]
    public class FlexTearableMesh : FlexProcessor
    {
            /// <summary>
          /// The maximum allowable strain on each edge
        /// </summary>
        public float m_maxStrain = 2.0f;

          /// <summary>
        /// The maximum number of constraint breaks that will be performed, this controls the 'rate' of mesh tearing
        /// </summary>
        public int m_maxSplits = 4;

     //   [HideInInspector]
        public float m_stretchKs = 1.0f;
        
    //    [HideInInspector]
        public float m_bendKs = 0.5f;

        [HideInInspector]
        public int[] mappings;

        [HideInInspector]
        public int[] mappingsRev;
        


        private IntPtr m_clothMeshPtr;

        private FlexParticles m_flexParticles;
        private FlexSprings m_flexSprings;
        private FlexTriangles m_flexTriangles;
        private FlexInflatable m_flexInflatable;

        private Mesh m_mesh;

        private Vector3[] m_vertices;
        private int[] m_triangles;
        private Vector2[] m_uvs;
        private Vector2[] m_origUvs;

        private float m_maxSearchDistance = 0.00001f;


        void Start()
        {

            m_flexParticles = GetComponent<FlexParticles>();
            m_flexSprings = GetComponent<FlexSprings>();
            m_flexTriangles = GetComponent<FlexTriangles>();
            m_flexInflatable = GetComponent<FlexInflatable>();

            m_mesh = GetComponent<MeshFilter>().mesh;

            m_vertices = m_mesh.vertices;
            m_triangles = m_mesh.triangles;
            m_uvs = m_mesh.uv;
            m_origUvs = m_mesh.uv;

            //if(m_uvs.Length == 0)
            //{
            //    m_uvs = new Vector2[m_vertices.Length];
            //    m_origUvs = new Vector2[m_vertices.Length];
            //}

            //texture seams (duplicate vertices) are currently not supported during tearing
            if(m_mesh.vertexCount != m_flexParticles.m_particlesCount)
            {
                Debug.LogWarning("Closed mesh with texture seams detected. This is currently not supported by uFlex v0.5. You may still tear closed meshes but plase remove any textures from object's material as UV mapping will be removed");


                m_mesh = new Mesh();

                m_vertices = new Vector3[m_flexParticles.m_particlesCount];
              //  m_uvs = new Vector2[m_flexParticles.m_particlesCount];
                m_uvs = new Vector2[0];

                for (int i = 0; i < m_flexParticles.m_particlesCount; i++)
                {
                    m_vertices[i] = transform.InverseTransformPoint(m_flexParticles.m_particles[i].pos);
                  //  m_uvs[i] = m_origUvs[mappingsRev[i]];
                }


                m_mesh.vertices = m_vertices;
              //  m_mesh.uv = m_uvs;
                m_mesh.triangles = m_flexTriangles.m_triangleIndices;

                m_mesh.RecalculateNormals();
                m_mesh.RecalculateBounds();

                GetComponent<MeshFilter>().mesh = m_mesh;
            }


            this.mappings = new int[m_mesh.vertexCount * 2];
            this.mappingsRev = new int[m_mesh.vertexCount * 2];
            for (int i = 0; i < m_mesh.vertexCount; i++)
            {
                Vector3 v = m_vertices[i];
                bool mappingFound = false;

                float minDistance = 100000.0f;
                int minId = 0;

                for (int j = 0; j < m_flexParticles.m_particlesCount; j++)
                {
                    // float dist = Vector3.Distance(v, transform.InverseTransformPoint(m_flexBody.m_particles[j]));
                    float dist = Vector3.Distance(v, m_flexParticles.m_particles[j].pos);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        minId = j;
                    }
                }

                if (minDistance < this.m_maxSearchDistance)
                {
                    this.mappings[i] = minId;
                    this.mappingsRev[minId] = i;
                    mappingFound = true;
                }

                if (!mappingFound)
                    Debug.Log("MappingMissing: " + i);

            }




            //int vertexCount = m_mesh.vertexCount;

            //int[] triangles = m_mesh.triangles;
            //int triIndicesCount = triangles.Length;
            //int trianglesCount = triIndicesCount / 3;

            //int[] uniqueVerticesIds = new int[vertexCount];
            //int[] origToUniqueVertMapping = new int[vertexCount];
            //int uniqueVerticesCount = FlexExt.flexExtCreateWeldedMeshIndices(m_vertices, vertexCount, uniqueVerticesIds, origToUniqueVertMapping, 0.00001f);

            //Debug.Log("Welding Mesh: " + uniqueVerticesCount + "/" + vertexCount);

            //Vector4[] uniqueVertices4 = new Vector4[uniqueVerticesCount];
            //for (int i = 0; i < uniqueVerticesCount; i++)
            //{
            //    uniqueVertices4[i] = m_vertices[uniqueVerticesIds[i]];
            //}

            //int[] uniqueTriangles = new int[trianglesCount * 3];
            //for (int i = 0; i < trianglesCount * 3; i++)
            //{
            //    uniqueTriangles[i] = origToUniqueVertMapping[triangles[i]];
            //}


            //m_clothMeshPtr = uFlexAPI.uFlexCreateTearableClothMesh(uniqueVertices4, uniqueVerticesCount, uniqueVerticesCount * 2, uniqueTriangles, trianglesCount, m_stretchKs, m_bendKs, 0);

            m_clothMeshPtr = uFlexAPI.uFlexCreateTearableClothMesh(m_flexParticles.m_particles, m_flexParticles.m_particlesCount, m_flexParticles.m_particlesCount * 2, m_flexTriangles.m_triangleIndices, m_flexTriangles.m_trianglesCount, m_stretchKs, m_bendKs, 0.0f);
        }


        void Update()
        {
            if (m_vertices.Length != m_flexParticles.m_particlesCount)
            {
                m_vertices = new Vector3[m_flexParticles.m_particlesCount];
                if (m_uvs.Length > 0)
                    m_uvs = new Vector2[m_flexParticles.m_particlesCount];
            }

            for (int i = 0; i < m_flexParticles.m_particlesCount; i++)
            {
             //   m_vertices[i] = transform.InverseTransformPoint(m_flexParticles.m_particles[mappings[i]].pos);
             //   m_vertices[i] = transform.InverseTransformPoint(m_flexParticles.m_particles[mappingsList[i]].pos);
                m_vertices[i] = transform.InverseTransformPoint(m_flexParticles.m_particles[i].pos);
                
                if (m_uvs.Length > 0)
                    m_uvs[i] = m_origUvs[mappingsRev[i]];
            }

            Array.Copy(m_flexTriangles.m_triangleIndices, m_triangles, m_flexTriangles.m_trianglesCount * 3);

            m_mesh.vertices = m_vertices;
            if(m_uvs.Length > 0)
                m_mesh.uv = m_uvs;

            m_mesh.triangles = m_triangles;

            m_mesh.RecalculateNormals();
            m_mesh.RecalculateBounds();

        }

        // Overrideto modify the data just after the solver was created
        public override void FlexStart(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

        }

        // Override to modify the data after the flex game objects were updated but before the data from them was copied to the container arrays
        // In other words, use this for working with flex game objects.
        public override void PreContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

            const int maxCopies = 2048;
            const int maxEdits = 2048;

            FlexExt.FlexExtTearingParticleClone[] particleCopies = new FlexExt.FlexExtTearingParticleClone[maxCopies];
            int numParticleCopies = 0;

            FlexExt.FlexExtTearingMeshEdit[] triangleEdits = new FlexExt.FlexExtTearingMeshEdit[maxEdits];
            int numTriangleEdits = 0;

            int numParticles = m_flexParticles.m_particlesCount;
            int numSprings = m_flexSprings.m_springsCount;

            uFlexAPI.uFlexTearClothMesh(ref numParticles, numParticles * 2, numSprings, m_flexParticles.m_particles, m_flexSprings.m_springIndices, m_flexSprings.m_springRestLengths, m_flexSprings.m_springCoefficients, m_flexTriangles.m_triangleIndices, m_clothMeshPtr,
                m_maxStrain, m_maxSplits, particleCopies, ref numParticleCopies, maxCopies, triangleEdits, ref numTriangleEdits, maxEdits);

            // copy particles
            for (int i = 0; i < numParticleCopies; ++i)
            {
                int srcIndex = particleCopies[i].srcIndex;
                int destIndex = particleCopies[i].destIndex;

                m_flexParticles.m_particles[destIndex] = m_flexParticles.m_particles[srcIndex];
                m_flexParticles.m_restParticles[destIndex] = m_flexParticles.m_restParticles[srcIndex];
                m_flexParticles.m_velocities[destIndex] = m_flexParticles.m_velocities[srcIndex];
                m_flexParticles.m_colours[destIndex] = m_flexParticles.m_colours[srcIndex];
                m_flexParticles.m_phases[destIndex] = m_flexParticles.m_phases[srcIndex];
                m_flexParticles.m_particlesActivity[destIndex] = m_flexParticles.m_particlesActivity[srcIndex];

                mappingsRev[destIndex] = mappingsRev[srcIndex];

                //deactivate inflation if there is any break
                if (m_flexInflatable != null)
                    m_flexInflatable.enabled = false;


            }

            // apply triangle modifications to index buffer
            for (int i = 0; i < numTriangleEdits; ++i)
            {
                int index = triangleEdits[i].triIndex;
                int newValue = triangleEdits[i].newParticleIndex;

                m_flexTriangles.m_triangleIndices[index] = newValue;
            }


            m_flexParticles.m_particlesCount += numParticleCopies;


        }

     
        // Override to modify the data after the container was updated
        // In other words, use for working directly with the container data.
        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

        }

        // Override this method to modify the data just before the solver is closed
        public override void FlexClose(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

        }

    }
}