// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ScreenSpaceFluids/SSF_BlurDepth" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DepthTex ("Base (RGB)", 2D) = "white" {}
		scaleX ("ScaleX", Float) = 0.1
		scaleY ("ScaleY", Float) = 0.1
		radius ("Radius", Int) = 5
		minDepth ("MinDepth", Float) = 0
		blurDepthFalloff ("Falloff", Float) = 2.0
	}
	SubShader 
	{
		Tags {"RenderType"="Opaque"}
		Pass 
		{
			Cull Off 
			ZWrite Off 
			ZTest Always
		//	ZTest LEqual
			Fog { Mode Off }

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			//#pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _DepthTex;

			float blurDepthFalloff;
			float scaleX;
			float scaleY;
			float minDepth;
			int radius;

			struct v2f 
			{
				float4 pos : POSITION;	
				float2 coord : TEXCOORD0;
			};

			struct fragOut 
			{
				float color : COLOR;	
			//	float depth : DEPTH;
			};
 
			v2f vert(appdata_img v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos (v.vertex);
			//	o.coord = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				o.coord = v.texcoord.xy;


				return o;
			}
 

			fragOut frag (v2f i)
			{

				fragOut OUT;
				float depth =  tex2D(_DepthTex,  i.coord);
				//float depth = Linear01Depth (tex2D(_DepthTex,  i.coord).x);
				//float depth = tex2D(_DepthTex,  i.coord).r;
				//float depth = DecodeFloatRGBA(tex2D(_DepthTex,  i.coord));

				//if (depth < minDepth) 
				//{
				//	OUT.color = EncodeFloatRGBA(depth);
				//	return OUT;
				//}

				//if(depth >= 1.0)
				//{
				//	OUT.color = EncodeFloatRGBA(1.0f);
				//	return OUT;
				// }

				
				float blurScale = 2.0 / radius;

				float sum = 0.0;
				float wsum = 0.0;
				
	
			//	for(int x = -radius; x <= radius; x+=1) 
				for(int x = -50; x <= 50; x++) 
				{
					float cur = tex2D(_DepthTex,  i.coord + (float)x * float2(scaleX, scaleY));
				//	float cur = tex2D(_DepthTex,  i.coord + (float)x * float2(scaleX, scaleY)).r;
			//		float cur = Linear01Depth (tex2D(_DepthTex,  i.coord + (float)x * float2(scale, scale)).x);

					//float cur = DecodeFloatRGBA(tex2D(_DepthTex,  i.coord + (float)x * float2(scaleX, scaleY)));
					
					//// range domain
					float r2 = (depth - cur) * blurDepthFalloff;
					float g = exp(-r2*r2);

					//// spatial domain
					float r = (float)x * blurScale;
					float w = exp(-r*r);
					//float w = exp(-r*r/2.0);

					//g = 1.0;

					sum += cur * w * g;
					wsum += w * g;


				}

				 // for(int r = -radius; r <= radius; r+=1) 
				 // {
					//float sample = tex2D(texSampler, uv + (float)r*direction*texelSize).x;

					//// spatial domain
					//float v = (float)r/sigma;
					//float w = exp(-v*v/2.);
					//sum += sample * w;
					//wsum += w;
				 // }
				
				if (wsum > 0.0) 
				{
					sum /= wsum;
				}

			//	OUT.depth = sum;
				OUT.color = sum;
			//	sum = clamp(sum, 0.0, 0.999);
			//	OUT.color = EncodeFloatRGBA(sum);


				return OUT;
 

			}
			ENDCG
		} 
	}
	FallBack Off

}
