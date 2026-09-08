// Recovered from original UI/UI Grey GLES program and serialized pass state.
Shader "UI/UI Grey"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Grayness ("Grayness", Range(0, 1)) = 1
        _Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Pass
        {
            ZWrite Off
            ZTest LEqual
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Grayness;
                float _Alpha;
            CBUFFER_END
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };
            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return output;
            }
            half4 Frag(Varyings input) : SV_Target
            {
                half4 sampleColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float gray = dot(sampleColor.rgb, float3(0.298999995, 0.587000012, 0.114));
                return half4(_Grayness * (gray.xxx - sampleColor.rgb) + sampleColor.rgb, sampleColor.a * _Alpha);
            }
            ENDHLSL
        }
    }
    Fallback "Diffuse"
}
