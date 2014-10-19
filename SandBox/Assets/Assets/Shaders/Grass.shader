Shader "MrNothing's Shaders/Grass" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _WindAmplitude("Wind Amplitude", Float) = 1
    _AmplitudeOffset("Amplitude Offset", Float) = 0.2
    _WindSpeed("Wind Speed", Float) = 5
    _RotationOffset("Rotation Offset", Float) = 0
    _AmbientShadow("Ambient Shadow", Range(0, 2)) = 1
    _AmbientLight("Ambient Light", Range(0, 1)) = 1
    _LodStart("Lod Start Distance", Float) = 30
    _Randomness("Randomness", Float) = 1
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
			uniform float _LodStart;
			uniform float _Randomness;
		
			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
				float2 texcoord:TEXCOORD0;
			};
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	     	};
	     	
	     	float4 _MainTex_ST;
			
			float4 pivotPointWithAxisXFromCenter(float4 input, float angle)
			{
				float Rotated_z = input.z * cos( angle ) - input.y * sin( angle );
				float Rotated_y = input.z * sin( angle ) + input.y * cos( angle ); 
				return float4(input.x, Rotated_y, Rotated_z, input.w);
			}
			
			float4 pivotPointWithAxisYFromCenter(float4 input, float angle)
			{
				float Rotated_x = input.x * cos( angle ) - input.z * sin( angle );
				float Rotated_z = input.x * sin( angle ) + input.z * cos( angle );
				return float4(Rotated_x, input.y, Rotated_z, input.w);
			}
			
			float4 pivotPointWithAxisZFromCenter(float4 input, float angle)
			{
				float Rotated_x = input.x * cos( angle ) - input.y * sin( angle );
				float Rotated_y = input.x * sin( angle ) + input.y * cos( angle );
				return float4(Rotated_x, Rotated_y, input.z, input.w);
			}
			
			v2f vert (input v)
			{
				v2f o;
				
				float4 posWorld = mul(_Object2World, v.vertex);
				
				float randomness = (posWorld.x+posWorld.y+posWorld.z);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_LodStart;
				
				if(ratio<1)
				{
					if(v.vertex.y>0.75)
					{
						v.vertex = pivotPointWithAxisXFromCenter(v.vertex, abs(sin(_Time.x*_WindSpeed+randomness*_AmplitudeOffset)*_WindAmplitude)+_RotationOffset);
						
						v.vertex = pivotPointWithAxisXFromCenter(v.vertex, sin(randomness/2)*_Randomness);
						
						
						v.color.rgb *=_AmbientLight;
					}
					else
						v.color.rgb *= _AmbientShadow*(0.5+abs(sin(randomness*_AmplitudeOffset))/2);
					
					v.color.a *=1-ratio*ratio*ratio;
				}
				else
				{
					v.color = fixed4(0, 0, 0, 0);
				}
				//v.vertex*=1-ratio*ratio;
					
				
				
				
				 o.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))- float4(v.vertex.x, v.vertex.z, 0.0, 0.0));
              
				//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.color = v.color;
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
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