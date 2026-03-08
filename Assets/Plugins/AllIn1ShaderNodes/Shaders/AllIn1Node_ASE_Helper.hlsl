#ifndef ALLIN1NODE_ASE_HELPER
#define ALLIN1NODE_ASE_HELPER

#include "./AllIn1Node_IncludeHelper.hlsl"

#if defined(ALLIN1NODE_HDRP)
	#define HDRP_EXPOSURE_CORRECTOR			GetCurrentExposureMultiplier() * 5.0
	#define HDRP_LIGHT_INTENSITY_CORRECTOR	GetCurrentExposureMultiplier() * 0.25
	#define WORLD_TO_OBJECT_MATRIX UNITY_MATRIX_I_M
	#define CUSTOM_SAMPLE_TEXCUBE(texcube, reflected_vector_ws, lod) SAMPLE_TEXTURECUBE_LOD(texcube, sampler##texcube, reflected_vector_ws, lod)
	#define CUSTOM_DECODE_HDR(hdr_color, hdr_decode) DecodeHDREnvironment(hdr_color, hdr_decode);
	
	#if defined(SHADERGRAPH_PREVIEW) || !defined(SHADERPASS_FORWARD)
		#define SAMPLE_SKY(res, worldRefl, lod) res = float3(0.5, 0.5, 0.5);
	#else
		#define SAMPLE_SKY(res, worldRefl, lod) res = SampleSkyTexture(worldRefl, lod, 0) * HDRP_EXPOSURE_CORRECTOR; 
	#endif

	#include "./AllIn1Node_CommonFunctions.hlsl"
	#include "./AllIn1Node_HDRP_Helper.hlsl"

#elif defined(ALLIN1NODE_URP)
	#define WORLD_TO_OBJECT_MATRIX unity_WorldToObject
	#define CUSTOM_SAMPLE_TEXCUBE(texcube, reflected_vector_ws, lod) SAMPLE_TEXTURECUBE_LOD(texcube, sampler##texcube, reflected_vector_ws, lod)
	#define CUSTOM_DECODE_HDR(hdr_color, hdr_decode) DecodeHDREnvironment(hdr_color, hdr_decode);
	#define SAMPLE_SKY(res, worldRefl, lod) \
		float4 skyData = CUSTOM_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl, lod); \
		res = CUSTOM_DECODE_HDR(skyData, unity_SpecCube0_HDR);

inline bool IsGammaSpace()
{
#ifdef UNITY_COLORSPACE_GAMMA
				return true;
#else
	return false;
#endif
}
	
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
	#include "./AllIn1Node_CommonFunctions.hlsl"
	#include "./AllIn1Node_URP_Helper.hlsl"

	#define UnityObjectToWorldNormal(normal) GetNormalWS(normal)
#define UnityObjectToWorldDir(dir) GetDirWS(dir)
#elif defined(ALLIN1NODE_BIRP)
	#define WORLD_TO_OBJECT_MATRIX unity_WorldToObject
	#define CUSTOM_SAMPLE_TEXCUBE(texcube, reflected_vector_ws, lod) UNITY_SAMPLE_TEXCUBE_LOD(texcube, reflected_vector_ws, lod)
	#define CUSTOM_DECODE_HDR(hdr_color, hdr_decode) DecodeHDR(hdr_color, hdr_decode);
	#define SAMPLE_SKY(res, worldRefl, lod) \
		float4 skyData = CUSTOM_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl, lod); \
		res = CUSTOM_DECODE_HDR(skyData, unity_SpecCube0_HDR);

	#include "./AllIn1Node_CommonFunctions.hlsl"
	#include "Lighting.cginc"
	#include "./AllIn1Node_BIRP_Helper.hlsl"
#endif

#define TEX_PARAM sampler2D
#define ACCESS_PROP(param) param

#define SAMPLE_TEX2D(texName, uv)		tex2D(texName, uv)
#define SAMPLE_TEX2D_LOD(texName, uv)	tex2Dlod(texName, uv)
#define SAMPLE_TEX2D_DERIVATIVES(texName, uv, ddx, ddy) tex2D(texName, uv, ddx, ddy)

#endif //ALLIN1NODE_ASE_HELPER