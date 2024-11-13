//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"

void DoMotionBlur_float(in SamplerState samplerState, in float maxSamples, in float2 UV, in float2 velocity, out float4 result)
{
    float nSamples = maxSamples;
    result = SAMPLE_TEXTURE2D(_BlitTexture , samplerState ,UV);
    
    [unroll(4)] for (int i = 1; i < nSamples; ++i) {
        float2 offset = velocity * (float(i) / float(nSamples - 1) - 0.5);
        result += SAMPLE_TEXTURE2D(_BlitTexture , samplerState, UV + offset);
        
    }
    result /= float(nSamples);
}