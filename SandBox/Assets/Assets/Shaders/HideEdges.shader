Shader "MrNothing's Shaders/Hide Edges" {
Properties {
	_Color ("_Color" , Color) = (0,0,0,0)
	_Offset ("_Offset" , Vector) = (0,0,0,0)
}
 
SubShader {
    Tags {"RenderType"="Transparent" "Queue"="Transparent"}
    
    Pass {
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
       	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float4 _Offset;
			uniform fixed4 _Color;
		
			struct input
			{
				float4 vertex:POSITION;
				fixed4 color:COLOR0;
				float3 normal:NORMAL;
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
				
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				
				float4 posWorld = mul(modelMatrix, v.vertex);
				float3 normalDir =  normalize(mul(float4(v.normal.x, v.normal.y, v.normal.z, 0), modelMatrixInverse).xyz);
				
				float3 normalDirection = normalize(normalDir);
 
				float3 viewDirection = normalize(_WorldSpaceCameraPos - posWorld.xyz);
				
				float viewIntensity = dot(normalDirection, viewDirection);
				
				if(viewIntensity<_Offset.z)
					viewIntensity = viewIntensity;
				else
					viewIntensity = _Offset.z*2-viewIntensity;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.color.a = viewIntensity/_Offset.x+_Offset.y;
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				return i.color*_Color;
			}
		
		ENDCG
    }
    
 
}
}