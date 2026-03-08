#ifndef ALLIN13DNODES_GLOBAL_ILLUMINATION
#define ALLIN13DNODES_GLOBAL_ILLUMINATION

float3 GetSkyColorRaw(float3 normalWS, float3 viewDirWS, float cubeLod)
{
	float3 worldRefl = normalize(reflect(-viewDirWS, normalWS));
	
	float3 res = 0;
	SAMPLE_SKY(res, worldRefl, cubeLod)

	return res;
}

float3 GetSkyColor(float3 normalWS, float3 viewDirWS, float reflectionsAtten, float cubeLod)
{
	float3 res = GetSkyColorRaw(normalWS, viewDirWS, cubeLod);

	res *= reflectionsAtten;

	return res;
}

float3 GetSkyColorToon(float3 normalWS, float3 viewDirWS, float reflectionsAtten, float cubeLod, float toonFactor)
{
	float3 res = GetSkyColorRaw(normalWS, viewDirWS, cubeLod);

	float posterizationLevel = lerp(2, 20, toonFactor);
	res = floor(res * posterizationLevel) / posterizationLevel;
	
	res *= reflectionsAtten;

	return res;
}

#endif //ALLIN13DNODES_GLOBAL_ILLUMINATION