#ifndef ALLIN1NODELIBRARY_DIFFUSEMODELS_NODEFUNCTIONS
#define ALLIN1NODELIBRARY_DIFFUSEMODELS_NODEFUNCTIONS

#include "./AllIn1NodeLibrary_DiffuseModels.hlsl"

float3 DiffuseLight_ToonRamp(float3 normalWS, sampler2D _ToonRamp)
{
	float rawNdotL = GetRawNdotL(normalWS);
	float NdotL = (rawNdotL * 0.5) + 0.5;
	float4 uv = float4(NdotL, 0, 0, 0);
	
	float3 toonRampColor = SAMPLE_TEX2D_LOD(_ToonRamp, float4(NdotL, 0, 0, 0)).rgb;
	
	float3 res = DiffuseLight_ToonRamp(toonRampColor);
	
	return res;
}

#endif //ALLIN1NODELIBRARY_DIFFUSEMODELS_NODEFUNCTIONS