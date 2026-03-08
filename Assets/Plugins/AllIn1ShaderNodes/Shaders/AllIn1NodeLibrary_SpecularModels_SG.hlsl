#ifndef ALLINNODESLIBRARY_SPECULAR_MODELS_SG
#define ALLINNODESLIBRARY_SPECULAR_MODELS_SG

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_SpecularModels.hlsl"

void SpecularModel_Classic_float(float3 normalWS, float3 viewDirWS, float glossiness, float specularAtten,
	out float3 Out)
{
	float3 res = SpecularModel_Classic(normalWS, viewDirWS, glossiness, specularAtten);
	Out = res;
}

void SpecularModel_Toon_float(float3 normalWS, float3 viewDirWS, float glossiness, float specularAtten,
	float specularToonSmoothness, float specularToonCutoff,
	out float3 Out)
{
	float3 res = SpecularModel_Toon(normalWS, viewDirWS, glossiness, specularAtten, specularToonSmoothness, specularToonCutoff);
	Out = res;
}

void SpecularModel_Anisotropic_float(float3 normalWS, float3 tangentWS, float3 bitangentWS, float3 viewDirWS, float specularAtten,
	float anisotropy, float anisoShininess,
	out float3 Out)
{
	float3 res = SpecularModel_Anisotropic(normalWS, tangentWS, bitangentWS, viewDirWS, specularAtten, anisotropy, anisoShininess);
	Out = res;
}

#endif //ALLINNODESLIBRARY_SPECULAR_MODELS_SG