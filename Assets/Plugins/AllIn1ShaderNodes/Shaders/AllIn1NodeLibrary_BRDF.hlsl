#ifndef ALLIN1NODELIBRARY_BRDF
#define ALLIN1NODELIBRARY_BRDF

/************/
#define MEDIUMP_FLT_MAX    65504.0
#define saturateMediump(x) min(x, MEDIUMP_FLT_MAX)
#define MIN_ROUGHNESS 0.01

struct BDRFPerLightData
{
	float3 H;
	float3 L;

	float3 lightColor;
	float distanceAttenuation;
	float3 shadowColor;
	float3 correctedLightColor;

	float rawNdotL;
	float NdotL;
	float TdotL;
	float BdotL;

	float NdotH;
	float TdotH;
	float BdotH;

	float VdotH;
	float LdotH;
	float LdotH_2;

	float LdotV;

	float3 F;
	float3 kS;
	float3 kD;
};

struct BDRFCommonData
{
	float3 N;
	float3 T;
	float3 B;
	float3 V;

	float NdotV;
	float TdotV;
	float BdotV;

	float metallic;
	float smoothness;
	float roughness;
	float roughness_2;
	float cubeLod;

	float3 F0;

	float2 mainUV;
};

float D_GGX_Anisotropic(float NoH, const float3 h,
        const float3 t, const float3 b, float at, float ab) {
   
	//TODO: Pass TdotH and BdotH through parameters
	float ToH = dot(t, h);
    float BoH = dot(b, h);
    float a2 = at * ab;
    float3 v = float3(ab * ToH, at * BoH, a2 * NoH);
    float v2 = dot(v, v);
    float w2 = a2 / v2;
    return a2 * w2 * w2 * (1.0 / ALLIN13DSHADER_PI);
}

float V_SmithGGXCorrelated_Anisotropic(float at, float ab, float ToV, float BoV,
        float ToL, float BoL, float NoV, float NoL) {
    float lambdaV = NoL * length(float3(at * ToV, ab * BoV, NoV));
    float lambdaL = NoV * length(float3(at * ToL, ab * BoL, NoL));
    float v = 0.5 / (lambdaV + lambdaL);
    return saturateMediump(v);
}
/************/

float DistributionGGX(float a, float NdotH)
{
	float a2     = max(a*a, MIN_ROUGHNESS * MIN_ROUGHNESS);
	float NdotH2 = NdotH*NdotH;
	
	float num   = a2;
	float denom = (NdotH2 * (a2 - 1.0) + 1.0);
	denom = ALLIN13DSHADER_PI * denom * denom;
	
	return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
	float r = (roughness + 1.0);
	float k = (r*r) / 8.0;

	float num   = NdotV;
	float denom = NdotV * (1.0 - k) + k;
	
	return num / denom;
}

float GeometrySmith(float NdotV, float NdotL, float roughness)
{
	float ggx2  = GeometrySchlickGGX(NdotV, roughness);
	float ggx1  = GeometrySchlickGGX(NdotL, roughness);

	return ggx1 * ggx2;
}

float3 fresnelSchlick(float3 F0, float VdotH)
{
	float OneMinusVdotH = 1 - VdotH;
	float OneMinusVdotH_5 = OneMinusVdotH * OneMinusVdotH * OneMinusVdotH * OneMinusVdotH * OneMinusVdotH;
	return F0 + (1.0 - F0) * OneMinusVdotH_5;
}

float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
	float oneMinusRoughness = 1.0 - roughness;
	float oneMinusCosTheta = 1.0 - cosTheta;

	return F0 + (max(oneMinusRoughness, F0) - F0) * pow(clamp(oneMinusCosTheta, 0.0, 1.0), 5.0);
}

inline float3 FresnelLerp (float3 F0, float3 F90, float cosA)
{
    float t = Pow_5 (1 - cosA);   // ala Schlick interpoliation
    return lerp (F0, F90, t);
}

//Cook Torrance
float3 SpecularTerm(float a, float roughness, float3 F, float NdotH, float NdotV, float NdotL, float VdotH)
{
	float D		= DistributionGGX(a, NdotH);
	float G		= GeometrySmith(NdotV, NdotL, roughness);
	float3 numerator = 4.0 * D * G * F;

	float denominator = 4 * NdotV * NdotL;
	denominator = max(denominator, 0.0001);

	float3 res = numerator / denominator;
	return res;
}


float3 SpecularAnisoTerm(
	float at, float ab, 
	float3 F, 
	float NdotH, float NdotV, float NdotL, 
	float TdotV, float BdotV, 
	float TdotL, float BdotL, 
	float TdotH, float BdotH, 
	float3 H, float3 T, float3 B)
{
	float D		= D_GGX_Anisotropic(NdotH, H, T, B, at, ab);
	float V		= V_SmithGGXCorrelated_Anisotropic(
					at, ab, 
					TdotV, BdotV, 
					TdotL, BdotL,
					NdotV, NdotL); 

	float3 res = D * V * F;

	return res;
}

float3 SpecularIBL(float3 normalWS, float3 viewDirWS, float cubeLod)
{
	float3 res = 0;
	
#ifdef REFLECTIONS_ON
	res = GetSkyColor(normalWS, viewDirWS, cubeLod);
#endif
	return res;
}

float3 DiffuseTerm(float LdotH, float LdotV, float NdotL, float roughness, float3 colorDiffuse)
{
	float LdotH_2 = LdotH * LdotH;

	float f0 = 1.0;
	float f90 = 0.5 + 2*(roughness * LdotH_2);

	float3 fDiffuse = lerp(f0, f90, NdotL) * lerp(f0, f90, LdotV);

	float3 res = (colorDiffuse / ALLIN13DSHADER_PI) * fDiffuse;
	return res;
}

float3 DiffuseTerm02(float NdotV, float NdotL, float LdotH, float perceptualRoughness, float3 albedo)
{
    float fd90 = 0.5 + 2 * LdotH * LdotH * perceptualRoughness;
    // Two schlick fresnel term
    float lightScatter   = (1 + (fd90 - 1) * Pow_5(1 - NdotL));
    float viewScatter    = (1 + (fd90 - 1) * Pow_5(1 - NdotV));

	float3 res = (albedo /*/ ALLIN13DSHADER_PI*/) * lightScatter * viewScatter;
	return res;
}

#endif //ALLIN1NODELIBRARY_BRDF