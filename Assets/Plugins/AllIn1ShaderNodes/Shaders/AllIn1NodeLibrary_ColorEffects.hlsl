#ifndef ALLIN1NODELIBRARY_COLOREFFECTS_INCLUDED
#define ALLIN1NODELIBRARY_COLOREFFECTS_INCLUDED

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

float4 AlbedoVertexColor_Multiply(float4 inputColor, float4 vertexColor, float _VertexColorBlending)
{
	float4 res = inputColor;
	
	float3 multipliedColor = res.rgb * vertexColor.rgb;
	res.rgb = lerp(res.rgb, multipliedColor, _VertexColorBlending);
	
	return res;
}

float4 AlbedoVertexColor_Replace(float4 inputColor, float4 vertexColor, float _VertexColorBlending)
{
	float4 res = inputColor;
	
	res.rgb = lerp(inputColor.rgb, vertexColor.rgb, _VertexColorBlending);
	
	return res;
}

float4 Hologram(float4 inputColor, float3 vertexWS, float shaderTime, float4 _HologramLineDirection,
	float _HologramFrequency, float _HologramScrollSpeed, float _HologramLineCenter, 
	float _HologramLineSpacing, float _HologramLineSmoothness, float _HologramBaseAlpha, 
	float _HologramAccentFrequency, float _HologramAccentSpeed, float _HologramAlpha, 
	float _HologramAccentAlpha, float4 _HologramColor)
{
	float4 res = inputColor;
	
	float3 dir = normalize(_HologramLineDirection.xyz);
	
	// Calculate primary hologram pattern using direction projection
	float3 scrollPos1 = vertexWS * _HologramFrequency + (shaderTime * _HologramScrollSpeed);
	float3 scrollUV1 = frac(scrollPos1);
	
	float projectedValue1 = dot(scrollUV1, normalize(dir));
	float distance1 = abs(projectedValue1 - _HologramLineCenter) * _HologramLineSpacing;
	float gradientMask1 = 1 - distance1;
	gradientMask1 = pow(saturate(gradientMask1), _HologramLineSmoothness);
	gradientMask1 = max(gradientMask1, _HologramBaseAlpha);
	
	// Calculate accent line pattern using direction projection
	float3 scrollPos2 = vertexWS * _HologramAccentFrequency + (shaderTime * _HologramAccentSpeed);
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
	
	//res = _HologramLineDirection;
	//res.a = 1;

	return res;
}

float3 GetNormalVSByNormalOS(float3 normalOS)
{
	float3 res = normalize(mul(UNITY_MATRIX_MV, float4(normalOS, 0.0)).xyz);

	return res;
}

float3 GetNormalVSByNormalWS(float3 normalWS)
{
	//float3 normalMapOS = normalize(mul(UNITY_MATRIX_IT_MV, float4(normalWS, 0.0)).xyz);
	float3 normalMapOS = normalize(TransformNormalWorldToObject(normalWS));


	float3 res = GetNormalVSByNormalOS(normalMapOS);

	return res;
}

float3 MatCap(float3 inputColor, float3 normalVS, float3 vertexOS, float _MatcapIntensity, float _MatcapBlend, TEX_PARAM _MatcapTex)
{
	float3 positionVSDir = normalize(GetPositionVS(vertexOS));
	
	float3 normalCrossPosition = cross(positionVSDir, normalVS);
	
	float u = (-normalCrossPosition.y * 0.5) + 0.5;
	float v = (normalCrossPosition.x * 0.5) + 0.5;
	
	float2 matcapUV = float2(u, v);
	float3 matcap = SAMPLE_TEX2D(_MatcapTex, matcapUV).rgb * _MatcapIntensity;
	
	float3 colorWithMatcapApplied = inputColor * matcap;
	float3 res = lerp(inputColor, colorWithMatcapApplied, _MatcapBlend);
	
	return res;
}

float3 Matcap_NormalWS(float3 inputColor, float3 normalWS, float3 vertexOS, float _MatcapIntensity, float _MatcapBlend, TEX_PARAM _MatcapTex)
{
	float3 normalVS = GetNormalVSByNormalWS(normalWS);
	float3 res = MatCap(inputColor, normalVS, vertexOS, _MatcapIntensity, _MatcapBlend, _MatcapTex);

	return res;
}

