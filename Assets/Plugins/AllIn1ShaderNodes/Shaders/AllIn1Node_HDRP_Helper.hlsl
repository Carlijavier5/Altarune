#ifndef ALLIN1NODE_HDRP_HELPER
#define ALLIN1NODE_HDRP_HELPER

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
	
#if defined(SHADERGRAPH_PREVIEW) || !defined(SHADERPASS_FORWARD)
	lightData.lightColor = float3(1.0, 1.0, 1.0);
	lightData.lightDir = float3(0.0, 1.0, 0.0);
	lightData.distanceAttenuation = 1.0;
	lightData.shadowColor = 1.0;
	lightData.realtimeShadow = 1.0;
#else
	DirectionalLightData mainLight = _DirectionalLightDatas[0];
	lightData.lightColor = mainLight.color * HDRP_LIGHT_INTENSITY_CORRECTOR;
	lightData.lightDir = -mainLight.forward.xyz;
	lightData.distanceAttenuation = 1.0;
	lightData.realtimeShadow = 1.0;
lightData.shadowColor = lightData.realtimeShadow;
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
	#ifdef HAS_LIGHTLOOP
	float3 res = EvaluateAmbientProbe(normalWS) * HDRP_EXPOSURE_CORRECTOR;
	#else
	float3 res = float3(0.5, 0.5, 0.5);
	#endif

	return res;
}

float3 GetPositionVS(float3 positionOS)
{
	float3 res = TransformWorldToView(positionOS);
	return res;
}

#define OBJECT_TO_CLIP_SPACE(v) TransformObjectToHClip(v.vertex.xyz)
#define OBJECT_TO_CLIP_SPACE_FLOAT4(pos) TransformObjectToHClip(pos.xyz)

#endif //ALLIN1NODE_HDRP_HELPER