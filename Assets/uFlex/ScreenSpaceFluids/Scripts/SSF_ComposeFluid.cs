using UnityEngine;
using System.Collections;


namespace SSF
{
 //   [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/ScreenSpaceFluids/SSF_ComposeFluid")]
    public class SSF_ComposeFluid : UnityStandardAssets.ImageEffects.ImageEffectBase
    {

        public Color m_color = Color.blue;
        public Color m_specular = Color.white;

        public float m_shininess = 64f;
        public float m_reflection = 0.1f;
        public float m_reflectionFalloff = 10f;

        public float m_maxDepth = 0.9999f;

        private float m_minDepth = 0.0f;

        private float m_xFactor = 0.001f;
        private float m_YFactor = 0.001f;

       // public RenderTexture m_colorTexture;
       // public RenderTexture m_depthTexture;
        public RenderTexture m_blurredDepthTexture;

        public Cubemap m_cubemap;

        protected override void Start()
        {
            base.Start();

       //     material.SetTexture("_ColorTex", m_colorTexture);
      //      material.SetTexture("_DepthTex", m_depthTexture);
            material.SetTexture("_BlurredDepthTex", m_blurredDepthTexture);
            material.SetTexture("_Cube", m_cubemap);
        }

        // Called by the camera to apply the image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            material.SetColor("_Color", m_color);
            material.SetColor("_Specular", m_specular);
            material.SetFloat("_Shininess", m_shininess);

            material.SetFloat("_FresnelFalloff", m_reflectionFalloff);
            material.SetFloat("_Fresnel", m_reflection);

            material.SetFloat("_XFactor", m_xFactor);
            material.SetFloat("_YFactor", m_YFactor);
            material.SetFloat("_MinDepth", m_minDepth);
            material.SetFloat("_MaxDepth", m_maxDepth);

            Graphics.Blit(source, destination, material);

            //Graphics.Blit(m_depthTexture, destination);
            //Graphics.Blit(m_blurredDepthTexture, destination);
            //Graphics.Blit(m_thicknessTexture, destination);
        }


    }

}