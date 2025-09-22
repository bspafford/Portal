﻿Shader "Custom/FaceNoise"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", Float) = 5.0
        _Speed ("Speed", Float) = 1.0
        _Threshold ("Threshold", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            float _NoiseScale;
            float _Speed;
            float _Threshold;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

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

            fixed4 frag(v2f i) : SV_Target
            {
                float2 noiseUV = floor(i.uv * _NoiseScale);
                float n = noise(noiseUV);

                fixed4 col = _Color;
                col.rgb *= n;
                col.a = n;

                if (n < _Threshold)
                    return _Color;
                return float4(0, 0, 0, 0);
            }

            ENDCG
        }
    }
}
