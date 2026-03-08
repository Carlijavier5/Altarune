#ifndef ALLINNODESLIBRARY_SPRITES
#define ALLINNODESLIBRARY_SPRITES

float3 GetSpriteData(float2 uv, out float spriteAlpha)
{
	float4 sampleTex	= SAMPLE_TEX2D(_MainTex, uv);
	float3 spriteRGB	= sampleTex.rgb;
	spriteAlpha			= sampleTex.a;
	
	return spriteRGB;
}

#endif //ALLINNODESLIBRARY_SPRITES