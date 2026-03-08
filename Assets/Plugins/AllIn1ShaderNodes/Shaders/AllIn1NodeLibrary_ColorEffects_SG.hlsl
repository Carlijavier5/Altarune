#ifndef ALLIN1NODELIBRARY_COLOREFFECTS_SG_INCLUDED
#define ALLIN1NODELIBRARY_COLOREFFECTS_SG_INCLUDED

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_ColorEffects.hlsl"

void HueShift_float(float3 inputColor, float hueShift, float hueSaturation, float hueBrightness, out float3 Out)
{
	float3 res = HueShift(inputColor, hueShift, hueSaturation, hueBrightness);
	Out = res;
}

void ContrastBrightness_float(float3 inputColor, float contrast, float brightness, out float3 Out)
{
	float3 res = ContrastBrightness(inputColor, contrast, brightness);
	Out = res;
}

void AlbedoVertexColor_Multiply_float(float4 inputColor, float4 vertexColor, float _VertexColorBlending, out float4 Out)
{
	float4 res = AlbedoVertexColor_Multiply(inputColor, vertexColor, _VertexColorBlending);
	Out = res;
}

void AlbedoVertexColor_Replace_float(float4 inputColor, float4 vertexColor, float _VertexColorBlending, out float4 Out)
{
	float4 res = AlbedoVertexColor_Replace(inputColor, vertexColor, _VertexColorBlending);
	Out = res;
}

void Hologram_float(float4 inputColor, float3 vertexWS, float shaderTime, float4 _HologramLineDirection,
	float _HologramFrequency, float _HologramScrollSpeed, float _HologramLineCenter, 
	float _HologramLineSpacing, float _HologramLineSmoothness, float _HologramBaseAlpha, 
	float _HologramAccentFrequency, float _HologramAccentSpeed, float _HologramAlpha, 
	float _HologramAccentAlpha, float4 _HologramColor, out float4 Out)
{
	float4 res = Hologram(inputColor, vertexWS, shaderTime, _HologramLineDirection,
		_HologramFrequency, _HologramScrollSpeed, _HologramLineCenter,
		_HologramLineSpacing, _HologramLineSmoothness, _HologramBaseAlpha,
		_HologramAccentFrequency, _HologramAccentSpeed, _HologramAlpha,
		_HologramAccentAlpha, _HologramColor);

	Out = res;
}

void Matcap_NormalOS_float(float3 inputColor, float3 normalOS, float3 vertexOS, float _MatcapIntensity, float _MatcapBlend, UnityTexture2D _MatcapTex, out float3 Out)
{
	float3 res = Matcap_NormalOS(inputColor, normalOS, vertexOS, _MatcapIntensity, _MatcapBlend, _MatcapTex); 
	Out = res;
}

void Matcap_NormalWS_float(float3 inputColor, float3 normalWS, float3 vertexOS, float _MatcapIntensity, float _MatcapBlend, UnityTexture2D _MatcapTex, out float3 Out)
{
	float3 res = Matcap_NormalWS(inputColor, normalWS, vertexOS, _MatcapIntensity, _MatcapBlend, _MatcapTex);
	Out = res;
}

void ColorRamp_float(float4 inputColor, float shaderTime, float _ColorRampLuminosity, float _ColorRampTiling, float _ColorRampScrollSpeed, float _ColorRampBlend, TEX_PARAM _ColorRampTex, out float4 Out)
{
	float4 res = ColorRamp(inputColor, shaderTime, _ColorRampLuminosity, _ColorRampTiling, _ColorRampScrollSpeed, _ColorRampBlend, _ColorRampTex);
	Out = res;
}

void Hit_float(float4 inputColor, float4 _HitColor, float _HitGlow, float _HitBlend, out float4 Out)
{
	float4 res = Hit(inputColor, _HitColor, _HitGlow, _HitBlend);
	Out = res;
}

void Rim_float(float3 inputColorRGB, float3 viewDirWS, float3 normalWS, float4 _RimOffset, float _MinRim, float _MaxRim, float _RimAttenuation, float4 _RimColor, out float3 Out)
{
	float3 res = Rim(inputColorRGB, viewDirWS, normalWS, _RimOffset, _MinRim, _MaxRim, _RimAttenuation, _RimColor);
	Out = res;
}

void Greyscale_float(float3 inputColorRGB, float _GreyscaleLuminosity, float4 _GreyscaleTintColor, float _GreyscaleBlend, out float3 Out)
{
	float3 res = Greyscale(inputColorRGB, _GreyscaleLuminosity, _GreyscaleTintColor, _GreyscaleBlend);
	Out = res;
}

void Posterize_float(float3 inputColorRGB, float _PosterizeGamma, float _PosterizeNumColors, out float3 Out)
{
	float3 res = Posterize(inputColorRGB, _PosterizeGamma, _PosterizeNumColors);
	Out = res;
}

void Highlights_float(float3 inputColorRGB, float3 viewDirWS, float3 normalWS, 
	float4 _HighlightOffset, float _HighlightCutoff, float _HighlightSmoothness, 
	float4 _HighlightsColor, float _HighlightsStrength, out float3 Out)
{
	float3 res = Highlights(inputColorRGB, viewDirWS, normalWS, 
		_HighlightOffset, _HighlightCutoff, _HighlightSmoothness,
		_HighlightsColor, _HighlightsStrength);

	Out = res;
}