float3 Matcap_NormalOS(float3 inputColor, float3 normalOS, float3 vertexOS, float _MatcapIntensity, float _MatcapBlend, TEX_PARAM _MatcapTex)
{
	float3 normalVS = GetNormalVSByNormalOS(normalOS);
	float3 res = MatCap(inputColor, normalVS, vertexOS, _MatcapIntensity, _MatcapBlend, _MatcapTex);

	return res;
}

float4 ColorRamp(float4 inputColor, float shaderTime, float _ColorRampLuminosity, float _ColorRampTiling, float _ColorRampScrollSpeed, float _ColorRampBlend, TEX_PARAM _ColorRampTex)
{
	float luminance = GetLuminance(inputColor);
	luminance = saturate(luminance + _ColorRampLuminosity);
	
	float2 rampUV = float2(luminance, 0);
	rampUV.x *= _ColorRampTiling;
	rampUV.x += frac(shaderTime * _ColorRampScrollSpeed);
	
	float3 colorRamp = SAMPLE_TEX2D(_ColorRampTex, rampUV).rgb;
	float3 resRGB = lerp(inputColor.rgb, colorRamp, _ColorRampBlend);
	
	float4 res = float4(resRGB, inputColor.a);
	
	return res;
}

float4 Hit(float4 inputColor, float4 _HitColor, float _HitGlow, float _HitBlend)
{
	float3 resRGB = lerp(inputColor.rgb, _HitColor.rgb * _HitGlow, _HitBlend);
	float4 res = float4(resRGB, inputColor.a);
	
	return res;
}

