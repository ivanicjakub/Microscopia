#ifndef COSMIC_VOLUME_INCLUDED
#define COSMIC_VOLUME_INCLUDED

float3 hash33(float3 p) 
{
    p = float3(
        dot(p, float3(127.1, 311.7, 74.7)),
        dot(p, float3(269.5, 183.3, 246.1)),
        dot(p, float3(113.5, 271.9, 124.6))
    );
    return frac(sin(p) * 43758.5453123);
}

float VoronoiNoise(float3 pos, float time) 
{
    float3 baseCell = floor(pos);
    float minDist = 10.0;
    
    [unroll]
    for(int z = -1; z <= 1; z++) {
        for(int y = -1; y <= 1; y++) {
            for(int x = -1; x <= 1; x++) {
                float3 cell = baseCell + float3(x, y, z);
                float3 cellPosition = cell + hash33(cell);
                cellPosition.xy += sin(time * 0.5 + 2.0 * PI * hash33(cell).xy);
                float3 diff = cellPosition - pos;
                float dist = length(diff);
                minDist = min(minDist, dist);
            }
        }
    }
    return minDist;
}

float3 CurlNoise(float3 p, float time) 
{
    const float e = 0.0009765625;
    const float e2 = 2.0 * e;
    
    float3 dx = float3(e, 0.0, 0.0);
    float3 dy = float3(0.0, e, 0.0);
    float3 dz = float3(0.0, 0.0, e);
    
    float3 p_x0 = hash33(p - dx);
    float3 p_x1 = hash33(p + dx);
    float3 p_y0 = hash33(p - dy);
    float3 p_y1 = hash33(p + dy);
    float3 p_z0 = hash33(p - dz);
    float3 p_z1 = hash33(p + dz);
    
    float x = (p_y1.z - p_y0.z) - (p_z1.y - p_z0.y);
    float y = (p_z1.x - p_z0.x) - (p_x1.z - p_x0.z);
    float z = (p_x1.y - p_x0.y) - (p_y1.x - p_y0.x);
    
    return float3(x, y, z) / e2;
}

float Fbm(float3 pos, float time, int octaves, float persistence) 
{
    float total = 0.0;
    float amplitude = 1.0;
    float frequency = 1.0;
    float maxValue = 0.0;
    
    for(int i = 0; i < octaves; i++) 
    {
        float3 p = pos * frequency;
        p.xy += time * (0.1 * frequency);
        
        total += VoronoiNoise(p, time) * amplitude;
        maxValue += amplitude;
        amplitude *= persistence;
        frequency *= 2.0;
    }
    
    return total / maxValue;
}

// Updated main functions with position spaces
void CosmicVolume_float(
    float3 ObjectPosition,
    float3 WorldPosition,
    bool UseWorldSpace,
    float Time,
    float Scale,
    float Complexity,
    float Density,
    float Energy,
    float TimeScale,
    float4 BaseColor,
    float4 EmissionColor,
    float Turbulence,
    float VorticityStrength,
    out float4 Color,
    out float Alpha,
    out float3 Emission)
{
    float3 pos = UseWorldSpace ? WorldPosition : ObjectPosition;
    
    float3 scaledPos = pos * Scale;
    float scaledTime = Time * TimeScale;
    
    float baseNoise = Fbm(scaledPos, scaledTime, int(Complexity * 4) + 2, 0.5);
    
    float3 turbulence = CurlNoise(scaledPos * Turbulence, scaledTime);
    float3 distortedPos = scaledPos + turbulence * VorticityStrength;
    
    float energyNoise = Fbm(distortedPos * 2.0, scaledTime * 0.5, 2, 0.5);
    float energyFactor = lerp(0.5, 1.5, energyNoise) * Energy;
    
    float densityNoise = Fbm(distortedPos + turbulence * 0.5, scaledTime * 0.2, 3, 0.5);
    float finalDensity = saturate(densityNoise * Density);
    
    float3 baseColorVar = lerp(BaseColor.rgb * 0.8, BaseColor.rgb * 1.2, energyNoise);
    float3 emissionColorVar = EmissionColor.rgb * energyFactor;
    
    float edge = 1.0 - abs(dot(normalize(pos), float3(0, 1, 0)));
    edge = pow(edge, 3.0) * 0.5;
    
    Color = float4(baseColorVar, finalDensity);
    Alpha = finalDensity;
    Emission = emissionColorVar * energyFactor * (finalDensity + edge);
}

void QuantumFoam_float(
    float3 ObjectPosition,
    float3 WorldPosition,
    bool UseWorldSpace,
    float Time,
    float Scale,
    float Energy,
    float Fluctuation,
    out float4 Color,
    out float Alpha)
{
    // Choose position space
    float3 pos = UseWorldSpace ? WorldPosition : ObjectPosition;
    
    float3 scaledPos = pos * Scale;
    float scaledTime = Time * 2.0;
    
    // High-frequency quantum fluctuations
    float3 foam = CurlNoise(scaledPos * 10.0, scaledTime);
    float foamNoise = length(foam) * 0.5;
    
    // Energy probability distribution
    float energyDist = VoronoiNoise(scaledPos + foam * Fluctuation, scaledTime);
    float probability = exp(-energyDist * Energy);
    
    // Quantum uncertainty visualization
    float uncertainty = sin(dot(foam, float3(1.0, 1.0, 1.0)) * 10.0 + scaledTime) * 0.5 + 0.5;
    
    float finalAlpha = saturate(probability * uncertainty * foamNoise);
    float3 finalColor = lerp(float3(0.5, 0.7, 1.0), float3(1.0, 0.9, 0.8), uncertainty);
    
    Color = float4(finalColor * Energy, finalAlpha);
    Alpha = finalAlpha;
}

#endif