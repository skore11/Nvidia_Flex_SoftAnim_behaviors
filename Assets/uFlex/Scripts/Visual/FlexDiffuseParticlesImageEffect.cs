using UnityEngine;
using System.Collections;

namespace uFlex
{
    //[ExecuteInEditMode]
    public class FlexDiffuseParticlesImageEffect : MonoBehaviour
    {

        public FlexDiffuseParticles m_flexDiffuseParticles;

        public Color m_color = Color.white;
        public float m_size = 1;
        public float m_radius = 1;


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
            if (m_flexDiffuseParticles == null)
                m_flexDiffuseParticles = GetComponent<FlexDiffuseParticles>();

            m_posBuffer = new ComputeBuffer(m_flexDiffuseParticles.m_maxDiffuseParticlesCount, 16);
            m_colorBuffer = new ComputeBuffer(m_flexDiffuseParticles.m_maxDiffuseParticlesCount, 16);

            m_posBuffer.SetData(m_flexDiffuseParticles.m_diffuseParticles);
            m_colorBuffer.SetData(m_flexDiffuseParticles.m_diffuseVelocities);

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

            m_material = new Material(Shader.Find("uFlex/DiffuseParticlesShader"));
     
            m_material.hideFlags = HideFlags.DontSave;

            m_material.SetBuffer("buf_Positions", m_posBuffer);
            m_material.SetBuffer("buf_Colors", m_colorBuffer);
            m_material.SetBuffer("buf_Vertices", m_quadVerticesBuffer);

        }



        // Use this for initialization
        void Start()
        {
         //   Debug.Log("START: " + Application.isPlaying);

        }

        // Update is called once per frame
        void Update()
        {
         //   Debug.Log("UPDATE");
        }

        void OnRenderObject() 
   //     void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
         //   Debug.Log("OnRenderObject");

            m_posBuffer.SetData(m_flexDiffuseParticles.m_diffuseParticles);
            m_colorBuffer.SetData(m_flexDiffuseParticles.m_diffuseVelocities);

            m_material.SetFloat("_PointRadius", m_radius);
            m_material.SetFloat("_PointScale", m_size);
            m_material.SetColor("_Color", m_color);



            m_material.SetPass(0);

            Graphics.DrawProcedural(MeshTopology.Triangles, 6, m_flexDiffuseParticles.m_diffuseParticlesCount);

        }


        void OnDisable()
        {
           // Debug.Log("DISABLED: "+ Application.isPlaying);
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

          //  Debug.Log("QUIT: " + Application.isPlaying);
            ReleaseBuffers();
        }
    }
}