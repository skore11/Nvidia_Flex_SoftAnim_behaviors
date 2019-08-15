Shader "uFlex/DiffuseParticlesShader" {
	Properties{
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1)
		_PointRadius("Point Radius", Float) = 1.0
		_PointScale("Point Scale", Float) = 1.0
		_InvFade("InvFade", Float) = 0.0
		}

		SubShader{
		Pass{
			//Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend SrcAlpha One
			ZTest Off // ZTest in shader

			Cull Off 
			ZWrite Off

			Fog{ Mode off }

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			//	#pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"

			StructuredBuffer<float4> buf_Positions;
			StructuredBuffer<float4> buf_Colors;
			StructuredBuffer<float4> buf_Vertices;
			StructuredBuffer<float2> buf_TexCoords;


			uniform sampler2D _MainTex;
			uniform float4 _LightColor0;

			
			float4 _Color;
			float _PointRadius;
			float _PointScale;
			float _InvFade;

			sampler2D_float _CameraDepthTexture;


		struct v2f
		{
			float4 position : POSITION;
			float4 color : COLOR0;
			float2 tex : TEXCOORD0;
		//	float3 posEye: TEXCOORD1;
			float3 viewPos: TEXCOORD2;
			float4 projPos: TEXCOORD3;
			float4 lightDir: TEXCOORD4;
			float lifeTime : PSIZE;
		};


		v2f vert(uint id : SV_VertexID, uint inst : SV_InstanceID)
		{
			v2f o;


			//vec3 worldPos = gl_Vertex.xyz;// - vec3(0.0, 0.1*0.25, 0.0);	// hack move towards ground to account for anisotropy;
			//vec4 eyePos = gl_ModelViewMatrix * vec4(worldPos, 1.0);

			//gl_Position = gl_ProjectionMatrix * eyePos;
			////gl_Position.z -= 0.0025;	// bias above fluid surface

			//// calculate window-space point size
			//gl_PointSize = pointRadius * (pointScale / gl_Position.w);

			//vec3 lVec = normalize(worldPos-lightPos);
			//float attenuation = max(smoothstep(spotMin, spotMax, dot(lVec, lightDir)), 0.2);

			//gl_TexCoord[0] = gl_MultiTexCoord0;    
			//gl_TexCoord[1] = vec4(worldPos, gl_Vertex.w);
			//gl_TexCoord[2] = eyePos;

			//gl_TexCoord[3].xyz = gl_ModelViewMatrix*vec4(gl_MultiTexCoord1.xyz, 0.0);
			//gl_TexCoord[3].w = attenuation;

			//gl_TexCoord[4].xyzw = color;

			//// hack to color different emitters 
			//if (gl_MultiTexCoord1.w == 2.0)
			//	gl_TexCoord[4].xyzw = vec4(0.85, 0.65, 0.65, color.w);
			//else if (gl_MultiTexCoord1.w == 1.0)
			//	gl_TexCoord[4].xyzw = vec4(0.65, 0.85, 0.65, color.w);

			//// compute ndc pos for frustrum culling in GS
			//vec4 ndcPos = gl_ModelViewProjectionMatrix * vec4(worldPos.xyz, 1.0);
			//gl_TexCoord[5] = ndcPos / ndcPos.w;

			float3 worldPos = buf_Positions[inst].xyz;
			float lifeTime = buf_Positions[inst].w;
			float4 viewPos = mul(UNITY_MATRIX_MV, float4(worldPos.xyz, 1.0)) + float4(buf_Vertices[id].x * _PointScale * lifeTime , buf_Vertices[id].y * _PointScale * lifeTime, 0.0, 0.0);


			o.position = mul(UNITY_MATRIX_P, viewPos);
			o.lifeTime = lifeTime;
			o.viewPos = viewPos.xyz;
			o.projPos = ComputeScreenPos(o.position);
			o.color = buf_Colors[inst] * _Color;
			o.tex = MultiplyUV(UNITY_MATRIX_TEXTURE0, buf_Vertices[id] + 0.5);

			o.lightDir = mul(UNITY_MATRIX_MV, float4(normalize(_WorldSpaceLightPos0.xyz), 0.0));
			return o;
		}

		struct fragOut
		{
			float4 color : COLOR;
			float depth : DEPTH;
		};

		//fragOut frag(v2f i)
		fixed4 frag(v2f i) : SV_Target
		{
			fragOut OUT;

			//discard single particles
			//if (i.position.a < _MinDensity)
			//	discard;

			// calculate eye-space sphere normal from texture coordinates
			float3 normal;
			normal.xy = i.tex * 2.0 - 1.0;
			//N.xy = i.tex.xy * float2(2.0, -2.0) + float2(-1.0, 1.0);
			float r2 = dot(normal.xy, normal.xy);
			if (r2 > 1.0)
				discard; // kill pixels outside circle

			normal.z = sqrt(1.0 - r2);


			float3 eyePos = i.viewPos + normal * _PointRadius;//*2.0;
			float4 ndcPos = mul(UNITY_MATRIX_P, float4(eyePos, 1.0));
			ndcPos.z /= ndcPos.w;

			float depth = ndcPos.z;
		//	OUT.depth = depth;

			//float sceneDepth = tex2D(_CameraDepthTexture, uv);


			float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
			float partZ = i.projPos.z;
			float fade = saturate (_InvFade * (sceneZ-partZ));
		//	i.color.a *= fade;

		//	OUT.color = float4(_Color.xyz, i.lifeTime * _Color.a);
			return float4(_Color.xyz, i.lifeTime * _Color.a * fade);

			//return OUT;

		}
		ENDCG
		}
		}

			Fallback off

	}
