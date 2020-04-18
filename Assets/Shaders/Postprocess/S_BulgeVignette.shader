Shader "Hidden/Custom/BulgeVignette"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_NoiseTex, sampler_NoiseTex);
	float4 _Color;

	float _Distort;
	float _DistortSub;
	float _Threshold;
	float _Smoothness;
	float4 _Tiling;
	float4 _Tiling2;

	float4 Frag(VaryingsDefault i) : SV_Target
	{	
		float2 uv = i.texcoord;
		float left = smoothstep(_Threshold, _Threshold + _Smoothness, uv.x);
		float right = smoothstep(_Threshold, _Threshold + _Smoothness, 1 - uv.x);

		float bot = smoothstep(_Threshold, _Threshold + _Smoothness, uv.y);
		float top = smoothstep(_Threshold, _Threshold + _Smoothness, 1- uv.y);

		float finalVignette = 1 - (left * right * bot * top);

		float u = ((atan2(uv.x - 0.5, uv.y - 0.5) / PI) + 1) / 2;
		float v = distance(uv, float2(0.5,0.5));
		float2 radialUv = float2(u * _Tiling.x + _Tiling.z * _Time.y, v * _Tiling.y + _Tiling.w * _Time.y);
		float2 radialUv2 = float2(u * _Tiling2.x + _Tiling2.z * _Time.y, v * _Tiling2.y + _Tiling2.w * _Time.y);


		float4 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, radialUv);
		float4 noise2 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, radialUv2);

		float4 noises = noise * noise2 * finalVignette * _Color;

		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + (noises - _DistortSub) * _Distort);

		return color + noises;
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag

			ENDHLSL
		}
	}
}