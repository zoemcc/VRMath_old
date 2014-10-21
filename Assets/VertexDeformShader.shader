Shader "Cg shader for plotting 2d functions" {
    Properties {
        //_QuadFormVec ("Quadratic Form Graph", Vector) = (1.0, 0.0, 1.0, 0.0)
        //_QuadFormVecxx ("Quadratic Form xx", Float) = 1.0
        //_QuadFormVecxz ("Quadratic Form xz", Float) = 0.5
        //_QuadFormVeczz ("Quadratic Form zz", Float) = 1.0
        //_QuadFormMatx ("Quadratic Form Matrix", Matrix4x4) = ((1.0, 0, 0, 0), (0, 1.0, 0, 0), (0, 0, 0, 0), (0, 0, 0, 0))
        //_QuadForm ("Quadratic Form Form", Matrix) = ((1.0, 0, 0, 0), (0, 1.0, 0, 0), (0, 0, 0, 0), (0, 0, 0, 0))
        //_Color ("Main Color", Color) = (1,0.5,0.5,1)

    }
   SubShader {
      Pass {   
         Cull Off
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         
         
 
         // Uniforms set by a script
         uniform float4x4 _QuadForm; // matrix for quadratic form that determines shape of function
         uniform float4x4 _EllipseTransformer;
         //float _QuadFormVecxx;
         //float _QuadFormVecxz;
         //float _QuadFormVeczz;
 
         struct vertexInput {
            float4 vertex : POSITION;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 col : COLOR;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
            //float4x4 _QuadFormMat = ((_QuadFormVecxx, _QuadFormVecxz, 0, 0), (_QuadFormVecxz, _QuadFormVeczz, 0, 0),
            //						 (0, 0, 0, 0), (0, 0, 0, 0));
            //float2 vec2d = (input.vertex.x, input.vertex.z);
            
            float4 blendedVertex = mul(_EllipseTransformer, input.vertex);
            
            
            float height = mul(blendedVertex, mul(_QuadForm, blendedVertex));
            
            float integralHeight;
            float remainderHeight = modf(10 * abs(height), integralHeight);
 
            //float weight0 = input.vertex.z + 0.5; 
               // depends on the mesh
            //float4 blendedVertex = 
              // weight0 * mul(_Trafo0, input.vertex) 
               //+ (1.0 - weight0) * mul(_Trafo1, input.vertex);
            //float4 blendedVertex = input.vertex;
            blendedVertex.y += height;
             
            output.pos = mul(UNITY_MATRIX_MVP, blendedVertex);
            
 
            output.col = float4(remainderHeight, 1.0 - remainderHeight, 0.0, 1.0); 
               // visualize weight0 as red and weight1 as green
            return output;
         }
 		 
         float4 frag(vertexOutput input) : COLOR
         {
            return input.col;
         }
         
 
         ENDCG
      }
   }
}