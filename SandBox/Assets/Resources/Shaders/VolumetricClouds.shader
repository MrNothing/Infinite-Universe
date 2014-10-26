Shader "MrNothing's Shaders/Volumetric Clouds" {
	Properties {
		_Layer1 ("Noise", 2D) = "white" {}
		_Clouds ("Cloud Texture", 2D) = "white" {}
		_Tint ("Clouds Tint", Color) = (1, 1, 1, 1)
		_SunColor ("Sun Color", Color) = (1, 1, 1, 1)
		_AnimationSpeed ("Animation Speed", Float) = 1
		_SunPos ("Sun Position", Vector) = (0,0,0,0)
		_FadeAlpha ("_FadeAlpha", Float) = 1
		_Amplification ("_Amplification", Float) = 1
		_Fading ("_Fading", Float) = 1
		_CloudLayersOffset ("_CloudLayersOffset", Float) = 0.5
		_CloudsHeight ("_CloudsHeight", Float) = 1
		_Shininess ("Shininess", Range(0.5, 10)) = 0
	}
	
	SubShader {
		Fog{Mode Off}
		Pass{
			Tags {"Queue" = "Transparent" "RenderType"="Transparent"} 
			Cull Front
			ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha 
           	ColorMask RGB
			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Layer1;
			uniform sampler2D _Clouds;
			uniform float _AnimationSpeed;
			uniform float _FadeAlpha;
			uniform float _Amplification;
			uniform float _Fading;
			uniform fixed4 _SunColor;
			uniform fixed4 _Tint;
			uniform float4 _SunPos;
			uniform float4 _Offset;
			uniform float _CloudLayersOffset;
			uniform float _CloudsHeight;
			uniform float _Shininess;
			
			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
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
	     	
	     	float4 _Layer1_ST;
	     	float4 _Clouds_ST;
			
			v2f vert (input v)
			{
				v2f o;
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				float4 posWorld = mul(modelMatrix, v.vertex);
				float4 _LightDir = normalize(_SunPos-posWorld);
				
				float amp = (1-v.color.r)+_CloudLayersOffset;
				if(amp>1)
					amp = 1;
				v.vertex.xyz*=1-((1-amp*amp)/10)*_CloudsHeight;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv =  v.texcoord.xy * _Layer1_ST.xy + _Layer1_ST.zw/2;
				o.uv1 = TRANSFORM_TEX (v.texcoord1, _Clouds);
				//note to myself: always use this to get world coordinates of a vertex.
				o.norm.r = dot(v.normal, _LightDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_Fading;
				if(ratio>1)
					ratio = 1;
					
				o.norm.b = (1-(1-ratio)*(1-_FadeAlpha)*_Amplification);
				
				o.norm.a = dot(v.normal, viewDirection);
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				i.uv1.x+=_Time*_AnimationSpeed/2;
				i.uv1.y+=_Time*_AnimationSpeed/2;
				
				fixed4 cloudTex = tex2D (_Clouds, i.uv1).r*_Tint+_Tint/2;
				
				i.uv.x+=_Time*_AnimationSpeed;
				i.uv.y+=_Time*_AnimationSpeed;
				
				fixed4 cloudTex2 = tex2D (_Clouds, i.uv)*_Tint/2;
				
				fixed4 cloudColor;
				cloudColor = cloudTex+_SunColor*(i.norm.g)*5;
				cloudColor.rgb*=(1+i.color.r)*(i.norm.r);
					
				//cloudColor.a += i.color.a*i.norm.g;
				cloudColor.a *= (i.color.r)*i.norm.b;
				
				
				return cloudColor;
			}
			
			ENDCG
		}
		
		
	
		Pass{
			Tags {"Queue" = "Transparent" "RenderType"="Transparent"} 
			Cull Back
			ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha 
           	ColorMask RGB
			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Layer1;
			uniform sampler2D _Clouds;
			uniform float _AnimationSpeed;
			uniform float _FadeAlpha;
			uniform float _Amplification;
			uniform float _Fading;
			uniform fixed4 _SunColor;
			uniform fixed4 _Tint;
			uniform float4 _SunPos;
			uniform float4 _Offset;
			uniform float _CloudLayersOffset;
			uniform float _CloudsHeight;
			uniform float _Shininess;
			
			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
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
	     	
	     	float4 _Layer1_ST;
	     	float4 _Clouds_ST;
			
			v2f vert (input v)
			{
				v2f o;
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				float4 posWorld = mul(modelMatrix, v.vertex);
				float4 _LightDir = normalize(_SunPos-posWorld);
				
				float amp = (1-v.color.r)+_CloudLayersOffset;
				if(amp>1)
					amp = 1;
				v.vertex.xyz*=1+((1-amp*amp)/10)*_CloudsHeight;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv =  v.texcoord.xy * _Layer1_ST.xy + _Layer1_ST.zw/2;
				o.uv1 = TRANSFORM_TEX (v.texcoord1, _Clouds);
				//note to myself: always use this to get world coordinates of a vertex.
				o.norm.r = dot(v.normal, _LightDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_Fading;
				if(ratio>1)
					ratio = 1;
					
				o.norm.b = (1-(1-ratio)*(1-_FadeAlpha)*_Amplification);
				
				o.norm.a = dot(v.normal, viewDirection);
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				i.uv1.x+=_Time*_AnimationSpeed/2;
				i.uv1.y+=_Time*_AnimationSpeed/2;
				
				fixed4 cloudTex = tex2D (_Clouds, i.uv1).r*_Tint+_Tint/2;
				
				i.uv.x+=_Time*_AnimationSpeed;
				i.uv.y+=_Time*_AnimationSpeed;
				
				fixed4 cloudTex2 = tex2D (_Clouds, i.uv)*_Tint/2;
				
				fixed4 cloudColor;
				cloudColor = cloudTex+_SunColor*(i.norm.g)*5;
				cloudColor.rgb*=(1+i.color.r)*(i.norm.r);
					
				//cloudColor.a += i.color.a*i.norm.g;
				cloudColor.a *= (i.color.r)*i.norm.b;
				
				
				return cloudColor;
			}
			
			ENDCG
		}
		
		
	} 
	FallBack "Diffuse"
}
