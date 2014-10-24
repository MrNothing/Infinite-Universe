Shader "MrNothing's Shaders/VertexColored" {
	Properties {
		_Color ("Base Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Luminosity ("Luminosity", Range(0, 2)) = 1
	}
	SubShader 
	{
		//Tags { "Queue"="Transparent" "RenderType"="Transparent" }
 		//ZWrite Off
		//Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		//Fog {Mode Off}
					
		Pass
		{
			
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members vpos)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float _Luminosity;
			uniform sampler2D _MainTex;
			uniform float4 _Color;
					
			struct VertIn
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	          float4 vertex : COLOR1;
	         // float fog;
	          // float4 screenPos;
	     	};
	     	
	     	float4 _MainTex_ST;
	     	
	 		v2f vert (VertIn v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				
				//float3 camPos =  ObjSpaceViewDir(v.vertex);
				//float camRatio = distance(camPos.xyz, o.pos)/50;
				
				//o.fog=camRatio;
				
				//if(o.fog<1)
				//	o.fog=1;
				
				//fixed4 sunColor = v.color;
				
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				return (i.color*_Color+tex2D (_MainTex, i.uv))*_Luminosity; 
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
