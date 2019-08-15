Shader "ScreenSpaceFluids/SSF_DepthShaderDensity" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_PointRadius("Point Radius", Float) = 1.0
	_PointScale("Point Scale", Float) = 1.0
	_MinDensity("Min Density", Float) = 0.0
}

SubShader {
	Tags {"RenderType"="Opaque"}
	Pass {
		ZTest LEqual 
	//	ZTest Always 
		Cull Off 
		ZWrite On
		Fog { Mode off }

		CGPROGRAM
		#pragma target 4.0
		#pragma vertex vert
		#pragma fragment frag
	//	#pragma enable_d3d11_debug_symbols

		#include "UnityCG.cginc"
		
		StructuredBuffer<float4> buf_Positions;
		StructuredBuffer<float4> buf_Vertices;
		StructuredBuffer<float> buf_Densities;
		StructuredBuffer<float2> buf_TexCoords;
		
		uniform sampler2D _MainTex;
		uniform sampler2D _CameraDepthTexture; //the depth texture

		float _PointRadius;
		float _PointScale;
		float _MinDensity;

		struct v2f 
		{
			float4 position : POSITION;	
			float density : PSIZE;
			float2 tex : TEXCOORD0;
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
			o.density = buf_Densities[inst];
			//	o.pointSize = -_PointScale * _PointRadius / viewPos.z;

			return o; 
		}

		struct fragOut
        {
			float color : COLOR;
			float depth : DEPTH;

			//	float4 color : SV_Target;
           //     float depth : SV_Depth;
        };
		
		fragOut frag (v2f i)
		{
			fragOut OUT;
		//	float sceneDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
			if (i.density < _MinDensity)
				discard;
			// calculate eye-space sphere normal from texture coordinates
			float3 normal;
			normal.xy = i.tex * 2.0 - 1.0;
			float r2 = dot(normal.xy, normal.xy);
			if (r2 > 1.0)
				discard; // kill pixels outside circle

			normal.z = sqrt(1.0 - r2);

			float3 eyePos = i.viewPos + normal * _PointRadius;//*2.0;
			float4 ndcPos = mul(UNITY_MATRIX_P, float4(eyePos, 1.0));
			ndcPos.z /= ndcPos.w;
			float depth = ndcPos.z;

		//	depth =  (sceneDepth < depth) ? 0 : depth;

			OUT.depth = depth;
			#if defined(UNITY_REVERSED_Z)
				OUT.color = 1.0 - depth;
			#else
				OUT.color = depth;
			#endif
		//	OUT.color = Linear01Depth(depth);
			return OUT;

		}
		ENDCG
	}
}

Fallback off

}