void HeightGradient_float(float4 inputColor, float3 position, float _MinGradientHeight, float _MaxGradientHeight, float4 _GradientHeightColor01, float4 _GradientHeightColor02, out float4 Out)
{
	float4 res = HeightGradient(inputColor, position, _MinGradientHeight, _MaxGradientHeight, _GradientHeightColor01, _GradientHeightColor02);
	
	Out = res;
}

void IntersectionGlow_float(float4 inputColor, float sceneDepthDiff, float _DepthGlowDist, float _DepthGlowPower, 
	float _DepthGlowGlobalIntensity, float4 _DepthGlowColor, float _DepthGlowColorIntensity, out float4 Out)
{
	float4 res = IntersectionGlow(inputColor, sceneDepthDiff, _DepthGlowDist, _DepthGlowPower, 
		_DepthGlowGlobalIntensity, _DepthGlowColor, _DepthGlowColorIntensity);
	
	Out = res;
}

void SubsurfaceScattering_float(float4 inputColor, float2 mainUV, float3 viewDirWS, float3 normalWS, 
	float _NormalInfluence, float _SSSPower, float _SSSFrontPower, 
	float _SSSFrontAtten, float _SSSAtten, float4 _SSSColor, UnityTexture2D _SSSMap, out float4 Out)
{
	float4 res = SubsurfaceScattering(inputColor, mainUV, viewDirWS, normalWS, 
		_NormalInfluence, _SSSPower, _SSSFrontPower, 
		_SSSFrontAtten, _SSSAtten, _SSSColor, _SSSMap);
	
	Out = res;
}

void Glow_float(float4 inputColor, float _Glow, float3 _GlowColor, float _GlowGlobal, out float4 Out)
{
	float4 res = Glow(inputColor, _Glow, _GlowColor, _GlowGlobal);

	Out = res;
}

void GlowTex_float(float4 inputColor, TEX_PARAM _GlowTex, float2 uv, float _Glow, float3 _GlowColor, float _GlowGlobal, out float4 Out)
{
	float4 res = GlowTex(inputColor, _GlowTex, uv, _Glow, _GlowColor, _GlowGlobal);

	Out = res;
}

void GradientColor_float(float4 inputColor, float2 uv, 
	float4 _GradTopLeftCol, float4 _GradTopRightCol, 
	float4 _GradBotLeftCol, float4 _GradBotRightCol,
	float _GradBoostX, float _GradBoostY, float _GradBlend, float4 tilingAndOffset, out float4 Out)
{
	float4 res = GradientColor(inputColor, uv, 
		_GradTopLeftCol, _GradTopRightCol, 
		_GradBotLeftCol, _GradBotRightCol,
		_GradBoostX, _GradBoostY, 
		_GradBlend, tilingAndOffset);

	Out = res;
}

void GradientColorRadial_float(float4 inputColor, float2 uv, 
	float4 _GradTopLeftCol, float4 _GradBotLeftCol, 
	float _GradBoostX, float _GradBlend, float4 tilingAndOffset, float4 texelSize, out float4 Out)
{
	float4 res = GradientColorRadial(inputColor, uv, 
	_GradTopLeftCol, _GradBotLeftCol, 
	_GradBoostX, _GradBlend, 
	tilingAndOffset, texelSize);

	Out = res;
}

void NegativeColor_float(float4 inputColor, float _NegativeAmount, out float4 Out)
{
	float4 res = NegativeColor(inputColor, _NegativeAmount);
	
	Out = res;
}

void ChromaticAberration_float(float4 inputColor, float2 uv, TEX_PARAM tex, float _AberrationAmount, float _AberrationAlpha, out float4 Out)
{
	float4 res = ChromaticAberration(inputColor, uv, tex, _AberrationAmount, _AberrationAlpha);
	Out = res;
}

void Shine_float(float4 inputColor, float2 uv, 
	float4 _ShineColor, float _ShineLocation, float _ShineRotate, 
	float _ShineWidth, float _ShineGlow, TEX_PARAM _ShineMask, out float4 Out)
{
	float4 res = Shine(inputColor, uv, _ShineColor, _ShineLocation, _ShineRotate, _ShineWidth, _ShineGlow, _ShineMask);
	Out = res;
}

void SpriteOutline_float(float4 inputColor, TEX_PARAM tex, float4 texelSize, 
	float4 _OutlineColor, float _OutlineWidth, float _OutlineAlpha, float _OutlineGlow, 
	float2 uv, out float4 Out)
{
	float4 res = SpriteOutline(inputColor, tex, texelSize, _OutlineColor, _OutlineWidth, _OutlineAlpha, _OutlineGlow, uv);
	Out = res;
}

void Luminosity_float(float4 col, out float Out)
{
	float res = Luminosity(col);
	Out = res;
}

#endif //ALLIN1NODELIBRARY_COLOREFFECTS_SG_INCLUDED