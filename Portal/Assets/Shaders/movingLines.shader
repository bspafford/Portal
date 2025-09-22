Shader "Custom/ProceduralTunnelLines"
{
    Properties
    {
        _Color ("Color", Color) = (0.2, 0.6, 1, 1)
        _RippleFrequency ("Ripple Frequency", Float) = 15.0
        _RippleSpeed ("Ripple Speed", Float) = 10.0
        _RippleStrength ("Ripple Strength", Float) = 0.3

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

            fixed4 _Color;
            float _RippleFrequency;
            float _RippleSpeed;
            float _RippleStrength;

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

            float DistanceBetweenLinesSegmented(float3 P1, float3 P2, float3 d1, float3 d2, out float3 intersection)
            {
                float3 _cross = cross(d1, d2);
                float denom = length(_cross);

                // If lines are parallel
                if (denom < 1e-6)
                {
                    float3 AP = P2 - P1;
                    float t = dot(AP, d1) / dot(d1, d1);

                    // If intersection occurs "before" the start of d1, return high number
                    if (t < 0) return 1e6;

                    return length(cross(AP, d1)) / length(d1);
                }

                // Lines are not parallel
                float3 diff = P2 - P1;
                float t1 = dot(cross(diff, d2), _cross) / (denom * denom);
                float t2 = dot(cross(diff, d1), _cross) / (denom * denom);

                // If intersection is "before" either line, return high number
                if (t1 < 0 || t2 < 0)
                    return 1e6;

                intersection = P1 + t1 * d1;
                return abs(dot(diff, _cross)) / denom;
            }

            float3 HSVtoRGB(float h, float s, float v)
            {
                float3 rgb = v.xxx;
                if (s > 0.0)
                {
                    h = frac(h) * 6.0; // map hue to 0-6
                    int i = (int)h;
                    float f = h - i;
                    float p = v * (1.0 - s);
                    float q = v * (1.0 - s * f);
                    float t = v * (1.0 - s * (1.0 - f));

                    if (i == 0) rgb = float3(v, t, p);
                    else if (i == 1) rgb = float3(q, v, p);
                    else if (i == 2) rgb = float3(p, v, t);
                    else if (i == 3) rgb = float3(p, q, v);
                    else if (i == 4) rgb = float3(t, p, v);
                    else rgb = float3(v, p, q);
                }
                return rgb;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 center = _PortalPosition.xyz;
                float3 offset = i.worldPos - center;
                offset.y /= 2.0;
                float dist = length(float3(offset.x, offset.y, offset.z));;
                float ripple = sin(dist * _RippleFrequency - _Time.y * _RippleSpeed);
                float brightness = 1.0 + ripple * _RippleStrength;

                // maybe see what the frag shaders pixel world position is, then run a line through the cube and see close that line is to the dot
                // if its close then make it the dot color, if its not close then make it clear.
                float3 pixelWorldPos = i.worldPos;
                
                // see if pixel is close to world pos
                float lineNum = 360;
                for (int i = 0; i < lineNum; i++) {
                    float3 forward = normalize(_PortalNormal);

                    // Pick an arbitrary vector that’s not parallel to the normal
                    float3 upGuess = abs(forward.y) < 0.99 ? float3(0,1,0) : float3(1,0,0);

                    // Calculate right and up vectors
                    float3 right = normalize(cross(upGuess, forward));
                    float3 up = cross(forward, right);

                    // Create a local offset
                    float angle = i / lineNum * 360 * (3.14159265 / 180);
                    float x = cos(angle);
                    float y = 2 * sin(angle);
                    float3 localOffset = float3(x, y, 0); // relative to local portal space

                    // Rotate local offset into world space using portal basis
                    float3 worldOffset =
                        localOffset.x * right +
                        localOffset.y * up +
                        localOffset.z * forward;

                    float3 lineStart = _PortalPosition + worldOffset;

                    // float3 lineStart = _PortalPosition + float3(x, y, 0);
                    float3 lineEnd = lineStart + _PortalNormal * 1;

                    float3 dir1 = normalize(pixelWorldPos - _CamPos);
                    float3 dir2 = normalize(lineEnd - lineStart);
                    float3 intersection;
                    float dist = DistanceBetweenLinesSegmented(_CamPos, lineStart, dir1, dir2, intersection);

                    float dist1 = .2 / distance(_PortalPosition, intersection) - .005;

                    if (dist < _DotRadius) {
                        return float4(0.1764705, 0.1921568, 0.4431372, 1) * dist1 * brightness;
                        // return float4(0.1764705, 0.1921568, 0.4431372, 1) * dist1;
                        // return float4(float3(HSVtoRGB(i / lineNum, 1, 1)), 1) * dist1;
                    }
                }
                return float4(0,0,0,1);
            }
            ENDHLSL
        }
    }
}
