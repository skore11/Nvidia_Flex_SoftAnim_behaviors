using UnityEngine;
using System.Collections;

namespace uFlex
{
    [ExecuteInEditMode]
    public class SSF_ParticlesRenderer : MonoBehaviour
    {

        public ParticleSystem m_ps;

        public int m_particlesCount = 0;

        public Color m_color = Color.gray;
        public float m_radius = 1;
        public float m_scale = 1;

        private Material m_material;

        private ComputeBuffer m_posBuffer;
        private ComputeBuffer m_colorBuffer;
        private ComputeBuffer m_quadVerticesBuffer;

        private Vector4[] m_positions;

        private Vector3[] m_velocities;

        private Color[] m_colors;

        private ParticleSystem.Particle[] m_particles;

        void Awake()
        {

        }

        void OnEnable()
        {
            m_particles = new ParticleSystem.Particle[m_ps.maxParticles];
            m_positions = new Vector4[m_ps.maxParticles];

            m_velocities = new Vector3[m_ps.maxParticles]; ;

            m_colors = new Color[m_ps.maxParticles]; ;

            m_posBuffer = new ComputeBuffer(m_ps.maxParticles, 16);
            m_colorBuffer = new ComputeBuffer(m_ps.maxParticles, 16);

            m_particlesCount = m_ps.GetParticles(m_particles);

            for (int i = 0; i < m_ps.particleCount; i++)
            {
                m_positions[i] = m_particles[i].position;
                m_positions[i].w = m_particles[i].GetCurrentSize(m_ps);

                m_velocities[i] = m_particles[i].velocity;
                m_colors[i] = m_particles[i].GetCurrentColor(m_ps);
            }

            m_quadVerticesBuffer = new ComputeBuffer(6, 16);
            m_quadVerticesBuffer.SetData(new[]
            {
                new Vector4(-1f, 1f),
                new Vector4(1f, 1f),
                new Vector4(1f, -1f),
                new Vector4(1f, -1f),
                new Vector4(-1f, -1f),
                new Vector4(-1f, 1f),
            });

       //     hideFlags = HideFlags.DontSave;

            m_material = new Material(Shader.Find("ScreenSpaceFluids/SSF_SpherePointsShader"));
            m_material.hideFlags = HideFlags.DontSave;

            m_material.SetBuffer("buf_Positions", m_posBuffer);
            m_material.SetBuffer("buf_Colors", m_colorBuffer);
            m_material.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

        }

        // Use this for initialization
        void Start()
        {


        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnRenderObject()
        {
            m_particlesCount = m_ps.GetParticles(m_particles);

            for (int i = 0; i < m_particlesCount; i++)
            {
                m_positions[i] = m_particles[i].position;
                m_positions[i].w = m_particles[i].GetCurrentSize(m_ps);

                m_velocities[i] = m_particles[i].velocity;
                m_colors[i] = m_particles[i].GetCurrentColor(m_ps);
            }

            m_posBuffer.SetData(m_positions);
            m_colorBuffer.SetData(m_colors);

            m_material.SetPass(0);
            m_material.SetFloat("_PointRadius", m_radius);
            m_material.SetFloat("_PointScale", m_scale);
            m_material.SetColor("_Color", m_color);

            Graphics.DrawProcedural(MeshTopology.Triangles, 6, m_ps.particleCount);
  
        }


        void OnDisable()
        {
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
            ReleaseBuffers();
        }
    }
}