#ifndef ALLIN1NODELIBRARY_ALPHAEFFECTS_SG_INCLUDED
#define ALLIN1NODELIBRARY_ALPHAEFFECTS_SG_INCLUDED

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_AlphaEffects.hlsl"

void Fade_float(float inputAlpha, float _FadePower, float _FadeAmount, float _FadeTransition, UnityTexture2D _FadeTex, float2 uv, float2 tiling, out float Out)
{
	float res = Fade(inputAlpha, _FadePower, _FadeAmount, _FadeTransition, _FadeTex, uv, tiling);
	Out = res;
}

void Fade_Burn_float(float3 inputRGB, float inputAlpha, float _FadePower, float _FadeAmount, float _FadeTransition, float _FadeBurnWidth, float4 _FadeBurnColor, UnityTexture2D _FadeTex, float2 uv, float2 tiling, out float3 OutRGB, out float OutAlpha)
{
	float4 res = Fade_Burn(inputRGB, inputAlpha, _FadePower, _FadeAmount, _FadeTransition, _FadeBurnWidth, _FadeBurnColor, _FadeTex, uv, tiling);
	
	OutRGB = res.rgb;
	OutAlpha = res.a;
}

void FadeByCamDistance_NearFade_float(float inputAlpha, float _MinDistanceToFade, float _MaxDistanceToFade, float3 cameraPositionWS, float3 vertexPositionWS, out float Out)
{
	float res = FadeByCamDistance_NearFade(inputAlpha, _MinDistanceToFade, _MaxDistanceToFade, cameraPositionWS, vertexPositionWS);
	Out = res;
}

void FadeByCamDistance_FarFade_float(float inputAlpha, float _MinDistanceToFade, float _MaxDistanceToFade, float3 cameraPositionWS, float3 vertexPositionWS, out float Out)
{
	float res = FadeByCamDistance_FarFade(inputAlpha, _MinDistanceToFade, _MaxDistanceToFade, cameraPositionWS, vertexPositionWS);
	Out = res;
}

void AlphaMask_float(float inputAlpha, UnityTexture2D _MaskTex, float2 uv, float2 tiling, float _MaskPower, out float Out)
{
	float res = AlphaMask(inputAlpha, _MaskTex, uv, tiling, _MaskPower);
	Out = res;
}

void SoftParticles_float(float inputAlpha, float sceneDepthDiff, float _SoftFactor, out float Out)
{
	float res = SoftParticles(inputAlpha, sceneDepthDiff, _SoftFactor);
	Out = res;
}

void IntersectionFade_float(float inputAlpha, float sceneDepthDiff, float _IntersectionFadeFactor, out float Out)
{
	float res = IntersectionFade(inputAlpha, sceneDepthDiff, _IntersectionFadeFactor);
	Out = res;
}

void AlphaRemap_float(float inputAlpha, float _AlphaStepMin, float _AlphaStepMax, out float Out)
{
	float res = AlphaRemap(inputAlpha, _AlphaStepMin, _AlphaStepMax);
	Out = res;
}

void AlphaRound_float(float inputAlpha, out float Out)
{
	float res = AlphaRound(inputAlpha);
	Out = res;
}

#endif //ALLIN1NODELIBRARY_ALPHAEFFECTS_SG_INCLUDED