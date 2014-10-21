Shader "MrNothing's Shaders/Tree" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _WindAmplitude("Wind Amplitude", Float) = 1
    _AmplitudeOffset("Amplitude Offset", Float) = 0.2
    _WindSpeed("Wind Speed", Float) = 5
    _RotationOffset("Rotation Offset", Float) = 0
    _AmbientShadow("Ambient Shadow", Range(0, 2)) = 1
    _AmbientLight("Ambient Light", Range(0, 1)) = 1
    _CutoutOffset("Cutout Offset", Range(0, 1)) = 0.05
    _randomnessOffset("Randomness", Float) = 1
    _LodStart("Lod Distance", Float) = 100
    _LevelZero("Level Zero", Float) = 3
	_LightDir ("Light Direction", Vector) = (0,0,0,0)
}
 				
SubShader {
    Tags {"RenderType"="Transparent" "Queue"="Transparent"}
    // Render normally
    
      Pass {
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
			uniform float _WindAmplitude;
			uniform float _AmplitudeOffset;
			uniform float _WindSpeed;
			uniform float _RotationOffset;
			uniform float _AmbientShadow;
			uniform float _AmbientLight;
			uniform float _CutoutOffset;
			uniform float _randomnessOffset;
			uniform float _LodStart;
			uniform float _LevelZero;
			uniform float4 _LightDir;
			
			struct input
			{
				float4 vertex:POSITION;
				float4 normal:NORMAL;
				fixed4 color:COLOR0;
				float2 texcoord:TEXCOORD0;
			};
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	          float realAlpha : COLOR1;
	     	};
	     	
	     	float4 _MainTex_ST;
			
			float4 pivotPointWithAxisXFromCenter(float4 input, float angle)
			{
				float Rotated_z = input.z * cos( angle ) - input.y * sin( angle );
				float Rotated_y = input.z * sin( angle ) + input.y * cos( angle ); 
				return float4(input.x, Rotated_y, Rotated_z, input.w);
			}
			
			
			
			v2f vert (input v)
			{
				v2f o;
				
				float4 posWorld = mul(_Object2World, v.vertex);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_LodStart;
				
				float randomness = (posWorld.x+posWorld.y+posWorld.z)*_randomnessOffset;
				
				if(v.vertex.y>_LevelZero)
				{
					v.vertex = pivotPointWithAxisXFromCenter(v.vertex, abs(sin(_Time.x*_WindSpeed+randomness*_AmplitudeOffset)*_WindAmplitude)+_RotationOffset);
					v.color.rgb *=0.1+_AmbientLight*abs(sin(randomness));
					
					float featherRatio = (abs(v.vertex.x)+abs(v.vertex.z))/0.05;
					
					if(featherRatio>1)
						featherRatio = 1;
					o.realAlpha = 0;
					v.color.rgb *= featherRatio;
				}
				else
				{
					o.realAlpha = 1;
					float featherRatio = (v.vertex.y)/_LevelZero*dot(v.normal, _LightDir);
					
					if(featherRatio<_AmbientLight)
						featherRatio = _AmbientLight;
						
					v.color.rgb *= featherRatio;
					//v.color.rgb *= _AmbientShadow*(0.5+abs(sin(randomness*_AmplitudeOffset))/2);
				}
				
				
				//v.vertex*=1-ratio*ratio;
					
				//v.color.a *=1-ratio*ratio;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.color = v.color;
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				fixed4 colo=tex2D(_MainTex, i.uv)*i.color*_Color;
				if(colo.a>_CutoutOffset)
					colo.a = 1;
				
				//if(i.realAlpha>0)
				//colo.a=1;
				
				return colo;
			}
		
		ENDCG
    }
    
    Pass {
    	ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
       	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float _WindAmplitude;
			uniform float _AmplitudeOffset;
			uniform float _WindSpeed;
			uniform float _RotationOffset;
			uniform float _AmbientShadow;
			uniform float _AmbientLight;
			uniform float _CutoutOffset;
			uniform float _randomnessOffset;
			uniform float _LodStart;
			uniform float _LevelZero;
			uniform float4 _LightDir;
			
			struct input
			{
				float4 vertex:POSITION;
				float4 normal:NORMAL;
				fixed4 color:COLOR0;
				float2 texcoord:TEXCOORD0;
			};
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	          float realAlpha : COLOR1;
	     	};
	     	
	     	float4 _MainTex_ST;
			
			float4 pivotPointWithAxisXFromCenter(float4 input, float angle)
			{
				float Rotated_z = input.z * cos( angle ) - input.y * sin( angle );
				float Rotated_y = input.z * sin( angle ) + input.y * cos( angle ); 
				return float4(input.x, Rotated_y, Rotated_z, input.w);
			}
			
			
			
			v2f vert (input v)
			{
				v2f o;
				
				float4 posWorld = mul(_Object2World, v.vertex);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_LodStart;
				
				float randomness = (posWorld.x+posWorld.y+posWorld.z)*_randomnessOffset;
				
				if(v.vertex.y>_LevelZero)
				{
					v.vertex = pivotPointWithAxisXFromCenter(v.vertex, abs(sin(_Time.x*_WindSpeed+randomness*_AmplitudeOffset)*_WindAmplitude)+_RotationOffset);
					v.color.rgb *=0.1+_AmbientLight*abs(sin(randomness));
					
					float featherRatio = (abs(v.vertex.x)+abs(v.vertex.z))/0.05;
					
					if(featherRatio>1)
						featherRatio = 1;
					o.realAlpha = 0;
					v.color.rgb *= featherRatio;
				}
				else
				{
					o.realAlpha = 1;
					
					float featherRatio = (v.vertex.y)/_LevelZero*dot(v.normal, _LightDir);
					
					if(featherRatio<_AmbientLight)
						featherRatio = _AmbientLight;
					v.color.rgb *= featherRatio;	
					//v.color.rgb *= _AmbientShadow*(0.5+abs(sin(randomness*_AmplitudeOffset))/2);
				}
				
				
					
				//v.vertex*=1-ratio*ratio;
					
				//v.color.a *=1-ratio*ratio;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.color = v.color;
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				fixed4 colo=tex2D(_MainTex, i.uv)*i.color*_Color;
				if(colo.a>_CutoutOffset)
					colo.a = 1;
				
				//if(i.realAlpha>0)
				//colo.a=1;
				
				return colo;
			}
		
		ENDCG
    }
}
}