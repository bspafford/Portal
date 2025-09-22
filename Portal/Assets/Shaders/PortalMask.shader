﻿Shader "Portals/PortalMask"
{
    Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 5.0
        _Threshold ("Threshold", Float) = 1.0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off
		Cull Back
		// ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Less
		
		Fog{ Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            float _NoiseScale;
			float _Threshold;

			float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f); // smoothstep

                return lerp(lerp(a, b, u.x), lerp(c, d, u.y), u.y);
            }

			fixed4 frag (v2f i) : SV_Target
			{
				float2 noiseUV = floor(i.uv * _NoiseScale);
                float n = noise(noiseUV);

				i.screenPos /= i.screenPos.w;
				fixed4 col = tex2D(_MainTex, float2(i.screenPos.x, i.screenPos.y));

                if (n < _Threshold)
                	return float4(0, 0, 0, 0);
				return col;

				
			}
			ENDCG
		}
	}
}