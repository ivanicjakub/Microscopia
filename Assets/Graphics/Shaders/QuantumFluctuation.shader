Shader "Custom/QuantumFluctuation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionAmount ("Distortion", Range(0, 1)) = 0.1
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
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
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float _DistortionAmount;
            float _WaveSpeed;
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                // Apply quantum distortion effect
                float2 distortion = float2(
                    sin(uv.y * 10 + _Time.y * _WaveSpeed),
                    cos(uv.x * 10 + _Time.y * _WaveSpeed)
                ) * _DistortionAmount;
                
                uv += distortion;
                
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                // Add quantum glow effect
                float glow = sin(_Time.y * _WaveSpeed + uv.x * 10) * 0.5 + 0.5;
                color.rgb += float3(0.1, 0.2, 0.3) * glow;
                
                return color;
            }
            ENDHLSL
        }
    }
}
