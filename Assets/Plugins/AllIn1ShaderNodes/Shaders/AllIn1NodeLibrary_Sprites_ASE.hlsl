#ifndef ALLINNODESLIBRARY_SRITES_ASE
#define ALLINNODESLIBRARY_SRITES_ASE

#include "./AllIn1NodeLibrary_Sprites.hlsl"

float3 GetSpriteData_ASE(float2 uv, out float spriteAlpha)
{
	float3 res = GetSpriteData(uv, spriteAlpha);
	return res;
}

#endif //ALLINNODESLIBRARY_SRITES_ASE