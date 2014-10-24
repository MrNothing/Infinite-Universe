Shader "MrNothing's Shaders/Transparent with Z" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
 
SubShader {
    Tags {"RenderType"="Transparent" "Queue"="Geometry"}
    // Render into depth buffer only
    Pass {
        ColorMask 0
    }
    // Render normally
    Pass {
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
       	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			//4 layers for 4 color channels...
			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
		
			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
				float2 texcoord:TEXCOORD0;
			};
			
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR0;
	          float2 uv : TEXCOORD0;
	     	};
	     	
	     	float4 _MainTex_ST;
			
			v2f vert (input v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				return tex2D(_MainTex, i.uv)*i.color*_Color;
			}
		
		ENDCG
    }
}
}