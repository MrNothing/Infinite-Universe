Shader "MrNothing's Shaders/MeshBlender" {
	Properties {
		_Color ("Color" , Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		influenceRange ("Influence Range" , Float) = 2.0
		amplification ("Amplification" , Float) = 1.0
		_worldPosition ("World Position", Vector) = (0,0,0,0) 
		_surroundingSize ("array size", float) = 0 
		_o1 ("Objects Array", Vector) = (0,0,0,0) 
		_o2 ("Objects Array", Vector) = (0,0,0,0) 
		_o3 ("Objects Array", Vector) = (0,0,0,0) 
		_o4 ("Objects Array", Vector) = (0,0,0,0) 
		_o5 ("Objects Array", Vector) = (0,0,0,0) 
		_o6 ("Objects Array", Vector) = (0,0,0,0) 
		_o7 ("Objects Array", Vector) = (0,0,0,0) 
		_o8 ("Objects Array", Vector) = (0,0,0,0) 
		_o9 ("Objects Array", Vector) = (0,0,0,0) 
		_o10 ("Objects Array", Vector) = (0,0,0,0) 
		_o11 ("Objects Array", Vector) = (0,0,0,0) 
		_o12 ("Objects Array", Vector) = (0,0,0,0) 
		_o13 ("Objects Array", Vector) = (0,0,0,0) 
		_o14 ("Objects Array", Vector) = (0,0,0,0) 
		_o15 ("Objects Array", Vector) = (0,0,0,0) 
		_o16 ("Objects Array", Vector) = (0,0,0,0) 
	}
	
	SubShader {
		Pass{
			//Tags { "RenderType"="Opaque" }
			LOD 40
			Lighting Off
			Cull Back
			 
			CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
				uniform float4 _Color;
				uniform float influenceRange;
				uniform float amplification;
				uniform float4 _worldPosition;
				uniform float _surroundingSize;
				uniform float4 _o1;
				uniform float4 _o2;
				uniform float4 _o3;
				uniform float4 _o4;
				uniform float4 _o5;
				uniform float4 _o6;
				uniform float4 _o7;
				uniform float4 _o8;
				uniform float4 _o9;
				uniform float4 _o10;
				uniform float4 _o11;
				uniform float4 _o12;
				uniform float4 _o13;
				uniform float4 _o14;
				uniform float4 _o15;
				uniform float4 _o16;
				
				struct blendResult
				{
					float4 pos;
					float colorRatio;
				};
				
				struct v2f 
				{
		          float4 pos : SV_POSITION;
		          fixed4 color : COLOR0;
		          float2 uv : TEXCOORD0;
		         // float4 screenPos;
		     	};
		     	
		     	float4 _MainTex_ST;
		     	
		     	float distance2(float4 p1, float4 p2)
		     	{
		     		return sqrt((p2.x-p1.x)*(p2.x-p1.x)+(p2.y-p1.y)*(p2.y-p1.y)+(p2.z-p1.z)*(p2.z-p1.z));
		     	}
		     	
		     	float4 uniformize(float4 o)
		     	{
		     		float fact=0.5;
		     		
		     		if(o.x>fact)
		     			o.x = fact;
		     		if(o.x<-fact)
		     			o.x = -fact;
		     		
		     		if(o.y>fact)
		     			o.y = fact;
		     		if(o.y<-fact)
		     			o.y = -fact;
		     		
		     		if(o.z>fact)
		     			o.z = fact;
		     		if(o.z<-fact)
		     			o.z = -fact;
		     		
		     		if(o.w>fact)
		     			o.w = fact;
		     		if(o.w<-fact)
		     			o.w = -fact;
		     			
		     		return o;
		     	}
		     	
		     	blendResult applyBlending(blendResult b, float4 vertex, float4 obj)
		     	{
		     		float4 worldVerticePos = _worldPosition+vertex;
		     		float ratio = distance2(worldVerticePos, obj)/influenceRange;
		     		if(ratio<1)
					{
						if(ratio<0.7)
							ratio = 0.7;
						
						b.pos += (obj-worldVerticePos)*(1-ratio)*amplification;
							
						b.colorRatio *= ratio;
					}
					
					return b;
		     	}
		     	
		     	v2f vert (appdata_base v)
				{
					v2f o;
					
					blendResult res;
					res.pos = float4(0,0,0,0);
					res.colorRatio = 1;
					
					if(_surroundingSize>=1)
						res = applyBlending(res, v.vertex,  _o1);
					if(_surroundingSize>=2)
						res = applyBlending(res, v.vertex,  _o2);
					if(_surroundingSize>=3)
						res = applyBlending(res, v.vertex,  _o3);
					if(_surroundingSize>=4)
						res = applyBlending(res, v.vertex,  _o4);
					if(_surroundingSize>=5)
						res = applyBlending(res, v.vertex,  _o5);
					if(_surroundingSize>=6)
						res = applyBlending(res, v.vertex,  _o6);
					if(_surroundingSize>=7)
						res = applyBlending(res, v.vertex,  _o7);
					if(_surroundingSize>=8)
						res = applyBlending(res, v.vertex,  _o8);
					if(_surroundingSize>=9)
						res = applyBlending(res, v.vertex,  _o9);
					if(_surroundingSize>=10)
						res = applyBlending(res, v.vertex,  _o10);
					if(_surroundingSize>=11)
						res = applyBlending(res, v.vertex,  _o11);
					if(_surroundingSize>=12)
						res = applyBlending(res, v.vertex,  _o12);
					if(_surroundingSize>=13)
						res = applyBlending(res, v.vertex,  _o13);
					if(_surroundingSize>=14)
						res = applyBlending(res, v.vertex,  _o14);
					if(_surroundingSize>=15)
						res = applyBlending(res, v.vertex,  _o15);
					if(_surroundingSize>=16)
						res = applyBlending(res, v.vertex,  _o16);
						
					res.pos = uniformize(res.pos);	
					
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex+res.pos);
					
					o.color.r = res.colorRatio*2;
					o.color.g = res.colorRatio*2;
					o.color.b = res.colorRatio*2;
					o.color.a = 1;
					
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
					
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR0 
				{
					return i.color*_Color*tex2D (_MainTex, i.uv); 
				}
	     	
			ENDCG
		}
	} 
	
	SubShader {
		Pass{
			Tags { "RenderType"="Opaque" }
			LOD 200
			Lighting Off
			Cull Back
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				uniform sampler2D _MainTex;
				uniform float4 _Color;
				uniform float influenceRange;
				uniform float amplification;
				uniform float4 _worldPosition;
				uniform float _surroundingSize;
				uniform float4 _o1;
				uniform float4 _o2;
				uniform float4 _o3;
				uniform float4 _o4;
				uniform float4 _o5;
				uniform float4 _o6;
				uniform float4 _o7;
				
				struct blendResult
				{
					float4 pos;
					float colorRatio;
				};
				
				struct v2f 
				{
		          float4 pos : SV_POSITION;
		          fixed4 color : COLOR0;
		          float2 uv : TEXCOORD0;
		         // float4 screenPos;
		     	};
		     	
		     	float4 _MainTex_ST;
		     	
		     	float distance2(float4 p1, float4 p2)
		     	{
		     		return sqrt((p2.x-p1.x)*(p2.x-p1.x)+(p2.y-p1.y)*(p2.y-p1.y)+(p2.z-p1.z)*(p2.z-p1.z));
		     	}
		     	
		     	blendResult applyBlending(blendResult b, float4 vertex, float4 obj)
		     	{
		     		float4 worldVerticePos = _worldPosition+vertex;
		     		float ratio = distance2(worldVerticePos, obj)/influenceRange;
		     		if(ratio<1)
					{
						if(ratio<0.7)
							ratio = 0.7;
						
						b.pos += (obj-worldVerticePos)*(1-ratio)*amplification;
						b.colorRatio *= ratio;
					}
					else
					
					
					return b;
		     	}
		     	
		     	v2f vert (appdata_base v)
				{
					v2f o;
					
					blendResult res;
					res.pos = float4(0,0,0,0);
					res.colorRatio = 1;
					
					if(_surroundingSize>=1)
						res = applyBlending(res, v.vertex,  _o1);
					if(_surroundingSize>=2)
						res = applyBlending(res, v.vertex,  _o2);
					if(_surroundingSize>=3)
						res = applyBlending(res, v.vertex,  _o3);
					if(_surroundingSize>=4)
						res = applyBlending(res, v.vertex,  _o4);
					if(_surroundingSize>=5)
						res = applyBlending(res, v.vertex,  _o5);
					if(_surroundingSize>=6)
						res = applyBlending(res, v.vertex,  _o6);
						
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex+res.pos);
					
					o.color.r = res.colorRatio;
					o.color.g = res.colorRatio;
					o.color.b = res.colorRatio;
					o.color.a = 1;
					
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
					
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR0 
				{
					return i.color*_Color*tex2D (_MainTex, i.uv); 
				}
	     	
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
