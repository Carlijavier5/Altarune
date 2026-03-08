#ifndef ALLIN1NODE_LIGHTINGEFFECTS_SG_INCLUDED
#define ALLIN1NODE_LIGHTINGEFFECTS_SG_INCLUDED

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_LightingEffects.hlsl"

void AOMap_float(float _AOContrast, float _AOMapStrength, float3 _AOColor, float2 uv, TEX_PARAM _AOMap, out float3 Out)
{
	float3 res = AOMap(_AOContrast, _AOMapStrength, _AOColor, uv, _AOMap);
	Out = res;
}

void GetNormalWSFromNormalMap_float(TEX_PARAM _NormalMap, float normalStrength, float2 uv, float3 normalWS, float3 tangentWS, float3 bitangentWS, out float3 Out)
{
	Out = GetNormalWSFromNormalMap(_NormalMap, normalStrength, uv, normalWS, tangentWS, bitangentWS);
}

#endif //ALLIN1NODE_LIGHTINGEFFECTS_SG_INCLUDED