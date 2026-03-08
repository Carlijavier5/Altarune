#ifndef ALLIN13DNODESLIBRARY_INCLUDED
#define ALLIN13DNODESLIBRARY_INCLUDED

float3 HueShift(float3 inputColor, float hueShift, float hueSaturation, float hueBrightness)
{
	float3 res = inputColor;
	
	float cosHsv = hueBrightness * hueSaturation * cos(hueShift * 3.14159265 / 180);
	float sinHsv = hueBrightness * hueSaturation * sin(hueShift * 3.14159265 / 180);
	res.r = (.299 * hueBrightness + .701 * cosHsv + .168 * sinHsv) * inputColor.x
	+ (.587 * hueBrightness - .587 * cosHsv + .330 * sinHsv) * inputColor.y
	+ (.114 * hueBrightness - .114 * cosHsv - .497 * sinHsv) * inputColor.z;
	res.g = (.299 * hueBrightness - .299 * cosHsv - .328 * sinHsv) *inputColor.x
	+ (.587 * hueBrightness + .413 * cosHsv + .035 * sinHsv) * inputColor.y
	+ (.114 * hueBrightness - .114 * cosHsv + .292 * sinHsv) * inputColor.z;
	res.b = (.299 * hueBrightness - .3 * cosHsv + 1.25 * sinHsv) * inputColor.x
	+ (.587 * hueBrightness - .588 * cosHsv - 1.05 * sinHsv) * inputColor.y
	+ (.114 * hueBrightness + .886 * cosHsv - .203 * sinHsv) * inputColor.z;

return res;
}

float3 ContrastBrightness(float3 inputColor, float contrast, float brightness)
{
	float3 res = max(0, (inputColor - float3(0.5, 0.5, 0.5)) * contrast + float3(0.5, 0.5, 0.5) + brightness);

return res;
}

float4 AlbedoVertexColor(float4 vertexColor, float _VertexColorBlending, float4 inputColor)
{
	float4 res = inputColor;
	
	#ifdef _ALBEDOVERTEXCOLORMODE_MULTIPLY
	float3 multipliedColor = res.rgb * vertexColor.rgb;
	res.rgb = lerp(res.rgb, multipliedColor, _VertexColorBlending);
	#elif _ALBEDOVERTEXCOLORMODE_REPLACE
	res.rgb = lerp(inputColor.rgb, vertexColor.rgb, _VertexColorBlending);
	#endif

return res;
}

float4 Hologram(float3 vertexWS, float3 shaderTime, float4 _HologramLineDirection, float _HologramFrequency, float _HologramScrollSpeed, float _HologramLineCenter, float _HologramLineSpacing, float _HologramLineSmoothness, float _HologramBaseAlpha, float _HologramAccentFrequency, float _HologramAccentSpeed, float _HologramAlpha, float _HologramAccentAlpha, float4 _HologramColor, float4 inputColor)
{
	float4 res = inputColor;
	
	float3 dir = normalize(_HologramLineDirection.xyz);
	
	// Calculate primary hologram pattern using direction projection
	float3 scrollPos1 = vertexWS * _HologramFrequency + (shaderTime.y * _HologramScrollSpeed);
	float3 scrollUV1 = frac(scrollPos1);
	
	float projectedValue1 = dot(scrollUV1, normalize(dir));
	float distance1 = abs(projectedValue1 - _HologramLineCenter) * _HologramLineSpacing;
	float gradientMask1 = 1 - distance1;
	gradientMask1 = pow(saturate(gradientMask1), _HologramLineSmoothness);
	gradientMask1 = max(gradientMask1, _HologramBaseAlpha);
	
	// Calculate accent line pattern using direction projection
	float3 scrollPos2 = vertexWS * _HologramAccentFrequency + (shaderTime.y * _HologramAccentSpeed);
	float3 scrollUV2 = frac(scrollPos2);
	
	float projectedValue2 = dot(scrollUV2, normalize(dir));
	float distance2 = abs(projectedValue2 - _HologramLineCenter) * _HologramLineSpacing;
	float gradientMask2 = 1 - distance2;
	gradientMask2 = pow(saturate(gradientMask2), _HologramLineSmoothness);
	
	// Combine both patterns
	float combinedMask = saturate(gradientMask1 * _HologramAlpha + gradientMask2 * _HologramAccentAlpha);
	
	float4 finalColor = _HologramColor;
	finalColor.a = combinedMask * _HologramColor.a;
	
	res *= finalColor;

return res;
}

