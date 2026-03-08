#ifndef ALLIN1NODELIBRARY_UVEFFECTS_INCLUDED
#define ALLIN1NODELIBRARY_UVEFFECTS_INCLUDED

#define DECLARE_CENTER_TILED \
	half2 center = half2(0.5, 0.5); \
	half2 centerTiled = half2(center.x *  tilingAndOffset.x, center.y *  tilingAndOffset.y); \

float3 StochasticSampling(float2 uv, TEX_PARAM tex, float _StochasticScale, float _StochasticSkew)
{
	float4x3 stochasticOffsets = getStochasticOffsets(uv, _StochasticScale, _StochasticSkew);
	
	float2 dx = 0;
	float2 dy = 0;

	dx = ddx(uv);
	dy = ddy(uv);
	float3 res = mul(SAMPLE_TEX2D_DERIVATIVES(tex, uv + hash2D2D(stochasticOffsets[0].xy), dx, dy), stochasticOffsets[3].x).xyz +
		mul(SAMPLE_TEX2D_DERIVATIVES(tex, uv + hash2D2D(stochasticOffsets[1].xy), dx, dy), stochasticOffsets[3].y).xyz +
		mul(SAMPLE_TEX2D_DERIVATIVES(tex, uv + hash2D2D(stochasticOffsets[2].xy), dx, dy), stochasticOffsets[3].z).xyz;

	return res;
}

float2 ScrollTexture(float2 inputUV, float shaderTime, float _ScrollTextureX, float _ScrollTextureY)
{
	float2 res = inputUV;
	
	res.x += frac(shaderTime * _ScrollTextureX);
	res.y += frac(shaderTime * _ScrollTextureY);
	
	return res;
}

float2 WaveUV(float2 inputUV, float shaderTime, float _WaveX, float2 _Tiling, float _WaveY, float _WaveAmount, float _WaveSpeed, float _WaveStrength)
{
	float2 res = inputUV;
	
	float2 uvWaveDiff = float2(_WaveX * _Tiling.x, _WaveY * _Tiling.y) - inputUV;
	
	uvWaveDiff.x *= _ScreenParams.x / _ScreenParams.y;
	float waveTime = shaderTime;
	float angWave = (sqrt(dot(uvWaveDiff, uvWaveDiff)) * _WaveAmount) - ((waveTime *  _WaveSpeed % 360.0));
	
	uvWaveDiff = normalize(uvWaveDiff) * sin(angWave) * (_WaveStrength / 1000.0);
	DISPLACE_ALL_UVS(res, uvWaveDiff);
	
	return res;
}

float2 HandDrawn(float2 inputUV, float shaderTime, float _HandDrawnSpeed, float _HandDrawnAmount)
{
	float2 uvCopy = inputUV;
	float2 res = inputUV;
	
	float drawnSpeed = (floor(frac(shaderTime) * 20 * _HandDrawnSpeed) / _HandDrawnSpeed) * _HandDrawnSpeed;
	uvCopy.x = sin((uvCopy.x * _HandDrawnAmount + drawnSpeed) * 4);
	uvCopy.y = cos((uvCopy.y * _HandDrawnAmount + drawnSpeed) * 4);
	res = lerp(res, res + uvCopy, 0.0005 * _HandDrawnAmount);
	
	return res;
}

float2 UVDistortion(float2 inputUV, float shaderTime, float _DistortTexXSpeed, float _DistortTexYSpeed, float _DistortAmount, TEX_PARAM _DistortTex)
{
	float2 res = inputUV;
	
	float2 distortTexUV = inputUV;
	
	distortTexUV.x += frac((shaderTime) * _DistortTexXSpeed);
	distortTexUV.y += frac((shaderTime) * _DistortTexYSpeed);
	
	float4 distortTexCol = SAMPLE_TEX2D_LOD(_DistortTex, float4(distortTexUV.x, distortTexUV.y, 0, 0));
	float distortAmnt = (distortTexCol.r - 0.5) * 0.2 * _DistortAmount;
	
	DISPLACE_ALL_UVS(res, distortAmnt);
	
	return res;
}

float2 Pixelate(float2 inputUV, float _PixelateSize, float4 texelSize)
{
	float2 res = inputUV;
	
	float aspectRatio = texelSize.x / texelSize.y;
	float2 pixelSize = float2(_PixelateSize, _PixelateSize * aspectRatio);

	res = floor(res * pixelSize) / pixelSize;
	
	return res;
}

