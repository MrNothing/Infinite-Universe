Shader "MrNothing's Shaders/VertexColored Fade" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Range ("Range", Float) = 100
    _DisappearSpeed ("_DisappearSpeed", Float) = 1
    _LightDir ("Light Direction", Vector) = (0,0,0,0)
    _Shininess ("Shininess", Range(0.5, 10)) = 0
}
 
SubShader {
    Tags {"RenderType"="Transparent" "Queue"="Transparent"}
    // Render into depth buffer only
    //Pass {
    //    ColorMask 0
    //}
    // Render normally
     Pass {
     	Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
       	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float _Range;
			uniform float _DisappearSpeed;
			uniform float4 _LightDir;
			uniform float _Shininess;
		
			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
				fixed4 normal:NORMAL;
				float2 texcoord:TEXCOORD0;
			};
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	          float2 spec:TEXCOORD1;
	     	};
	     	
	     	float4 _MainTex_ST;
			
			v2f vert (input v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float3 camPos =  ObjSpaceViewDir(v.vertex);
				float camRatio = distance(camPos.xyz, o.pos)/_Range;
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				float spec = pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				o.color = (v.color+v.color*spec)/2;
				
				o.color.a = camRatio+_DisappearSpeed;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				return tex2D(_MainTex, i.uv)*i.color*_Color;
			}
		
		ENDCG
    }
    Pass {
    	Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
       	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float _Range;
			uniform float _DisappearSpeed;
			uniform float4 _LightDir;
			uniform float _Shininess;
		
			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
				fixed4 normal:NORMAL;
				float2 texcoord:TEXCOORD0;
			};
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	          float2 spec:TEXCOORD1;
	     	};
	     	
	     	float4 _MainTex_ST;
			
			v2f vert (input v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float3 camPos =  ObjSpaceViewDir(v.vertex);
				float camRatio = distance(camPos.xyz, o.pos)/_Range;
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				float spec = pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				o.color = (v.color+v.color*spec)/2;
				
				o.color.a = camRatio+_DisappearSpeed;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				return tex2D(_MainTex, i.uv)*i.color*_Color;
			}
		
		ENDCG
    }
}
}