float3 Matcap(float3 normalWS, float3 normalOS, float3 vertexOS, float _MatcapIntensity, sampler2D _MatcapTex)
{
	#ifdef _NORMAL_MAP_ON
	float3 normalMapOS = normalize(mul(UNITY_MATRIX_IT_MV, float4(normalWS, 0.0)).xyz);
	float3 normalVS = normalize(mul(UNITY_MATRIX_MV, float4(normalMapOS, 0.0)).xyz);
	#else
	float3 normalVS = normalize(mul(UNITY_MATRIX_MV, float4(normalOS, 0.0)).xyz);
	#endif
	
	float3 positionVSDir = normalize(GetPositionVS(vertexOS));
	
	float3 normalCrossPosition = cross(positionVSDir, normalVS);
	
	float u = (-normalCrossPosition.y * 0.5) + 0.5;
	float v = (normalCrossPosition.x * 0.5) + 0.5;
	
	float2 matcapUV = float2(u, v);
	float3 res = SAMPLE_TEX2D(_MatcapTex, matcapUV).rgb * _MatcapIntensity;

return res;
}

float4 ColorRamp(float3 shaderTime, float _ColorRampLuminosity, float _ColorRampTiling, float _ColorRampScrollSpeed, float _ColorRampBlend, sampler2D _ColorRampTex, float4 inputColor)
{
	float luminance = GetLuminance(inputColor);
	luminance = saturate(luminance + _ColorRampLuminosity);
	
	float2 rampUV = float2(luminance, 0);
	rampUV.x *= _ColorRampTiling;
	rampUV.x += frac(shaderTime.x * _ColorRampScrollSpeed);
	
	float3 colorRamp = SAMPLE_TEX2D(_ColorRampTex, rampUV).rgb;
	float3 resRGB = lerp(inputColor.rgb, colorRamp, _ColorRampBlend);
	
	float4 res = float4(resRGB, inputColor.a);

return res;
}

float4 Hit(float4 _HitColor, float _HitGlow, float _HitBlend, float4 inputColor)
{
	float3 resRGB = lerp(inputColor.rgb, _HitColor.rgb * _HitGlow, _HitBlend);
	float4 res = float4(resRGB, inputColor.a);

return res;
}

float3 Rim(float3 viewDirWS, float3 normalWS, float4 _RimOffset, float _MinRim, float _MaxRim, float _RimAttenuation, float4 _RimColor, float3 inputColorRGB)
{
	float3 rimOffset = _RimOffset;
	float rimIntensity = 0;
	
	UNITY_BRANCH
	if(dot(rimOffset, rimOffset) > 0.001f) //If we have an offset we calculate it
	{
	float3 viewSpaceOffset = mul((float3x3)UNITY_MATRIX_V, -rimOffset);
	float3 normalizedViewDir = normalize(viewDirWS);
	float3 biasedViewDir = normalize(normalizedViewDir + viewSpaceOffset);
	float NdotV = saturate(dot(normalWS, biasedViewDir));
	rimIntensity = smoothstep(_MinRim, _MaxRim, 1 - NdotV) * _RimAttenuation;
	}
	else //Otherwise we take quick path
	{
	float3 normalizedViewDir = normalize(viewDirWS);
	float NdotV = saturate(dot(normalWS, normalizedViewDir));
	rimIntensity = smoothstep(_MinRim, _MaxRim, 1 - NdotV) * _RimAttenuation;
	}
	
	float3 res = lerp(inputColorRGB, _RimColor.rgb, rimIntensity);

return res;
}

