Shader "MrNothing's Shaders/Vertex Colored Billboard (No Tex)" {
   Properties {
   }
   SubShader {
		Fog {Mode Off}
      Pass {   
         CGPROGRAM
 		
         #pragma vertex vert  
         #pragma fragment frag 
 
         // User-specified uniforms            
         uniform sampler2D _MainTex;        
 
         struct vertexInput {
            float4 vertex : POSITION;
            fixed4 color : COLOR;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            //output.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) - float4(input.vertex.x, input.vertex.z, 0.0, 0.0));
			output.color  = input.color;
			output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            return input.color;   
         }
 
         ENDCG
      }
   }
}