using UnityEngine;
using System.Collections;

namespace uFlex
{
    [ExecuteInEditMode]
    public class FlexParticlesRenderer : MonoBehaviour
    {

        public FlexParticles m_flexParticles;

        public Color m_color = Color.white;
        public float m_size = 1;
        public float m_radius = 1;
     
        public float m_minDensity = 0.0f;
        public bool m_showDensity = false;



        private Material m_material;

        private ComputeBuffer m_posBuffer;
        private ComputeBuffer m_colorBuffer;
        private ComputeBuffer m_quadVerticesBuffer;

        void Awake()
        {
        //    Debug.Log("AWAKE: " + Application.isPlaying);
        }

        void OnEnable()
        {
        //    Debug.Log("ENABLED: " + Application.isPlaying);
            if (m_flexParticles == null)
                m_flexParticles = GetComponent<FlexParticles>();

            //m_posBuffer = new ComputeBuffer(m_flexParticles.m_particlesCount, 16);
            //m_colorBuffer = new ComputeBuffer(m_flexParticles.m_particlesCount, 16);

            m_posBuffer   = new ComputeBuffer(m_flexParticles.m_maxParticlesCount, 16);
            m_colorBuffer = new ComputeBuffer(m_flexParticles.m_maxParticlesCount, 16);

            m_posBuffer.SetData(m_flexParticles.m_particles);
            m_colorBuffer.SetData(m_flexParticles.m_colours);

            m_quadVerticesBuffer = new ComputeBuffer(6, 16);
            m_quadVerticesBuffer.SetData(new[]
            {
                new Vector4(-0.5f, 0.5f),
                new Vector4(0.5f, 0.5f),
                new Vector4(0.5f, -0.5f),
                new Vector4(0.5f, -0.5f),
                new Vector4(-0.5f, -0.5f),
                new Vector4(-0.5f, 0.5f),
            });

            //    hideFlags = HideFlags.DontSave;

            m_material = new Material(Shader.Find("uFlex/SpherePointsSpritesShader"));
     
            m_material.hideFlags = HideFlags.DontSave;

            m_material.SetBuffer("buf_Positions", m_posBuffer);
            m_material.SetBuffer("buf_Colors", m_colorBuffer);
            m_material.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

        }



        // Use this for initialization
        void Start()
        {
        //    Debug.Log("START: " + Application.isPlaying);

        }

        // Update is called once per frame
        void Update()
        {
         //   Debug.Log("UPDATE");
        }

        void OnRenderObject()
        {
         //   Debug.Log("OnRenderObject");
            if (Application.isPlaying)
            {
                m_posBuffer.SetData(m_flexParticles.m_particles);
            }
            else
            {
                Vector4[] tmpPos = new Vector4[m_flexParticles.m_maxParticlesCount];
                for (int i = 0; i < m_flexParticles.m_particlesCount; i++)
                {
                    tmpPos[i] = transform.TransformPoint(m_flexParticles.m_particles[i].pos);
                }
                m_posBuffer.SetData(tmpPos);
            }       

            if (m_showDensity)
            {
                for (int i = 0; i < m_flexParticles.m_particlesCount; i++)
                {
                       m_flexParticles.m_colours[i] = m_flexParticles.m_colour * m_flexParticles.m_densities[i];
                   //  m_flexParticles.m_colours[i] = new Color(m_flexParticles.m_densities[i],0,0,1);
                }
            }

            m_colorBuffer.SetData(m_flexParticles.m_colours);

            m_material.SetFloat("_PointRadius", m_radius);
            m_material.SetFloat("_PointScale", m_size);
            m_material.SetFloat("_MinDensity", m_minDensity);
            m_material.SetColor("_Color", m_color);

            m_material.SetPass(0);

            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, m_flexParticles.m_particlesCount);

        }


        void OnDisable()
        {
        //    Debug.Log("DISABLED: "+ Application.isPlaying);
            DestroyImmediate(m_material);
            ReleaseBuffers();
        }

        void ReleaseBuffers()
        {
            if (m_posBuffer != null)
                m_posBuffer.Release();

            if (m_colorBuffer != null)
                m_colorBuffer.Release();

            if (m_quadVerticesBuffer != null)
                m_quadVerticesBuffer.Release();
        }

        void OnApplicationQuit()
        {

        //    Debug.Log("QUIT: " + Application.isPlaying);
            ReleaseBuffers();
        }
    }
}