#ifndef ALLIN1NODELIBRARY_LIGHTINGEFFECTS_INCLUDED
#define ALLIN1NODELIBRARY_LIGHTINGEFFECTS_INCLUDED

float3 AOMap(float _AOContrast, float _AOMapStrength, float3 _AOColor, float2 uv, TEX_PARAM _AOMap)
{
	float aoTex = SAMPLE_TEX2D(_AOMap, uv).r;
	
	float3 ao = max(0, (aoTex - float3(0.5, 0.5, 0.5)) * _AOContrast + float3(0.5, 0.5, 0.5));

	ao = saturate(ao + 1 - _AOMapStrength);
	float3 aoMapTerm = lerp(_AOColor.rgb, 1, ao);
	
	float3 res = aoMapTerm;
	
	return res;
}

float3 GetNormalWSFromNormalMap(TEX_PARAM _NormalMap, float normalStrength, float2 uv, float3 normalWS, float3 tangentWS, float3 bitangentWS)
{
	float3 tspace0 = float3(tangentWS.x, bitangentWS.x, normalWS.x);
	float3 tspace1 = float3(tangentWS.y, bitangentWS.y, normalWS.y);
	float3 tspace2 = float3(tangentWS.z, bitangentWS.z, normalWS.z);
		
	float4 sampledNormal = SAMPLE_TEX2D(_NormalMap, uv);
	float3 tnormal = UnpackNormal(sampledNormal);
	tnormal.xy *= normalStrength;

	float3 res = 0;
	res.x = dot(tspace0, tnormal);
	res.y = dot(tspace1, tnormal);
	res.z = dot(tspace2, tnormal);
    
	res = normalize(res);
	
	return res;
}

#endif //ALLIN1NODE_LIGHTINGEFFECTS_INCLUDED