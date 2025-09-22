Shader "Custom/ProceduralTunnel"
{
    Properties
    {
        _DotRadius("Dot Size", Float) = 0.05
        _Speed("Scroll Speed", Float) = 1.0
        _LineCount("Radial Lines", Float) = 20.0
        _RingCount("Rings", Float) = 10.0
        _Color("Tunnel Color", Color) = (1,1,1,1)
        _Background("Background Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float3 _CamPos;
            uniform float3 _PortalNormal;
            uniform float3 _PortalPosition;
            float _Speed;
            float _DotRadius;
            static const int dotNum = 2;
            uniform int _NumPoints;
            uniform float4 _Points[100];

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }

            // Compute distance from point P to line defined by points A and B
            float DistanceToLine(float3 A, float3 B, float3 P)
            {
                float3 AB = B - A;
                float3 AP = P - A;
                
                float distance = length(cross(AP, AB)) / length(AB);
                return distance;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // maybe see what the frag shaders pixel world position is, then run a line through the cube and see close that line is to the dot
                // if its close then make it the dot color, if its not close then make it clear.
                float3 pixelWorldPos = i.worldPos;
                
                // see if pixel is close to world pos
                for (int i = 0; i < _NumPoints; i++) {
                    float dist = DistanceToLine(_CamPos, pixelWorldPos, _Points[i] + _PortalPosition + (_PortalNormal * _Time.x * _Speed));
                    if (dist < _DotRadius * _Points[i].w)
                        return float4(1, 1, 1, 1);
                }
                return float4(0,0,0,1);
            }
            ENDHLSL
        }
    }
}
