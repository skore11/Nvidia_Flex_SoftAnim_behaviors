using UnityEngine;
using System;
using System.Collections;
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
    public class FlexClothMesh : MonoBehaviour
    {

        [HideInInspector]
        public int[] mappings;

        private FlexParticles m_flexBody;

        private Mesh m_mesh;

        //private MeshCollider m_collider;

        private Vector3[] m_vertices;

        private float m_maxSearchDistance = 0.00001f;


        void Start()
        {
            m_flexBody = GetComponent<FlexParticles>();
            m_mesh = GetComponent<MeshFilter>().mesh;
            //m_collider = GetComponent<MeshCollider>();
            m_vertices = m_mesh.vertices;

            this.mappings = new int[m_mesh.vertexCount];
            for (int i = 0; i < m_mesh.vertexCount; i++)
            {
                Vector3 v = m_vertices[i];
                bool mappingFound = false;

                float minDistance = 100000.0f;
                int minId = 0;

                for (int j = 0; j < m_flexBody.m_particlesCount; j++)
                {
                    // float dist = Vector3.Distance(v, transform.InverseTransformPoint(m_flexBody.m_particles[j]));
                    float dist = Vector3.Distance(v, m_flexBody.m_particles[j].pos);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        minId = j;
                    }
                }

                if (minDistance < this.m_maxSearchDistance)
                {
                    this.mappings[i] = minId;
                    mappingFound = true;
                }

                if (!mappingFound)
                    Debug.Log("MappingMissing: " + i);

            }
        }
        void Update()
        {
            for (int i = 0; i < m_mesh.vertexCount; i++)
            {
                m_vertices[i] = transform.InverseTransformPoint(m_flexBody.m_particles[mappings[i]].pos);
            }
            
            m_mesh.vertices = m_vertices;

            m_mesh.RecalculateNormals();
            m_mesh.RecalculateBounds();

            //hack for updating the mesh collider. slow
            //if (m_collider != null)
            //{
            //    m_collider.sharedMesh = null;
            //    m_collider.sharedMesh = m_mesh;
            //}

        }

    }
}