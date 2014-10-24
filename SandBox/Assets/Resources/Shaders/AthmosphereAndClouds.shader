Shader "MrNothing's Shaders/Athmosphere and Clouds" {
	Properties {
		_Color ("Planet Color", Color) = (1,1,1,1)
		_Sun ("Sun Direction", Vector) = (0,0,0,0)
		_FadeDistance ("Fade Distance", Float) = 10
		_HorizonDistance1 ("Horizon Distance", Float) = 700
		_SpecularAmplification("Specular Amplification", Float) = 1
		_SunAmplification("Sun Amplification", Float) = 1
		_Offset ("_Offset" , Vector) = (0,0,0,0)
		
		_Layer1 ("Cloud Noise", 2D) = "white" {}
		_Clouds ("Cloud Texture", 2D) = "white" {}
		_CloudTint ("Clouds Tint", Color) = (1, 1, 1, 1)
		_SunColor ("Sun Color", Color) = (1, 1, 1, 1)
		_LightDir ("Light Direction", Vector) = (0,0,0,0)
		_AnimationSpeed ("Animation Speed", Float) = 1
		_Amplification ("_Amplification", Float) = 1
		_Fading ("_Fading", Float) = 1
	}
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
 		
 		LOD 200
		
		Fog {Mode Off}		
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			 
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members vpos)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _Color;
			uniform float4 _Sun;
			uniform float _FadeDistance;
			uniform float _SpecularAmplification;
			uniform float _SunAmplification;
			uniform float _HorizonDistance1;
			uniform float4 _Offset;
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	          float4 vertex;
	          // float4 screenPos;
	     	};
	     	
	     	float4 _MainTex_ST;
	     	
	     	fixed4 getAtmosphereColor(float4 vertex, float4 opos)
	     	{
	     		float3 camPos =  ObjSpaceViewDir(vertex);
				
				float sunspecular = dot(camPos.xyz, _Sun.xyz);
				float sunIntensity = dot(_Sun.xyz, vertex.xyz);
				float realDistance = distance(camPos.xyz, opos);
				float camRatio = distance(camPos.xyz, opos)/_FadeDistance;
				float camRatio2 = distance(camPos.xyz, opos)/_HorizonDistance1;
				float camVertexRatio = dot(camPos.xyz, opos);
				
				float sunSetFact = 0;
				
				//if(sunspecular<0)
				//	sunSetFact = sunspecular;
				
				//sunspecular = abs(sunspecular);
				
				float specularCanceler = 0;
				
				float sunspecularP = 0;
				
				if(sunspecular>0.5)
				{
					camRatio*=0.5+sunspecular;
					sunspecular = 0.5;
				}
				
				if(sunspecular<-0.5)
					sunspecular = -0.5;
				
				if(sunspecular<0)
				{
					sunIntensity*=1;
				}
				else
					sunspecularP = sunspecular;
				
				sunspecular*=_SpecularAmplification;
				sunIntensity*=_SunAmplification;
				
				
				if(camRatio>1)
					camRatio=1;
					
				if(camRatio<0.1)
					camRatio=0.1;
					
				if(camRatio2<0.1)
					camRatio2=0.1;
				
				if(camRatio2>1)
					camRatio2=1;
					
				if(camRatio2>1)
					camRatio2=1;
				
				float horizon = 0;
				if(camRatio2>0.4 && camRatio2<0.7)
					horizon = (camRatio2-0.4)*3;
				
				if(camRatio2>0.7 && camRatio2<0.9)
					horizon = (0.9-(camRatio2-0.7))*0.3*3;
				if(realDistance>_HorizonDistance1/2)
				{	
					horizon*=1-(realDistance-_HorizonDistance1*0.5)/(_HorizonDistance1*0.5);
					//camRatio2*=1-(realDistance-_HorizonDistance*0.5)/(_HorizonDistance*0.5);
				}
				
				//horizon*=(1-sunspecularP*2)
				horizon=0;
				
				fixed4 sunColor = fixed4(sunIntensity-(1-camRatio2)+horizon, sunIntensity+sunspecular/2-sunSetFact*(1-camRatio)/2-(1-camRatio2)/4+horizon, sunIntensity+sunspecular-sunSetFact*(1-camRatio)+horizon, camRatio-0.2f-specularCanceler);
				
				if(sunColor.r>1)
					sunColor.r = 1;
				
				if(sunColor.g>1)
					sunColor.g = 1;
				
				if(sunColor.b>1)
					sunColor.b = 1;
					
				if(sunColor.a>1)
					sunColor.a = 1;
					
				float rangeI = 0.7f;
				if(sunColor.r+sunColor.g+sunColor.b>rangeI*3f)
				{
					sunColor*=1+(sunColor.r+sunColor.g+sunColor.b-rangeI*3f);
				}	
					
				if(sunspecular<0)
				{
					
				}
				else
				{
					/*if(sunColor.r>0.8f)
						sunColor.a-=(sunColor.r-0.8f)/2;
					
					if(sunColor.g>0.8f)
						sunColor.a-=(sunColor.g-0.8f)/2;
					
					if(sunColor.b>0.8f)
						sunColor.a-=(sunColor.b-0.8f)/2;*/
				}
				
				return sunColor;
	     	}
	     	
	 		v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				//outline
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 normalDir =  normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0), modelMatrixInverse).xyz);
				
				float3 normalDirection = normalize(normalDir);
 
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				
				float viewIntensity = dot(normalDirection, viewDirection);
				
				if(viewIntensity<_Offset.z)
					viewIntensity = viewIntensity;
				else
					viewIntensity = _Offset.z*2-viewIntensity;
				//end outline
				
				fixed4 sunColor = getAtmosphereColor(v.vertex, o.pos);
				
				o.color = sunColor;
				
				o.color.a = viewIntensity/_Offset.x+_Offset.y;
				
				if(o.color.a<0)
					o.color.a=0;
				
				if(o.color.a>1)
					o.color.a=1;
				
				o.color.a = o.color.a*sunColor.a;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				float new_alpha = (i.color.r+i.color.g+i.color.b)/3;
				i.color.a *= new_alpha;
				return i.color*_Color; 
				//return tex2D (_MainTex, i.uv); 
			}
			
			ENDCG
		}
		
		Pass{
			Tags {"Queue" = "Transparent" "RenderType"="Transparent"} 
			ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha 
           	ColorMask RGB
			LOD 200
			 // blend based on the fragment's alpha value
			
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
			uniform fixed4 _Tint;
			uniform fixed4 _SunColor;
			uniform float4 _LightDir;

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
				o.uv =  v.texcoord.xy * _Layer1_ST.xy + _Layer1_ST.zw;
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
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				i.uv.x+=_Time*_AnimationSpeed;
				i.uv.y+=_Time*_AnimationSpeed;
				
				i.uv1.x+=_Time*_AnimationSpeed;
				i.uv1.y+=_Time*_AnimationSpeed;
				
				fixed4 noiseTex = tex2D (_Layer1, i.uv);
				fixed4 cloudTex = tex2D (_Clouds, i.uv1);
				
				fixed4 cloudColor = fixed4(1, 1, 1, 1);
				cloudColor.a = (1-i.color.a)*(0.5+noiseTex.r/2)/2*i.norm.b;
				
				return cloudColor;
			}
			
			ENDCG
		}
		
		Pass{
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
				
				fixed4 cloudColor = _Tint*(0.5+cloudTex.r);
				cloudColor.a = (1-i.color.a)*(0.5+cloudTex.r/2);
				cloudColor = cloudColor+i.norm.r*_SunColor*(1-i.color.a)*(0.5+cloudColor.a/2);
				cloudColor.a *= i.norm.b;
				
				return cloudColor;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
