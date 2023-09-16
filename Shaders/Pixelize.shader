Shader "Gamer025/Pixelize"
{
	Properties
	{
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


			sampler2D _ScreenTexture;
			float4 _ScreenTexture_TexelSize;
			float Gamer025_PixelizeIntens;

			//Inspired by https://luka712.github.io/2018/07/01/Pixelate-it-Shadertoy-Unity/
			fixed4 frag(v2f i) : SV_Target
			{
				float pixelX = 1 / _ScreenTexture_TexelSize / Gamer025_PixelizeIntens;
				float pixelY = 1 / _ScreenTexture_TexelSize / Gamer025_PixelizeIntens;

				return tex2D(_ScreenTexture, float2(floor(pixelX * i.grabPos.x) / pixelX, floor(pixelY * i.grabPos.y) / pixelY));
			 }
			 ENDCG
		 }
	}
}