float3 Greyscale(float _GreyscaleLuminosity, float4 _GreyscaleTintColor, float _GreyscaleBlend, float3 inputColorRGB)
{
	float3 res = inputColorRGB;
	float luminance = GetLuminance(res);
	luminance = saturate(luminance + _GreyscaleLuminosity);
	res = lerp(res, luminance * _GreyscaleTintColor.rgb, _GreyscaleBlend);

return res;
}

float3 Posterize(float _PosterizeGamma, float _PosterizeNumColors, float3 inputColorRGB)
{
	float3 res = inputColorRGB;
	float gamma = _PosterizeGamma;
	float numColors = _PosterizeNumColors;
	res.rgb = pow(res.rgb, gamma) * numColors;
	res.rgb = floor(res.rgb) / numColors;
	res.rgb = pow(res.rgb, 1.0 / gamma);

return res;
}

float3 Highlights(float3 lightDir, float3 viewDirWS, float3 normalWS, float4 _HighlightOffset, float _HighlightCutoff, float _HighlightSmoothness, float4 _HighlightsColor, float _HighlightsStrength, float3 inputColorRGB)
{
	float3 normalizedLightDir = normalize(lightDir);
	float3 offsetLightDir = normalize(normalizedLightDir + _HighlightOffset);
	float3 normalizedViewDir = normalize(viewDirWS);
	
	float NdotL = saturate(dot(normalWS, offsetLightDir));
	float NdotV = saturate(dot(normalWS, normalizedViewDir));
	float rimFactor = 1.0 - NdotV;
	
	float highlightCutoff = _HighlightCutoff;
	float highlightSmoothness = _HighlightSmoothness;
	float highlightsIntensity = smoothstep(highlightCutoff, highlightCutoff + highlightSmoothness, rimFactor);
	
	float3 highlights = highlightsIntensity * NdotL * _HighlightsColor.rgb * _HighlightsStrength;
	
	//return inputColorRGB + highlights;
	
	float3 res = inputColorRGB * (highlights + 1);

return res;
}

float4 HeightGradient_WorldSpace(float3 vertexOS, float3 vertexWS, float _MinGradientHeight, float _MaxGradientHeight, float4 _GradientHeightColor01, float4 _GradientHeightColor02, float4 inputColor)
{
	float4 res = inputColor;
	
	float3 selectedPos = vertexOS;
	
	selectedPos = vertexWS;
	
	
	float gradient = RemapFloat(selectedPos.y, _MinGradientHeight, _MaxGradientHeight, 0.0, 1.0);
	gradient = saturate(gradient);
	
	float4 gradientColor = lerp(_GradientHeightColor01, _GradientHeightColor02, gradient);
	
	res *= gradientColor;

return res;
}

float4 HeightGradient(float3 vertexOS, float _MinGradientHeight, float _MaxGradientHeight, float4 _GradientHeightColor01, float4 _GradientHeightColor02, float4 inputColor)
{
	float4 res = inputColor;
	
	float3 selectedPos = vertexOS;
	
	
	float gradient = RemapFloat(selectedPos.y, _MinGradientHeight, _MaxGradientHeight, 0.0, 1.0);
	gradient = saturate(gradient);
	
	float4 gradientColor = lerp(_GradientHeightColor01, _GradientHeightColor02, gradient);
	
	res *= gradientColor;

return res;
}

float4 IntersectionGlow(float sceneDepthDiff, float _DepthGlowDist, float _DepthGlowPower, float _DepthGlowGlobalIntensity, float4 _DepthGlowColor, float _DepthGlowColorIntensity, float4 inputColor)
{
	float4 res = inputColor;
	
	float depthGlowMask = saturate(_DepthGlowDist * pow(max(0, 1 - sceneDepthDiff), _DepthGlowPower));
	
	res.rgb = lerp(res.rgb, _DepthGlowGlobalIntensity * res.rgb, depthGlowMask);
	res.rgb += _DepthGlowColor.rgb * _DepthGlowColorIntensity * depthGlowMask * res.a;

return res;
}

