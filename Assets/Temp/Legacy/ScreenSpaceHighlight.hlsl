#ifndef CF_SCREENSPACE_HIGHLIGHT_INCLUDED
#define CF_SCREENSPACE_HIGHLIGHT_INCLUDED

inline float ComputeHighlightImpl(float4 clipPos,
                                  float2 screenSize,
                                  float center01,
                                  float widthPx,
                                  float featherPx,
                                  float intensity)
{
    float2 uv = (clipPos.xy / max(1e-6, clipPos.w)) * 0.5 + 0.5;
    float dxPx = abs((uv.x - center01) * screenSize.x);

    float halfW = max(widthPx * 0.5, 0.0);
    float feather = max(featherPx, 1e-3);

    float t = 1.0 - smoothstep(halfW, halfW + feather, dxPx);
    t *= saturate(intensity);
    return saturate(t);
}

inline void ComputeHighlight_float(float4 clipPos,
                                   float2 screenSize,
                                   float center01,
                                   float widthPx,
                                   float featherPx,
                                   float intensity,
                                   out float t)
{
    t = ComputeHighlightImpl(clipPos, screenSize, center01, widthPx, featherPx, intensity);
}

inline void ComputeHighlight_half(half4 clipPos,
                                  half2 screenSize,
                                  half center01,
                                  half widthPx,
                                  half featherPx,
                                  half intensity,
                                  out half t)
{
    t = (half) ComputeHighlightImpl((float4) clipPos, (float2) screenSize,
                                   (float) center01, (float) widthPx, (float) featherPx, (float) intensity);
}

#endif