Shader "MrNothing's Shaders/Sea" {
	Properties {
		_Layer1 ("Water", 2D) = "white" {}
		_Foam ("Foam", 2D) = "white" {}
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_SunColor ("Sun Color", Color) = (1, 1, 1, 1)
		_AnimationSpeed ("Animation Speed", Float) = 1
		_FoamSpeed ("Foam Speed", Float) = 1
		_LightDir ("Light Direction", Vector) = (0,0,0,0)
		_FoamRange ("Foam Range", Float) = 0.1
		_FoamIntensity ("Foam Intensity", Float) = 2
		_Shininess("Shininess", Float) = 1
		_ReflectionIntensity("Reflection Intensity", Float) = 1
		_GlobalAlpha ("Global Alpha", Range(0, 1)) = 1
		_Fading ("_Fading", Float) = 50
		_SeaTransparencyRange ("_SeaTransparencyRange", Float) = 10
	}
	
	SubShader {
		
			Pass{
			Cull Front
			Tags {"Queue" = "Geometry"} 
			 ZWrite On // don't write to depth buffer 
            // in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha 
            // blend based on the fragment's alpha value
			 ColorMask RGB
			 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Layer1;
			uniform sampler2D _Foam;
			uniform float _AnimationSpeed;
			uniform float _FoamSpeed;
			uniform float _FoamRange;
			uniform float _FoamIntensity;
			uniform fixed4 _Tint;
			uniform fixed4 _SunColor;
			uniform float4 _LightDir;
			uniform float _Shininess;
			uniform float _ReflectionIntensity;
			uniform float _GlobalAlpha;
			uniform float _Fading;
			uniform float _SeaTransparencyRange;
			
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
	     	float4 _Foam_ST;
			
			v2f vert (input v)
			{
				v2f o;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.color = v.color;
				o.uv =  v.texcoord.xy * _Layer1_ST.xy + _Layer1_ST.zw;
				o.uv1 = TRANSFORM_TEX (v.texcoord1, _Foam);
				//note to myself: always use this to get world coordinates of a vertex.
				o.norm.r = dot(v.normal, _LightDir);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				
				float dist = distance(_WorldSpaceCameraPos, posWorld);
					
				float ratio = dist/_SeaTransparencyRange;
				
				if(ratio<1)
					o.norm.b = ratio;
				else
					o.norm.b = 1;
				
				
				if(_Fading>0)
				{
					float ratio2 = dist/_Fading;
					float ratio3 = dist/_Fading-2;
					
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
					o.norm.a = 1;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				if(i.norm.r<0.2)
					i.norm.r = 0.2;
				float ux = i.uv1.x+_Time*_FoamSpeed;
				float uy = i.uv1.y+_Time*_FoamSpeed;
				float2 uvC1 = float2(ux, uy);
				
				float ux2 = i.uv1.x/2+_Time*_FoamSpeed*0.5;
				float uy2 = i.uv1.y/2+_Time*_FoamSpeed*0.2;
				
				float2 uvC2 = float2(ux2, uy2);
				
				float ux3 = i.uv.x+_Time*_FoamSpeed;
				float uy3 = i.uv.y+_Time*_FoamSpeed;
				float2 uvC3 = float2(ux3, uy3);
				
				fixed4 noiseTex = tex2D (_Layer1,uvC3);
				fixed4 foamTex = tex2D (_Layer1, uvC1);
				fixed4 foamTex2 = tex2D (_Layer1, uvC3);
				
				foamTex = foamTex*foamTex2*5+0.5;
				
				fixed4 cloudColor = fixed4(1,1,1,1);
				
					float foamVal = (foamTex.r+foamTex.g+foamTex.b)/3;
				
				if(i.color.a<1 && i.color.a>_FoamRange)
				{	
					cloudColor.a = (1-i.color.a)*foamVal*2;
				}
				
				
				if(i.color.a<_FoamRange)
				{
					cloudColor.a = (1-(_FoamRange-i.color.a)/_FoamRange)*foamVal;
				}
				else
				{
					_Tint.a = (1-i.color.a);
				}
				
				_Tint+=cloudColor.a*(_FoamIntensity+i.norm.g*foamVal);
				_Tint.rgb*=i.norm.r+i.norm.g*foamTex.r*2*_SunColor;
				_Tint.a *= (i.norm.b*i.norm.b+i.norm.g)*_GlobalAlpha*i.norm.a;
				noiseTex.rgb*=2;
				return _Tint;
			}
			
			ENDCG
		}
		
		Pass{
			Tags {"Queue" = "Transparent"} 
			 ZWrite On // don't write to depth buffer 
            // in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha 
            // blend based on the fragment's alpha value
			 ColorMask RGB
			 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _Layer1;
			uniform sampler2D _Foam;
			uniform float _AnimationSpeed;
			uniform float _FoamSpeed;
			uniform float _FoamRange;
			uniform float _FoamIntensity;
			uniform fixed4 _Tint;
			uniform fixed4 _SunColor;
			uniform float4 _LightDir;
			uniform float _Shininess;
			uniform float _ReflectionIntensity;
			uniform float _GlobalAlpha;
			uniform float _Fading;
			uniform float _SeaTransparencyRange;
			
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
	     	float4 _Foam_ST;
			
			v2f vert (input v)
			{
				v2f o;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.color = v.color;
				o.uv =  v.texcoord.xy * _Layer1_ST.xy + _Layer1_ST.zw;
				o.uv1 = TRANSFORM_TEX (v.texcoord1, _Foam);
				//note to myself: always use this to get world coordinates of a vertex.
				o.norm.r = dot(v.normal, _LightDir);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				o.norm.g =  pow(max(0.0, dot( reflect(-_LightDir, v.normal), viewDirection)), _Shininess);
				
				float dist = distance(_WorldSpaceCameraPos, posWorld);
					
				float ratio = dist/_SeaTransparencyRange;
				
				if(ratio<1)
					o.norm.b = ratio;
				else
					o.norm.b = 1;
				
				
				if(_Fading>0)
				{
					float ratio2 = dist/_Fading;
					float ratio3 = dist/_Fading-2;
					
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
					o.norm.a = 1;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				if(i.norm.r<0.2)
					i.norm.r = 0.2;
				float ux = i.uv1.x+_Time*_FoamSpeed;
				float uy = i.uv1.y+_Time*_FoamSpeed;
				float2 uvC1 = float2(ux, uy);
				
				float ux2 = i.uv1.x/2+_Time*_FoamSpeed*0.5;
				float uy2 = i.uv1.y/2+_Time*_FoamSpeed*0.2;
				
				float2 uvC2 = float2(ux2, uy2);
				
				float ux3 = i.uv.x+_Time*_FoamSpeed;
				float uy3 = i.uv.y+_Time*_FoamSpeed;
				float2 uvC3 = float2(ux3, uy3);
				
				fixed4 noiseTex = tex2D (_Layer1,uvC3);
				fixed4 foamTex = tex2D (_Layer1, uvC1);
				fixed4 foamTex2 = tex2D (_Layer1, uvC3);
				
				foamTex = foamTex*foamTex2*5+0.5;
				
				fixed4 cloudColor = fixed4(1,1,1,1);
				
				cloudColor.a = (1-i.color.a);
				
				float foamVal = (foamTex.r+foamTex.g+foamTex.b)/3;
				
				if(i.color.a<1 && i.color.a>_FoamRange)
				{	
					cloudColor.a = (1-i.color.a)*foamVal*2;
				}
				
				
				if(i.color.a<_FoamRange)
				{
					cloudColor.a = (1-(_FoamRange-i.color.a)/_FoamRange)*foamVal;
				}
				else
				{
					_Tint.a = (1-i.color.a);
				}
				
				_Tint+=cloudColor.a*(_FoamIntensity+i.norm.g*foamVal);
				_Tint.rgb*=i.norm.r+i.norm.g*foamTex.r*2*_SunColor;
				_Tint.a *= (i.norm.b*i.norm.b+i.norm.g)*_GlobalAlpha*i.norm.a;
				noiseTex.rgb*=2;
				return _Tint;
			}
			
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