float4 SubsurfaceScattering(float2 mainUV, float3 viewDirWS, float3 normalWS, float3 lightDir, float _NormalInfluence, float _SSSPower, float _SSSFrontPower, float _SSSFrontAtten, float _SSSAtten, float4 _SSSColor, sampler2D _SSSMap, float4 inputColor)
{
	float4 res = inputColor;
	
	float4 sssMapColor = SAMPLE_TEX2D(_SSSMap, mainUV);
	
	//Why do we need to add 1000 for this to look good?
	float3 lightColor = GetMainLightColorRGB() * 5;
	float lightIntensity = GetMainLightIntensity();
	
	float3 normalizedViewDir = normalize(viewDirWS);
	float3 scatterNormal = (normalWS * _NormalInfluence) + lightDir;
	scatterNormal = -normalize(scatterNormal);
	
	float VdotScatterNormal = saturate(dot(scatterNormal, normalizedViewDir));
	
	float sssPower = max(1.0, _SSSPower * sssMapColor.r);
	float backScatterFactor = pow(VdotScatterNormal, sssPower);
	
	float frontScatter = saturate(dot(normalWS, lightDir)); // How directly light hits surface
	float frontVdotN = 1.0 - saturate(dot(normalizedViewDir, normalWS)); // Fresnel-like front falloff
	frontVdotN = pow(frontVdotN, _SSSFrontPower);
	float frontScatterFactor = frontScatter * frontVdotN * _SSSFrontAtten;
	
	float scatterIntensity = max(backScatterFactor, frontScatterFactor) * _SSSAtten * sssMapColor.a;
	
	// Simulate wavelength-dependent scattering with original scatter calculation
	float3 deepScatterColor = float3(1.0, 0.3, 0.2);
	float3 scatterColor = scatterIntensity * lightColor.rgb * lightIntensity * _SSSColor.rgb;
	scatterColor *= lerp(float3(1,1,1), deepScatterColor, VdotScatterNormal);
	
	res.rgb += scatterColor * inputColor.rgb;

return res;
}

float3 VertexShake(float _ShakeSpeedMult, float4 _ShakeSpeed, float4 _ShakeMaxDisplacement, float _ShakeBlend, float3 vertexPos, float3 shaderTime)
{
	float3 res = vertexPos;
	
	float3 speedOffset = float3(1.0f, 1.13f, 1.07f) * _ShakeSpeedMult;
	float3 displacement = sin(shaderTime.y * _ShakeSpeed.xyz * speedOffset) * _ShakeMaxDisplacement.xyz;
	displacement *= _ShakeBlend;
	
	res += displacement;

return res;
}

float3 VertexInflate(float _MinInflate, float _MaxInflate, float _InflateBlend, float3 vertexPos, float3 normalOS, float3 shaderTime)
{
	float3 res = vertexPos;
	
	float inflateValue = lerp(_MinInflate, _MaxInflate, _InflateBlend);
	res += normalOS * inflateValue;

return res;
}

float3 VertexDistortion(float4 _VertexDistortionNoiseSpeed, float _VertexDistortionAmount, sampler2D _VertexDistortionNoiseTex, float4 _VertexDistortionNoiseTex_ST, float3 vertexPos, float3 normalOS, float3 shaderTime)
{
	float3 res = vertexPos;
	
	float noisePower = 1.0;
	
	float2 noiseUV = SIMPLE_CUSTOM_TRANSFORM_TEX(vertexPos.xy, _VertexDistortionNoiseTex);
	float4 correctedNoiseUV = float4(noiseUV.x, noiseUV.y, 0, 0);
	
	correctedNoiseUV.x += frac(shaderTime.x * _VertexDistortionNoiseSpeed.x);
	correctedNoiseUV.y += frac(shaderTime.x * _VertexDistortionNoiseSpeed.y);
	
	noisePower = SAMPLE_TEX2D_LOD(_VertexDistortionNoiseTex, correctedNoiseUV).r;
	
	res += normalOS * noisePower * _VertexDistortionAmount;

return res;
}

