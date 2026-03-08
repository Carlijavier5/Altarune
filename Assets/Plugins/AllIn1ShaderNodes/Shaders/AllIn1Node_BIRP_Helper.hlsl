#ifndef ALLIN1NODE_BIRP_HELPER
#define ALLIN1NODE_BIRP_HELPER

struct FogStruct
{
	float fogCoord : TEXCOORD0;
};

float3 GetMainLightDir()
{
	#if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD)
		float3 res = normalize(_WorldSpaceLightPos0.xyz);
	#else
		float3 res = float3(0, 0, 0);
	#endif
	
	return res;
}

AllIn1LightData GetMainLightData()
{
	AllIn1LightData lightData;
	
	lightData.lightColor = float3(1.0, 1.0, 1.0);
	lightData.lightDir = float3(0.0, 1.0, 0.0);
	lightData.distanceAttenuation = 1.0;
	lightData.shadowColor = 1.0;
	lightData.realtimeShadow = 1.0;

#ifdef SHADERGRAPH_PREVIEW
		lightData.lightColor = float3(1.0, 1.0, 1.0);
		lightData.lightDir = float3(0.0, 1.0, 0.0);
		lightData.distanceAttenuation = 1.0;
		lightData.shadowColor = 1.0;
		lightData.realtimeShadow = 1.0;
#else
	#ifdef _LIGHTMODEL_NONE
			lightData.lightColor = float3(1.0, 1.0, 1.0);
			lightData.lightDir = float3(0.0, 1.0, 0.0);
			lightData.distanceAttenuation = 1.0;
			lightData.shadowColor = 1.0;
			lightData.realtimeShadow = 1.0;
	#elif _LIGHTMODEL_FASTLIGHTING
			lightData.lightColor = global_lightColor;
			lightData.lightDir = global_lightDirection;
			lightData.distanceAttenuation = 1.0;
			lightData.shadowColor = 1.0;
			lightData.realtimeShadow = 1.0;
	#else
		lightData.lightColor = _LightColor0.rgb;
		lightData.lightDir = normalize(_WorldSpaceLightPos0.xyz);

	#if defined(_AFFECTED_BY_LIGHTMAPS_ON)
				lightData.distanceAttenuation = mainLight.distanceAttenuation;
	#else
		lightData.distanceAttenuation = 1.0;
	#endif
	
			//float4 shadowCoords = TransformWorldToShadowCoord(vertexWS);
	#ifdef _CAST_SHADOWS_ON
			lightData.realtimeShadow = MainLightRealtimeShadow(shadowCoords);
	#else
		lightData.realtimeShadow = 1.0;
	#endif

		lightData.shadowColor = lightData.realtimeShadow;
	#endif
#endif

	
	return lightData;
}

float3 GetMainLightColorRGB()
{
	float3 res = 1.0;

#ifndef SHADERGRAPH_PREVIEW
	res = GetMainLightData().lightColor;
#endif
	
	return res;
}

float GetMainLightIntensity()
{
	return length(GetMainLightColorRGB());
}

float3 GetCompletedMainLightColor()
{
	AllIn1LightData lightData = GetMainLightData();
	
	float3 res = lightData.lightColor.rgb * lightData.distanceAttenuation * lightData.shadowColor.rgb;
	return res;
}

float3 GetAmbientColor(float3 normalWS)
{
	#ifdef SHADERGRAPH_PREVIEW
		float3 res = 1.0;
	#else
		float3 res = ShadeSH9(float4(normalWS, 1.0));
	#endif

	return res;
}

float3 GetViewDirWS(float3 vertexWS)
{
#ifdef SHADERGRAPH_PREVIEW
	float3 res = float3(0, 0, -1);
#else
	float3 res = UnityWorldSpaceViewDir(vertexWS);
#endif
	
	return res;
}

float3 GetPositionVS(float3 positionOS)
{
#ifdef SHADERGRAPH_PREVIEW
	float3 res = float3(0, 0, 0);
#else
	float3 res = UnityObjectToViewPos(positionOS);
#endif

	return res;
}

float GetFogFactor(float4 clipPos)
{
	float res = 0;
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	FogStruct fogStruct;
	UNITY_TRANSFER_FOG(fogStruct, clipPos);
	res = fogStruct.fogCoord;
#endif
	
	return res;
}

float4 CustomMixFog(float fogFactor, float4 col)
{
	float4 res = col;
	
#if !defined(SHADERGRAPH_PREVIEW)
	UNITY_APPLY_FOG(fogFactor, res);
#endif
	
	return res;
}

#define OBJECT_TO_CLIP_SPACE(v)				mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1.0))
#define OBJECT_TO_CLIP_SPACE_FLOAT4(pos)	mul(UNITY_MATRIX_MVP, pos)

#endif //ALLIN1NODE_BIRP_HELPER