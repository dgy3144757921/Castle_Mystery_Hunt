// Universal Render Pipeline version of the orb core material.
// The previous implementation was a built-in "Standard" surface shader
// (#pragma surface surf Standard alpha:fade), which URP cannot render.
Shader "Custom/OrbCoreEmission"
{
    Properties
    {
        _Color ("Base Color", Color) = (0.1, 0.5, 0.2, 1)
        _EmissionColor ("Emission Color", Color) = (0.3, 2.2, 0.6, 1)
        _RimColor ("Rim Color", Color) = (0.6, 2.6, 0.9, 1)
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.0
        _AlphaBase ("Alpha Base", Range(0, 1)) = 0.15
        _AlphaRim ("Alpha Rim", Range(0, 1)) = 0.9
        _PulseSpeed ("Pulse Speed", Float) = 2.5
        _PulseStrength ("Pulse Strength", Range(0, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _EmissionColor;
                float4 _RimColor;
                float  _RimPower;
                float  _AlphaBase;
                float  _AlphaRim;
                float  _PulseSpeed;
                float  _PulseStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 viewDirWS  : TEXCOORD1;
                float  fogFactor  : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = positionInputs.positionCS;
                OUT.normalWS = normalInputs.normalWS;
                OUT.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                OUT.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);

                Light mainLight = GetMainLight();
                half noL = saturate(dot(normalWS, mainLight.direction));
                half3 ambient = SampleSH(normalWS);
                half3 diffuse = _Color.rgb * (mainLight.color * noL + ambient);

                half rim = 1.0h - saturate(dot(viewDirWS, normalWS));
                half rimTerm = pow(rim, (half)_RimPower);
                half pulse = 1.0h + (half)_PulseStrength * (half)sin(_Time.y * _PulseSpeed);

                half3 color = diffuse + (_EmissionColor.rgb + _RimColor.rgb * rimTerm) * pulse;
                half alpha = saturate((half)_AlphaBase + (half)_AlphaRim * rimTerm);

                color = MixFog(color, IN.fogFactor);
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