float3 VertexVoxel(float _VoxelSize, float _VoxelBlend, float3 vertexPos)
{
	float3 voxelizedPosition = round(vertexPos * _VoxelSize) / _VoxelSize;
	float3 res = lerp(vertexPos, voxelizedPosition, _VoxelBlend);

return res;
}

float3 Glitch(float4 _GlitchOffset, float _GlitchWorldSpace, float _GlitchSpeed, float _GlitchTiling, float _GlitchAmount, float3 vertexPos, float3 shaderTime)
{
	float3 res = vertexPos;
	
	float3 glitchDir = mul((float3x3)unity_WorldToObject, _GlitchOffset);
	float3 scale = float3(length(unity_ObjectToWorld[0].xyz),
	length(unity_ObjectToWorld[1].xyz),
	length(unity_ObjectToWorld[2].xyz));
	float pos = _GlitchWorldSpace ? mul(unity_ObjectToWorld, float4(vertexPos, 1.0)).y : vertexPos.y;
	float time = shaderTime.y * _GlitchSpeed;
	
	// Add high frequency noise to the main UV
	float2 glitchUV = float2(pos * _GlitchTiling + time, time * 0.89);
	float mainNoise = noise2D(glitchUV);
	float fastNoise = noise2D(glitchUV * 2.5 + float2(time * 3.7, 0));
	mainNoise = mainNoise * 0.6 + fastNoise * 0.4;
	
	float2 periodicUV = float2(time * 0.5, time * 0.14);
	float periodicNoise = saturate(noise2D(periodicUV) + 0.1);
	
	float detailNoise = noise2D(float2(20.0 * glitchUV.x, glitchUV.y));
	
	float glitchValue = (2.0 * mainNoise - 1.0) * periodicNoise;
	glitchValue += glitchValue * lerp(0, saturate(2.0 * detailNoise - 1.0), 2.0);
	
	res += (glitchDir / scale) * glitchValue * _GlitchAmount;

return res;
}

float2 ScrollTexture(float _ScrollTextureX, float _ScrollTextureY, float2 inputUV, float3 shaderTime)
{
	float2 res = inputUV;
	
	res.x += frac(shaderTime.x * _ScrollTextureX);
	res.y += frac(shaderTime.x * _ScrollTextureY);

return res;
}

float2 WaveUV(float2 initialValue, float3 shaderTime, float _WaveX, float4 _MainTex_ST, float _WaveY, float _WaveAmount, float _WaveSpeed, float _WaveStrength)
{
	float2 res = initialValue;
	
	float2 uvWaveDiff = float2(_WaveX * _MainTex_ST.x, _WaveY * _MainTex_ST.y) - initialValue;
	
	uvWaveDiff.x *= _ScreenParams.x / _ScreenParams.y;
	float waveTime = shaderTime.y;
	float angWave = (sqrt(dot(uvWaveDiff, uvWaveDiff)) * _WaveAmount) - ((waveTime *  _WaveSpeed % 360.0));
	
	uvWaveDiff = normalize(uvWaveDiff) * sin(angWave) * (_WaveStrength / 1000.0);
	DISPLACE_ALL_UVS(res, uvWaveDiff);

return res;
}

float2 HandDrawn(float _HandDrawnSpeed, float _HandDrawnAmount, float2 inputUV, float3 shaderTime)
{
	float2 uvCopy = inputUV;
	float2 res = inputUV;
	
	
	float drawnSpeed = (floor(frac(shaderTime.x) * 20 * _HandDrawnSpeed) / _HandDrawnSpeed) * _HandDrawnSpeed;
	uvCopy.x = sin((uvCopy.x * _HandDrawnAmount + drawnSpeed) * 4);
	uvCopy.y = cos((uvCopy.y * _HandDrawnAmount + drawnSpeed) * 4);
	res = lerp(res, res + uvCopy, 0.0005 * _HandDrawnAmount);

return res;
}

