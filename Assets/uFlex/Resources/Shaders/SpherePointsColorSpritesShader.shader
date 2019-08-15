Shader "uFlex/SpherePointsColorSpritesShader" {
Properties {

	_SphereRadius ("Sphere Radius", Float) = 0.001
}

SubShader {
	Pass {
	//	Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual 
	//	ZTest Always 

	//	Cull On 
		ZWrite On
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


		float _SphereRadius;

		struct v2f 
		{
			float4 position : POSITION;	
			float4 color : COLOR0;
			float2 tex : TEXCOORD0;
			float3 posEye: TEXCOORD1;
			float3 viewPos: TEXCOORD2;
			//float4 projPos: TEXCOORD3;
			float pointSize : PSIZE;
		};


		 v2f vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
		{
			v2f o;

			float4 posEye = mul(UNITY_MATRIX_MV, float4( buf_Positions[inst].xyz, 1.0)) + float4( buf_Vertices[id].xyz, 1.0);
			
		//	if (buf_Vertices[id].x < 10)
		//		discard;
			//o.position = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4( buf_Positions[inst].xyz, 1.0)) + float4( buf_Vertices[id].xyz, 1.0));
			o.position = mul(UNITY_MATRIX_P, posEye);

		//	o.projPos = ComputeScreenPos(o.position);

			o.color = buf_Colors[inst];



			o.tex = MultiplyUV (UNITY_MATRIX_TEXTURE0, buf_Vertices[id] + 0.5);

			o.posEye = posEye.xyz/posEye.w;
			o.viewPos = posEye.xyz;

			float dist = length(o.posEye);
		//	float dist = o.position.w;


			//vec3 posEye = (u_ModelView * vec4(Position.xyz, 1.0f)).xyz;
			//float dist = length(posEye);
			//gl_PointSize = pointRadius * (pointScale/dist);

			float scale = 1.0;
		//	float scale = _ScreenParams.x * (1.0f /tan(60.0 * 0.5));

		//	width / aspectRatio * (1.0f / tanf(cam.zoom * 0.5f));

			//width / aspectRatio * (1.0f / tanf(cam.zoom * 0.5f)), "pointScale");
		//m_window_h / tanf(m_fov*0.5f*(float)M_PI/180.0f) 



			o.pointSize = _SphereRadius * scale / dist;
			
			return o; 
		}
		
		
		struct fragOut
        {
                float4 color : COLOR;
                float depth : DEPTH;
        };

		fragOut frag (v2f i)
		{
			fragOut OUT;
			// calculate eye-space sphere normal from texture coordinates
			float3 N;
			N.xy = i.tex*2.0-1.0;
			float r2 = dot(N.xy, N.xy);
			if (r2 > 1.0) 
				discard; // kill pixels outside circle
			N.z = sqrt(1.0 - r2);

		//	float4 pixelPos = float4(i.viewPos, 1.0);
			float4 pixelPos = float4(i.viewPos + N * i.pointSize, 1.0);
		//	float4 pixelPos = float4(i.viewPos + N * _SphereRadius, 1.0);

		//	float4 pixelPos = float4(i.posEye + N * i.pointSize, 1.0);
		//	float4 pixelPos = float4(i.posEye + N * _SphereRadius, 1.0);
		//	float4 pixelPos = float4(i.posEye, 1.0);

			float4 clipSpacePos = mul(UNITY_MATRIX_P, pixelPos);


			float far = 50.0;
			float near = 0.3;
			float depth = clipSpacePos.z / clipSpacePos.w;
			
			//float depth = far*((clipSpacePos.z / clipSpacePos.w) + near)/((clipSpacePos.z / clipSpacePos.w)*(far-near));
			//float ndc_depth = clip_space_pos.z / clip_space_pos.w;

		//	float depth = (((far-near) * clipSpacePos.z / clipSpacePos.w) + near + far) / 2.0;
		//	float depth = (far-near) * 0.5 * (clipSpacePos.z / clipSpacePos.w) + (near + far) * 0.5;
		//	gl_FragDepth = (1.0 - 0.0) * 0.5 * f_ndc_depth + (1.0 + 0.0) * 0.5;
			//float depth = 1.0 - (i.projPos.z / i.projPos.w);
			//float depth = 1.0 - i.projPos.z;
			OUT.depth = depth;

			float3 lightDir = normalize(float3(1,1,1));
			float diffuse = max(0.0, dot(N, lightDir)) * 0.5 + 0.5;
		//	float diffuse = max(0.0, dot(N, lightDir));
		//	float linearDepth = Linear01Depth(depth);
		//	OUT.color = float4(linearDepth, linearDepth, linearDepth, 1);
			OUT.color = diffuse * i.color;
		//	OUT.color = float4(N, 1);

			return OUT;

		}
		ENDCG
	}
}

Fallback off

}