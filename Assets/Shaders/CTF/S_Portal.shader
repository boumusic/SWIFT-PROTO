// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Portal"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Tex2("Tex2", 2D) = "white" {}
		_Distort("Distort", Float) = 0
		_DF_Emissive("DF_Emissive", Float) = 0
		_DF_Distance("DF_Distance", Float) = 0
		_Sub("Sub", Float) = 0
		[HDR]_Color("Color", Color) = (1,1,1,0)
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Vector1("Vector 1", Vector) = (1,1,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _MainTex;
		uniform float4 _Tiling;
		uniform float4 _Vector1;
		uniform sampler2D _TextureSample0;
		uniform sampler2D _Sampler041;
		uniform float4 _TextureSample0_ST;
		uniform sampler2D _Sampler02;
		uniform float4 _MainTex_ST;
		uniform sampler2D _Tex2;
		uniform sampler2D _Sampler026;
		uniform float4 _Tex2_ST;
		uniform float4 _Color;
		uniform sampler2D _GrabTexture;
		uniform float _Sub;
		uniform float _Distort;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DF_Distance;
		uniform float _DF_Emissive;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord3 = i.uv_texcoord * _Tiling.xy + _Vector1.xy;
			float2 uv_TexCoord37 = i.uv_texcoord * _TextureSample0_ST.xy;
			float2 panner36 = ( 1.0 * _Time.y * _TextureSample0_ST.zw + uv_TexCoord37);
			float temp_output_39_0 = ( tex2D( _TextureSample0, panner36 ).r * 0.05 );
			float2 temp_output_11_0_g4 = ( uv_TexCoord3 + temp_output_39_0 );
			float2 break12_g4 = temp_output_11_0_g4;
			float2 break15_g4 = _MainTex_ST.zw;
			float mulTime16_g4 = _Time.y * break15_g4.x;
			float2 break20_g4 = _MainTex_ST.xy;
			float mulTime7_g4 = _Time.y * break15_g4.y;
			float4 appendResult5_g4 = (float4(( ( atan2( ( break12_g4.y + -0.5 ) , ( break12_g4.x + -0.5 ) ) + mulTime16_g4 ) * break20_g4.x ) , ( ( distance( temp_output_11_0_g4 , float2( 0.5,0.5 ) ) + mulTime7_g4 ) * break20_g4.y ) , 0.0 , 0.0));
			float2 uv_TexCoord24 = i.uv_texcoord * _Tiling.xy + _Vector1.xy;
			float2 temp_output_11_0_g5 = ( uv_TexCoord24 + temp_output_39_0 );
			float2 break12_g5 = temp_output_11_0_g5;
			float2 break15_g5 = _Tex2_ST.zw;
			float mulTime16_g5 = _Time.y * break15_g5.x;
			float2 break20_g5 = _Tex2_ST.xy;
			float mulTime7_g5 = _Time.y * break15_g5.y;
			float4 appendResult5_g5 = (float4(( ( atan2( ( break12_g5.y + -0.5 ) , ( break12_g5.x + -0.5 ) ) + mulTime16_g5 ) * break20_g5.x ) , ( ( distance( temp_output_11_0_g5 , float2( 0.5,0.5 ) ) + mulTime7_g5 ) * break20_g5.y ) , 0.0 , 0.0));
			float temp_output_28_0 = max( tex2D( _MainTex, appendResult5_g4.xy ).r , tex2D( _Tex2, appendResult5_g5.xy ).r );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 screenColor6 = tex2D( _GrabTexture, ( ase_screenPosNorm + ( ( temp_output_28_0 - _Sub ) * _Distort ) ).xy );
			float screenDepth13 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth13 = saturate( abs( ( screenDepth13 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DF_Distance ) ) );
			o.Emission = ( temp_output_28_0 * _Color * ( screenColor6 + ( ( 1.0 - distanceDepth13 ) * _DF_Emissive ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15900
119;329;1298;665;3185.206;240.3203;1.411288;True;False
Node;AmplifyShaderEditor.TextureTransformNode;41;-2442.869,358.0659;Float;False;35;1;0;SAMPLER2D;_Sampler041;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;37;-2338.433,274.7999;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;36;-2126.74,346.7756;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;31;-2084.1,-144.7677;Float;False;Property;_Tiling;Tiling;7;0;Create;True;0;0;False;0;1,1,0,0;0.25,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;33;-2100.407,68.84706;Float;False;Property;_Vector1;Vector 1;8;0;Create;True;0;0;False;0;1,1,0,0;0.37,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;35;-1928.488,234.5195;Float;True;Property;_TextureSample0;Texture Sample 0;9;0;Create;True;0;0;False;0;None;06bc81ca31f5f974da7a73f9a1528cb3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1604.564,288.9128;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1689.821,68.95267;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1621.698,-94.84857;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureTransformNode;26;-1578.684,395.664;Float;False;23;1;0;SAMPLER2D;_Sampler026;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.TextureTransformNode;2;-1571.321,-253.1817;Float;False;1;1;0;SAMPLER2D;_Sampler02;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-1459.201,195.7678;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-1367.467,-15.92533;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;30;-1104.52,-218.1044;Float;False;SF_RadialUV;-1;;4;11a46320daa475d48879fd69b6a77111;0;3;19;FLOAT2;1,1;False;11;FLOAT2;0,0;False;14;FLOAT2;1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;29;-1208.042,134.8209;Float;False;SF_RadialUV;-1;;5;11a46320daa475d48879fd69b6a77111;0;3;19;FLOAT2;1,1;False;11;FLOAT2;0,0;False;14;FLOAT2;1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;1;-856.6324,-113.7513;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;06bc81ca31f5f974da7a73f9a1528cb3;06bc81ca31f5f974da7a73f9a1528cb3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-934.973,70.78266;Float;True;Property;_Tex2;Tex2;1;0;Create;True;0;0;False;0;06bc81ca31f5f974da7a73f9a1528cb3;06bc81ca31f5f974da7a73f9a1528cb3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;12;-79.96454,216.8781;Float;False;Property;_Sub;Sub;5;0;Create;True;0;0;False;0;0;0.27;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;28;-245.682,29.47919;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;9;14.80707,60.56677;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-46.63111,566.4259;Float;False;Property;_DF_Distance;DF_Distance;4;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;45.80707,205.5668;Float;False;Property;_Distort;Distort;2;0;Create;True;0;0;False;0;0;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;13;146.3649,336.137;Float;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;199.8071,131.5668;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;7;-159.8018,-180.5285;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;19;471.403,324.2774;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;247.2921,-16.37085;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;15;243.8679,534.4767;Float;False;Property;_DF_Emissive;DF_Emissive;3;0;Create;True;0;0;False;0;0;2.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;6;446.0982,69.57152;Float;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;625.0135,324.8172;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;639.3513,184.2939;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;21;182.9659,-220.4418;Float;False;Property;_Color;Color;6;1;[HDR];Create;True;0;0;False;0;1,1,1,0;2.981444,1.758288,6.498019,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;740.5529,-9.415573;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;34;-1829.719,5.251787;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;-4,-4,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1124.348,90.40193;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Portal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;37;0;41;0
WireConnection;36;0;37;0
WireConnection;36;2;41;1
WireConnection;35;1;36;0
WireConnection;39;0;35;1
WireConnection;24;0;31;0
WireConnection;24;1;33;0
WireConnection;3;0;31;0
WireConnection;3;1;33;0
WireConnection;38;0;24;0
WireConnection;38;1;39;0
WireConnection;40;0;3;0
WireConnection;40;1;39;0
WireConnection;30;19;2;0
WireConnection;30;11;40;0
WireConnection;30;14;2;1
WireConnection;29;19;26;0
WireConnection;29;11;38;0
WireConnection;29;14;26;1
WireConnection;1;1;30;0
WireConnection;23;1;29;0
WireConnection;28;0;1;1
WireConnection;28;1;23;1
WireConnection;9;0;28;0
WireConnection;9;1;12;0
WireConnection;13;0;16;0
WireConnection;10;0;9;0
WireConnection;10;1;11;0
WireConnection;19;0;13;0
WireConnection;8;0;7;0
WireConnection;8;1;10;0
WireConnection;6;0;8;0
WireConnection;14;0;19;0
WireConnection;14;1;15;0
WireConnection;17;0;6;0
WireConnection;17;1;14;0
WireConnection;20;0;28;0
WireConnection;20;1;21;0
WireConnection;20;2;17;0
WireConnection;34;0;31;0
WireConnection;0;2;20;0
ASEEND*/
//CHKSM=775D1D9FD6D197A0CDC565397E5CE436ACB15DB3