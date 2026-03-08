#ifndef ALLIN1NODELIBRARY_VERTEXEFFECTS_SG_INCLUDED
#define ALLIN1NODELIBRARY_VERTEXEFFECTS_SG_INCLUDED

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_VertexEffects.hlsl"

void VertexShake_float(float3 vertexPos, float _ShakeSpeedMult, float4 _ShakeSpeed, float4 _ShakeMaxDisplacement, float _ShakeBlend, float shaderTime, out float3 Out)
{
	float3 res = VertexShake(vertexPos, _ShakeSpeedMult, _ShakeSpeed, _ShakeMaxDisplacement, _ShakeBlend, shaderTime);
	Out = res;
}

void VertexInflate_float(float3 vertexPos, float3 normalOS, float _MinInflate, float _MaxInflate, float _InflateBlend, out float3 Out)
{
	float3 res = VertexInflate(vertexPos, normalOS, _MinInflate, _MaxInflate, _InflateBlend);
	Out = res;
}

void VertexDistortion_float(float3 vertexPos, float3 normalOS, float2 _VertexDistortionNoiseSpeed, float _VertexDistortionAmount, 
	UnityTexture2D _VertexDistortionNoiseTex, float4 _TilingAndOffset, float shaderTime, out float3 Out)
{
	float3 res = VertexDistortion(vertexPos, normalOS, _VertexDistortionNoiseSpeed, _VertexDistortionAmount, _VertexDistortionNoiseTex, _TilingAndOffset, shaderTime);
	Out = res;
}

void Glitch_float(float3 vertexPos, float4 _GlitchOffset, float _GlitchWorldSpace, float _GlitchSpeed, float _GlitchTiling, float _GlitchAmount, float shaderTime, out float3 Out)
{
	float3 res = Glitch(vertexPos, _GlitchOffset, _GlitchWorldSpace, _GlitchSpeed, _GlitchTiling, _GlitchAmount, shaderTime);
	Out = res;
}

void VertexVoxel_float(float3 vertexPos, float _VoxelSize, float _VoxelBlend, out float3 Out)
{
	float3 res = VertexVoxel(vertexPos, _VoxelSize, _VoxelBlend);
	Out = res;
}

#endif //ALLIN1NODELIBRARY_VERTEXEFFECTS_SG_INCLUDED