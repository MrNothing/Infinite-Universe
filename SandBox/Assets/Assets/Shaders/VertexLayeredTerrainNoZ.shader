Shader "MrNothing's Shaders/Vertex Layered Terrain No Z" {
	Properties {
		_Shininess ("Shininess", Range(0.5, 10)) = 0
		
		_Layer1 ("Layer 1", 2D) = "white" {}
		_Layer2 ("Layer 2", 2D) = "white" {}
		_Layer3 ("Layer 3", 2D) = "white" {}
		_Layer4 ("Layer 4", 2D) = "white" {}
		_Layer5 ("Layer 5", 2D) = "white" {}
		
		_Layer1Specular ("Layer 1 Specular", Range(0, 5)) = 0
		_Layer2Specular ("Layer 2 Specular", Range(0, 5)) = 0
		_Layer3Specular ("Layer 3 Specular", Range(0, 5)) = 0
		_Layer4Specular ("Layer 4 Specular", Range(0, 5)) = 0
		_Layer5Specular ("Layer 5 Specular", Range(0, 5)) = 0
		
		_Layer1Emission ("Layer 1 Emission", Range(0, 5)) = 0
		_Layer2Emission ("Layer 2 Emission", Range(0, 5)) = 0
		_Layer3Emission ("Layer 3 Emission", Range(0, 5)) = 0
		_Layer4Emission ("Layer 4 Emission", Range(0, 5)) = 0
		_Layer5Emission ("Layer 5 Emission", Range(0, 5)) = 0
		
		_Layer1Anim ("Layer 1 Anim", Range(0, 5)) = 0
		_Layer2Anim ("Layer 2 Anim", Range(0, 5)) = 0
		_Layer3Anim ("Layer 3 Anim", Range(0, 5)) = 0
		_Layer4Anim ("Layer 4 Anim", Range(0, 5)) = 0
		_Layer5Anim ("Layer 5 Anim", Range(0, 5)) = 0
		
		_LightDir ("Light Direction", Vector) = (0,0,0,0)
		
		_GlobalAlpha ("Global Alpha", Range(0, 1)) = 1
		_Fading ("Fading", Float) = 10
		_Disappear ("Disappear", Float) = -1
		_FadeDistanceAtmo ("Atmosphere Fade Distance", Float) = 10
		_HorizonDistance1Atmo ("Atmosphere Horizon Distance", Float) = 700
		_SpecularAmplificationAtmo("Atmosphere Specular Amplification", Float) = 1
		_SunAmplificationAtmo("Atmosphere Sun Amplification", Float) = 1
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
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Layer1;
			uniform sampler2D _Layer2;
			uniform sampler2D _Layer3;
			uniform sampler2D _Layer4;
			uniform sampler2D _Layer5;
			uniform float _Layer1Specular;
			uniform float _Layer2Specular;
			uniform float _Layer3Specular;
			uniform float _Layer4Specular;
			uniform float _Layer5Specular;
			
			uniform float _Layer1Emission;
			uniform float _Layer2Emission;
			uniform float _Layer3Emission;
			uniform float _Layer4Emission;
			uniform float _Layer5Emission;
			
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
	          fixed4 atmo:TEXCOORD1;
	         // float4 screenPos;
	     	};
	     	
	     	float4 _Layer1_ST;
	     	
	     	fixed4 getAtmosphereColor(float4 vertex, float4 opos)
	     	{
	     		float3 camPos =  ObjSpaceViewDir(vertex);
				
				float sunspecular = dot(camPos.xyz, _LightDir.xyz);
				float sunIntensity = dot(_LightDir.xyz, vertex.xyz);
				float realDistance = distance(camPos.xyz, opos);
				float camRatio = distance(camPos.xyz, opos)/_FadeDistanceAtmo;
				float camRatio2 = distance(camPos.xyz, opos)/_HorizonDistance1Atmo;
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
				
				sunspecular*=_SpecularAmplificationAtmo;
				sunIntensity*=_SunAmplificationAtmo;
				
				
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
				
				
				fixed4 sunColor = fixed4(sunIntensity-(1-camRatio2), sunIntensity+sunspecular/2-sunSetFact*(1-camRatio)/2-(1-camRatio2)/4, sunIntensity+sunspecular-sunSetFact*(1-camRatio), camRatio-0.2f-specularCanceler);
				
				if(sunColor.r>1)
					sunColor.r = 1;
				
				if(sunColor.g>1)
					sunColor.g = 1;
				
				if(sunColor.b>1)
					sunColor.b = 1;
					
				if(sunColor.a>1)
					sunColor.a = 1;
					
				if(sunColor.a<0)
					sunColor.a = 0;
					
				sunColor*=sunColor.a;	
				
				return sunColor;
	     	}
	     	
	     	fixed4 melt(fixed4 color1, fixed4 color2)
	     	{
	     		float a = color1.a;
	     		if(color1.a<color2.a)
	     			color1.a = color2.a;
	     			
	     		float r = color1.r;
	     		if(color1.r<color2.r)
	     			color1.r = color2.r;
	     			
	     		float g = color1.g;
	     		if(color1.g<color2.g)
	     			color1.g = color2.g;
	     			
	     		float b = color1.b;
	     		if(color1.b<color2.b)
	     			color1.b = color2.b;
	     			
	     		return fixed4(r,g,b,a);
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
				
				//o.atmo = getAtmosphereColor(v.vertex, o.pos)*0.5f;
				
				//o.norm.a = _GlobalAlpha;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				float total = i.color.r+i.color.g+i.color.b+i.color.a;
				
				float4 specularity = 0;
				
				//if(i.norm.g>0)
					specularity = i.norm.g*i.norm.g*2*_DisableSpec1;
				
				if(total>0)
				{
					float rRatio = i.color.r/total; 
					float gRatio = i.color.g/total; 
					float bRatio = i.color.b/total; 
					float aRatio = i.color.a/total; 
					
					float antiRatio = 1-total;
					
					if(antiRatio<0)
						antiRatio=0;
					
					fixed4 layer1 = tex2D (_Layer1, i.uv)*(1+specularity*_Layer1Specular+_Layer1Emission)*i.color.r;
					fixed4 layer2 = tex2D (_Layer2, i.uv)*(1+specularity*_Layer2Specular+_Layer2Emission)*i.color.g;
					fixed4 layer3 = tex2D (_Layer3, i.uv+_Time*_Layer3Anim)*(1+specularity*_Layer3Specular+_Layer3Emission)*i.color.b;
					fixed4 layer4 = tex2D (_Layer4, i.uv+_Time*_Layer4Anim)*(1+specularity*_Layer4Specular+_Layer4Emission)*i.color.a;
					fixed4 layer5 = tex2D (_Layer5, i.uv+_Time*_Layer5Anim)*(1+specularity*_Layer5Specular+_Layer5Emission);
					
					fixed4 colo = (layer1*rRatio+layer2*gRatio+layer3*bRatio+layer4*aRatio+layer5*antiRatio)*i.norm.r;
					colo.a = i.norm.a*_GlobalAlpha;
					
					//colo = i.atmo*(1-colo.a)+colo*colo.a;
					
					return colo; 
				}
				else
				{
					fixed4 colo = tex2D (_Layer5, i.uv)*(1+specularity*_Layer5Specular)*i.norm.r;
					colo.a = i.norm.a*_GlobalAlpha;
					
					//if(colo.a>1)
					//	colo.a=1;
						
					//if(colo.a<0)
					//	colo.a=0;
					
					//colo = i.atmo*(1-colo.a)+colo*colo.a;
					
					return colo; 
				}
			}
			
			ENDCG
		}
		
		Pass {
	        ZWrite Off
	       	Blend SrcAlpha OneMinusSrcAlpha
	      	ColorMask RGB
			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Layer1;
			uniform sampler2D _Layer2;
			uniform sampler2D _Layer3;
			uniform sampler2D _Layer4;
			uniform sampler2D _Layer5;
			uniform float _Layer1Specular;
			uniform float _Layer2Specular;
			uniform float _Layer3Specular;
			uniform float _Layer4Specular;
			uniform float _Layer5Specular;
			
			uniform float _Layer1Emission;
			uniform float _Layer2Emission;
			uniform float _Layer3Emission;
			uniform float _Layer4Emission;
			uniform float _Layer5Emission;
			
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
	          fixed4 atmo:TEXCOORD1;
	         // float4 screenPos;
	     	};
	     	
	     	float4 _Layer1_ST;
	     	
	     	fixed4 getAtmosphereColor(float4 vertex, float4 opos)
	     	{
	     		float3 camPos =  ObjSpaceViewDir(vertex);
				
				float sunspecular = dot(camPos.xyz, _LightDir.xyz);
				float sunIntensity = dot(_LightDir.xyz, vertex.xyz);
				float realDistance = distance(camPos.xyz, opos);
				float camRatio = distance(camPos.xyz, opos)/_FadeDistanceAtmo;
				float camRatio2 = distance(camPos.xyz, opos)/_HorizonDistance1Atmo;
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
				
				sunspecular*=_SpecularAmplificationAtmo;
				sunIntensity*=_SunAmplificationAtmo;
				
				
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
				
				
				fixed4 sunColor = fixed4(sunIntensity-(1-camRatio2), sunIntensity+sunspecular/2-sunSetFact*(1-camRatio)/2-(1-camRatio2)/4, sunIntensity+sunspecular-sunSetFact*(1-camRatio), camRatio-0.2f-specularCanceler);
				
				if(sunColor.r>1)
					sunColor.r = 1;
				
				if(sunColor.g>1)
					sunColor.g = 1;
				
				if(sunColor.b>1)
					sunColor.b = 1;
					
				if(sunColor.a>1)
					sunColor.a = 1;
					
				if(sunColor.a<0)
					sunColor.a = 0;
					
				sunColor*=sunColor.a;	
				
				return sunColor;
	     	}
	     	
	     	fixed4 melt(fixed4 color1, fixed4 color2)
	     	{
	     		float a = color1.a;
	     		if(color1.a<color2.a)
	     			color1.a = color2.a;
	     			
	     		float r = color1.r;
	     		if(color1.r<color2.r)
	     			color1.r = color2.r;
	     			
	     		float g = color1.g;
	     		if(color1.g<color2.g)
	     			color1.g = color2.g;
	     			
	     		float b = color1.b;
	     		if(color1.b<color2.b)
	     			color1.b = color2.b;
	     			
	     		return fixed4(r,g,b,a);
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
				
				//o.atmo = getAtmosphereColor(v.vertex, o.pos)*0.5f;
				
				//o.norm.a = _GlobalAlpha;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				float total = i.color.r+i.color.g+i.color.b+i.color.a;
				
				float4 specularity = 0;
				
				//if(i.norm.g>0)
					specularity = i.norm.g*i.norm.g*2*_DisableSpec1;
				
				if(total>0)
				{
					float rRatio = i.color.r/total; 
					float gRatio = i.color.g/total; 
					float bRatio = i.color.b/total; 
					float aRatio = i.color.a/total; 
					
					float antiRatio = 1-total;
					
					if(antiRatio<0)
						antiRatio=0;
					
					fixed4 layer1 = tex2D (_Layer1, i.uv)*(1+specularity*_Layer1Specular+_Layer1Emission)*i.color.r;
					fixed4 layer2 = tex2D (_Layer2, i.uv)*(1+specularity*_Layer2Specular+_Layer2Emission)*i.color.g;
					fixed4 layer3 = tex2D (_Layer3, i.uv+_Time*_Layer3Anim)*(1+specularity*_Layer3Specular+_Layer3Emission)*i.color.b;
					fixed4 layer4 = tex2D (_Layer4, i.uv+_Time*_Layer4Anim)*(1+specularity*_Layer4Specular+_Layer4Emission)*i.color.a;
					fixed4 layer5 = tex2D (_Layer5, i.uv+_Time*_Layer5Anim)*(1+specularity*_Layer5Specular+_Layer5Emission);
					
					fixed4 colo = (layer1*rRatio+layer2*gRatio+layer3*bRatio+layer4*aRatio+layer5*antiRatio)*i.norm.r;
					colo.a = i.norm.a*_GlobalAlpha;
					
					//colo = i.atmo*(1-colo.a)+colo*colo.a;
					
					return colo; 
				}
				else
				{
					fixed4 colo = tex2D (_Layer5, i.uv)*(1+specularity*_Layer5Specular)*i.norm.r;
					colo.a = i.norm.a*_GlobalAlpha;
					
					//if(colo.a>1)
					//	colo.a=1;
						
					//if(colo.a<0)
					//	colo.a=0;
					
					//colo = i.atmo*(1-colo.a)+colo*colo.a;
					
					return colo; 
				}
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
