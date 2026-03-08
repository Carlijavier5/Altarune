#ifndef ALLINNODESLIBRARY_SPECULAR_MODELS
#define ALLINNODESLIBRARY_SPECULAR_MODELS

#include "./AllIn1Node_CommonFunctions.hlsl"
#include "./AllIn1NodeLibrary_BRDF.hlsl"

#define FINAL_SPECULAR_COMPOSITION(res) \
	float3 lightColor = lightData.lightColor.rgb * lightData.distanceAttenuation * lightData.shadowColor.rgb; \
	float3 specularModel = specularIntensity * lightColor; \
	float3 res = specularModel * specularAtten; \
	float NormalDotLight = saturate(dot(normalWS, lightData.lightDir)); \
	res *= NormalDotLight;

float GetRawSpecularIntensity(float3 normalWS, float3 viewDirWS, float3 lightDir, float glossiness)
{
	float3 halfVector = normalize(viewDirWS + lightDir);
	float NdotH = saturate(dot(normalWS, halfVector));

	float glosiness2 = glossiness * glossiness;
	float specularIntensity = pow(NdotH, glosiness2);
	
	return specularIntensity;
}

float3 SpecularModel_Classic(float3 normalWS, float3 viewDirWS, float glossiness, float specularAtten)
{
	AllIn1LightData lightData = GetMainLightData();

	float specularIntensity = GetRawSpecularIntensity(normalWS, viewDirWS, lightData.lightDir, glossiness);
	
	FINAL_SPECULAR_COMPOSITION(res)
	
	return res;
}

float3 SpecularModel_Toon(float3 normalWS, float3 viewDirWS, float glossiness, float specularAtten,
	float specularToonSmoothness, float specularToonCutoff)
{
	AllIn1LightData lightData = GetMainLightData();

	float specularIntensity = GetRawSpecularIntensity(normalWS, viewDirWS, lightData.lightDir, glossiness);
	
	float specularSmoothness = max(specularToonSmoothness, 0.001);
	specularIntensity = smoothstep(specularToonCutoff, specularToonCutoff + specularSmoothness, specularIntensity);
	
	FINAL_SPECULAR_COMPOSITION(res)
	
	return res;
}

float3 SpecularModel_Anisotropic(float3 normalWS, float3 tangentWS, float3 bitangentWS, float3 viewDirWS, float specularAtten,
	float anisotropy, float anisoShininess)
{
	AllIn1LightData lightData = GetMainLightData();

	float3 H = normalize(viewDirWS + lightData.lightDir);

	float TdotH = saturate(dot(tangentWS, H));
	float BdotH = saturate(dot(bitangentWS, H));

	float TdotV = saturate(dot(tangentWS, viewDirWS));
	float BdotV = saturate(dot(bitangentWS, viewDirWS));

	float TdotL = saturate(dot(tangentWS, lightData.lightDir));
	float BdotL = saturate(dot(bitangentWS, lightData.lightDir));

	float NdotH = saturate(dot(normalWS, H));
	float NdotL = saturate(dot(normalWS, lightData.lightDir));
	float NdotV = saturate(dot(normalWS, viewDirWS));

	float VdotH = saturate(dot(viewDirWS, H));

	float at = max((1 - anisoShininess) * (1.0 + anisotropy), 0.001);
	float ab = max((1 - anisoShininess) * (1.0 - anisotropy), 0.001);

	float3 specularIntensity = SpecularAnisoTerm(
		at, ab,
		float3(1.0, 1.0, 1.0),
		NdotH, NdotV, NdotL,
		TdotV, BdotV,
		TdotL, BdotL,
		TdotH, BdotH,
		H, tangentWS.xyz, bitangentWS.xyz) * NdotL;

	specularIntensity = saturate(specularIntensity);
	
	FINAL_SPECULAR_COMPOSITION(res)
	
	return res;
}

//float3 SpecularTerm(float3 objectColor, AllIn1LightData lightData,
//		float3 normalWS, float3 tangentWS, float3 bitangentWS,
//		float3 viewDirWS, float glossiness, float2 mainUV, float4 specularTex)
//{
//	float3 lightColor = lightData.lightColor.rgb * lightData.distanceAttenuation * lightData.shadowColor.rgb;
//	float3 reflectionDir = reflect(-lightData.lightDir, normalWS);
//	float rawVdotL = dot(viewDirWS, reflectionDir);
	
//	float3 specularModel = 0;
	

//#if defined(_SPECULARMODEL_CLASSIC) || defined(_SPECULARMODEL_TOON)
//	float3 halfVector = normalize(viewDirWS + lightData.lightDir);
//	float NdotH = saturate(dot(normalWS, halfVector));
//	float specularIntensity = pow(NdotH, glossiness);
//	//#ifdef _SPECULARMODEL_TOON
//	//	float specularSmoothness = max(ACCESS_PROP(_SpecularToonSmoothness), 0.001);
//	//	specularIntensity = smoothstep(ACCESS_PROP(_SpecularToonCutoff), ACCESS_PROP(_SpecularToonCutoff) + specularSmoothness, specularIntensity);
//	//#endif
//#elif defined(_SPECULARMODEL_ANISOTROPIC) || defined(_SPECULARMODEL_ANISOTROPICTOON)
//	float3 H = normalize(viewDirWS + lightData.lightDir);

//	float TdotH = saturate(dot(tangentWS, H));
//	float BdotH = saturate(dot(bitangentWS, H));

//	float TdotV = saturate(dot(tangentWS, viewDirWS));
//	float BdotV = saturate(dot(bitangentWS, viewDirWS));

//	float TdotL = saturate(dot(tangentWS, lightData.lightDir));
//	float BdotL = saturate(dot(bitangentWS, lightData.lightDir));

//	float NdotH = saturate(dot(normalWS, H));
//	float NdotL = saturate(dot(normalWS, lightData.lightDir));
//	float NdotV = saturate(dot(normalWS, viewDirWS));

//	float VdotH = saturate(dot(viewDirWS, H));

//	float anisotropy = ACCESS_PROP(_Anisotropy);
//	float anisoShininess = ACCESS_PROP(_AnisoShininess);
//	float at = max((1 - anisoShininess) * (1.0 + anisotropy), 0.001);
//	float ab = max((1 - anisoShininess) * (1.0 - anisotropy), 0.001);

//	float3 specularIntensity = SpecularAnisoTerm(
//		at, ab, 
//		float3(1.0, 1.0, 1.0), 
//		NdotH, NdotV, NdotL,
//		TdotV, BdotV, 
//		TdotL, BdotL, 
//		TdotH, BdotH,
//		H, tangentWS.xyz, bitangentWS.xyz) * NdotL;

//	specularIntensity = saturate(specularIntensity);
//#endif

//#if defined(_SPECULARMODEL_ANISOTROPICTOON) || defined(_SPECULARMODEL_TOON)
//	float specularSmoothness = max(ACCESS_PROP(_SpecularToonSmoothness), 0.001);
//	specularIntensity = smoothstep(ACCESS_PROP(_SpecularToonCutoff), ACCESS_PROP(_SpecularToonCutoff) + specularSmoothness, specularIntensity);
//#endif

//	specularModel = specularIntensity * lightColor;
//	float3 res = specularModel * ACCESS_PROP(_SpecularAtten) * specularTex.r;
	
//	return res;
//}

#endif //ALLINNODESLIBRARY_SPECULAR_MODELS