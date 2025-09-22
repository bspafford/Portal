Shader "Custom/WorldSpaceRadialDistortion"
{
    Properties
    {
        _Strength("Distortion Strength", Float) = 0.05
        _Radius("Distortion Radius (World Units)", Float) = 1.0
        _Falloff("Falloff Sharpness", Float) = 3.0
        _Center("Distortion Center (World)", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Distortion"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenUV : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _CameraOpaqueTexture;
            float4 _Center;       // world-space distortion center
            float _Strength;      // distortion strength
            float _Radius;        // world-space radius
            float _Falloff;       // how quickly distortion falls off

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.screenUV = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 worldPos = IN.worldPos;

                // Full 3D offset from center
                float3 offset = worldPos - _Center.xyz;
                float dist = length(offset);

                // No distortion beyond radius
                if (dist >= _Radius)
                {
                    float2 baseUV = IN.screenUV.xy / IN.screenUV.w;
                    return tex2D(_CameraOpaqueTexture, baseUV);
                }

                // Smooth falloff to 0 at the edge
                float fade = 1.0 - (dist / _Radius);
                fade = smoothstep(0.0, 1.0, fade); // smooth edge

                // Push outward in 3D
                float3 displacedWorldPos = worldPos + normalize(offset) * _Strength * fade;

                // Project both positions to clip space
                float4 originalClipPos = TransformWorldToHClip(worldPos);
                float4 pushedClipPos   = TransformWorldToHClip(displacedWorldPos);

                // Convert to screen-space UVs
                float2 originalUV = ComputeScreenPos(originalClipPos).xy / originalClipPos.w;
                float2 pushedUV   = ComputeScreenPos(pushedClipPos).xy / pushedClipPos.w;

                float2 screenOffset = pushedUV - originalUV;

                // Apply UV distortion
                float2 distortedUV = IN.screenUV.xy / IN.screenUV.w + screenOffset;

                // Sample and return
                return tex2D(_CameraOpaqueTexture, distortedUV);
            }
            ENDHLSL
        }
    }
}