float2 ScreenSpaceUV(float2 inputUV, float3 vertexWS, float4 projPos, float _ScaleWithCameraDistance)
{
	float2 res = inputUV;

	float aspect = _ScreenParams.x / _ScreenParams.y;
	
	float4 pivotCS = OBJECT_TO_CLIP_SPACE_FLOAT4(float4(0, 0, 0, 1)); 
	pivotCS.xy /= pivotCS.w;
	pivotCS.y *= -1;
	pivotCS.xy += 1.0;
	pivotCS.xy *= 0.5;
	
	float3 positionVS = mul(UNITY_MATRIX_V, float4(vertexWS, 1.0)).xyz;
	
	float2 screenUV = (projPos.xy / projPos.w);
	screenUV -= 0.5;
	screenUV.x *= aspect;
	
	
	float2 screenUVMinusPivot = screenUV - pivotCS.xy;
	float2 stableUVs = (screenUV - pivotCS.xy + 0.5) * -positionVS.z;
	stableUVs *= 0.1;
	
	res = lerp(screenUV, stableUVs, _ScaleWithCameraDistance);
	return res;
}

float2 TwistUV(float2 inputUV, float _TwistUvAmount, float _TwistUvPosX, float _TwistUvPosY, float _TwistUvRadius, float4 tilingAndOffset)
{
	float2 tempUv = inputUV - float2(_TwistUvPosX *  tilingAndOffset.x, _TwistUvPosY *  tilingAndOffset.y);
	_TwistUvRadius *= (tilingAndOffset.x + tilingAndOffset.y) / 2;
	float percent = (_TwistUvRadius - length(tempUv)) / _TwistUvRadius;
	float theta = percent * percent * (2.0 * sin(_TwistUvAmount)) * 8.0;
	float s = sin(theta);
	float c = cos(theta);
	float beta = max(sign(_TwistUvRadius - length(tempUv)), 0.0);
	tempUv = float2(dot(tempUv, float2(c, -s)), dot(tempUv, float2(s, c))) * beta +	tempUv * (1 - beta);
	tempUv += float2(_TwistUvPosX *  tilingAndOffset.x, _TwistUvPosY *  tilingAndOffset.y);
	
	float2 res = tempUv;
	return res;
}

float2 ZoomUV(float2 inputUV, float _ZoomUvAmount, float4 tilingAndOffset)
{
	float2 res = inputUV;

	DECLARE_CENTER_TILED

	res -= centerTiled;
	res = res * _ZoomUvAmount;
	res += centerTiled;

	return res;
}

float2 FisheyeUV(float2 inputUV, float _FishEyeUvAmount, float4 tilingAndOffset)
{
	DECLARE_CENTER_TILED

	float bind = length(centerTiled);
	float2 dF = inputUV - centerTiled;
	float dFlen = length(dF);
	float fishInt = (3.14159265359 / bind) * (_FishEyeUvAmount + 0.001);
	
	float2 res = centerTiled + (dF / (max(0.0001, dFlen))) * tan(dFlen * fishInt) * bind / tan(bind * fishInt);
	return res;
}

float2 PinchUV(float2 inputUV, float _PinchAmount, float4 tilingAndOffset)
{
	DECLARE_CENTER_TILED

	float2 dP = inputUV - centerTiled;
	float pinchInt = (3.141592 / length(centerTiled)) * (-_PinchAmount + 0.001);
	
	float2 res = centerTiled + normalize(dP) * atan(length(dP) * -pinchInt * 10.0) * 0.5 / atan(-pinchInt * 5);
	return res;
}

float2 WindUV(float2 inputUV, float _GrassSpeed, float _GrassWind, float _GrassRadialBend, float shaderTime)
{
	float2 uvRect = inputUV;

	float windOffset = sin(shaderTime * _GrassSpeed * 10);
	float2 windCenter = float2(0.5, 0.1);
	
	float2 res = inputUV;
	res.x = fmod(abs(lerp(res.x, res.x + (_GrassWind * 0.01 * windOffset), uvRect.y)), 1);
	
	float2 delta = res - windCenter;
	float delta2 = dot(delta.xy, delta.xy);
	float2 delta_offset = delta2 * windOffset;
	res = res + float2(delta.y, -delta.x) * delta_offset * _GrassRadialBend;

	return res;
}

float2 RoundWaveUV(float2 inputUV, float _RoundWaveStrength, float _RoundWaveSpeed, float4 tilingAndOffset, float4 texelSize, float shaderTime)
{
	float2 uvRect = inputUV;

	half xWave = ((0.5 * tilingAndOffset.x) - uvRect.x);
	half yWave = ((0.5 * tilingAndOffset.y) - uvRect.y) * (texelSize.w / texelSize.z);
	half ripple = -sqrt(xWave*xWave + yWave* yWave);

	float2 res = inputUV;
	res += (sin((ripple + shaderTime * (_RoundWaveSpeed/10.0)) / 0.015) * (_RoundWaveStrength/10.0)) % 1;

	return res;
}

#endif //ALLIN1NODELIBRARY_UVEFFECTS_INCLUDED