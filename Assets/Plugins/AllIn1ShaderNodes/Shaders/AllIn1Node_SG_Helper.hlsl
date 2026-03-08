#ifndef ALLIN1NODE_SG_HELPER
#define ALLIN1NODE_SG_HELPER

#include "./AllIn1Node_IncludeHelper.hlsl"



#define CUSTOM_SAMPLE_TEXCUBE(texcube, reflected_vector_ws, lod) SAMPLE_TEXTURECUBE_LOD(texcube, sampler##texcube, reflected_vector_ws, lod)
#define CUSTOM_DECODE_HDR(hdr_color, hdr_decode) DecodeHDREnvironment(hdr_color, hdr_decode);


#if defined(ALLIN1NODE_HDRP)
	#define WORLD_TO_OBJECT_MATRIX UNITY_MATRIX_I_M
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
	#define SAMPLE_SKY(res, worldRefl, lod) \
		float4 skyData = CUSTOM_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl, lod); \
		res = CUSTOM_DECODE_HDR(skyData, unity_SpecCube0_HDR);

	#include "./AllIn1Node_CommonFunctions.hlsl"
	#include "./AllIn1Node_URP_Helper.hlsl"

#elif defined(ALLIN1NODE_BIRP)
	#define WORLD_TO_OBJECT_MATRIX unity_WorldToObject
	#define SAMPLE_SKY(res, worldRefl, lod) \
		float4 skyData = CUSTOM_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl, lod); \
		res = CUSTOM_DECODE_HDR(skyData, unity_SpecCube0_HDR);

	#include "./AllIn1Node_CommonFunctions.hlsl"
	#include "./AllIn1Node_BIRP_Helper.hlsl"

#endif

#define TEX_PARAM UnityTexture2D
#define ACCESS_PROP(param) param

#define CUSTOM_TRANSFORM_TEX(uv, increment, name) ((uv.xy + increment.xy) * name.scaleTranslate.xy + name.scaleTranslate.zw)
#define SIMPLE_CUSTOM_TRANSFORM_TEX(uv, name) uv.xy * name.scaleTranslate.xy + name.scaleTranslate.zw

#define SAMPLE_TEX2D(texName, uv)		tex2D(texName, uv)
#define SAMPLE_TEX2D_LOD(texName, uv)	SAMPLE_TEXTURE2D_LOD(texName, texName.samplerstate, uv.xy, 0)
#define SAMPLE_TEX2D_DERIVATIVES(texName, uv, ddx, ddy) texName.SampleGrad(texName.samplerstate, uv, ddx, ddy)	

#endif //ALLIN1NODE_SG_HELPER