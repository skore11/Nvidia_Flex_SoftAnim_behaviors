// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ScreenSpaceFluids/SSF_ComposeFluidHQ"
{
	Properties
	{
		_Color("Fluid Color", Color) = (1,1,1,0)
		_Specular("Specular Color", Color) = (1,1,1,0)
		_ColorFalloff("Falloff Color", Color) = (1,0,1,0)

		_Fresnel("Fresnel", Float) = 0.1
		_FresnelFalloff("Fresnel Falloff", Float) = 10.0

		_MinDepth("Min Depth", Float) = 0.000
		_MaxDepth("Max Depth", Float) = 0.999

		_XFactor("X Factor", Float) = 0.001
		_YFactor("Y Factor", Float) = 0.001

		_Shininess("Shininess", Float) = 0.1

		_MainTex("Texture", 2D) = "white" {}
		_DepthTex("Depth Tex", 2D) = "white" {}
		_BlurredDepthTex("Blurred Depth Tex", 2D) = "white" {}
		_Cube("Cubemap", CUBE) = "" {}
	}

		SubShader
	{
		// No culling or depth
			Cull Off
			ZWrite Off
			ZTest Always

			Pass
		{
			CGPROGRAM
			#pragma target 5.0
			//#pragma enable_d3d11_debug_symbols
			#pragma vertex vert
			#pragma fragment frag
		

			#include "UnityCG.cginc"
			float4 _Color;
			float4 _Specular;

			float _MinDepth;
			float _MaxDepth;
			float _XFactor;
			float _YFactor;
			float _FresnelFalloff;
			float _Shininess;
			float _Fresnel;

			sampler2D _MainTex;
			sampler2D _ColorTex;
			sampler2D _DepthTex;
			sampler2D _BlurredDepthTex;

			float4 _MainTex_TexelSize;

			samplerCUBE _Cube;

			uniform sampler2D _CameraDepthTexture; //the depth texture

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			//	float2 coord : TEXCOORD1;
			//	float4 projPos : TEXCOORD2; //Screen position of vertex
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
			//	o.projPos = ComputeScreenPos(o.vertex);
				//o.coord = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.uv.xy);

				return o;
			}

			struct fragOut
			{
				float4 color : COLOR;
				float depth : DEPTH;
			};


			float3 uvToEye(float2 texCoord, float z)
			{
				// Convert texture coordinate to homogeneous space
				float zFar = _ProjectionParams.z;
				float zNear = _ProjectionParams.y;

				float2 xyPos = (texCoord * 2.0 - 1.0);
				float a = zFar / (zFar - zNear);
				float b = zFar*zNear / (zNear - zFar);
				float rd = b / (z - a);
				return float3(xyPos.x, xyPos.y, -1.0) * rd;
			}

			fragOut frag(v2f i)
			{
				fragOut OUT;

				float2 uv = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					uv.y = 1 - uv.y;
				#endif

				float4 sceneCol = tex2D(_MainTex, i.uv);

				float depth = tex2D(_BlurredDepthTex, uv);
				//float sceneDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
				float sceneDepth = tex2D(_CameraDepthTexture, uv.xy);
				
				#if defined(UNITY_REVERSED_Z)
					sceneDepth = 1.0f - sceneDepth;
					//depth = 1.0f - depth;
				#endif

				if (depth <= _MinDepth)
				{
					OUT.color = _Color;
					OUT.depth = 0.0;
					return OUT;
				}
				else if (depth >= _MaxDepth)
				{
					OUT.color = sceneCol;
					OUT.depth = 1.0;
					return OUT;
				}


				// reconstruct eye space pos from depth
				float3 eyePos = uvToEye(uv, depth);

				//HQ NORMAL
			//	float2 texCoord1 = float2(i.coord.x + _XFactor, i.coord.y);
			//	float2 texCoord2 = float2(i.coord.x - _XFactor, i.coord.y);
				float2 texCoord1 = float2(uv.x + _XFactor, uv.y);
				float2 texCoord2 = float2(uv.x - _XFactor, uv.y);

				float3 ddx1 = uvToEye(texCoord1, tex2D(_BlurredDepthTex, texCoord1.xy)) - eyePos;
				float3 ddx2 = eyePos - uvToEye(texCoord2, tex2D(_BlurredDepthTex, texCoord2.xy));

				if (abs(ddx1.z) > abs(ddx2.z))
				{
					ddx1 = ddx2;
				}

			//	texCoord1 = float2(i.coord.x, i.coord.y + _YFactor);
			//	texCoord2 = float2(i.coord.x, i.coord.y - _YFactor);
				texCoord1 = float2(uv.x, uv.y + _YFactor);
				texCoord2 = float2(uv.x, uv.y - _YFactor);

				float3 ddy1 = uvToEye(texCoord1, tex2D(_BlurredDepthTex, texCoord1.xy)) - eyePos;
				float3 ddy2 = eyePos - uvToEye(texCoord2, tex2D(_BlurredDepthTex, texCoord2.xy));
				if (abs(ddy1.z) > abs(ddy2.z))
				{
					ddy1 = ddy2;
				}

				float3 normal = cross(ddx1, ddy1);
				normal = normalize(normal);

				//NORMAL
				//float3 normal = -normalize(cross(ddx(eyePos.xyz), ddy(eyePos.xyz)));

				//AMBIENT
				float3 ambient = _Color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb;

				//DIFFUSE
				//float3 lightDir = normalize(float3(1, 1, 1));
				float3 lightDir = mul(UNITY_MATRIX_MV, float4(normalize(_WorldSpaceLightPos0.xyz), 0.0));
				float diffuseMul = max(0.0, dot(normal, lightDir)) * 0.5 + 0.5;
				//float diffuseMul = dot(normal, lightDir) * 0.5 + 0.5;
				//loat diffuseMul = max(0.0, dot(normal, lightDir));
				float3 diffuse = _Color.rgb * diffuseMul;

				//SPEC
				float3 specular = float3(0, 0, 0);
				float3 v = normalize(-eyePos);
				if (_Shininess > 0)
				{

					float3 h = normalize(lightDir + v);
					float specularMul = pow(max(0.0, dot(normal, h)), _Shininess);
					specular = _Specular.xyz * specularMul * _Specular.a;
				}

				//REFL
				float3 reflection = float3(0, 0, 0);
				if (_Fresnel > 0)
				{
					float3 reflectVec = reflect(-v, normal);
					//float3 reflection = texCUBE(_Cube, reflectVec) * _Fresnel;
					reflection = pow(texCUBE(_Cube, reflectVec), _FresnelFalloff) * _Fresnel;
				}

				float4 fluidCol = float4(diffuse + ambient + specular + reflection, 1.0f);

				//	float4 normalCol = float4(normal*0.5f + 0.5f, 1.0f);

				bool culled = (sceneDepth < depth);

				OUT.color = (culled) ? sceneCol : fluidCol;
				OUT.depth = (culled) ? sceneDepth : depth;

				return OUT;

			}
			ENDCG
		}
	}
}

