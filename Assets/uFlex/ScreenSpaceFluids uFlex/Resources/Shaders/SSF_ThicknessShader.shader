Shader "ScreenSpaceFluids/SSF_ThicknessShader" {
	Properties {

		_Thickness ("Thickness", Float) = 0.2
		_Softness("Softness", Float) = 2.0
		_PointRadius("Point Radius", Float) = 1.0
		_PointScale("Point Scale", Float) = 1.0
	}

	SubShader {

		//Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Pass {

		//Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
		//Blend One OneMinusSrcAlpha // Premultiplied transparency
		//Blend One One // Additive
		Blend OneMinusDstColor One // Soft Additive
		//Blend DstColor Zero // Multiplicative
		//Blend DstColor SrcColor // 2x Multiplicative

		//	BlendOp Add
		//	ZTest Always 
			ZTest LEqual
			Cull Back 
			ZWrite Off
			Fog { Mode off }



			//glEnable(GL_BLEND);
			//glBlendFunc(GL_ONE, GL_ONE);
			//glBlendEquation(GL_FUNC_ADD);
			//glDepthMask(GL_FALSE);
			////glEnable(GL_DEPTH_TEST);

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
		//	#pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"
		
			StructuredBuffer<float4> buf_Positions;
			StructuredBuffer<float4> buf_Vertices;
			StructuredBuffer<float2> buf_TexCoords;
		

			uniform sampler2D _MainTex;
			uniform sampler2D _CameraDepthTexture; //the depth texture

			float _PointRadius;
			float _PointScale;
			float _Thickness;
			float _Softness;

			struct v2f 
			{
				float4 position : POSITION;	
			//	float4 color : COLOR0;
				float2 tex : TEXCOORD0;
			//	float3 posEye: TEXCOORD1;
				float3 viewPos: TEXCOORD1;
				float4 projPos : TEXCOORD2; //Screen position of vertex
			//	float pointSize : PSIZE;
			};

			 v2f vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
				v2f o;

				float4 viewPos = mul(UNITY_MATRIX_MV, float4(buf_Positions[inst].xyz, 1.0)) + float4(buf_Vertices[id].x * _PointScale, buf_Vertices[id].y * _PointScale, 0.0, 0.0);

				o.position = mul(UNITY_MATRIX_P, viewPos);

				o.tex = MultiplyUV(UNITY_MATRIX_TEXTURE0, buf_Vertices[id] + 0.5);
				
				o.viewPos = viewPos.xyz;
				o.projPos = ComputeScreenPos(o.position);
				return o; 
			}


			 //struct fragOut
			 //{
				// float4 color : COLOR;
				// float depth : DEPTH;
			 //};

			float frag (v2f i) : COLOR
			{
			//	fragOut OUT;

				float sceneDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
				#if defined(UNITY_REVERSED_Z)
					sceneDepth = 1.0f - sceneDepth;
					//depth = 1.0f - depth;
				#endif

				// calculate eye-space sphere normal from texture coordinates
				float3 N;
				N.xy = i.tex * 2.0 - 1.0;
				float r2 = dot(N.xy, N.xy);
				if (r2 > 1.0) 
					discard; // kill pixels outside circle
				N.z = sqrt(1.0 - r2);

				float thickness = N.z * _Thickness;
				float alpha = exp(-r2 * _Softness);
				

				float3 eyePos = i.viewPos + N * _PointRadius;//*2.0;
				float4 ndcPos = mul(UNITY_MATRIX_P, float4(eyePos, 1.0));
				ndcPos.z /= ndcPos.w;

				float depth = ndcPos.z;
			//	OUT.depth = depth;



				//Gaussian Distribution
				//float dist = length(i.tex- float2(0.5f, 0.5f));
				//float sigma = 3.0f;
				//float mu = 0.0f;
				//float g_dist = 0.02f * exp(-(dist-mu)*(dist-mu)/(2*sigma));
				//thickness = g_dist;

				//NORMAL
				//OUT.color = float4(normal * 0.5 + 0.5, 1);

				//DEPTH
				//float linearDepth = Linear01Depth(depth);
				//float linearDepth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
				//OUT.color = float4(linearDepth, linearDepth, linearDepth, 1);


			//	float color = thickness * alpha;
				float color = (sceneDepth < depth) ? 0 : thickness * alpha;
			
				return color = clamp(color, 0.0, 1.0);


			}
			ENDCG
		}
	}

	Fallback off

}