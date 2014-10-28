Shader "MrNothing's Shaders/Sun" {
	Properties {
		_Layer1 ("SunWaves", 2D) = "white" {}
		_Color ("Planet Color", Color) = (1,1,1,1)
		_AnimSpeed("_AnimSpeed", Float) = 1
		_Offset ("_Offset" , Vector) = (-0.37,-0.62,0,0)
		_Shininess ("_Shininess" , Float) = 0.72
		_Fading ("_Fading" , Float) = 1000
	}
	SubShader 
	{
		Tags { "Queue"="Transparent+10000" "RenderType"="Transparent" }
 		
 		LOD 200
		
		Fog {Mode Off}		
		Pass
		{
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D _Layer1;
			uniform float4 _Color;
			uniform float _AnimSpeed;
			uniform float4 _Offset;
			uniform float _Shininess;
			uniform float _Fading;
			
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
	          float4 vertex:COLOR1;
	          fixed4 norm:COLOR1;
	     	};
			
			float4 _Layer1_ST;
			
			
			v2f vert (input v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv = TRANSFORM_TEX (v.texcoord, _Layer1);
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				//outline
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 normalDir =  normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0), modelMatrixInverse).xyz);
				
				float3 normalDirection = normalize(normalDir);
 
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				
				float viewIntensity = dot(normalDirection, viewDirection);
				o.norm.g =  dot( reflect(-float4(0, 0, 0, _Shininess), v.normal), viewDirection);
				
				float ratio = distance(_WorldSpaceCameraPos, posWorld)/_Fading;
				o.norm.a = 2-ratio;
				
				if(viewIntensity<_Offset.z)
					viewIntensity = viewIntensity;
				else
					viewIntensity = _Offset.z*2-viewIntensity;
				//end outline
				
				o.color = fixed4(1, 1, 1, 1);
				
				o.color.a = viewIntensity/_Offset.x+_Offset.y;
				
				
				o.uv = TRANSFORM_TEX (v.texcoord, _Layer1);
				return o;
			}
				
				fixed4 frag (v2f i) : COLOR0 
				{
					float2 uv2 =i.uv*2+_Time*_AnimSpeed/2;
					i.uv+=_Time*_AnimSpeed;
					
					fixed4 finalColor = _Color;
					
					float colorRatio = (tex2D (_Layer1, i.uv).r+tex2D (_Layer1, i.uv).g+tex2D (_Layer1, i.uv).b)/3;
					colorRatio-=0.5;
					colorRatio*=2;
					
					float colorRatio2 = (tex2D (_Layer1, uv2).r+tex2D (_Layer1, uv2).g+tex2D (_Layer1, uv2).b)/3;
					colorRatio2-=0.5;
					colorRatio2*=2;
					
					finalColor.a = i.color.a+((1-i.color.a)*colorRatio+(1-i.color.a)*colorRatio2);
					finalColor = (finalColor+i.norm.g*colorRatio+i.norm.g*i.norm.g)*(i.norm.a); 
					return finalColor; 
				}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}