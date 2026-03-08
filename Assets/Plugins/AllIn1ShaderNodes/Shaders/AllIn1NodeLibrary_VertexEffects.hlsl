#ifndef ALLIN1NODELIBRARY_VERTEXEFFECTS_INCLUDED
#define ALLIN1NODELIBRARY_VERTEXEFFECTS_INCLUDED

float3 VertexShake(float3 vertexPos, float _ShakeSpeedMult, float4 _ShakeSpeed, float4 _ShakeMaxDisplacement, float _ShakeBlend, float shaderTime)
{
	float3 res = vertexPos;
	
	float3 speedOffset = float3(1.0f, 1.13f, 1.07f) * _ShakeSpeedMult;
	float3 displacement = sin(shaderTime * _ShakeSpeed.xyz * speedOffset) * _ShakeMaxDisplacement.xyz;
	displacement *= _ShakeBlend;
	
	res += displacement;
	
	return res;
}

float3 VertexInflate(float3 vertexPos, float3 normalOS, float _MinInflate, float _MaxInflate, float _InflateBlend)
{
	float3 res = vertexPos;
	
	float inflateValue = lerp(_MinInflate, _MaxInflate, _InflateBlend);
	res += normalOS * inflateValue;
	
	return res;
}

float3 VertexDistortion(float3 vertexPos, float3 normalOS, float2 _VertexDistortionNoiseSpeed, float _VertexDistortionAmount, TEX_PARAM _VertexDistortionNoiseTex, float4 _TilingAndOffset, float shaderTime)
{
	float3 res = vertexPos;
	
	float noisePower = 1.0;
	
	float2 noiseUV = vertexPos.xy * _TilingAndOffset.xy + _TilingAndOffset.zw;
	float4 correctedNoiseUV = float4(noiseUV.x, noiseUV.y, 0, 0);
	
	correctedNoiseUV.x += frac(shaderTime * _VertexDistortionNoiseSpeed.x);
	correctedNoiseUV.y += frac(shaderTime * _VertexDistortionNoiseSpeed.y);
	
	noisePower = SAMPLE_TEX2D_LOD(_VertexDistortionNoiseTex, correctedNoiseUV).r;
	
	res += normalOS * noisePower * _VertexDistortionAmount;
	
	return res;
}

float3 Glitch(float3 vertexPos, float4 _GlitchOffset, float _GlitchWorldSpace, float _GlitchSpeed, float _GlitchTiling, float _GlitchAmount, float shaderTime)
{
	float3 res = vertexPos;
	
	float3 glitchDir = mul(WORLD_TO_OBJECT_MATRIX, float4(_GlitchOffset.xyz, 0)).xyz;
	float3 scale = float3(length(UNITY_MATRIX_M[0].xyz),
	length(UNITY_MATRIX_M[1].xyz),
	length(UNITY_MATRIX_M[2].xyz));
	float pos = _GlitchWorldSpace ? mul(UNITY_MATRIX_M, float4(vertexPos, 1.0)).y : vertexPos.y;
	float time = shaderTime * _GlitchSpeed;
	
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

float3 VertexVoxel(float3 vertexPos, float _VoxelSize, float _VoxelBlend)
{
	float3 voxelizedPosition = round(vertexPos * _VoxelSize) / _VoxelSize;
	float3 res = lerp(vertexPos, voxelizedPosition, _VoxelBlend);
	
	return res;
}

#endif //ALLIN1NODELIBRARY_VERTEXEFFECTS_INCLUDED