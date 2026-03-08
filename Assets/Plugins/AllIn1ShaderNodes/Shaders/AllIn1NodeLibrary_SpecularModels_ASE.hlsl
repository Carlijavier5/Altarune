#ifndef ALLINNODESLIBRARY_SPECULAR_MODELS_ASE
#define ALLINNODESLIBRARY_SPECULAR_MODELS_ASE

#include "./AllIn1NodeLibrary_SpecularModels.hlsl"

float3 SpecularModel_Classic_ASE(float3 normalWS, float3 viewDirWS, float glossiness, float specularAtten)
{
	float3 res = SpecularModel_Classic(normalize(normalWS), normalize(viewDirWS), glossiness, specularAtten);
	return res;
}

float3 SpecularModel_Toon_ASE(float3 normalWS, float3 viewDirWS, float glossiness, float specularAtten,
	float specularToonSmoothness, float specularToonCutoff)
{
	float3 res = SpecularModel_Toon(normalize(normalWS), normalize(viewDirWS), glossiness, specularAtten, specularToonSmoothness, specularToonCutoff);
	return res;
}

float3 SpecularModel_Anisotropic_ASE(float3 normalWS, float3 tangentWS, float3 bitangentWS, float3 viewDirWS, float specularAtten,
	float anisotropy, float anisoShininess)
{
	float3 res = SpecularModel_Anisotropic(normalize(normalWS), normalize(tangentWS), normalize(bitangentWS), normalize(viewDirWS), specularAtten, anisotropy, anisoShininess);
	return res;
}

#endif //ALLINNODESLIBRARY_SPECULAR_MODELS_ASE