#ifndef ALLIN1NODELIBRARY_PROCEDURALSHAPES_INCLUDED
#define ALLIN1NODELIBRARY_PROCEDURALSHAPES_INCLUDED

#define UV_TO_SIGNED(uv) (uv * 2) - 1.0


float2 MirrorUV(float2 uv, float subdivisions, float height)
{
	float2 scaledUV = uv * subdivisions;
	float2 scaledUVFloor = floor(scaledUV);

	float2 diff = scaledUV - scaledUVFloor;

	float2 res = min(1 - diff, diff) * height;
	return res;
}

float2 ScaleUV(float2 uv, float2 scale, float2 pivotPoint)
{
	float2 correctedScale = 1 / max(scale, float2(0.001, 0.001));
	float2 res = (uv - pivotPoint) * correctedScale;
	res += pivotPoint;

	return res;
}

float AspectRatio(float2 widthHeight)
{
	float horizontalAspect = widthHeight.x / widthHeight.y;
	float verticalAspect = widthHeight.y / widthHeight.x;

	float res = max(horizontalAspect, verticalAspect);
	return res;
}

float HorizontalGradient(float2 uv)
{
	float res = uv.x;
	return res;
}

float VerticalGradient(float2 uv)
{
	float res = uv.y;
	return res;
}

float AntiAliasing(float gradient, float cutOff)
{
	cutOff = max(0.01, cutOff);
	float gradientCutOff = cutOff - gradient;

	float2 ddxVector = float2(ddx(gradientCutOff), ddy(gradientCutOff));

	float res = gradientCutOff / length(ddxVector);

	res = saturate(0.5 - res);

	return res;
}

float SDFCircleBase(float2 uv, float size, float2 center)
{
	float2 signedUV = UV_TO_SIGNED(uv);
	float res = size - distance(signedUV, center);

	return res;
}

float SDFCircle(float2 uv, float size, float cutOff, float2 center)
{
	float res = SDFCircleBase(uv, size, center);
	res = AntiAliasing(res, cutOff);

	return res;
}

float CircleGradient(float2 uv, float size, float2 center, float smooth)
{
	float res = SDFCircleBase(uv, size, center);
	res = saturate(res);

	res = smoothstep(0, max(0.0, smooth), res);
	return res;
}


float SDFSquareBase(float2 uv, float sizeX, float sizeY, float edgeMin, float edgeMax, float2 center)
{
	float2 rawUVSDF = abs(UV_TO_SIGNED(uv) - center) - float2(sizeX, sizeY);
	
	float outsideDistance = length(max(rawUVSDF, float2(0, 0)));
	float insideDistance = min(max(rawUVSDF.x, rawUVSDF.y), 0);

	float res = outsideDistance + insideDistance;
	
	res = INVERSE_LERP(edgeMin, edgeMax, res);
	res = 1 - saturate(res);
	
	return res;
}

float SDFSquare(float2 uv, float sizeX, float sizeY, float edgeMin, float edgeMax, float cutOff, float2 center)
{
	float res = SDFSquareBase(uv, sizeX, sizeY, edgeMin, edgeMax, center);
	res = AntiAliasing(res, cutOff);

	return res;
}

float SquareGradient(float2 uv, float sizeX, float sizeY, float edgeMin, float edgeMax, float2 center, float smooth)
{
	float res = SDFSquareBase(uv, sizeX, sizeY, edgeMin, edgeMax, center);
	res = smoothstep(0, max(0.0, smooth), res);

	return res;
}

float2 ProceduralGrid(float2 uv, float tileX, float tileY, float2 widthHeight)
{
	float horizontlTileAspect = tileX / tileY;
	float verticalTileAspect = tileY / tileX;

	float2 tileApsects = saturate(float2(horizontlTileAspect, verticalTileAspect));
	
	float globalAspect = AspectRatio(widthHeight);

	float2 scaledUV = uv * float2(tileX, tileY);
	float2 fracUV = frac(scaledUV);

	float2 finalScale = tileApsects / globalAspect;

	float2 res = ScaleUV(fracUV, finalScale, float2(0.5, 0.5));

	return res;
}

float ApplySpacingAndBlurToRadialPattern(float radialPattern, float spacing, float blur)
{
	float res = radialPattern;

	blur = max(0.001, blur);
	res = INVERSE_LERP(spacing, spacing + blur, res);
	res = saturate(res);

	res = AntiAliasing(res, 0.2);

	return res;
}

float BaseRadialPattern(float2 polarUV, float segments, float rotationSlider)
{
	float polarGardient = polarUV.y + 0.5;

	float gradient = frac(polarGardient + rotationSlider);
	
	float segmentFloor = floor(segments);
	float res = MirrorUV((gradient * segmentFloor), 1, 1).x;

	return res;
}

float RadialPatternWedges(float2 uv, float segments, float rotationSlider, float spacing, float blur)
{
	float2 polarUV = PolarUV(uv);
	
	float res = BaseRadialPattern(polarUV, segments, rotationSlider);
	res = ApplySpacingAndBlurToRadialPattern(res, spacing, blur);

	return res;
}

float RadialPatternLines(float2 uv, float segments, float rotationSlider, float spacing, float blur)
{
	float2 polarUV = PolarUV(uv);
	
	float res = BaseRadialPattern(polarUV, segments, rotationSlider);

	res *= polarUV.x;
	res = saturate(res);
	res *= 2;
	res = 1 - res;

	res = ApplySpacingAndBlurToRadialPattern(res, spacing, blur);

	return res;
}

//float2 Debug(float2 uv, float size, float edgeMin, float edgeMax)
//{
//	float2 center = float2(0.2, 0.2);
//	float2 rawUVSDF = abs(UV_TO_SIGNED(uv) + center) - size - center;
	
//	float outsideDistance = length(max(rawUVSDF, float2(0, 0)));
//	float insideDistance = min(max(rawUVSDF.x, rawUVSDF.y), 0);

//	float res = outsideDistance + insideDistance;
	
//	res = INVERSE_LERP(edgeMin, edgeMax, res);
//	res = 1 - saturate(res);

//	res = AntiAliasing(res, 0.2);

//	//return rawUVSDF;

//	return res;
//}

#endif //ALLIN1NODELIBRARY_PROCEDURALSHAPES_INCLUDED