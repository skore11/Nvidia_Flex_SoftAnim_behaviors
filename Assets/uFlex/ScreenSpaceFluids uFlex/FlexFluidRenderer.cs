using UnityEngine;
using System.Collections;

namespace uFlex
{
    /// <summary>
    /// Renders screen space fluids to a number of render textures.
    /// Needs a camera post-processing effect to compose and draw final effect
    /// </summary>
    public class FlexFluidRenderer : MonoBehaviour
    {
        public FlexParticles m_flexParticles;

        public float m_pointScale = 1.0f;
        public float m_pointRadius = 1.0f;
        public float m_minDensity = 0.0f;

        public RenderTexture m_colorTexture;
        public RenderTexture m_depthTexture;
        public RenderTexture m_blurredDepthTexture;
        public RenderTexture m_blurredDepthTempTexture;
        public RenderTexture m_thicknessTexture;

    

        public bool m_blur = true;
        public float m_blurScale = 1f;
        public int m_blurRadius = 10;
     //   public float m_minDepth = 0.0f;
        public float m_blurDepthFalloff = 100.0f;

        private Material m_colorMaterial;
        private Material m_depthMaterial;
        private Material m_blurDepthMaterial;
        private Material m_thicknessMaterial;


        private ComputeBuffer m_posBuffer;
        private ComputeBuffer m_colorBuffer;
        private ComputeBuffer m_densityBuffer;
        private ComputeBuffer m_quadVerticesBuffer;




        // Use this for initialization
        void Start()
        {

            if (m_flexParticles == null)
                m_flexParticles = GetComponent<FlexParticles>();

            m_colorMaterial = new Material(Shader.Find("ScreenSpaceFluids/SSF_SpherePointsShader"));
            m_depthMaterial = new Material(Shader.Find("ScreenSpaceFluids/SSF_DepthShaderDensity"));
            m_thicknessMaterial = new Material(Shader.Find("ScreenSpaceFluids/SSF_ThicknessShader"));
            m_blurDepthMaterial = new Material(Shader.Find("ScreenSpaceFluids/SSF_BlurDepth"));

            m_posBuffer = new ComputeBuffer(m_flexParticles.m_particlesCount, 16);
            m_colorBuffer = new ComputeBuffer(m_flexParticles.m_particlesCount, 16);
            m_densityBuffer = new ComputeBuffer(m_flexParticles.m_particlesCount, 4);
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


            m_depthMaterial.SetBuffer("buf_Positions", m_posBuffer);
            m_depthMaterial.SetBuffer("buf_Velocities", m_colorBuffer);
            m_depthMaterial.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

            m_thicknessMaterial.SetBuffer("buf_Positions", m_posBuffer);
            m_thicknessMaterial.SetBuffer("buf_Velocities", m_colorBuffer);
            m_thicknessMaterial.SetBuffer("buf_Vertices", m_quadVerticesBuffer);


        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnRenderObject()
        //   void OnPostRender()
        {
            

            //   Debug.Log("OnRENDER: " + Application.isPlaying);
            m_posBuffer.SetData(m_flexParticles.m_particles);
            m_densityBuffer.SetData(m_flexParticles.m_densities);
            m_colorBuffer.SetData(m_flexParticles.m_colours);

            // m_material.SetPass(0);

            DrawColors();

            DrawDepth();

            if (m_blur)
            {
                BlurDepth();
            }
            else
            {
                Graphics.Blit(m_depthTexture, m_blurredDepthTexture);
            }

            DrawThickness();


            //Graphics.DrawProcedural(MeshTopology.Triangles, sphereVertexCount, body.pointsCount);
            Camera cam = Camera.main;
            Graphics.SetRenderTarget(cam.targetTexture);
        }

        void DrawColors()
        {
      //      m_colorMaterial.SetColor("_Color", m_color);
            m_colorMaterial.SetFloat("_PointRadius", m_pointRadius);
            m_colorMaterial.SetFloat("_PointScale", m_pointScale);
            m_colorMaterial.SetBuffer("buf_Positions", m_posBuffer);
            m_colorMaterial.SetBuffer("buf_Colors", m_colorBuffer);
            m_colorMaterial.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

            Graphics.SetRenderTarget(m_colorTexture);
            GL.Clear(true, true, Color.white);

            m_colorMaterial.SetPass(0);

            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, m_flexParticles.m_particlesCount);
        }

        void DrawDepth()
        {
 
            m_depthMaterial.SetFloat("_PointRadius", m_pointRadius);
            m_depthMaterial.SetFloat("_PointScale", m_pointScale);
            m_depthMaterial.SetFloat("_MinDensity", m_minDensity);
      
            m_depthMaterial.SetBuffer("buf_Positions", m_posBuffer);
            m_depthMaterial.SetBuffer("buf_Densities", m_densityBuffer);
            m_depthMaterial.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

            Graphics.SetRenderTarget(m_depthTexture);
            GL.Clear(true, true, Color.white);

            //    m_drawMaterial.SetBuffer("buf_Normals", sphereNormalsBuffer);
            //m_drawBuffer.SetVector("_LightDir", m_light.transform.forward);
            //m_drawBuffer.SetVector("_ViewDir", m_cam.transform.forward);

            m_depthMaterial.SetPass(0);

            
            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, m_flexParticles.m_particlesCount);
        }

        void BlurDepth()
        {
            //  Graphics.SetRenderTarget(m_blurredDepthTexture);
            //  GL.Clear(true, true, Color.white);

            //  Graphics.SetRenderTarget(m_blurredDepthTempTexture);
            //   GL.Clear(true, true, Color.white);


            m_blurDepthMaterial.SetTexture("_DepthTex", m_depthTexture);

            m_blurDepthMaterial.SetInt("radius", m_blurRadius);
        //    m_blurDepthMaterial.SetFloat("minDepth", m_minDepth);
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

        void DrawThickness()
        {
            m_thicknessMaterial.SetFloat("_PointRadius", m_pointRadius);
            m_thicknessMaterial.SetFloat("_PointScale", m_pointScale);
            m_thicknessMaterial.SetFloat("_MinDensity", m_minDensity);

            m_thicknessMaterial.SetBuffer("buf_Positions", m_posBuffer);
            m_thicknessMaterial.SetBuffer("buf_Velocities", m_colorBuffer);
            m_thicknessMaterial.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

            Graphics.SetRenderTarget(m_thicknessTexture);
            GL.Clear(true, true, Color.black);

            m_thicknessMaterial.SetPass(0);

            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, m_flexParticles.m_particlesCount);
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

            if (m_densityBuffer != null)
                m_densityBuffer.Release();

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