//test 
Shader "MrNothing's Shaders/Planet Generator Shader" 
{
	Properties {
		_Scale ("Perlin Scale" , Float) = 1.0
		_Height ("Perlin Height" , Float) = 1.0
		_noiseOffset ("Noise Offset" , Vector) = (0.0, 0.0, 0.0, 0.0)
		_Frequency ("Fractal Frequency" , Float) = 1.0
		_Lacunarity ("Fractal Lacunarity" , Float) = 1.0
		_Gain ("Fractal Gain" , Float) = 1.0
		_Color ("Color" , Color) = (1.0, 1.0, 1.0, 1.0)
		permutation ("Noise Tex" , 2D) = "white"
		_LightDir ("Light Direction", Vector) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader { 
		Pass{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, Xbox360, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 xbox360 gles
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members normalDir)
#pragma exclude_renderers d3d11 xbox360
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				uniform float _Scale;
				uniform float _Height;
				uniform float _Frequency;
				uniform float _Lacunarity;
				uniform float _Gain;
				uniform fixed4 _Color; // low precision type is enough for colors
				uniform sampler2D permutation;
				uniform float4 _LightDir;
				uniform float4 _noiseOffset;
				
				struct v2f {
		          float4 pos : SV_POSITION;
		          fixed4 color : COLOR;
		          float4 texcoord : TEXCOORD0;
				  float3 normalDir;
				  float3 specularDir;
		          // float4 screenPos;
		     	};
				
		 		int perm(int d)
				{
					d = d % 256;
					float2 t = float2(d%16,d/16)/15.0;
					return tex2D(permutation,t).r *255;
				}
				
				float fade(float t) { return t * t * t * (t * (t * 6.0 - 15.0) + 10.0); }
				
				float lerp(float t,float a,float b) { return a + t * (b - a); }
				
				float grad(int hash,float x,float y,float z)
				{
					int h	= hash % 16;										// & 15;
					float u = h<8 ? x : y;
					float v = h<4 ? y : (h==12||h==14 ? x : z);
					return ((h%2) == 0 ? u : -u) + (((h/2)%2) == 0 ? v : -v); 	// h&1, h&2 
				}
				
				float noise(float x, float y,float z)
				{	
					int X = (int)floor(x) % 256;	// & 255;
					int Y = (int)floor(y) % 256;	// & 255;
					int Z = (int)floor(z) % 256;	// & 255;
					
					x -= floor(x);
					y -= floor(y);
					z -= floor(z);
				      
					float u = fade(x);
					float v = fade(y);
					float w = fade(z);
					
					int A	= perm(X  	)+Y;
					int AA	= perm(A	)+Z;
					int AB	= perm(A+1	)+Z; 
					int B	= perm(X+1	)+Y;
					int BA	= perm(B	)+Z;
					int BB	= perm(B+1	)+Z;
						
					return lerp(w, lerp(v, lerp(u, grad(perm(AA  ), x  , y  , z   ),
				                                   grad(perm(BA  ), x-1, y  , z   )),
				                           lerp(u, grad(perm(AB  ), x  , y-1, z   ),
				                                   grad(perm(BB  ), x-1, y-1, z   ))),
				                   lerp(v, lerp(u, grad(perm(AA+1), x  , y  , z-1 ),
				                                   grad(perm(BA+1), x-1, y  , z-1 )),
				                           lerp(u, grad(perm(AB+1), x  , y-1, z-1 ),
				                                   grad(perm(BB+1), x-1, y-1, z-1 ))));
				}

						
				float4 scale(float4 input, float scale)
		 		{
		 			return float4(input.x*scale, input.y*scale, input.z*scale, input.w);
		 		}
				
				v2f vert (appdata_base v)
				{
					v2f o;
					
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					
					o.normalDir = v.normal;
					o.specularDir = mul(UNITY_MATRIX_MVP, v.vertex);
					//o.screenPos = ComputeScreenPos(o.pos);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR0 
				{
					float nx 	= i.texcoord.x*_Scale +_noiseOffset.x;
					float ny 	= i.texcoord.y*_Scale +_noiseOffset.y;
					float ns 	= noise(nx, ny, 0)*_Height;
					float4 colo = float4(ns, ns, ns, 1);
					float lightIntensity = dot(i.normalDir, _LightDir);
					float specularIntensity = 1-dot(i.specularDir, _LightDir);
					return colo*_Color*lightIntensity; 
		 		}
		 		
		 		
		 		
			
			ENDCG
		}
	} 
}
