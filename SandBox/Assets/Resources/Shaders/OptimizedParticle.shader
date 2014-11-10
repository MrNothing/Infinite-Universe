Shader "MrNothing's Shaders/Optimized Particle" {
	Properties {
		_Clouds ("Cloud Texture", 2D) = "white" {}
		_Noise ("Noise Texture", 2D) = "white" {}
		_Tint ("Clouds Tint", Color) = (1, 1, 1, 1)
		_SunColor ("Sun Color", Color) = (1, 1, 1, 1)
		_LightDir ("Light Direction", Vector) = (1,0,0,0)
		_AnimationSpeed ("Animation Speed", Float) = 2
		_Fading ("_Fading", Float) = 75
		_ShadowIntensity ("_ShadowIntensity", Float) = 0.65
		_Shininess ("_Shininess", Float) = 20
		_Scale ("Space Scale (see code)", Vector) = (1, 1, 1, 1)
		_Test ("_Test (DO NOT TOUCH)", Float) = 2
	}
	
	SubShader {
			Cull Off
			Tags {"Queue" = "Transparent+10000"} 
		Pass{
			
			ZWrite Off // don't write to depth buffer 
			//ZTest Less
			//AlphaTest Greater 0.3
			
			// in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha
			// blend based on the fragment's alpha value
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Clouds;
			uniform sampler2D _Noise;
			uniform float _AnimationSpeed;
			uniform float _Fading;
			uniform fixed4 _Tint;
			uniform fixed4 _SunColor;
			uniform float4 _LightDir;
			uniform float _ShadowIntensity;
			uniform float _Shininess;
			uniform fixed4 _Scale;
			uniform float _Test;

			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
				fixed4 color2:COLOR1;
				fixed4 normal:NORMAL;
				float2 texcoord:TEXCOORD0;
				float2 texcoord1:TEXCOORD1;
			};
			
			struct v2f 
			{
			  float4 pos : SV_POSITION;
			  fixed4 color : COLOR0;
			  float2 uv : TEXCOORD0;
			  float2 uv1 : TEXCOORD1;
			  fixed4 norm:COLOR1;
			 // float4 screenPos;
			};
			
			float4 _Clouds_ST;
			float4 _Noise_ST;
			
			v2f vert (input v)
			{
				v2f o;
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				float4 posWorld = mul(modelMatrix, v.vertex);
				float4 posObject = mul(modelMatrix, float4(0,0,0,0));
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				
				//v.vertex*=1+sin(posObject.x*0.1);
				
				o.norm.r = dot(viewDirection, _LightDir);
				
				//o.norm.r = o.norm.r*o.norm.r;
				
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				
				
				o.color = v.color;
				
				//Colors protocol:
				//r:[-0.5,0.5](real 0,1)*scale = x corrdinates of the center of my Vertex
				//g:[-0.5,0.5](real 0,1)*scale = y corrdinates of the center of my Vertex
				//b:[-0.5,0.5](real 0,1)*scale = z corrdinates of the center of my Vertex
				
				float x = (v.color.r-0.5)*_Scale.x*_Test;
				float y = (v.color.g-0.5)*_Scale.y*_Test;
				float z = (v.color.b-0.5)*_Scale.z*_Test;
				
				float3 center = float3(x, y, z);
				
				o.color = v.color;
				
				//v.vertex.xyz = v.vertex.xyz+(v.vertex-center)*_ParticleSize;
				
				float4 localPosition = v.vertex-float4(center, 1);
				
				localPosition = mul(UNITY_MATRIX_V, localPosition);
				
				o.norm.b = sin((center.x+center.y+center.z)/8);
				
				v.vertex = float4(center, 1)+localPosition;
				//o.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1))- float4(v.vertex.x, v.vertex.y, 0, 0.0));
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv =  v.texcoord.xy * _Clouds_ST.xy + _Clouds_ST.zw;
				o.uv1 =  v.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
				//note to myself: always use this to get world coordinates of a vertex.
				
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_Fading;
				if(ratio>1)
						ratio = 1;
				
				if(ratio<0.05)
					ratio = (ratio-0.025)/0.025;
				else
					ratio = 1;
				o.norm.a = ratio;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				i.uv1.x+=_Time*_AnimationSpeed;
				i.uv1.y+=_Time*_AnimationSpeed;
				
				float avgNoiseColor = tex2D (_Noise, i.uv1).r+tex2D (_Noise, i.uv1).g+tex2D (_Noise, i.uv1).b;
				
				fixed4 cloudTex = _Tint*(1-i.norm.g*i.norm.g)+_SunColor*i.norm.g*i.norm.g*4;
				cloudTex.a *= tex2D (_Clouds, i.uv).r*avgNoiseColor*_Tint*i.norm.a;
				
				i.norm.r = max(i.norm.r, _ShadowIntensity*i.color.a);
				
				cloudTex.rgb *= i.norm.r;
				//cloudTex.rgb+=i.norm.g*i.norm.g*tex2D (_Clouds, i.uv).r*_SunColor*4;
				//cloudTex.a*=i.norm.r*i.norm.r+0.2f;
				
				return cloudTex;
			}
			
			ENDCG
		}
		
	}
	
	FallBack "Diffuse"
}
