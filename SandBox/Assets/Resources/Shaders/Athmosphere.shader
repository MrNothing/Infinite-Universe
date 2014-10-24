Shader "MrNothing's Shaders/Athmosphere" {
	Properties {
		_Color ("Planet Color", Color) = (1,1,1,1)
		_Sun ("Sun Direction", Vector) = (0,0,0,0)
		_FadeDistance ("Fade Distance", Float) = 10
		_HorizonDistance1 ("Horizon Distance", Float) = 700
		_SpecularAmplification("Specular Amplification", Float) = 1
		_SunAmplification("Sun Amplification", Float) = 1
		_Offset ("_Offset" , Vector) = (0,0,0,0)
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
	          float4 vertex:COLOR1;
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
				
				sunspecular*=2;
				if(sunspecular<0)
					sunspecular = 0;
				
				fixed4 sunColor = fixed4(sunIntensity*0.5-(1-camRatio2)+horizon, sunIntensity*0.75+sunspecular/2-sunSetFact*(1-camRatio)/2-(1-camRatio2)/4+horizon, sunIntensity+sunspecular-sunSetFact*(1-camRatio)+horizon, camRatio-0.2f-specularCanceler);
				
				if(sunColor.r>1)
					sunColor.r = 1;
				
				if(sunColor.g>1)
					sunColor.g = 1;
				
				if(sunColor.b>1)
					sunColor.b = 1;
					
				if(sunColor.a>1)
					sunColor.a = 1;
					
				float rangeI = 0.7;
				if(sunColor.r+sunColor.g+sunColor.b>rangeI*3)
				{
					sunColor*=1+(sunColor.r+sunColor.g+sunColor.b-rangeI*3);
				}
				
				rangeI = 0.3;
				if(sunColor.r+sunColor.g+sunColor.b<rangeI*3)
				{
					sunColor.a*=1-(sunColor.r+sunColor.g+sunColor.b-rangeI*3);
				}
					
				//sunColor.a *= 1-sunSetFact;
				
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
	} 
	FallBack "Diffuse"
}
