void Iridescent_float(in float3 V, in float3 N, in float Noise, in float3 IridescentVector, in float WaveFactor, out float3 Iridescent)
{
    float3 grey = normalize(float3(1, 1, 1));
    float wave = dot(N, V) * Noise * PI * WaveFactor;
    float3 i = IridescentVector;

    float3 blueWaves = i * cos(wave);
    float3 redGreenWaves = cross(grey, i) * sin(wave);
    float3 waveBalance = grey * dot(grey, i) * (1 - cos(wave));
    
    Iridescent = blueWaves + redGreenWaves + waveBalance;
}
	