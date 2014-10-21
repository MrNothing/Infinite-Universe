Shader "MrNothing's Shaders/Vertex Blended Terrain" {
	Properties {
		_Blending ("Blending", Range(0.01, 0.1)) = 0.03
		_Shininess ("Shininess", Range(0.5, 10)) = 0
		
		_Layer1 ("Layer 1", 2D) = "white" {}
		_Layer2 ("Layer 2", 2D) = "white" {}
		_Layer3 ("Layer 3", 2D) = "white" {}
		_Layer4 ("Layer 4", 2D) = "white" {}
		_Layer5 ("Layer 5", 2D) = "white" {}
		_Layer6 ("Layer 6", 2D) = "white" {}
		_Layer7 ("Layer 7", 2D) = "white" {}
		_Layer8 ("Layer 8", 2D) = "white" {}
		_Layer9 ("Layer 9", 2D) = "white" {}
		_Layer10 ("Layer 10", 2D) = "white" {}
		
		_Layer1Specular ("Layer 1 Specular", Range(0, 5)) = 0
		_Layer2Specular ("Layer 2 Specular", Range(0, 5)) = 0
		_Layer3Specular ("Layer 3 Specular", Range(0, 5)) = 0
		_Layer4Specular ("Layer 4 Specular", Range(0, 5)) = 0
		_Layer5Specular ("Layer 5 Specular", Range(0, 5)) = 0
		_Layer6Specular ("Layer 6 Specular", Range(0, 5)) = 0
		_Layer7Specular ("Layer 7 Specular", Range(0, 5)) = 0
		_Layer8Specular ("Layer 8 Specular", Range(0, 5)) = 0
		_Layer9Specular ("Layer 9 Specular", Range(0, 5)) = 0
		_Layer10Specular ("Layer 10 Specular", Range(0, 5)) = 0
		
		_Layer1Emission ("Layer 1 Emission", Range(0, 5)) = 0
		_Layer2Emission ("Layer 2 Emission", Range(0, 5)) = 0
		_Layer3Emission ("Layer 3 Emission", Range(0, 5)) = 0
		_Layer4Emission ("Layer 4 Emission", Range(0, 5)) = 0
		_Layer5Emission ("Layer 5 Emission", Range(0, 5)) = 0
		
		_LightDir ("Light Direction", Vector) = (0,0,0,0)
		
		_GlobalAlpha ("Global Alpha", Range(0, 1)) = 1
		_Fading ("Fading", Float) = 10
		_Disappear ("Disappear", Float) = -1
		_DisableSpec1("Disable Spec", Float) = 0
		
		
		}
		SubShader {
	   
	   Tags {"RenderType"="Opaque"}
		    // Render into depth buffer only
	    //Pass {
		//     ColorMask 0
	    //
		    // Render normally
	    Pass {
	        ZWrite On
	       	Blend SrcAlpha OneMinusSrcAlpha
	      	ColorMask RGB
			LOD 200
			
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Layer1;
			uniform sampler2D _Layer2;
			uniform sampler2D _Layer3;
			uniform sampler2D _Layer4;
			uniform sampler2D _Layer5;
			uniform sampler2D _Layer6;
			uniform sampler2D _Layer7;
			uniform sampler2D _Layer8;
			uniform sampler2D _Layer9;
			uniform sampler2D _Layer10;
			
			uniform float _Layer1Specular;
			uniform float _Layer2Specular;
			uniform float _Layer3Specular;
			uniform float _Layer4Specular;
			uniform float _Layer5Specular;
			uniform float _Layer6Specular;
			uniform float _Layer7Specular;
			uniform float _Layer8Specular;
			uniform float _Layer9Specular;
			uniform float _Layer10Specular;
			
			uniform float _Layer1Emission;
			uniform float _Layer2Emission;
			uniform float _Layer3Emission;
			uniform float _Layer4Emission;
			uniform float _Layer5Emission;
			uniform float _Layer6Emission;
			uniform float _Layer7Emission;
			uniform float _Layer8Emission;
			uniform float _Layer9Emission;
			uniform float _Layer10Emission;
			
			uniform float _Layer1Anim;
			uniform float _Layer2Anim;
			uniform float _Layer3Anim;
			uniform float _Layer4Anim;
			uniform float _Layer5Anim;
			
			uniform float _Shininess;
			uniform float _Fading;
			uniform float _GlobalAlpha;
			uniform float4 _LightDir;
			uniform float _FadeDistanceAtmo;
			uniform float _HorizonDistance1Atmo;
			uniform float _SpecularAmplificationAtmo;
			uniform float _SunAmplificationAtmo;
			uniform float _DisableSpec1;
			uniform float _Disappear;
			
			uniform float _Blending;
			
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
	          fixed4 norm:COLOR1;
	          // float4 screenPos;
	     	};
	     	
	     	float4 _Layer1_ST;
	     	
	     	float rangeFactor(float t, float _min, float _max)
			{
				float result=0;
				if(t<_max && t>_min)
				{
					result= 1;
				}
				else
				{
					if(t<=_min)
						result= 1-abs(t-_min)/_Blending;
					if(t>=_max)
						result= 1-abs(t-_max)/_Blending;
				}
				
				if(result<0)
					result = 0;
					
				return result;
			}
			
			v2f vert (input v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX (v.texcoord, _Layer1);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				
				o.norm.r = dot(v.normal, _LightDir);
				if(o.norm.r<0.2)
					o.norm.r=0.2;
					
				//o.atmo = getAtmosphereColor(v.vertex, o.pos);
					
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_Fading;
				
				if(ratio<1)
				{
					o.norm.a = ratio*ratio-0.5;
					
					//if(ratio<0.5)
					//o.pos = mul(UNITY_MATRIX_MVP, float4(0, 0, 0, 0));
				}
				else
				{
					if(_Disappear>0)
					{
						float dist = distance(_WorldSpaceCameraPos, posWorld);
						float ratio2 = dist/_Disappear;
						float ratio3 = dist/_Disappear-2;
						
						if(ratio3>0)
						{
							o.norm.a = ratio3;
						}
						else
						{
							if(ratio2>=0)
							{
								o.norm.a = 2-ratio2*ratio2;
							}
							else
							{
								o.norm.a = 0;
							}
						}
					}
					else
					{
						o.norm.a = 1;
					}
				}
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				float4 specularity = 0;
				
				//if(i.norm.g>0)
					specularity = i.norm.g*i.norm.g*2*_DisableSpec1;
				
				fixed4 colo = (
					tex2D (_Layer1, i.uv)*rangeFactor(i.color.r, 0, 0.1)*(1+specularity*_Layer1Specular+_Layer1Emission)+
					tex2D (_Layer2, i.uv)*rangeFactor(i.color.r, 0.1, 0.2)*(1+specularity*_Layer2Specular+_Layer2Emission)+
					tex2D (_Layer3, i.uv)*rangeFactor(i.color.r, 0.2, 0.3)*(1+specularity*_Layer3Specular+_Layer3Emission)+
					tex2D (_Layer4, i.uv)*rangeFactor(i.color.r, 0.3, 0.4)*(1+specularity*_Layer4Specular+_Layer4Emission)+
					tex2D (_Layer5, i.uv)*rangeFactor(i.color.r, 0.4, 0.5)*(1+specularity*_Layer5Specular+_Layer5Emission)+
					tex2D (_Layer6, i.uv)*rangeFactor(i.color.r, 0.5, 0.6)*(1+specularity*_Layer6Specular+_Layer6Emission)+
					tex2D (_Layer7, i.uv)*rangeFactor(i.color.r, 0.6, 0.7)*(1+specularity*_Layer7Specular+_Layer7Emission)+
					tex2D (_Layer8, i.uv)*rangeFactor(i.color.r, 0.7, 0.8)*(1+specularity*_Layer8Specular+_Layer8Emission)+
					tex2D (_Layer9, i.uv)*rangeFactor(i.color.r, 0.8, 0.9)*(1+specularity*_Layer9Specular+_Layer9Emission)+
					tex2D (_Layer10, i.uv)*rangeFactor(i.color.r, 0.9, 1)*(1+specularity*_Layer10Specular+_Layer10Emission)
					)*i.norm.r/2*(1-i.color.g);
					colo.a = i.norm.a*_GlobalAlpha;
					
					//colo = i.atmo*(1-colo.a)+colo*colo.a;
					
					return colo; 
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
