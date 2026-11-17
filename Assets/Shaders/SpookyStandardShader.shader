// Universal Render Pipeline version of the spooky character/ghost shader.
// The previous implementation was a built-in "Standard" surface shader
// (#pragma surface surf Standard alpha), which URP cannot render.
Shader "Custom/SpookyStandardShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _BumpScale ("Scale", Float) = 1.0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimMask ("Rim Mask", 2D) = "white" {}
        _RimPower ("Rim Power", Range(0.01,10.0)) = 3
        _RimBias ("Rim Bias", Range(0.1,1.0)) = 0.5
        _EmissiveMap ("Emission", 2D) = "black" {}
        [HDR] _EmissiveColor ("Emission Color", Color) = (0,0,0,0)
        _DetailMask ("Detail Mask(A)", 2D) = "gray" {}
        _Detail ("Detail (RGB)", 2D) = "gray" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent+1"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BumpMap_ST;
                float4 _RimMask_ST;
                float4 _Detail_ST;
                float4 _DetailMask_ST;
                float4 _EmissiveMap_ST;
                float4 _Color;
                float4 _RimColor;
                float4 _EmissiveColor;
                float  _Glossiness;
                float  _Metallic;
                float  _BumpScale;
                float  _RimPower;
                float  _RimBias;
            CBUFFER_END

            TEXTURE2D(_MainTex);      SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);      SAMPLER(sampler_BumpMap);
            TEXTURE2D(_RimMask);      SAMPLER(sampler_RimMask);
            TEXTURE2D(_Detail);       SAMPLER(sampler_Detail);
            TEXTURE2D(_DetailMask);   SAMPLER(sampler_DetailMask);
            TEXTURE2D(_EmissiveMap);  SAMPLER(sampler_EmissiveMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 tangentWS  : TEXCOORD2;
                float3 bitangentWS: TEXCOORD3;
                float3 viewDirWS  : TEXCOORD4;
                float  fogFactor  : TEXCOORD5;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

                OUT.positionCS = positionInputs.positionCS;
                OUT.uv = IN.uv;
                OUT.normalWS = normalInputs.normalWS;
                OUT.tangentWS = normalInputs.tangentWS;
                OUT.bitangentWS = normalInputs.bitangentWS;
                OUT.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                OUT.fogFactor = ComputeFogFactor(positionInputs.positionCS.z);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);

                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                half3 detail = SAMPLE_TEXTURE2D(_Detail, sampler_Detail, IN.uv).rgb;
                half4 mask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, IN.uv);
                albedo.rgb *= lerp(half3(1.0h, 1.0h, 1.0h), detail * 2.0h, mask.a);

                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv), (real)_BumpScale);
                half3 bitangentWS = normalize(IN.bitangentWS);
                half3 tangentWS = normalize(IN.tangentWS);
                half3 normalWSMapped = normalize(
                    normalTS.x * tangentWS + normalTS.y * bitangentWS + normalTS.z * normalWS);

                Light mainLight = GetMainLight();
                half noL = saturate(dot(normalWSMapped, mainLight.direction));
                half3 ambient = SampleSH(normalWSMapped);
                half3 diffuse = albedo.rgb * (mainLight.color * noL + ambient);

                // Cheap Blinn-Phong stand-in for the Standard specular term.
                half3 halfDir = normalize(mainLight.direction + viewDirWS);
                half specPower = exp2(_Glossiness * 10.0h + 1.0h);
                half spec = pow(saturate(dot(normalWSMapped, halfDir)), specPower) * _Glossiness;
                half3 specular = mainLight.color * spec * lerp(0.04h, 1.0h, _Metallic);

                half rim = _RimBias - saturate(dot(viewDirWS, normalWSMapped));
                half rimMask = SAMPLE_TEXTURE2D(_RimMask, sampler_RimMask, IN.uv).r;
                half rimTerm = saturate((rim - rimMask) / max(_RimBias, 1e-3h));
                half3 emission = SAMPLE_TEXTURE2D(_EmissiveMap, sampler_EmissiveMap, IN.uv).rgb * _EmissiveColor.rgb;
                emission += _RimColor.rgb * (rimTerm * _RimPower);

                half3 color = diffuse + specular + emission;
                color = MixFog(color, IN.fogFactor);
                return half4(color, albedo.a);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
