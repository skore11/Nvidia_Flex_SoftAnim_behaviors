using UnityEngine;
using System.Collections;

namespace SSF
{
 //   [ExecuteInEditMode]
    public class SSF_FluidRenderer : MonoBehaviour
    {

        public ParticleSystem m_ps;
        public int m_particlesCount = 0;

        public RenderTexture m_depthTexture;
        public RenderTexture m_blurredDepthTexture;
        public RenderTexture m_blurredDepthTempTexture;

        public float m_pointScale = 1.0f;
        public float m_pointRadius = 1.0f;

        public bool m_blur = true;
        public float m_blurScale = 0.001f;
        public int m_blurRadius = 5;
        public float m_minDepth = 0.0f;
        public float m_blurDepthFalloff = 2.0f;


        private Material m_depthMaterial;
        private Material m_blurDepthMaterial;

        private ComputeBuffer m_posBuffer;
        private ComputeBuffer m_colorBuffer;
        private ComputeBuffer m_quadVerticesBuffer;

        private Vector4[] m_positions;

        private Vector3[] m_velocities;

        private Color[] m_colors;


        private ParticleSystem.Particle[] m_particles;

        

        // Use this for initialization
        void Start()
        {


            m_depthMaterial = new Material(Shader.Find("ScreenSpaceFluids/SSF_DepthShader"));

            m_blurDepthMaterial = new Material(Shader.Find("ScreenSpaceFluids/SSF_BlurDepth"));

           
            m_depthMaterial.hideFlags = HideFlags.HideAndDontSave;

            m_blurDepthMaterial.hideFlags = HideFlags.HideAndDontSave;

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

            m_posBuffer.SetData(m_positions);
            m_colorBuffer.SetData(m_colors);

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


            m_depthMaterial.SetBuffer("buf_Positions", m_posBuffer);
            m_depthMaterial.SetBuffer("buf_Velocities", m_colorBuffer);
            m_depthMaterial.SetBuffer("buf_Vertices", m_quadVerticesBuffer);


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

            //   Debug.Log("OnRENDER: " + Application.isPlaying);
            m_posBuffer.SetData(m_positions);
            m_colorBuffer.SetData(m_colors);

            // m_material.SetPass(0);



            DrawDepth();

            if (m_blur)
            {
                BlurDepth();
            }
            else
            {
                Graphics.Blit(m_depthTexture, m_blurredDepthTexture);
            }




            //Graphics.DrawProcedural(MeshTopology.Triangles, sphereVertexCount, body.pointsCount);
            Graphics.SetRenderTarget(Camera.main.targetTexture);
        }



        void DrawDepth()
        {
            m_depthMaterial.SetFloat("_PointRadius", m_pointRadius);
            m_depthMaterial.SetFloat("_PointScale", m_pointScale);

            m_depthMaterial.SetBuffer("buf_Positions", m_posBuffer);
            m_depthMaterial.SetBuffer("buf_Colors", m_colorBuffer);
            m_depthMaterial.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

            Graphics.SetRenderTarget(m_depthTexture);
            GL.Clear(true, true, Color.white);

            m_depthMaterial.SetPass(0);

            
            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, m_ps.particleCount);
        }

        void BlurDepth()
        {

            m_blurDepthMaterial.SetTexture("_DepthTex", m_depthTexture);

            m_blurDepthMaterial.SetInt("radius", m_blurRadius);
            m_blurDepthMaterial.SetFloat("minDepth", m_minDepth);
            m_blurDepthMaterial.SetFloat("blurDepthFalloff", m_blurDepthFalloff);

            m_blurDepthMaterial.SetTexture("_DepthTex", m_depthTexture);
            //   m_blurDepthMaterial.SetFloat("scaleX", 1.0f / Screen.width);
            m_blurDepthMaterial.SetFloat("scaleX", 1.0f / 1024 * m_blurScale);
            m_blurDepthMaterial.SetFloat("scaleY", 0.0f);
            Graphics.Blit(m_depthTexture, m_blurredDepthTempTexture, m_blurDepthMaterial);

            m_blurDepthMaterial.SetTexture("_DepthTex", m_blurredDepthTempTexture);
            m_blurDepthMaterial.SetFloat("scaleX", 0.0f);
            //   m_blurDepthMaterial.SetFloat("scaleY", 1.0f / Screen.height);
            m_blurDepthMaterial.SetFloat("scaleY", 1.0f / 1024 * m_blurScale);
            Graphics.Blit(m_blurredDepthTempTexture, m_blurredDepthTexture, m_blurDepthMaterial);


        }

       

        void OnDisable()
        {

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