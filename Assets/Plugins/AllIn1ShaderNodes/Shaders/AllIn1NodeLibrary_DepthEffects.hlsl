#ifndef ALLIN1NODELIBRARY_DEPTHEFFECTS_INCLUDED
#define ALLIN1NODELIBRARY_DEPTHEFFECTS_INCLUDED

float3 IntersectionGlow(float3 inputRGB, float inputAlpha, float sceneDepthDiff, 
	float _DepthGlowDist, float _DepthGlowPower, float _DepthGlowColorIntensity, 
	float _DepthGlowGlobalIntensity, float4 _DepthGlowColor)
{
	float3 res = inputRGB;
	
	float depthGlowMask = saturate(_DepthGlowDist * pow(max(0, 1 - sceneDepthDiff), _DepthGlowPower));
	
	res = lerp(inputRGB, _DepthGlowGlobalIntensity * inputRGB, depthGlowMask);
	res += _DepthGlowColor.rgb * _DepthGlowColorIntensity * depthGlowMask * inputAlpha;

	return res;
}

#endif //ALLIN1NODELIBRARY_DEPTHEFFECTS_INCLUDED