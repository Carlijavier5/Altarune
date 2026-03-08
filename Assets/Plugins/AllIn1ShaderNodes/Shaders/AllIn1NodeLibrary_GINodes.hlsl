#ifndef ALLINNODESLIBRARY_GI_NODES
#define ALLINNODESLIBRARY_GI_NODES

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1Node_GlobalIllumination.hlsl"

void Reflections_float(float3 normalWS, float3 viewDirWS, float reflectionsAtten, float cubeLod, out float3 Out)
{
	float3 res = GetSkyColor(normalWS, viewDirWS, reflectionsAtten, cubeLod);
	Out = res;
}

void ReflectionsToon_float(float3 normalWS, float3 viewDirWS, float reflectionsAtten, float cubeLod, float toonFactor, out float3 Out)
{
	float3 res = GetSkyColorToon(normalWS, viewDirWS, reflectionsAtten, toonFactor, cubeLod);
	Out = res;
}

void AmbientColor_float(float3 normalWS, out float3 Out)
{
	float3 res = GetAmbientColor(normalWS);
	Out = res;
}

#endif //ALLINNODESLIBRARY_GI_NODES