float3 Rim(float3 inputColorRGB, float3 viewDirWS, float3 normalWS, float4 _RimOffset, float _MinRim, float _MaxRim, float _RimAttenuation, float4 _RimColor)
{
	float3 rimOffset = _RimOffset.xyz;
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

float3 Greyscale(float3 inputColorRGB, float _GreyscaleLuminosity, float4 _GreyscaleTintColor, float _GreyscaleBlend)
{
	float3 res = inputColorRGB;
	float luminance = GetLuminance(res);
	luminance = saturate(luminance + _GreyscaleLuminosity);
	res = lerp(res, luminance * _GreyscaleTintColor.rgb, _GreyscaleBlend);
	
	return res;
}

float3 Posterize(float3 inputColorRGB, float _PosterizeGamma, float _PosterizeNumColors)
{
	float3 res = inputColorRGB;
	float gamma = _PosterizeGamma;
	float numColors = _PosterizeNumColors;
	res.rgb = pow(saturate(res.rgb), gamma) * numColors;
	res.rgb = floor(res.rgb) / numColors;
	res.rgb = pow(abs(res.rgb), 1.0 / gamma);
	
	return res;
}

float3 Highlights(float3 inputColorRGB, float3 viewDirWS, float3 normalWS, 
	float4 _HighlightOffset, float _HighlightCutoff, float _HighlightSmoothness, 
	float4 _HighlightsColor, float _HighlightsStrength)
{
	AllIn1LightData lightData = GetMainLightData();

	float3 normalizedLightDir = normalize(lightData.lightDir);
	float3 offsetLightDir = normalize(normalizedLightDir + _HighlightOffset.xyz);
	float3 normalizedViewDir = normalize(viewDirWS);
	
	float NdotL = saturate(dot(normalWS, offsetLightDir));
	float NdotV = saturate(dot(normalWS, normalizedViewDir));
	float rimFactor = 1.0 - NdotV;
	
	float highlightCutoff = _HighlightCutoff;
	float highlightSmoothness = _HighlightSmoothness;
	
	float edge0 = highlightCutoff;
	float edge1 = highlightCutoff + highlightSmoothness + EPSILON;
	
	float highlightsIntensity = smoothstep(edge0, edge1, rimFactor);
	
	float3 highlights = highlightsIntensity * NdotL * _HighlightsColor.rgb * _HighlightsStrength;
	
	//return inputColorRGB + highlights;
	
	float3 res = inputColorRGB * (highlights + 1);
	
	return res;
}


float4 HeightGradient(float4 inputColor, float3 position, float _MinGradientHeight, float _MaxGradientHeight, float4 _GradientHeightColor01, float4 _GradientHeightColor02)
{
	float4 res = inputColor;
	
	float gradient = RemapFloat(position.y, _MinGradientHeight, _MaxGradientHeight, 0.0, 1.0);
	gradient = saturate(gradient);
	
	float4 gradientColor = lerp(_GradientHeightColor01, _GradientHeightColor02, gradient);
	
	res *= gradientColor;
	
	return res;
}

float4 IntersectionGlow(float4 inputColor, float sceneDepthDiff, float _DepthGlowDist, float _DepthGlowPower, 
	float _DepthGlowGlobalIntensity, float4 _DepthGlowColor, float _DepthGlowColorIntensity)
{
	float4 res = inputColor;
	
	float depthGlowMask = saturate(_DepthGlowDist * pow(max(0, 1 - sceneDepthDiff), _DepthGlowPower));
	
	res.rgb = lerp(res.rgb, _DepthGlowGlobalIntensity * res.rgb, depthGlowMask);
	res.rgb += _DepthGlowColor.rgb * _DepthGlowColorIntensity * depthGlowMask * res.a;
	
	return res;
}

float4 SubsurfaceScattering(float4 inputColor, float2 mainUV, float3 viewDirWS, float3 normalWS, 
	float _NormalInfluence, float _SSSPower, float _SSSFrontPower, 
	float _SSSFrontAtten, float _SSSAtten, float4 _SSSColor, TEX_PARAM _SSSMap)
{
	float4 res = inputColor;
	
	AllIn1LightData lightData = GetMainLightData();
	float3 lightDir = normalize(lightData.lightDir);
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

float4 GlowInternal(float4 emissionColor, half4 inputColor, float _Glow, float3 _GlowColor, float _GlowGlobal)
{
	float4 res = inputColor;
	
	res.rgb *= _GlowGlobal;

	emissionColor.rgb *= emissionColor.a * res.a * _Glow * _GlowColor;
	res.rgb += emissionColor.rgb;

	return res;
}

float4 Glow(float4 inputColor, float _Glow, float3 _GlowColor, float _GlowGlobal)
{
	float4 res = GlowInternal(inputColor, inputColor, _Glow, _GlowColor, _GlowGlobal);
	return res;
}

float4 GlowTex(float4 inputColor, TEX_PARAM _GlowTex, float2 uv, float _Glow, float3 _GlowColor, float _GlowGlobal)
{
	float4 emissionColor = SAMPLE_TEX2D(_GlowTex, uv);
	float4 res = GlowInternal(emissionColor, inputColor, _Glow, _GlowColor, _GlowGlobal);
	return res;
}

float4 GradientColor(float4 inputColor, float2 uv, 
	float4 _GradTopLeftCol, float4 _GradTopRightCol, 
	float4 _GradBotLeftCol, float4 _GradBotRightCol,
	float _GradBoostX, float _GradBoostY, float _GradBlend, float4 tilingAndOffset)
{
	float4 res = inputColor;

	float2 uvRect = uv;

	float2 tiledUvGrad = float2(uvRect.x / tilingAndOffset.x, uvRect.y / tilingAndOffset.y);
	
	float gradXLerpFactor = saturate(pow(abs(tiledUvGrad.x), _GradBoostX));
	float4 gradientResult = lerp(lerp(_GradBotLeftCol, _GradBotRightCol, gradXLerpFactor),
	lerp(_GradTopLeftCol, _GradTopRightCol, gradXLerpFactor), saturate(pow(abs(tiledUvGrad.y), _GradBoostY)));
	
	gradientResult = lerp(res, gradientResult, _GradBlend);
	res.rgb = gradientResult.rgb * res.a;
	res.a *= gradientResult.a;

	return res;
}

float4 GradientColorRadial(float4 inputColor, float2 uv, 
	float4 _GradTopLeftCol, float4 _GradBotLeftCol, 
	float _GradBoostX, float _GradBlend, float4 tilingAndOffset, float4 texelSize)
{
	float4 res = inputColor;

	float2 uvRect = uv;

	float2 tiledUvGrad = float2(uvRect.x / tilingAndOffset.x, uvRect.y / tilingAndOffset.y);
	
	half radialDist = 1 - length(tiledUvGrad - half2(0.5, 0.5));
	radialDist *= (texelSize.w / texelSize.z);
	radialDist = saturate(_GradBoostX * radialDist);
	half4 gradientResult = lerp(_GradTopLeftCol, _GradBotLeftCol, radialDist);

	gradientResult = lerp(res, gradientResult, _GradBlend);
	res.rgb = gradientResult.rgb * res.a;
	res.a *= gradientResult.a;

	return res;
}

float4 NegativeColor(float4 inputColor, float _NegativeAmount)
{
	float4 res = inputColor;

	res.rgb = lerp(res.rgb, 1 - res.rgb, _NegativeAmount);

	return res;
}

float4 ChromaticAberration(float4 inputColor, float2 uv, TEX_PARAM tex, float _AberrationAmount, float _AberrationAlpha)
{
	float4 res = inputColor;

	float4 r = SAMPLE_TEX2D(tex, uv + float2(_AberrationAmount / 10, 0));
	float4 b = SAMPLE_TEX2D(tex, uv + float2(-_AberrationAmount / 10, 0));
	
	res = float4(r.r * r.a, res.g * res.a, b.b * b.a, max(max(r.a, b.a) * _AberrationAlpha, res.a));
	
	return res;
}

float4 Shine(float4 inputColor, float2 uv, float4 _ShineColor, float _ShineLocation, float _ShineRotate, float _ShineWidth, float _ShineGlow, TEX_PARAM _ShineMask)
{
	float4 res = inputColor;

	float2 uvRect = uv;

	float2 uvShine = uvRect;
	float cosAngle = cos(_ShineRotate);
	float sinAngle = sin(_ShineRotate);
	half2x2 rot = half2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
	uvShine -= float2(0.5, 0.5);
	uvShine = mul(rot, uvShine);
	uvShine += float2(0.5, 0.5);
	float shineMask = SAMPLE_TEX2D(_ShineMask, uv).a;
	float currentDistanceProjection = (uvShine.x + uvShine.y) / 2;
	float whitePower = 1 - (abs(currentDistanceProjection - _ShineLocation) / _ShineWidth);
	
	res.rgb +=  res.a * whitePower * _ShineGlow * max(sign(currentDistanceProjection - (_ShineLocation - _ShineWidth)), 0.0)
	* max(sign((_ShineLocation + _ShineWidth) - currentDistanceProjection), 0.0) * _ShineColor.rgb * shineMask;

	return res;
}

float4 SpriteOutline(float4 inputColor, TEX_PARAM tex, float4 texelSize, 
	float4 _OutlineColor, float _OutlineWidth, float _OutlineAlpha, float _OutlineGlow, 
	float2 uv)
{
	float4 res = inputColor;

	float originalAlpha = inputColor.a;
	float2 destUv = float2(_OutlineWidth * texelSize.x * 200, _OutlineWidth * texelSize.y * 200);

	float spriteLeft	= SAMPLE_TEX2D(tex, uv	+ float2(destUv.x, 0)).a;
	float spriteRight	= SAMPLE_TEX2D(tex, uv	- float2(destUv.x, 0)).a;
	float spriteBottom	= SAMPLE_TEX2D(tex, uv	+ float2(0, destUv.y)).a;
	float spriteTop		= SAMPLE_TEX2D(tex, uv	- float2(0, destUv.y)).a;

	float spriteTopLeft		= SAMPLE_TEX2D(tex, uv + float2(destUv.x, destUv.y)).a;
	float spriteTopRight	= SAMPLE_TEX2D(tex, uv + float2(-destUv.x, destUv.y)).a;
	float spriteBotLeft		= SAMPLE_TEX2D(tex, uv + float2(destUv.x, -destUv.y)).a;
	float spriteBotRight	= SAMPLE_TEX2D(tex, uv + float2(-destUv.x, -destUv.y)).a;

	float result = spriteLeft + spriteRight + spriteBottom + spriteTop + 
		spriteTopLeft + spriteTopRight + spriteBotLeft + spriteBotRight;

	result = step(0.05, saturate(result));

	result *= (1 - originalAlpha) * _OutlineAlpha;

	float4 outline = _OutlineColor;
	outline.rgb *= _OutlineGlow;
	outline.a = result;

	res = lerp(res, outline, result);
	//res = res.a;

	return res;
}

float Luminosity(float4 col)
{
	float res = 0.3 * col.r + 0.59 * col.g + 0.11 * col.b;
	res *= col.a;

	return res;
}

#endif //ALLIN1NODELIBRARY_COLOREFFECTS_INCLUDED