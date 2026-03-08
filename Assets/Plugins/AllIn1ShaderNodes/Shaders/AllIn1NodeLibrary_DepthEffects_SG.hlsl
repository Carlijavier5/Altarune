#ifndef ALLIN1NODELIBRARY_DEPTHEFFECTS_SG_INCLUDED
#define ALLIN1NODELIBRARY_DEPTHEFFECTS_SG_INCLUDED

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_DepthEffects.hlsl"

void IntersectionGlow_float(float3 inputRGB, float inputAlpha, float sceneDepthDiff,
	float _DepthGlowDist, float _DepthGlowPower, float _DepthGlowColorIntensity, float _DepthGlowGlobalIntensity, float4 _DepthGlowColor, 
	out float3 Out)
{
	float3 res = IntersectionGlow(inputRGB, inputAlpha, sceneDepthDiff, 
		_DepthGlowDist, _DepthGlowPower, _DepthGlowColorIntensity, 
		_DepthGlowGlobalIntensity, _DepthGlowColor);

	Out = res;
}

#endif //ALLIN1NODELIBRARY_DEPTHEFFECTS_SG_INCLUDED