float2 UVDistortion(float2 initialValue, float2 uv_dist, float3 shaderTime, float _DistortTexXSpeed, float _DistortTexYSpeed, float _DistortAmount, sampler2D _DistortTex)
{
	float2 res = initialValue;
	
	float2 distortTexUV = uv_dist;
	
	distortTexUV.x += frac((shaderTime.x) * _DistortTexXSpeed);
	distortTexUV.y += frac((shaderTime.x) * _DistortTexYSpeed);
	
	float4 distortTexCol = SAMPLE_TEX2D_LOD(_DistortTex, float4(distortTexUV.x, distortTexUV.y, 0, 0));
	float distortAmnt = (distortTexCol.r - 0.5) * 0.2 * _DistortAmount;
	
	DISPLACE_ALL_UVS(res, distortAmnt);

return res;
}

float2 Pixelate(float2 initialValue, float _PixelateSize, sampler2D _MainTex, float2 _MainTex_TexelSize)
{
	float2 res = initialValue;
	
	half aspectRatio = _MainTex_TexelSize.x / _MainTex_TexelSize.y;
	half2 pixelSize = float2(_PixelateSize, _PixelateSize * aspectRatio);
	
	QUANTIZE_ALL_UVS(res, pixelSize)

return res;
}

float4 Fade_Burn(float _FadePower, float _FadeAmount, float _FadeTransition, float _FadeBurnWidth, float4 _FadeBurnColor, sampler2D _FadeTex, float4 _FadeTex_ST, float4 inputColor, float2 uv)
{
	float4 res = inputColor;
	
	float2 fadeUV = SIMPLE_CUSTOM_TRANSFORM_TEX(uv, _FadeTex);
	float fadeSample = SAMPLE_TEX2D(_FadeTex, fadeUV).r;
	
	fadeSample = pow(saturate(fadeSample), _FadePower);
	
	
	float fadeAmount = lerp(_FadeAmount - _FadeTransition - _FadeBurnWidth, 1.0, _FadeAmount);
	float fade = smoothstep(fadeAmount, fadeAmount + _FadeTransition, fadeSample);
	
	float fadePlusBurn = smoothstep(fadeAmount + _FadeBurnWidth, fadeAmount + _FadeBurnWidth + _FadeTransition, fadeSample);
	
	float diff = saturate(fade - fadePlusBurn);
	
	float3 burnColor = diff * _FadeBurnColor.rgb;
	
	res.rgb += burnColor;
	
	
	res.a = fade;

return res;
}

float4 Fade(float _FadePower, float _FadeAmount, float _FadeTransition, sampler2D _FadeTex, float4 _FadeTex_ST, float4 inputColor, float2 uv)
{
	float4 res = inputColor;
	
	float2 fadeUV = SIMPLE_CUSTOM_TRANSFORM_TEX(uv, _FadeTex);
	float fadeSample = SAMPLE_TEX2D(_FadeTex, fadeUV).r;
	
	fadeSample = pow(saturate(fadeSample), _FadePower);
	
	
	float fadeAmount = lerp(_FadeAmount - _FadeTransition, 1.0, _FadeAmount);
	float fade = smoothstep(fadeAmount, fadeAmount + _FadeTransition, fadeSample);
	
	
	res.a = fade;

return res;
}

float4 FadeByCamDistance(float _MinDistanceToFade, float _MaxDistanceToFade, float4 inputColor, float camDistance)
{
	float4 res = inputColor;
	
	float t = 0;
	#ifdef _FADE_BY_CAM_DISTANCE_NEAR_FADE
	t = 1 - smoothstep(_MinDistanceToFade, _MaxDistanceToFade, camDistance);
	#else
	t = smoothstep(_MinDistanceToFade, _MaxDistanceToFade, camDistance);
	#endif
	
	
	res.a = lerp(res.a, 0, t);
	


return res;
}



#endif