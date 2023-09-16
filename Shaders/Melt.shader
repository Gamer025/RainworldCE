Shader "Gamer025/Melt"
{
	Properties
	{
		//Gamer025_MeltTex("MeltTexture", 2D) = "black" {}
	}
		SubShader
		 {
			 // No culling or depth
			 Cull Off ZWrite Off ZTest Always

			  GrabPass
			 {
				"_ScreenTexture"
			 }

			 Pass
			 {
				 CGPROGRAM
				 #pragma vertex vert
				 #pragma fragment frag
				 #pragma enable_d3d11_debug_symbols
				 #include "UnityCG.cginc"

				 struct appdata
				 {
					 float4 vertex : POSITION;
					 float2 uv : TEXCOORD0;
				 };

				 struct v2f
				 {
					 float2 grabPos : TEXCOORD0;
					 float4 vertex : SV_POSITION;
				 };


				 float when_gt(float x, float y) {
					 return max(sign(x - y), 0.0);
				 }

				 float when_lt(float x, float y) {
					 return max(sign(y - x), 0.0);
				 }

				 v2f vert(appdata v)
				 {
					 v2f o;
					 o.vertex = UnityObjectToClipPos(v.vertex);
					 o.grabPos = ComputeGrabScreenPos(o.vertex);
					 return o;
				 }


				 sampler2D Gamer025_MeltTex;
				 sampler2D _ScreenTexture;
				 float4 _ScreenTexture_TexelSize;

				 fixed4 frag(v2f i) : SV_Target
				 {
					 float meltAmt = tex2D(Gamer025_MeltTex, i.grabPos).r;
					 fixed4 above = tex2D(_ScreenTexture, i.grabPos - float2(0, _ScreenTexture_TexelSize.y * meltAmt * 255));
					 fixed4 col = fixed4(0, 0, 0, 1);
					 if (above.r + above.g + above.b > 0)
						 col = above;
					 return col;
				 }
				 ENDCG
			 }
		 }
}
