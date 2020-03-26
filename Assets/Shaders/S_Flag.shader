// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Flag"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,0)
		_Speed("Speed", Float) = 2
		_Strength("Strength", Float) = 2
		_Freq("Freq", Float) = 5
		_Emissive("Emissive", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			half filler;
		};

		uniform float _Speed;
		uniform float _Freq;
		uniform float _Strength;
		uniform float4 _Color;
		uniform float _Emissive;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float mulTime4 = _Time.y * _Speed;
			v.vertex.xyz += ( sin( ( mulTime4 + ( v.texcoord.xy.x * _Freq ) ) ) * float3(0,0.5,1) * _Strength * v.texcoord.xy.x );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = ( _Color * _Emissive ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15900
315;409;1298;610;611.4625;173.0046;1.414205;True;False
Node;AmplifyShaderEditor.RangedFloatNode;5;-679.3856,215.6846;Float;False;Property;_Speed;Speed;1;0;Create;True;0;0;False;0;2;-20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-487.8331,389.4987;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-360.5284,305.1981;Float;False;Property;_Freq;Freq;3;0;Create;True;0;0;False;0;5;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;4;-486.6988,188.8939;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-226.1568,334.4672;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-92.96305,185.0331;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;139.7073,93.66248;Float;False;Property;_Strength;Strength;2;0;Create;True;0;0;False;0;2;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;14;118.4197,189.4522;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-64,-262;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;1,1,1,0;1,0.1745282,0.257992,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;102.711,-58.45398;Float;False;Property;_Emissive;Emissive;4;0;Create;True;0;0;False;0;0;13.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;13;281.6306,518.295;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;0,0.5,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;9;171.6371,375.71;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;557.4702,233.9209;Float;False;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;283.7292,-195.6319;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;805.305,-80.41805;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Flag;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;5;0
WireConnection;15;0;8;1
WireConnection;15;1;16;0
WireConnection;7;0;4;0
WireConnection;7;1;15;0
WireConnection;14;0;7;0
WireConnection;10;0;14;0
WireConnection;10;1;13;0
WireConnection;10;2;11;0
WireConnection;10;3;8;1
WireConnection;17;0;1;0
WireConnection;17;1;18;0
WireConnection;0;0;17;0
WireConnection;0;11;10;0
ASEEND*/
//CHKSM=44D696AEF5BCCF532CB71E8C2C6F35BF22F478A8