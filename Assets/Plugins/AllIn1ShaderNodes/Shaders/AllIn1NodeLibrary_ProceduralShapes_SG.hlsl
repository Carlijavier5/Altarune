#ifndef ALLIN1NODELIBRARY_PROCEDURALSHAPES_SG_INCLUDED
#define ALLIN1NODELIBRARY_PROCEDURALSHAPES_SG_INCLUDED

#include "./AllIn1Node_SG_Helper.hlsl"
#include "./AllIn1NodeLibrary_ProceduralShapes.hlsl"

void SDFCircle_float(float2 uv, float size, float cutOff, float2 center, out float Out)
{
	float res = SDFCircle(uv, size, cutOff, center);
	Out = res;
}

void SDFSquare_float(float2 uv, float sizeX, float sizeY, float edgeMin, float edgeMax, float cutOff, float2 center, out float Out)
{
	float res = SDFSquare(uv, sizeX, sizeY, edgeMin, edgeMax, cutOff, center);
	Out = res;
}

void ProceduralGrid_float(float2 uv, float tileX, float tileY, float2 widthHeight, out float2 Out)
{
	float2 res = ProceduralGrid(uv, tileX, tileY, widthHeight);
	Out = res;
}

void RadialPatternWedges_float(float2 uv, float segments, float rotationSlider, float spacing, float blur, out float Out)
{
	float res = RadialPatternWedges(uv, segments, rotationSlider, spacing, blur);
	Out = res;
}

void RadialPatternLines_float(float2 uv, float segments, float rotationSlider, float spacing, float blur, out float Out)
{
	float res = RadialPatternLines(uv, segments, rotationSlider, spacing, blur);
	Out = res;
}

void CircleGradient_float(float2 uv, float size, float2 center, float smooth, out float Out)
{
	float res = CircleGradient(uv, size, center, smooth);
	Out = res;
}

void SquareGradient_float(float2 uv, float sizeX, float sizeY, float edgeMin, float edgeMax, float2 center, float smooth, out float Out)
{
	float res = SquareGradient(uv, sizeX, sizeY, edgeMin, edgeMax, center, smooth);
	Out = res;
}

//void DebugMethod_float(float2 uv, float size, float edgeMin, float edgeMax, out float2 Out)
//{
//	float2 res = Debug(uv, size, edgeMin, edgeMax);
//	Out = res;
//}

#endif //ALLIN1NODELIBRARY_PROCEDURALSHAPES_SG_INCLUDED