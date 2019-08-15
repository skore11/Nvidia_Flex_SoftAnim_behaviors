// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DepthBufWriteTest" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{
	    Pass
        {
		//	Tags { "RenderType"="Opaque" }
					//ZTest Off
			ZWrite On
			//Cull Off
			//Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct v2f 
			{
				float4 position : POSITION;	
				float4 color : COLOR0;
				float2 depth : TEXCOORD0;
				float3 posEye: TEXCOORD1;
				float3 pos: TEXCOORD2;
				float3 viewPos: TEXCOORD3;
				float pointSize : PSIZE;
			};

			v2f vert(float4 v:POSITION)
			{
					v2f o;
				o.position =  UnityObjectToClipPos (v);
				 UNITY_TRANSFER_DEPTH(o.depth);
				return o;
			}

			
			struct fragOut
			{
					float4 color : COLOR;
			//	    float depth : DEPTH;

			//		float4 color : SV_Target;
			//	    float depth : SV_Depth;

				
			};

			fragOut frag(v2f i)
			{
				fragOut OUT;
				OUT.color =  fixed4(0.0, 1.0, 1.0, 1.0);
			//	OUT.depth = 0.5;
				 UNITY_TRANSFER_DEPTH(i.depth);
			//  return tex2D(_MainTex, i.uv);
				return OUT;
			}
			ENDCG
		}
	} 
   Fallback Off
}
