Shader "Custom/ColoredMaskStencil"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // Stencil settings
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            // DEPTH SETTINGS GO HERE
            ZWrite On      // Write to depth buffer
            ZTest LEqual   // Only draw if closer than current depth
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
