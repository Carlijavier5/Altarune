#ifndef ALLIN1NODELIBRARY_ALPHAEFFECTS_INCLUDED
#define ALLIN1NODELIBRARY_ALPHAEFFECTS_INCLUDED

float Fade(float inputAlpha, float _FadePower, float _FadeAmount, float _FadeTransition, TEX_PARAM _FadeTex, float2 uv, float2 tiling)
{
	float res = inputAlpha;
	
	float2 fadeUV = uv * tiling;
	float fadeSample = SAMPLE_TEX2D(_FadeTex, fadeUV).r;
	
	fadeSample = pow(saturate(fadeSample), _FadePower);
	
	
	float fadeAmount = lerp(_FadeAmount - _FadeTransition, 1.0, _FadeAmount);
	float fade = smoothstep(fadeAmount, fadeAmount + _FadeTransition, fadeSample);
	
	
	res *= fade;
	
	return res;
}

float4 Fade_Burn(float3 inputRGB, float inputAlpha, float _FadePower, float _FadeAmount, float _FadeTransition, float _FadeBurnWidth, float4 _FadeBurnColor, TEX_PARAM _FadeTex, float2 uv, float2 tiling)
{
	float4 res = float4(inputRGB.r, inputRGB.g, inputRGB.b, inputAlpha);
	
	float2 fadeUV = uv * tiling;
	float fadeSample = SAMPLE_TEX2D(_FadeTex, fadeUV).r;
	
	fadeSample = pow(saturate(fadeSample), _FadePower);
	
	
	float fadeAmount = lerp(_FadeAmount - _FadeTransition - _FadeBurnWidth, 1.0, _FadeAmount);
	float fade = smoothstep(fadeAmount, fadeAmount + _FadeTransition, fadeSample);
	
	float fadePlusBurn = smoothstep(fadeAmount + _FadeBurnWidth, fadeAmount + _FadeBurnWidth + _FadeTransition, fadeSample);
	
	float diff = saturate(fade - fadePlusBurn);
	
	float3 burnColor = diff * _FadeBurnColor.rgb;
	
	res.rgb += burnColor;
	
	
	res.a *= fade;
	
	return res;
}

float FadeByCamDistance(float inputAlpha, float _MinDistanceToFade, float _MaxDistanceToFade, float3 cameraPositionWS, float3 vertexPositionWS)
{
	float res = inputAlpha;
	
	float camDistance = distance(cameraPositionWS, vertexPositionWS);

	float t = 0;
	#ifdef _FADE_BY_CAM_DISTANCE_NEAR_FADE
	t = 1 - smoothstep(_MinDistanceToFade, _MaxDistanceToFade, camDistance);
	#else
	t = smoothstep(_MinDistanceToFade, _MaxDistanceToFade, camDistance);
	#endif
	
	
	res = lerp(res, 0, t);
	
	return res;
}

float FadeByCamDistance_NearFade(float inputAlpha, float _MinDistanceToFade, float _MaxDistanceToFade, float3 cameraPositionWS, float3 vertexPositionWS)
{
	float res = inputAlpha;
	
	float camDistance = distance(cameraPositionWS, vertexPositionWS);

	float t = 0;
	t = 1 - smoothstep(_MinDistanceToFade, _MaxDistanceToFade, camDistance);
	
	res = lerp(res, 0, t);
	
	return res;
}

float FadeByCamDistance_FarFade(float inputAlpha, float _MinDistanceToFade, float _MaxDistanceToFade, float3 cameraPositionWS, float3 vertexPositionWS)
{
	float res = inputAlpha;

	float camDistance = distance(cameraPositionWS, vertexPositionWS);

	float t = 0;
	t = smoothstep(_MinDistanceToFade, _MaxDistanceToFade, camDistance);

	res = lerp(res, 0, t);

	return res;
}

// Alpha Mask - Samples a mask texture and multiplies alpha
float AlphaMask(float inputAlpha, TEX_PARAM _MaskTex, float2 uv, float2 tiling, float _MaskPower)
{
	float res = inputAlpha;

	float2 maskUV = uv * tiling;
	float4 maskSample = SAMPLE_TEX2D(_MaskTex, maskUV);
	float mask = pow(min(maskSample.r, maskSample.a), _MaskPower);

	res *= mask;

	return res;
}

// Soft Particles / Intersection Fade - Fades based on scene depth difference
float SoftParticles(float inputAlpha, float _SoftFactor, float sceneDepthDiff)
{
	float res = inputAlpha;

	float sceneZMult = saturate(_SoftFactor * sceneDepthDiff);
	res *= sceneZMult;

	return res;
}

// Intersection Fade - Fades based on scene depth difference (same as soft particles with different naming)
float IntersectionFade(float inputAlpha, float _IntersectionFadeFactor, float sceneDepthDiff)
{
	float res = inputAlpha;

	res *= saturate(_IntersectionFadeFactor * sceneDepthDiff);

	return res;
}

// Alpha Remap - Remaps alpha using smoothstep
float AlphaRemap(float inputAlpha, float _AlphaStepMin, float _AlphaStepMax)
{
	float res = inputAlpha;

	res = smoothstep(_AlphaStepMin, _AlphaStepMax, res);

	return res;
}

// Alpha Round - Rounds alpha to 0 or 1
float AlphaRound(float inputAlpha)
{
	float res = inputAlpha;

	res = round(res);

	return res;
}


#endif //ALLIN1NODELIBRARY_ALPHAEFFECTS_INCLUDED