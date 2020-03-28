Shader "Hidden/Custom/CustomVignette"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_NoiseTex, sampler_NoiseTex);
	float4 _Color;
	float _Threshold;
	float _Smoothness;
	float _Tiling;
	float _Scroll;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float left = smoothstep(_Threshold, _Threshold + _Smoothness, i.texcoord.x);
		float right = smoothstep(_Threshold, _Threshold + _Smoothness, 1-  i.texcoord.x);

		float bot = smoothstep(_Threshold, _Threshold + _Smoothness, i.texcoord.y);
		float top = smoothstep(_Threshold, _Threshold + _Smoothness, 1 - i.texcoord.y);

		float finalVignette = 1 - (left * right * bot * top);

		float2 coords = normalize(i.texcoord - 0.5) * _Tiling;
		float4 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, coords + _Time.y * _Scroll) * finalVignette;

		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

		float4 final = color + noise * _Color;
		return final;
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