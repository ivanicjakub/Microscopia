float3 LightingSpecular(float3 lightColor, float3 lightDir, float3 normal, float3 viewDir, float3 specular, float smoothness)
{
    float3 halfVec = SafeNormalize(lightDir + viewDir);
    float NdotH = saturate(dot(normal, halfVec));
    float modifier = pow(NdotH, smoothness);
    float3 specularReflection = specular.rgb * modifier;
    return lightColor * specularReflection;
}

void DirectSpecular_float(float3 Specular, float Smoothness, float3 Direction, float3 Color, float3 WorldNormal, float3 WorldView, out float3 Out)
{
   Smoothness = exp2(10 * Smoothness + 1);
   WorldNormal = normalize(WorldNormal);
   WorldView = SafeNormalize(WorldView);
   Out = LightingSpecular(Color, Direction, WorldNormal, WorldView, Specular, Smoothness);
}