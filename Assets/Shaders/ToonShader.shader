// Universal Render Pipeline version of the toon character shader.
// The previous implementation was a built-in surface shader
// (#pragma surface surf Ramp), which URP cannot render.
Shader "Custom/ToonShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _SpecularPower ("Specular Power", Range(0.0,1.0)) = 0.01
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SpecTex ("Specular (B&W)", 2D) = "white" {}
        _BumpScale ("Scale", Float) = 1.0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Ramp ("Toon Ramp", 2D) = "white" {}
        _Border ("Border size", Range(0.0,0.1)) = 0.01
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        [Toggle(SPECULAR_ENABLE)] _SpecularEnable ("Enable Specular", Int) = 0
        [Toggle(OUTLINE_FRONT)] _OutlineFront ("Enable Outline", Int) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Transparent+2"
        }

        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Front
            ZWrite Off
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BumpMap_ST;
                float4 _Ramp_ST;
                float4 _Color;
                float4 _BorderColor;
                float  _SpecularPower;
                float  _BumpScale;
                float  _Border;
                float  _SpecularEnable;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4  color      : COLOR;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float4 positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 normalVS = TransformWorldToViewDir(normalWS);
                float2 direction = normalVS.xy;
                direction *= rsqrt(max(dot(direction, direction), 1e-8));
                positionCS.xy += direction * _Border * positionCS.w * 2.0;

                OUT.positionCS = positionCS;
                OUT.color = half4(_BorderColor.rgb, 1.0h);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return IN.color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment SPECULAR_ENABLE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BumpMap_ST;
                float4 _Ramp_ST;
                float4 _Color;
                float4 _BorderColor;
                float  _SpecularPower;
                float  _BumpScale;
                float  _Border;
                float  _SpecularEnable;
            CBUFFER_END

            TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
            TEXTURE2D(_SpecTex);  SAMPLER(sampler_SpecTex);
            TEXTURE2D(_BumpMap);  SAMPLER(sampler_BumpMap);
            TEXTURE2D(_Ramp);     SAMPLER(sampler_Ramp);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 tangentWS   : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float3 viewDirWS   : TEXCOORD4;
                float  fogFactor   : TEXCOORD5;
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
                half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv), (real)_BumpScale);
                half3 normalWSMapped = normalize(
                    normalTS.x * normalize(IN.tangentWS) +
                    normalTS.y * normalize(IN.bitangentWS) +
                    normalTS.z * normalWS);

                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;

                Light mainLight = GetMainLight();
                half noL = dot(normalWSMapped, mainLight.direction);
                half rampCoord = saturate(noL * 0.5h + 0.5h);
                half3 ramp = SAMPLE_TEXTURE2D(_Ramp, sampler_Ramp, float2(rampCoord, rampCoord)).rgb;

                half3 color = albedo.rgb * mainLight.color * ramp;

#if defined(SPECULAR_ENABLE)
                half3 halfDir = normalize(mainLight.direction + normalize(IN.viewDirWS));
                half spec = pow(saturate(dot(normalWSMapped, halfDir)), 128.0h);
                half3 specTex = SAMPLE_TEXTURE2D(_SpecTex, sampler_SpecTex, IN.uv).rgb;
                color += mainLight.color * spec * specTex * _SpecularPower;
#endif

                color = MixFog(color, IN.fogFactor);
                return half4(color, 1.0h);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
