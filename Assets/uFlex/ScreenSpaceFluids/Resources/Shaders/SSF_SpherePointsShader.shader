Shader "ScreenSpaceFluids/SSF_SpherePointsShader" {
Properties {
	_Color("Color", Color) = (0.5, 0.5, 0.5, 1)
	_PointRadius ("Point Radius", Float) = 1.0
	_PointScale("Point Scale", Float) = 1.0
}

SubShader 
{
	Pass 
{
	//	Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual 
	//	ZTest Always 

	//	Cull On 
	//	ZWrite On
	//	ZWrite Off
		Fog { Mode off }

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

		float _PointRadius;
		float _PointScale;
		float4 _Color;

		struct v2f 
		{
			float4 position : POSITION;	
			float4 color : COLOR0;
			float2 tex : TEXCOORD0;
		//	float3 posEye: TEXCOORD1;
			float3 viewPos: TEXCOORD2;
		//	float4 projPos: TEXCOORD3;
			float4 lightDir: TEXCOORD4;
		//	float pointSize : PSIZE;
		};


		v2f vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
		{
			v2f o;

			float4 viewPos = mul(UNITY_MATRIX_MV, float4(buf_Positions[inst].xyz, 1.0)) + float4(buf_Vertices[id].x * _PointScale, buf_Vertices[id].y * _PointScale, 0.0, 0.0);

			o.position = mul(UNITY_MATRIX_P, viewPos);

	//		o.pointSize = -_PointScale * _PointRadius / viewPos.z;

			o.viewPos = viewPos.xyz;

	//		o.projPos = ComputeScreenPos(o.position);

			o.color = buf_Colors[inst] * _Color;

			o.tex = MultiplyUV (UNITY_MATRIX_TEXTURE0, buf_Vertices[id] + 0.5);

			o.lightDir = mul(UNITY_MATRIX_MV, float4(normalize(_WorldSpaceLightPos0.xyz), 0.0));

			return o; 
		}
		
		uniform sampler2D _CameraDepthTexture; //the depth texture

		struct fragOut
        {
                float4 color : COLOR;
                float depth : DEPTH;
        };

		fragOut frag (v2f i)
		{
			fragOut OUT;

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
			OUT.depth = depth;

			//NORMAL
			//OUT.color = float4(normal * 0.5 + 0.5, 1);

			//DEPTH
			//float linearDepth = Linear01Depth(depth);
			//float linearDepth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
			//OUT.color = float4(linearDepth, linearDepth, linearDepth, 1);

			//LIGHT
			//float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
			float3 lightDir = i.lightDir;

		//	float diffuseMul = max(0.0, dot(normal, lightDir));
			float diffuseMul = max(0.0, dot(normal, lightDir) * 0.5 + 0.5);
			float3 diffuse = i.color * _LightColor0.rgb * diffuseMul;
			
			float3 ambient = i.color * UNITY_LIGHTMODEL_AMBIENT.rgb;

			OUT.color = float4(ambient + diffuse, 1.0);
			
			return OUT;

		}
		ENDCG
	}
}

Fallback off

}
