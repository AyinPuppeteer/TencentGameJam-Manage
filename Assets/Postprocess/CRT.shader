Shader "Custom/CRT"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _LowResTex ("Low Res Texture", 2D) = "white" {}

        [Header(Main)]
        _Brightness ("Brightness", Range(0.5, 2.0)) = 1.0
        _Contrast ("Contrast", Range(0.5, 2.0)) = 1.0
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.25

        [Header(Scanline)]
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.2
        _ScanlineDensity ("Scanline Density", Range(100, 2000)) = 800

        [Header(RGB Split)]
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.01)) = 0.0015

        [Header(Dot Mask)]
        _DotMaskScale ("Dot Mask Scale", Range(100, 2000)) = 700
        _DotMaskStrength ("Dot Mask Strength", Range(0, 1)) = 0.35
        _DotRadius ("Dot Radius", Range(0.05, 0.45)) = 0.22
        _DotSoftness ("Dot Softness", Range(0.001, 0.2)) = 0.08

        [Header(Flicker)]
        _FlickerStrength ("Flicker Strength", Range(0, 0.2)) = 0.03
        _FlickerSpeed ("Flicker Speed", Range(0, 20)) = 8.0

        [Header(Low Res Info From Script)]
        _LowResWidth ("Low Res Width", Float) = 320
        _LowResHeight ("Low Res Height", Float) = 180
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Overlay"
        }

        Pass
        {
            Name "CRT DotMask Pass"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_LowResTex);
            SAMPLER(sampler_LowResTex);

            CBUFFER_START(UnityPerMaterial)
                float _Brightness;
                float _Contrast;
                float _VignetteStrength;

                float _ScanlineIntensity;
                float _ScanlineDensity;

                float _ChromaticAberration;

                float _DotMaskScale;
                float _DotMaskStrength;
                float _DotRadius;
                float _DotSoftness;

                float _FlickerStrength;
                float _FlickerSpeed;

                float _LowResWidth;
                float _LowResHeight;
            CBUFFER_END

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            Varyings Vert(Attributes input)
            {
                Varyings output;

                float2 uv = float2((input.vertexID << 1) & 2, input.vertexID & 2);
                output.uv = float2(uv.x, 1.0 - uv.y);
                output.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);

                return output;
            }

            float3 ApplyContrast(float3 color, float contrast)
            {
                return (color - 0.5) * contrast + 0.5;
            }

            float CircleMask(float2 p, float radius, float softness)
            {
                float d = length(p);
                return 1.0 - smoothstep(radius, radius + softness, d);
            }

            float3 SampleLowResCRT(float2 uv)
            {
                float2 uvR = uv + float2(_ChromaticAberration, 0.0);
                float2 uvG = uv;
                float2 uvB = uv - float2(_ChromaticAberration, 0.0);

                float r = SAMPLE_TEXTURE2D(_LowResTex, sampler_LowResTex, uvR).r;
                float g = SAMPLE_TEXTURE2D(_LowResTex, sampler_LowResTex, uvG).g;
                float b = SAMPLE_TEXTURE2D(_LowResTex, sampler_LowResTex, uvB).b;

                return float3(r, g, b);
            }

            float3 BuildDotMask(float2 uv)
            {
                float2 grid = uv * _DotMaskScale;

                float rowParity = fmod(floor(grid.y), 2.0);
                grid.x += rowParity * 0.5;

                float2 cell = frac(grid) - 0.5;

                float3 mask = float3(0.18, 0.18, 0.18);

                float2 redCenter   = float2(-0.22, 0.0);
                float2 greenCenter = float2( 0.00, 0.0);
                float2 blueCenter  = float2( 0.22, 0.0);

                float redDot   = CircleMask(cell - redCenter,   _DotRadius, _DotSoftness);
                float greenDot = CircleMask(cell - greenCenter, _DotRadius, _DotSoftness);
                float blueDot  = CircleMask(cell - blueCenter,  _DotRadius, _DotSoftness);

                mask.r = lerp(mask.r, 1.0, redDot);
                mask.g = lerp(mask.g, 1.0, greenDot);
                mask.b = lerp(mask.b, 1.0, blueDot);

                return mask;
            }

            float BuildScanline(float2 uv)
            {
                float s = sin(uv.y * _ScanlineDensity * 3.14159265);
                s = s * 0.5 + 0.5;
                return lerp(1.0, s, _ScanlineIntensity);
            }

            float BuildVignette(float2 uv)
            {
                float2 centered = uv * 2.0 - 1.0;
                float dist = dot(centered, centered);
                float vignette = 1.0 - dist * _VignetteStrength;
                return saturate(vignette);
            }

            float BuildFlicker()
            {
                float flicker = sin(_Time.y * _FlickerSpeed) * 0.5 + 0.5;
                return lerp(1.0 - _FlickerStrength, 1.0, flicker);
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                float3 color = SampleLowResCRT(uv);

                float3 dotMask = BuildDotMask(uv);
                float  scan    = BuildScanline(uv);
                float  vig     = BuildVignette(uv);
                float  flicker = BuildFlicker();

                color *= lerp(float3(1.0, 1.0, 1.0), dotMask, _DotMaskStrength);
                color *= scan;
                color *= vig;
                color *= flicker;

                color = ApplyContrast(color, _Contrast);
                color *= _Brightness;

                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}