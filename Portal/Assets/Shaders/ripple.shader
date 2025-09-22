Shader "Custom/CenterOutwardRipple"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.6, 1, 1)
        _RippleFrequency ("Ripple Frequency", Float) = 15.0
        _RippleSpeed ("Ripple Speed", Float) = 10.0
        _RippleStrength ("Ripple Strength", Float) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _RippleFrequency;
            float _RippleSpeed;
            float _RippleStrength;
            uniform float3 _PortalPosition;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 center = _PortalPosition.xyz;

                float3 offset = i.worldPos - center;
                offset.y /= 2.0;
                float dist = length(float3(offset.x, offset.y, offset.z));;
                float ripple = sin(dist * _RippleFrequency - _Time.y * _RippleSpeed);
                float brightness = 1.0 + ripple * _RippleStrength;

                return _Color * brightness;
            }
            ENDCG
        }
    }
    FallBack Off
}
