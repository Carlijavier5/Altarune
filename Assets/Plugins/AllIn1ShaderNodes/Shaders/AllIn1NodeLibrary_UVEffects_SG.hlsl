#ifndef ALLIN1NODELIBRARY_UVEFFECTS_SG_INCLUDED
#define ALLIN1NODELIBRARY_UVEFFECTS_SG_INCLUDED

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_UVEffects.hlsl"

void ScrollTexture_float(float2 inputUV, float shaderTime, float _ScrollTextureX, float _ScrollTextureY, out float2 Out)
{
	float2 res = ScrollTexture(inputUV, shaderTime, _ScrollTextureX, _ScrollTextureY);
	Out = res;
}

void WaveUV_float(float2 inputUV, float shaderTime, float _WaveX, float2 _Tiling, float _WaveY, float _WaveAmount, float _WaveSpeed, float _WaveStrength, out float2 Out)
{
	float2 res = WaveUV(inputUV, shaderTime, _WaveX, _Tiling, _WaveY, _WaveAmount, _WaveSpeed, _WaveStrength);
	Out = res;
}

void HandDrawn_float(float2 inputUV, float shaderTime, float _HandDrawnSpeed, float _HandDrawnAmount, out float2 Out)
{
	float2 res = HandDrawn(inputUV, shaderTime, _HandDrawnSpeed, _HandDrawnAmount);
	Out = res;
}

void UVDistortion_float(float2 inputUV, float shaderTime, float _DistortTexXSpeed, float _DistortTexYSpeed, float _DistortAmount, UnityTexture2D _DistortTex, out float2 Out)
{
	float2 res = UVDistortion(inputUV, shaderTime, _DistortTexXSpeed, _DistortTexYSpeed, _DistortAmount, _DistortTex);
	
	Out = res;
}

void Pixelate_float(float2 inputUV, float _PixelateSize, float4 texelSize, out float2 Out) 
{
	float2 res = Pixelate(inputUV, _PixelateSize, texelSize);
	Out = res;
}

void ScreenSpaceUV_float(float2 inputUV, float3 vertexWS, float4 projPos, float _ScaleWithCameraDistance, out float2 Out)
{
	float2 res = ScreenSpaceUV(inputUV, vertexWS, projPos, _ScaleWithCameraDistance);
	Out = res;
}

void StochasticSampling_float(float2 inputUV, UnityTexture2D tex, float _StochasticScale, float _StochasticSkew, out float3 Out)
{
	float3 res = StochasticSampling(inputUV, tex, _StochasticScale, _StochasticSkew);
	Out = res;
}

void TwistUV_float(float2 inputUV, float _TwistUvAmount, float _TwistUvPosX, float _TwistUvPosY, float _TwistUvRadius, float4 tilingAndOffset, out float2 Out)
{
	float2 res = TwistUV(inputUV, _TwistUvAmount, _TwistUvPosX, _TwistUvPosY, _TwistUvRadius, tilingAndOffset);
	Out = res;
}

void ZoomUV_float(float2 inputUV, float _ZoomUvAmount, float4 tilingAndOffset, out float2 Out)
{
	float2 res = ZoomUV(inputUV, _ZoomUvAmount, tilingAndOffset);
	Out = res;
}

void FisheyeUV_float(float2 inputUV, float _FishEyeUvAmount, float4 tilingAndOffset, out float2 Out)
{
	float2 res = FisheyeUV(inputUV, _FishEyeUvAmount, tilingAndOffset);
	Out = res;
}

void PinchUV_float(float2 inputUV, float _PinchAmount, float4 tilingAndOffset, out float2 Out)
{
	float2 res = PinchUV(inputUV, _PinchAmount, tilingAndOffset);
	Out = res;
}

void WindUV_float(float2 inputUV, float _GrassSpeed, float _GrassWind, float _GrassRadialBend, float shaderTime, out float2 Out)
{
	float2 res = WindUV(inputUV, _GrassSpeed, _GrassWind, _GrassRadialBend, shaderTime);
	Out = res;
}

void RoundWaveUV_float(float2 inputUV, float _RoundWaveStrength, float _RoundWaveSpeed, float4 tilingAndOffset, float4 texelSize, float shaderTime, out float2 Out)
{
	float2 res = RoundWaveUV(inputUV, _RoundWaveStrength, _RoundWaveSpeed, tilingAndOffset, texelSize, shaderTime);
	Out = res;
}

#endif //ALLIN1NODELIBRARY_UVEFFECTS_SG_INCLUDED