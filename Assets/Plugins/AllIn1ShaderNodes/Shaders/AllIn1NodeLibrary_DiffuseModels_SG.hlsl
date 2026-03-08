#ifndef ALLIN1NODELIBRARY_DIFFUSEMODELS_SG
#define ALLIN1NODELIBRARY_DIFFUSEMODELS_SG

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_DiffuseModels.hlsl"

void DiffuseLight_Classic_float(float3 normalWS, out float3 Out)
{
	float3 res = DiffuseLight_Classic(normalWS);
	Out = res;
}

void DiffuseLight_Toon_float(float3 normalWS, float _ToonCutoff, float _ToonSmoothness, out float3 Out)
{
	float3 res = DiffuseLight_Toon(normalWS, _ToonCutoff, _ToonSmoothness);
	Out = res;
}

void DiffuseLight_ToonRamp_float(float3 normalWS, UnityTexture2D _ToonRamp, out float3 Out)
{
	float3 res = DiffuseLight_ToonRamp(normalWS, _ToonRamp);
	Out = res;
}

void DiffuseLight_HalfLambert_float(float3 normalWS, float _HalfLambertWrap, out float3 Out)
{
	float3 res = DiffuseLight_HalfLambert(normalWS, _HalfLambertWrap);
	Out = res;
}

void DiffuseLight_FakeGI_float(float3 normalWS, float _HardnessFakeGI, out float3 Out)
{
	float3 res = DiffuseLight_FakeGI(normalWS, _HardnessFakeGI);
	Out = res;
}

#endif //ALLIN1NODELIBRARY_DIFFUSEMODELS_SG