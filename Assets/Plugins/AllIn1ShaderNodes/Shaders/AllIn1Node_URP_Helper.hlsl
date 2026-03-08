#ifndef ALLIN1NODE_URP_HELPER
#define ALLIN1NODE_URP_HELPER

#define OBJECT_TO_CLIP_SPACE(v) TransformObjectToHClip(v.vertex.xyz)
#define OBJECT_TO_CLIP_SPACE_FLOAT4(pos) TransformObjectToHClip(pos.xyz)

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
		Light mainLight = GetMainLight();
		lightData.lightColor = mainLight.color;
		lightData.lightDir = mainLight.direction;

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
		float3 res = SampleSH(normalWS);
	#endif

	return res;
}

float3 GetViewDirWS(float3 vertexWS)
{
	#ifdef SHADERGRAPH_PREVIEW
	float3 res = float3(0, 0, -1);
	#else
	float3 res = GetWorldSpaceViewDir(vertexWS);
	#endif

	return res;
}

float3 GetPositionVS(float3 positionOS)
{
	float3 res = TransformWorldToView(positionOS);
	return res;
}

float3 GetPositionWS(float4 positionOS)
{
	return TransformObjectToWorld(positionOS.xyz);
}

float3 GetNormalWS(float3 normalOS)
{
	float3 normalWS = TransformObjectToWorldNormal(normalOS);
	return normalWS;
}

float3 GetDirWS(float4 dirOS)
{
	return TransformObjectToWorldDir(dirOS.xyz);
}

float GetFogFactor(float4 clipPos)
{
	float res = 0;

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
	res = ComputeFogFactor(clipPos.z);
#endif

	return res;
}

float4 CustomMixFog(float fogFactor, float4 col)
{
	float4 res = col;

#if !defined(SHADERGRAPH_PREVIEW)
	res.rgb = MixFog(res.rgb, fogFactor);
#endif

	return res;
}

#endif //ALLIN1NODE_URP_HELPER