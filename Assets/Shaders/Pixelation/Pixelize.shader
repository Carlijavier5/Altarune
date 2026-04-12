

Shader "Hidden/Pixelize"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "Pixelation"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            SamplerState sampler_point_clamp;

            uniform float2 _BlockCount;
            uniform float2 _BlockSize;
            uniform float2 _HalfBlockSize;

            half4 frag(Varyings IN) : SV_TARGET
            {
                float2 blockPos = floor(IN.texcoord * _BlockCount);
                float2 blockCenter = blockPos * _BlockSize + _HalfBlockSize;
                return SAMPLE_TEXTURE2D(_BlitTexture, sampler_point_clamp, blockCenter);
            }
            ENDHLSL
        }
    }
}