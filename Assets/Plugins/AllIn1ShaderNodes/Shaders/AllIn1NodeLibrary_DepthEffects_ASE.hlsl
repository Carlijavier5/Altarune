#ifndef ALLIN1NODELIBRARY_DEPTHEFFECTS_ASE_INCLUDED
#define ALLIN1NODELIBRARY_DEPTHEFFECTS_ASE_INCLUDED

#include "./AllIn1NodeLibrary_DepthEffects.hlsl"

float3 IntersectionGlowInternal(float3 inputRGB, float inputAlpha,
	float _DepthGlowDist, float _DepthGlowPower, float _DepthGlowColorIntensity, float _DepthGlowGlobalIntensity, 
	float4 _DepthGlowColor, float sceneDepthDiff)
{
	float3 res = inputRGB;
	
	float depthGlowMask = saturate(_DepthGlowDist * pow(max(0, 1 - sceneDepthDiff), _DepthGlowPower));
	
	res = lerp(inputRGB, _DepthGlowGlobalIntensity * inputRGB, depthGlowMask);
	res += _DepthGlowColor.rgb * _DepthGlowColorIntensity * depthGlowMask * inputAlpha;

	return res;
}

float SceneDepthDiff(float4 screenPositionRaw, float4 clipPos)
{
	float4 screenPosPD = screenPositionRaw / screenPositionRaw.w;
	
#ifdef ALLIN1NODE_BIRP
	float sceneLinearEyeDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenPosPD.xy));
	float vertexEyeDepth = LinearEyeDepth(clipPos.z);
#else
	float sceneLinearEyeDepth = LinearEyeDepth(SampleSceneDepth(screenPosPD.xy), _ZBufferParams);
	float vertexEyeDepth = LinearEyeDepth(clipPos.z, _ZBufferParams);
#endif
	

	
	
	float res = sceneLinearEyeDepth - vertexEyeDepth;
	return res;
}

#endif //ALLIN1NODELIBRARY_DEPTHEFFECTS_SG_INCLUDED