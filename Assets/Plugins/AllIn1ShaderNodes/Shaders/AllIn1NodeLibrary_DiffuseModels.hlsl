#ifndef ALLIN1NODELIBRARY_DIFFUSEMODELS
#define ALLIN1NODELIBRARY_DIFFUSEMODELS


float GetRawNdotL(float3 normalWS)
{
	AllIn1LightData lightData = GetMainLightData();

	float res = dot(normalWS, lightData.lightDir);
	
	return res;
}

float3 DiffuseLight_Toon(float3 normalWS, float _ToonCutoff, float _ToonSmoothness)
{
	float rawNdotL = GetRawNdotL(normalWS);
	
	float3 lightModel = 0;
	float3 lightColor = GetCompletedMainLightColor();
	
	float NdotL = saturate(rawNdotL);
	
	float edge0 = max(0.001, _ToonCutoff);
	float edge1 = _ToonCutoff + _ToonSmoothness + EPSILON;
	lightModel = smoothstep(edge0, edge1, NdotL);
	
	float3 res = lightModel * lightColor;
	
	return res;
}

float3 DiffuseLight_Classic(float3 normalWS)
{
	float rawNdotL = GetRawNdotL(normalWS);
	
	float3 lightModel = 0;
	float3 lightColor = GetCompletedMainLightColor();
	
	float NdotL = saturate(rawNdotL);
	lightModel = NdotL;
	
	float3 res = lightModel * lightColor;
	
	return res;
}

float3 DiffuseLight_ToonRamp(float3 normalWS, TEX_PARAM _ToonRamp)
{
	float rawNdotL = GetRawNdotL(normalWS);
	float NdotL = (rawNdotL * 0.5) + 0.5;
	float4 uv = float4(NdotL, 0, 0, 0);

	float3 toonRampColor = SAMPLE_TEX2D_LOD(_ToonRamp, float4(NdotL, 0, 0, 0)).rgb;

	float3 lightColor = GetCompletedMainLightColor();
	
	float3 res = toonRampColor * lightColor;
	
	return res;
}

float3 DiffuseLight_HalfLambert(float3 normalWS, float _HalfLambertWrap)
{
	float rawNdotL = GetRawNdotL(normalWS);
	
	float3 lightModel = 0;
	float3 lightColor = GetCompletedMainLightColor();
	
	float NdotL = saturate(rawNdotL);
	float halfLambertTerm = (NdotL * _HalfLambertWrap) + (1 - _HalfLambertWrap);
	lightModel = halfLambertTerm * halfLambertTerm;
	
	float3 res = lightModel * lightColor;
	
	return res;
}

float3 DiffuseLight_FakeGI(float3 normalWS, float _HardnessFakeGI)
{
	float rawNdotL = GetRawNdotL(normalWS);

	float3 lightColor = GetCompletedMainLightColor();
	float3 lightModel = (saturate(rawNdotL) * _HardnessFakeGI) + 1.0 - _HardnessFakeGI;

	float3 res = lightModel * lightColor;
	return res;
}

#endif //ALLIN1NODELIBRARY_DIFFUSEMODELS