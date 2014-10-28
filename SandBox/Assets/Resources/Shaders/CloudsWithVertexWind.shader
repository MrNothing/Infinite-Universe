Shader "MrNothing's Shaders/Clouds with vertex wind" {
	Properties {
		_Layer1 ("Noise", 2D) = "white" {}
		_Clouds ("Cloud Texture", 2D) = "white" {}
		_Tint ("Clouds Tint", Color) = (1, 1, 1, 1)
		_SunColor ("Sun Color", Color) = (1, 1, 1, 1)
		_AnimationSpeed ("Animation Speed", Float) = 1
		_LightDir ("Light Direction", Vector) = (0,0,0,0)
		_FadeAlpha ("_FadeAlpha", Float) = 1
		_Amplification ("_Amplification", Float) = 1
		_Fading ("_Fading", Float) = 1
		_Offset ("Outline Offset", Vector) = (0,0,0,0)
	}
	
	SubShader {
	
	  	Fog {Mode Off}
		Pass{
			Cull Back
			Tags {"Queue" = "Transparent+30000" "RenderType"="Transparent"} 
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
			uniform float4 _LightDir;
			uniform float4 _Offset;

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
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv =  v.texcoord.xy * _Layer1_ST.xy + _Layer1_ST.zw/2;
				o.uv1 = TRANSFORM_TEX (v.texcoord1, _Clouds);
				//note to myself: always use this to get world coordinates of a vertex.
				o.norm.r = dot(v.normal, _LightDir);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_Fading;
				if(ratio>1)
						ratio = 1;
					
				o.norm.b = (1-(1-ratio)*(1-_FadeAlpha)*_Amplification);
					
				//outline
				float3 normalDir =  normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0), modelMatrixInverse).xyz);
				
				float3 normalDirection = normalize(normalDir);
 
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), 1.5);
				
				float viewIntensity = dot(normalDirection, viewDirection);
				
				if(viewIntensity<_Offset.z)
					viewIntensity = viewIntensity;
				else
					viewIntensity = _Offset.z*2-viewIntensity;
				//end outline
				
				o.norm.a = viewIntensity/_Offset.x+_Offset.y;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				i.uv.x+=_Time*_AnimationSpeed/2;
				i.uv.y+=_Time*_AnimationSpeed/2;
				
				i.uv1.x+=_Time*_AnimationSpeed/2;
				i.uv1.y+=_Time*_AnimationSpeed/2;
				
				fixed4 noiseTex = tex2D (_Layer1, i.uv);
				fixed4 cloudTex = tex2D (_Clouds, i.uv1);
				
				noiseTex = noiseTex*cloudTex;
			
				float noiseFactor = (noiseTex.r+noiseTex.g+noiseTex.b)/3;
				
				fixed4 cloudColor = _Tint;
				cloudColor = cloudColor+_SunColor*noiseFactor*i.norm.g*i.norm.g*(0.5+i.norm.r/2)*15;
				cloudColor.rgb *= (i.norm.r*2-1)+i.norm.g*i.norm.a;
				cloudColor.a *= i.norm.b*i.norm.a+i.norm.g*i.norm.g*10*i.norm.a;
				
				return cloudColor;
			}
			
			ENDCG
		}
		
		Pass{
			Cull Front
			Tags {"Queue" = "Transparent" "RenderType"="Transparent"} 
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
			uniform float4 _LightDir;
			uniform float4 _Offset;

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
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv =  v.texcoord.xy * _Layer1_ST.xy + _Layer1_ST.zw/2;
				o.uv1 = TRANSFORM_TEX (v.texcoord1, _Clouds);
				//note to myself: always use this to get world coordinates of a vertex.
				o.norm.r = dot(v.normal, _LightDir);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_Fading;
				if(ratio>1)
						ratio = 1;
					
				o.norm.b = (1-(1-ratio)*(1-_FadeAlpha)*_Amplification);
					
				//outline
				float3 normalDir =  normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0), modelMatrixInverse).xyz);
				
				float3 normalDirection = normalize(normalDir);
 
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), 1);
				
				float viewIntensity = dot(normalDirection, viewDirection);
				
				if(viewIntensity<_Offset.z)
					viewIntensity = viewIntensity;
				else
					viewIntensity = _Offset.z*2-viewIntensity;
				//end outline
				
				o.norm.a = viewIntensity/_Offset.x+_Offset.y;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				i.uv.x+=_Time*_AnimationSpeed/2;
				i.uv.y+=_Time*_AnimationSpeed/2;
				
				i.uv1.x+=_Time*_AnimationSpeed/2;
				i.uv1.y+=_Time*_AnimationSpeed/2;
				
				fixed4 noiseTex = tex2D (_Layer1, i.uv);
				fixed4 cloudTex = tex2D (_Clouds, i.uv1);
				
				noiseTex = noiseTex*cloudTex;
			
				float noiseFactor = (noiseTex.r+noiseTex.g+noiseTex.b)/3;
				
				fixed4 cloudColor = _Tint;
				cloudColor = cloudColor+_SunColor*noiseFactor*i.norm.g*i.norm.g*(0.5+i.norm.r/2)*15;
				cloudColor.rgb *= (i.norm.r*2-1)+i.norm.g*i.norm.a;
				cloudColor.a *= i.norm.b*i.norm.a+i.norm.g*i.norm.g*10*i.norm.a;
				
				return cloudColor;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
