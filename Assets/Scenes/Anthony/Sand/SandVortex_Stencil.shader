Shader "Shader Graphs/SandVortex"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _SandMovement("SandMovement", Vector) = (0.5, -1, 0, 0)
        [NoScaleOffset]_HeightTex("HeightTex", 2D) = "white" {}
        _Height("Height", Float) = 1
        _Tiling("Tiling", Vector) = (1, 2, 0, 0)
        [HDR]_ColorDark("ColorDark", Color) = (0.8553459, 0.5471174, 0.2662868, 1)
        [HDR]_ColorBright("ColorBright", Color) = (0.8553459, 0.5471174, 0.2662868, 1)
        _OpacityFalloff("OpacityFalloff", Range(0.01, 2)) = 1.27
        _DepthFade("DepthFade", Float) = 1
        _VortexColorDark("VortexColorDark", Color) = (0, 0, 0, 1)
        [HDR]_VortexColorLight("VortexColorLight", Color) = (1, 1, 1, 1)
        _Hole("Hole", Range(0, 1)) = 0
        _HoleSmoothness("HoleSmoothness", Range(0, 0.1)) = 0.05
        _DitherPower("DitherPower", Float) = 1
        _HoleColor("HoleColor", Color) = (0.1509434, 0.1409754, 0.1409754, 0)
        _Depth("Depth", Float) = 2
        _HoleFlatness("HoleFlatness", Range(0, 2)) = 1
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="AlphaTest"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        
        Stencil
        {
            Ref 6
            Comp Always
            Pass Replace
        }
        
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On
        AlphaToMask On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 TangentSpaceViewDirection;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float4 uv0;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float2 _SandMovement;
        float4 _HeightTex_TexelSize;
        float _Height;
        float2 _Tiling;
        float4 _ColorBright;
        float4 _ColorDark;
        float _OpacityFalloff;
        float _DepthFade;
        float4 _VortexColorLight;
        float4 _VortexColorDark;
        float _Hole;
        float _DitherPower;
        float _HoleSmoothness;
        float4 _HoleColor;
        float _Depth;
        float _HoleFlatness;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_HeightTex);
        SAMPLER(sampler_HeightTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDX' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddx(In);
        }
        
        void Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddy(In);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_af823673df7b48f989d31c60d916c461_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_af823673df7b48f989d31c60d916c461_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_af823673df7b48f989d31c60d916c461_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_af823673df7b48f989d31c60d916c461_A_4_Float = 0;
            float2 _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2);
            float _Split_24aad51159224185ac48e810ac524d08_R_1_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[0];
            float _Split_24aad51159224185ac48e810ac524d08_G_2_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[1];
            float _Split_24aad51159224185ac48e810ac524d08_B_3_Float = 0;
            float _Split_24aad51159224185ac48e810ac524d08_A_4_Float = 0;
            float _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float = _HoleFlatness;
            float _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float;
            Unity_Divide_float(_Split_24aad51159224185ac48e810ac524d08_R_1_Float, _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float, _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float);
            float _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float;
            Unity_OneMinus_float(_Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float, _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float);
            float _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float, _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float);
            float _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float;
            Unity_Power_float(_Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float, 2, _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float);
            float _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float = _Depth;
            float _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float;
            Unity_Multiply_float_float(_Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float, _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float);
            float _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float;
            Unity_Subtract_float(_Split_af823673df7b48f989d31c60d916c461_G_2_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float);
            float3 _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3 = float3(_Split_af823673df7b48f989d31c60d916c461_R_1_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float, _Split_af823673df7b48f989d31c60d916c461_B_3_Float);
            description.Position = _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ab38fb67a7224a1da53421f435ad3fa9_Out_0_Vector4 = _HoleColor;
            float4 _Property_d88d0e8a1d7a47acb18cc3e80e1b3256_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_ColorDark) : _ColorDark;
            float4 _Property_a55b2288f366462dab98f1b10e486c40_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_ColorBright) : _ColorBright;
            UnityTexture2D _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            UnityTexture2D _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HeightTex);
            float _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float = _Height;
            float4 _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4 = IN.uv0;
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[0];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[1];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_B_3_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[2];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_A_4_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[3];
            float4 _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4;
            float3 _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3;
            float2 _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2;
            Unity_Combine_float(_Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float, _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float, 0, 0, _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4, _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3, _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2);
            float2 _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2, float2(2, 2), _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2);
            float2 _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2, float2(1, 1), _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2);
            float2 _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2 = _Tiling;
            float _Split_3478904ad028424cb72b998728199740_R_1_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[0];
            float _Split_3478904ad028424cb72b998728199740_G_2_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[1];
            float _Split_3478904ad028424cb72b998728199740_B_3_Float = 0;
            float _Split_3478904ad028424cb72b998728199740_A_4_Float = 0;
            float _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float;
            Unity_Length_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float);
            float _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float;
            Unity_Lerp_float(_Split_3478904ad028424cb72b998728199740_R_1_Float, _Split_3478904ad028424cb72b998728199740_G_2_Float, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float, _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float);
            float2 _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, (_Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float.xx), _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2);
            float2 _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2, float2(0.5, 0.5), _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2);
            float2 _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2;
            Unity_Add_float2(_Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2, float2(0.5, 0.5), _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2);
            float2 _ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2 = _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2) + ParallaxMappingChannel(TEXTURE2D_ARGS(_Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.tex, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.samplerstate), IN.TangentSpaceViewDirection, _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float * 0.01, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2), 0);
            float2 _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2;
            Unity_PolarCoordinates_float(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2);
            float2 _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2 = _SandMovement;
            float2 _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2);
            float2 _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2;
            Unity_Add_float2(_PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2, _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2);
            float4 _UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4 = IN.uv0;
            float4 _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4;
            Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4);
            float4 _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4;
            Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4);
            float4 _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D_GRAD(_Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.tex, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.samplerstate, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.GetTransformedUV(_Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2) , (_DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4.xy), (_DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4.xy));
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.r;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_G_5_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.g;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_B_6_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.b;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_A_7_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.a;
            float4 _Lerp_98b7676e2bf34a56b9c318a757e08680_Out_3_Vector4;
            Unity_Lerp_float4(_Property_d88d0e8a1d7a47acb18cc3e80e1b3256_Out_0_Vector4, _Property_a55b2288f366462dab98f1b10e486c40_Out_0_Vector4, (_SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float.xxxx), _Lerp_98b7676e2bf34a56b9c318a757e08680_Out_3_Vector4);
            float _Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float = _Hole;
            float _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float = _HoleSmoothness;
            float _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float;
            Unity_Subtract_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float);
            float _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float;
            Unity_Add_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float);
            float2 _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2;
            Unity_Multiply_float2_float2(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2(2, 2), _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2);
            float2 _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2, float2(1, 1), _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2);
            float _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float;
            Unity_Length_float2(_Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2, _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float);
            float _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float;
            Unity_OneMinus_float(_Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float, _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float);
            float _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float;
            Unity_Saturate_float(_OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float, _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float);
            float _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float;
            Unity_OneMinus_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float);
            float _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float);
            float _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float;
            Unity_Smoothstep_float(_Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float, _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float);
            float4 _Lerp_4262c9d56b614ed4aecbae4f78f80aef_Out_3_Vector4;
            Unity_Lerp_float4(_Property_ab38fb67a7224a1da53421f435ad3fa9_Out_0_Vector4, _Lerp_98b7676e2bf34a56b9c318a757e08680_Out_3_Vector4, (_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float.xxxx), _Lerp_4262c9d56b614ed4aecbae4f78f80aef_Out_3_Vector4);
            float _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float = _OpacityFalloff;
            float _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float;
            Unity_Power_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float, _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float);
            float _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float;
            Unity_OneMinus_float(_Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float, _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float);
            float _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float);
            float _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float);
            float _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float, _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float);
            float _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float = _DitherPower;
            float _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float, _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float, _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float);
            float _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            Unity_Dither_float(_Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float, float4(IN.NDCPosition.xy, 0, 0), _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float);
            surface.BaseColor = (_Lerp_4262c9d56b614ed4aecbae4f78f80aef_Out_3_Vector4.xyz);
            surface.Alpha = 1;
            surface.AlphaClipThreshold = _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.uv0 =                                        input.uv0;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            float3x3 tangentSpaceTransform = float3x3(output.WorldSpaceTangent, output.WorldSpaceBiTangent, output.WorldSpaceNormal);
            output.TangentSpaceViewDirection = mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask R
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 TangentSpaceViewDirection;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float4 uv0;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float2 _SandMovement;
        float4 _HeightTex_TexelSize;
        float _Height;
        float2 _Tiling;
        float4 _ColorBright;
        float4 _ColorDark;
        float _OpacityFalloff;
        float _DepthFade;
        float4 _VortexColorLight;
        float4 _VortexColorDark;
        float _Hole;
        float _DitherPower;
        float _HoleSmoothness;
        float4 _HoleColor;
        float _Depth;
        float _HoleFlatness;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_HeightTex);
        SAMPLER(sampler_HeightTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDX' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddx(In);
        }
        
        void Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddy(In);
        }
        
        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_af823673df7b48f989d31c60d916c461_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_af823673df7b48f989d31c60d916c461_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_af823673df7b48f989d31c60d916c461_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_af823673df7b48f989d31c60d916c461_A_4_Float = 0;
            float2 _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2);
            float _Split_24aad51159224185ac48e810ac524d08_R_1_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[0];
            float _Split_24aad51159224185ac48e810ac524d08_G_2_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[1];
            float _Split_24aad51159224185ac48e810ac524d08_B_3_Float = 0;
            float _Split_24aad51159224185ac48e810ac524d08_A_4_Float = 0;
            float _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float = _HoleFlatness;
            float _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float;
            Unity_Divide_float(_Split_24aad51159224185ac48e810ac524d08_R_1_Float, _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float, _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float);
            float _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float;
            Unity_OneMinus_float(_Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float, _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float);
            float _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float, _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float);
            float _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float;
            Unity_Power_float(_Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float, 2, _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float);
            float _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float = _Depth;
            float _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float;
            Unity_Multiply_float_float(_Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float, _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float);
            float _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float;
            Unity_Subtract_float(_Split_af823673df7b48f989d31c60d916c461_G_2_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float);
            float3 _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3 = float3(_Split_af823673df7b48f989d31c60d916c461_R_1_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float, _Split_af823673df7b48f989d31c60d916c461_B_3_Float);
            description.Position = _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float = _Hole;
            float _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float = _HoleSmoothness;
            float _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float;
            Unity_Subtract_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float);
            float _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float;
            Unity_Add_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float);
            UnityTexture2D _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HeightTex);
            float _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float = _Height;
            float4 _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4 = IN.uv0;
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[0];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[1];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_B_3_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[2];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_A_4_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[3];
            float4 _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4;
            float3 _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3;
            float2 _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2;
            Unity_Combine_float(_Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float, _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float, 0, 0, _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4, _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3, _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2);
            float2 _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2, float2(2, 2), _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2);
            float2 _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2, float2(1, 1), _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2);
            float2 _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2 = _Tiling;
            float _Split_3478904ad028424cb72b998728199740_R_1_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[0];
            float _Split_3478904ad028424cb72b998728199740_G_2_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[1];
            float _Split_3478904ad028424cb72b998728199740_B_3_Float = 0;
            float _Split_3478904ad028424cb72b998728199740_A_4_Float = 0;
            float _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float;
            Unity_Length_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float);
            float _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float;
            Unity_Lerp_float(_Split_3478904ad028424cb72b998728199740_R_1_Float, _Split_3478904ad028424cb72b998728199740_G_2_Float, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float, _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float);
            float2 _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, (_Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float.xx), _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2);
            float2 _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2, float2(0.5, 0.5), _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2);
            float2 _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2;
            Unity_Add_float2(_Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2, float2(0.5, 0.5), _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2);
            float2 _ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2 = _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2) + ParallaxMappingChannel(TEXTURE2D_ARGS(_Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.tex, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.samplerstate), IN.TangentSpaceViewDirection, _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float * 0.01, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2), 0);
            float2 _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2;
            Unity_Multiply_float2_float2(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2(2, 2), _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2);
            float2 _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2, float2(1, 1), _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2);
            float _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float;
            Unity_Length_float2(_Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2, _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float);
            float _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float;
            Unity_OneMinus_float(_Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float, _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float);
            float _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float;
            Unity_Saturate_float(_OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float, _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float);
            float _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float;
            Unity_OneMinus_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float);
            float _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float);
            float _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float;
            Unity_Smoothstep_float(_Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float, _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float);
            float _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float = _OpacityFalloff;
            float _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float;
            Unity_Power_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float, _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float);
            float _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float;
            Unity_OneMinus_float(_Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float, _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float);
            UnityTexture2D _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2;
            Unity_PolarCoordinates_float(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2);
            float2 _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2 = _SandMovement;
            float2 _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2);
            float2 _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2;
            Unity_Add_float2(_PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2, _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2);
            float4 _UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4 = IN.uv0;
            float4 _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4;
            Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4);
            float4 _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4;
            Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4);
            float4 _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D_GRAD(_Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.tex, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.samplerstate, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.GetTransformedUV(_Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2) , (_DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4.xy), (_DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4.xy));
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.r;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_G_5_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.g;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_B_6_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.b;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_A_7_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.a;
            float _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float);
            float _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float);
            float _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float, _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float);
            float _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float = _DitherPower;
            float _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float, _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float, _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float);
            float _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            Unity_Dither_float(_Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float, float4(IN.NDCPosition.xy, 0, 0), _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float);
            surface.Alpha = 1;
            surface.AlphaClipThreshold = _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.uv0 =                                        input.uv0;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            float3x3 tangentSpaceTransform = float3x3(output.WorldSpaceTangent, output.WorldSpaceBiTangent, output.WorldSpaceNormal);
            output.TangentSpaceViewDirection = mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 TangentSpaceViewDirection;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float4 uv0;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float2 _SandMovement;
        float4 _HeightTex_TexelSize;
        float _Height;
        float2 _Tiling;
        float4 _ColorBright;
        float4 _ColorDark;
        float _OpacityFalloff;
        float _DepthFade;
        float4 _VortexColorLight;
        float4 _VortexColorDark;
        float _Hole;
        float _DitherPower;
        float _HoleSmoothness;
        float4 _HoleColor;
        float _Depth;
        float _HoleFlatness;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_HeightTex);
        SAMPLER(sampler_HeightTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDX' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddx(In);
        }
        
        void Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddy(In);
        }
        
        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_af823673df7b48f989d31c60d916c461_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_af823673df7b48f989d31c60d916c461_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_af823673df7b48f989d31c60d916c461_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_af823673df7b48f989d31c60d916c461_A_4_Float = 0;
            float2 _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2);
            float _Split_24aad51159224185ac48e810ac524d08_R_1_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[0];
            float _Split_24aad51159224185ac48e810ac524d08_G_2_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[1];
            float _Split_24aad51159224185ac48e810ac524d08_B_3_Float = 0;
            float _Split_24aad51159224185ac48e810ac524d08_A_4_Float = 0;
            float _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float = _HoleFlatness;
            float _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float;
            Unity_Divide_float(_Split_24aad51159224185ac48e810ac524d08_R_1_Float, _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float, _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float);
            float _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float;
            Unity_OneMinus_float(_Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float, _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float);
            float _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float, _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float);
            float _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float;
            Unity_Power_float(_Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float, 2, _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float);
            float _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float = _Depth;
            float _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float;
            Unity_Multiply_float_float(_Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float, _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float);
            float _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float;
            Unity_Subtract_float(_Split_af823673df7b48f989d31c60d916c461_G_2_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float);
            float3 _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3 = float3(_Split_af823673df7b48f989d31c60d916c461_R_1_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float, _Split_af823673df7b48f989d31c60d916c461_B_3_Float);
            description.Position = _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float = _Hole;
            float _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float = _HoleSmoothness;
            float _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float;
            Unity_Subtract_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float);
            float _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float;
            Unity_Add_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float);
            UnityTexture2D _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HeightTex);
            float _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float = _Height;
            float4 _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4 = IN.uv0;
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[0];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[1];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_B_3_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[2];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_A_4_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[3];
            float4 _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4;
            float3 _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3;
            float2 _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2;
            Unity_Combine_float(_Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float, _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float, 0, 0, _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4, _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3, _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2);
            float2 _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2, float2(2, 2), _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2);
            float2 _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2, float2(1, 1), _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2);
            float2 _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2 = _Tiling;
            float _Split_3478904ad028424cb72b998728199740_R_1_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[0];
            float _Split_3478904ad028424cb72b998728199740_G_2_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[1];
            float _Split_3478904ad028424cb72b998728199740_B_3_Float = 0;
            float _Split_3478904ad028424cb72b998728199740_A_4_Float = 0;
            float _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float;
            Unity_Length_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float);
            float _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float;
            Unity_Lerp_float(_Split_3478904ad028424cb72b998728199740_R_1_Float, _Split_3478904ad028424cb72b998728199740_G_2_Float, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float, _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float);
            float2 _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, (_Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float.xx), _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2);
            float2 _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2, float2(0.5, 0.5), _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2);
            float2 _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2;
            Unity_Add_float2(_Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2, float2(0.5, 0.5), _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2);
            float2 _ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2 = _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2) + ParallaxMappingChannel(TEXTURE2D_ARGS(_Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.tex, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.samplerstate), IN.TangentSpaceViewDirection, _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float * 0.01, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2), 0);
            float2 _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2;
            Unity_Multiply_float2_float2(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2(2, 2), _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2);
            float2 _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2, float2(1, 1), _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2);
            float _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float;
            Unity_Length_float2(_Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2, _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float);
            float _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float;
            Unity_OneMinus_float(_Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float, _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float);
            float _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float;
            Unity_Saturate_float(_OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float, _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float);
            float _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float;
            Unity_OneMinus_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float);
            float _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float);
            float _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float;
            Unity_Smoothstep_float(_Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float, _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float);
            float _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float = _OpacityFalloff;
            float _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float;
            Unity_Power_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float, _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float);
            float _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float;
            Unity_OneMinus_float(_Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float, _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float);
            UnityTexture2D _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2;
            Unity_PolarCoordinates_float(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2);
            float2 _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2 = _SandMovement;
            float2 _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2);
            float2 _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2;
            Unity_Add_float2(_PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2, _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2);
            float4 _UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4 = IN.uv0;
            float4 _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4;
            Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4);
            float4 _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4;
            Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4);
            float4 _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D_GRAD(_Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.tex, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.samplerstate, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.GetTransformedUV(_Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2) , (_DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4.xy), (_DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4.xy));
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.r;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_G_5_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.g;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_B_6_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.b;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_A_7_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.a;
            float _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float);
            float _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float);
            float _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float, _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float);
            float _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float = _DitherPower;
            float _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float, _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float, _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float);
            float _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            Unity_Dither_float(_Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float, float4(IN.NDCPosition.xy, 0, 0), _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float);
            surface.Alpha = 1;
            surface.AlphaClipThreshold = _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.uv0 =                                        input.uv0;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            float3x3 tangentSpaceTransform = float3x3(output.WorldSpaceTangent, output.WorldSpaceBiTangent, output.WorldSpaceNormal);
            output.TangentSpaceViewDirection = mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile _ LOD_FADE_CROSSFADE
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 TangentSpaceViewDirection;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float4 uv0;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP0;
            #endif
             float4 tangentWS : INTERP1;
             float4 texCoord0 : INTERP2;
             float3 positionWS : INTERP3;
             float3 normalWS : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float2 _SandMovement;
        float4 _HeightTex_TexelSize;
        float _Height;
        float2 _Tiling;
        float4 _ColorBright;
        float4 _ColorDark;
        float _OpacityFalloff;
        float _DepthFade;
        float4 _VortexColorLight;
        float4 _VortexColorDark;
        float _Hole;
        float _DitherPower;
        float _HoleSmoothness;
        float4 _HoleColor;
        float _Depth;
        float _HoleFlatness;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_HeightTex);
        SAMPLER(sampler_HeightTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDX' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddx(In);
        }
        
        void Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddy(In);
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_af823673df7b48f989d31c60d916c461_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_af823673df7b48f989d31c60d916c461_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_af823673df7b48f989d31c60d916c461_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_af823673df7b48f989d31c60d916c461_A_4_Float = 0;
            float2 _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2);
            float _Split_24aad51159224185ac48e810ac524d08_R_1_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[0];
            float _Split_24aad51159224185ac48e810ac524d08_G_2_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[1];
            float _Split_24aad51159224185ac48e810ac524d08_B_3_Float = 0;
            float _Split_24aad51159224185ac48e810ac524d08_A_4_Float = 0;
            float _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float = _HoleFlatness;
            float _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float;
            Unity_Divide_float(_Split_24aad51159224185ac48e810ac524d08_R_1_Float, _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float, _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float);
            float _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float;
            Unity_OneMinus_float(_Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float, _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float);
            float _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float, _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float);
            float _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float;
            Unity_Power_float(_Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float, 2, _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float);
            float _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float = _Depth;
            float _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float;
            Unity_Multiply_float_float(_Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float, _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float);
            float _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float;
            Unity_Subtract_float(_Split_af823673df7b48f989d31c60d916c461_G_2_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float);
            float3 _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3 = float3(_Split_af823673df7b48f989d31c60d916c461_R_1_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float, _Split_af823673df7b48f989d31c60d916c461_B_3_Float);
            description.Position = _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ab38fb67a7224a1da53421f435ad3fa9_Out_0_Vector4 = _HoleColor;
            float4 _Property_d88d0e8a1d7a47acb18cc3e80e1b3256_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_ColorDark) : _ColorDark;
            float4 _Property_a55b2288f366462dab98f1b10e486c40_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_ColorBright) : _ColorBright;
            UnityTexture2D _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            UnityTexture2D _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HeightTex);
            float _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float = _Height;
            float4 _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4 = IN.uv0;
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[0];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[1];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_B_3_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[2];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_A_4_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[3];
            float4 _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4;
            float3 _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3;
            float2 _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2;
            Unity_Combine_float(_Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float, _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float, 0, 0, _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4, _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3, _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2);
            float2 _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2, float2(2, 2), _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2);
            float2 _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2, float2(1, 1), _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2);
            float2 _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2 = _Tiling;
            float _Split_3478904ad028424cb72b998728199740_R_1_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[0];
            float _Split_3478904ad028424cb72b998728199740_G_2_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[1];
            float _Split_3478904ad028424cb72b998728199740_B_3_Float = 0;
            float _Split_3478904ad028424cb72b998728199740_A_4_Float = 0;
            float _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float;
            Unity_Length_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float);
            float _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float;
            Unity_Lerp_float(_Split_3478904ad028424cb72b998728199740_R_1_Float, _Split_3478904ad028424cb72b998728199740_G_2_Float, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float, _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float);
            float2 _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, (_Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float.xx), _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2);
            float2 _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2, float2(0.5, 0.5), _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2);
            float2 _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2;
            Unity_Add_float2(_Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2, float2(0.5, 0.5), _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2);
            float2 _ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2 = _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2) + ParallaxMappingChannel(TEXTURE2D_ARGS(_Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.tex, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.samplerstate), IN.TangentSpaceViewDirection, _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float * 0.01, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2), 0);
            float2 _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2;
            Unity_PolarCoordinates_float(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2);
            float2 _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2 = _SandMovement;
            float2 _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2);
            float2 _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2;
            Unity_Add_float2(_PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2, _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2);
            float4 _UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4 = IN.uv0;
            float4 _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4;
            Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4);
            float4 _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4;
            Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4);
            float4 _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D_GRAD(_Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.tex, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.samplerstate, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.GetTransformedUV(_Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2) , (_DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4.xy), (_DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4.xy));
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.r;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_G_5_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.g;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_B_6_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.b;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_A_7_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.a;
            float4 _Lerp_98b7676e2bf34a56b9c318a757e08680_Out_3_Vector4;
            Unity_Lerp_float4(_Property_d88d0e8a1d7a47acb18cc3e80e1b3256_Out_0_Vector4, _Property_a55b2288f366462dab98f1b10e486c40_Out_0_Vector4, (_SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float.xxxx), _Lerp_98b7676e2bf34a56b9c318a757e08680_Out_3_Vector4);
            float _Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float = _Hole;
            float _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float = _HoleSmoothness;
            float _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float;
            Unity_Subtract_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float);
            float _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float;
            Unity_Add_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float);
            float2 _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2;
            Unity_Multiply_float2_float2(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2(2, 2), _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2);
            float2 _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2, float2(1, 1), _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2);
            float _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float;
            Unity_Length_float2(_Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2, _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float);
            float _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float;
            Unity_OneMinus_float(_Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float, _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float);
            float _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float;
            Unity_Saturate_float(_OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float, _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float);
            float _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float;
            Unity_OneMinus_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float);
            float _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float);
            float _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float;
            Unity_Smoothstep_float(_Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float, _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float);
            float4 _Lerp_4262c9d56b614ed4aecbae4f78f80aef_Out_3_Vector4;
            Unity_Lerp_float4(_Property_ab38fb67a7224a1da53421f435ad3fa9_Out_0_Vector4, _Lerp_98b7676e2bf34a56b9c318a757e08680_Out_3_Vector4, (_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float.xxxx), _Lerp_4262c9d56b614ed4aecbae4f78f80aef_Out_3_Vector4);
            float _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float = _OpacityFalloff;
            float _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float;
            Unity_Power_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float, _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float);
            float _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float;
            Unity_OneMinus_float(_Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float, _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float);
            float _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float);
            float _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float);
            float _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float, _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float);
            float _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float = _DitherPower;
            float _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float, _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float, _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float);
            float _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            Unity_Dither_float(_Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float, float4(IN.NDCPosition.xy, 0, 0), _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float);
            surface.BaseColor = (_Lerp_4262c9d56b614ed4aecbae4f78f80aef_Out_3_Vector4.xyz);
            surface.Alpha = 1;
            surface.AlphaClipThreshold = _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.uv0 =                                        input.uv0;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            float3x3 tangentSpaceTransform = float3x3(output.WorldSpaceTangent, output.WorldSpaceBiTangent, output.WorldSpaceNormal);
            output.TangentSpaceViewDirection = mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 TangentSpaceViewDirection;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float4 uv0;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float2 _SandMovement;
        float4 _HeightTex_TexelSize;
        float _Height;
        float2 _Tiling;
        float4 _ColorBright;
        float4 _ColorDark;
        float _OpacityFalloff;
        float _DepthFade;
        float4 _VortexColorLight;
        float4 _VortexColorDark;
        float _Hole;
        float _DitherPower;
        float _HoleSmoothness;
        float4 _HoleColor;
        float _Depth;
        float _HoleFlatness;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_HeightTex);
        SAMPLER(sampler_HeightTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDX' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddx(In);
        }
        
        void Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddy(In);
        }
        
        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_af823673df7b48f989d31c60d916c461_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_af823673df7b48f989d31c60d916c461_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_af823673df7b48f989d31c60d916c461_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_af823673df7b48f989d31c60d916c461_A_4_Float = 0;
            float2 _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2);
            float _Split_24aad51159224185ac48e810ac524d08_R_1_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[0];
            float _Split_24aad51159224185ac48e810ac524d08_G_2_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[1];
            float _Split_24aad51159224185ac48e810ac524d08_B_3_Float = 0;
            float _Split_24aad51159224185ac48e810ac524d08_A_4_Float = 0;
            float _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float = _HoleFlatness;
            float _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float;
            Unity_Divide_float(_Split_24aad51159224185ac48e810ac524d08_R_1_Float, _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float, _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float);
            float _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float;
            Unity_OneMinus_float(_Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float, _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float);
            float _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float, _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float);
            float _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float;
            Unity_Power_float(_Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float, 2, _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float);
            float _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float = _Depth;
            float _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float;
            Unity_Multiply_float_float(_Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float, _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float);
            float _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float;
            Unity_Subtract_float(_Split_af823673df7b48f989d31c60d916c461_G_2_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float);
            float3 _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3 = float3(_Split_af823673df7b48f989d31c60d916c461_R_1_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float, _Split_af823673df7b48f989d31c60d916c461_B_3_Float);
            description.Position = _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float = _Hole;
            float _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float = _HoleSmoothness;
            float _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float;
            Unity_Subtract_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float);
            float _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float;
            Unity_Add_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float);
            UnityTexture2D _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HeightTex);
            float _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float = _Height;
            float4 _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4 = IN.uv0;
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[0];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[1];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_B_3_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[2];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_A_4_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[3];
            float4 _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4;
            float3 _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3;
            float2 _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2;
            Unity_Combine_float(_Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float, _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float, 0, 0, _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4, _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3, _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2);
            float2 _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2, float2(2, 2), _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2);
            float2 _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2, float2(1, 1), _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2);
            float2 _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2 = _Tiling;
            float _Split_3478904ad028424cb72b998728199740_R_1_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[0];
            float _Split_3478904ad028424cb72b998728199740_G_2_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[1];
            float _Split_3478904ad028424cb72b998728199740_B_3_Float = 0;
            float _Split_3478904ad028424cb72b998728199740_A_4_Float = 0;
            float _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float;
            Unity_Length_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float);
            float _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float;
            Unity_Lerp_float(_Split_3478904ad028424cb72b998728199740_R_1_Float, _Split_3478904ad028424cb72b998728199740_G_2_Float, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float, _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float);
            float2 _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, (_Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float.xx), _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2);
            float2 _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2, float2(0.5, 0.5), _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2);
            float2 _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2;
            Unity_Add_float2(_Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2, float2(0.5, 0.5), _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2);
            float2 _ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2 = _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2) + ParallaxMappingChannel(TEXTURE2D_ARGS(_Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.tex, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.samplerstate), IN.TangentSpaceViewDirection, _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float * 0.01, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2), 0);
            float2 _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2;
            Unity_Multiply_float2_float2(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2(2, 2), _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2);
            float2 _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2, float2(1, 1), _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2);
            float _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float;
            Unity_Length_float2(_Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2, _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float);
            float _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float;
            Unity_OneMinus_float(_Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float, _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float);
            float _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float;
            Unity_Saturate_float(_OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float, _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float);
            float _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float;
            Unity_OneMinus_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float);
            float _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float);
            float _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float;
            Unity_Smoothstep_float(_Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float, _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float);
            float _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float = _OpacityFalloff;
            float _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float;
            Unity_Power_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float, _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float);
            float _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float;
            Unity_OneMinus_float(_Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float, _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float);
            UnityTexture2D _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2;
            Unity_PolarCoordinates_float(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2);
            float2 _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2 = _SandMovement;
            float2 _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2);
            float2 _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2;
            Unity_Add_float2(_PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2, _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2);
            float4 _UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4 = IN.uv0;
            float4 _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4;
            Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4);
            float4 _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4;
            Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4);
            float4 _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D_GRAD(_Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.tex, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.samplerstate, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.GetTransformedUV(_Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2) , (_DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4.xy), (_DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4.xy));
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.r;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_G_5_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.g;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_B_6_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.b;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_A_7_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.a;
            float _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float);
            float _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float);
            float _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float, _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float);
            float _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float = _DitherPower;
            float _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float, _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float, _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float);
            float _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            Unity_Dither_float(_Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float, float4(IN.NDCPosition.xy, 0, 0), _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float);
            surface.Alpha = 1;
            surface.AlphaClipThreshold = _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.uv0 =                                        input.uv0;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            float3x3 tangentSpaceTransform = float3x3(output.WorldSpaceTangent, output.WorldSpaceBiTangent, output.WorldSpaceNormal);
            output.TangentSpaceViewDirection = mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpaceNormal;
             float3 WorldSpaceTangent;
             float3 WorldSpaceBiTangent;
             float3 WorldSpaceViewDirection;
             float3 TangentSpaceViewDirection;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
             float4 uv0;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float3 positionWS : INTERP2;
             float3 normalWS : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float2 _SandMovement;
        float4 _HeightTex_TexelSize;
        float _Height;
        float2 _Tiling;
        float4 _ColorBright;
        float4 _ColorDark;
        float _OpacityFalloff;
        float _DepthFade;
        float4 _VortexColorLight;
        float4 _VortexColorDark;
        float _Hole;
        float _DitherPower;
        float _HoleSmoothness;
        float4 _HoleColor;
        float _Depth;
        float _HoleFlatness;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_HeightTex);
        SAMPLER(sampler_HeightTex);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
        {
            float2 delta = UV - Center;
            float radius = length(delta) * 2 * RadialScale;
            float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
            Out = float2(radius, angle);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Lerp_float(float A, float B, float T, out float Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDX' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddx(In);
        }
        
        void Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(float4 In, out float4 Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = ddy(In);
        }
        
        void Unity_Dither_float(float In, float4 ScreenPosition, out float Out)
        {
            float2 uv = ScreenPosition.xy * _ScreenParams.xy;
            float DITHER_THRESHOLDS[16] =
            {
                1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
            };
            uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
            Out = In - DITHER_THRESHOLDS[index];
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_af823673df7b48f989d31c60d916c461_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_af823673df7b48f989d31c60d916c461_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_af823673df7b48f989d31c60d916c461_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_af823673df7b48f989d31c60d916c461_A_4_Float = 0;
            float2 _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2;
            Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2);
            float _Split_24aad51159224185ac48e810ac524d08_R_1_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[0];
            float _Split_24aad51159224185ac48e810ac524d08_G_2_Float = _PolarCoordinates_07a0848e2b5543c4a0e7b19a9897ac63_Out_4_Vector2[1];
            float _Split_24aad51159224185ac48e810ac524d08_B_3_Float = 0;
            float _Split_24aad51159224185ac48e810ac524d08_A_4_Float = 0;
            float _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float = _HoleFlatness;
            float _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float;
            Unity_Divide_float(_Split_24aad51159224185ac48e810ac524d08_R_1_Float, _Property_67cfede4803f42e49e6fdc98ce6054a7_Out_0_Float, _Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float);
            float _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float;
            Unity_OneMinus_float(_Divide_90567d23d9774352aa5cbaf40bc3b38e_Out_2_Float, _OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float);
            float _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ec1a69c9378d48b098059e6e55548ce0_Out_1_Float, _Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float);
            float _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float;
            Unity_Power_float(_Saturate_a09f08f16f47409d971e52850078adbe_Out_1_Float, 2, _Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float);
            float _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float = _Depth;
            float _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float;
            Unity_Multiply_float_float(_Power_d8b26faa66be4bbf9a732f8fdf66ed92_Out_2_Float, _Property_002ed57a2d1c4fc882d600bad00ac83c_Out_0_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float);
            float _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float;
            Unity_Subtract_float(_Split_af823673df7b48f989d31c60d916c461_G_2_Float, _Multiply_7424a1ab56da4f98b30d5581c8a08359_Out_2_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float);
            float3 _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3 = float3(_Split_af823673df7b48f989d31c60d916c461_R_1_Float, _Subtract_8081d0fa3aea4ed292d8933e446358d7_Out_2_Float, _Split_af823673df7b48f989d31c60d916c461_B_3_Float);
            description.Position = _Vector3_2aa090634b264659836ed54d62e32003_Out_0_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float = _Hole;
            float _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float = _HoleSmoothness;
            float _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float;
            Unity_Subtract_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float);
            float _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float;
            Unity_Add_float(_Property_9c5ed06cbb8f49dfb897b35058f70159_Out_0_Float, _Property_26ee02a41cff4f02a62155c44a040860_Out_0_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float);
            UnityTexture2D _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HeightTex);
            float _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float = _Height;
            float4 _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4 = IN.uv0;
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[0];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[1];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_B_3_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[2];
            float _Split_be94f7aa4c22482ab85a32d5242d1aa0_A_4_Float = _UV_3bb0aba4f0d94ce58c20ad9d88746850_Out_0_Vector4[3];
            float4 _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4;
            float3 _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3;
            float2 _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2;
            Unity_Combine_float(_Split_be94f7aa4c22482ab85a32d5242d1aa0_R_1_Float, _Split_be94f7aa4c22482ab85a32d5242d1aa0_G_2_Float, 0, 0, _Combine_3bcb6e83be4043bd949245e9963273c0_RGBA_4_Vector4, _Combine_3bcb6e83be4043bd949245e9963273c0_RGB_5_Vector3, _Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2);
            float2 _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Combine_3bcb6e83be4043bd949245e9963273c0_RG_6_Vector2, float2(2, 2), _Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2);
            float2 _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_cf384cf8551945bea6b079f25ff067fc_Out_2_Vector2, float2(1, 1), _Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2);
            float2 _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2 = _Tiling;
            float _Split_3478904ad028424cb72b998728199740_R_1_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[0];
            float _Split_3478904ad028424cb72b998728199740_G_2_Float = _Property_7028d03c4f1c4c9ea1b09b01028d3f79_Out_0_Vector2[1];
            float _Split_3478904ad028424cb72b998728199740_B_3_Float = 0;
            float _Split_3478904ad028424cb72b998728199740_A_4_Float = 0;
            float _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float;
            Unity_Length_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float);
            float _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float;
            Unity_Lerp_float(_Split_3478904ad028424cb72b998728199740_R_1_Float, _Split_3478904ad028424cb72b998728199740_G_2_Float, _Length_f17ff091c2484892a6f7fe585bf7bc8c_Out_1_Float, _Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float);
            float2 _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_c3b21972e6544bd2ae5833a0ceba05e9_Out_2_Vector2, (_Lerp_bd4dbfa504824fcfad7c84f6d4167d56_Out_3_Float.xx), _Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2);
            float2 _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_fdee873116d447bcbfbc08f190586f86_Out_2_Vector2, float2(0.5, 0.5), _Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2);
            float2 _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2;
            Unity_Add_float2(_Multiply_dfaef5c9890e4e438f4314ca12121294_Out_2_Vector2, float2(0.5, 0.5), _Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2);
            float2 _ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2 = _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2) + ParallaxMappingChannel(TEXTURE2D_ARGS(_Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.tex, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.samplerstate), IN.TangentSpaceViewDirection, _Property_a44e0bca8f044b51a4ca2bd482e0a63c_Out_0_Float * 0.01, _Property_dc7f40e7aa204b41b01067a318811efe_Out_0_Texture2D.GetTransformedUV(_Add_963d8f643cae45f985d2e9a3a5747b36_Out_2_Vector2), 0);
            float2 _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2;
            Unity_Multiply_float2_float2(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2(2, 2), _Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2);
            float2 _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_80a7bf713b7a4babae4a3e9fb1d00fee_Out_2_Vector2, float2(1, 1), _Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2);
            float _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float;
            Unity_Length_float2(_Subtract_78fda1a120bf4aa99eaf0f3fe03f9b4f_Out_2_Vector2, _Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float);
            float _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float;
            Unity_OneMinus_float(_Length_a47214ec688149518061d0e183cf7cfc_Out_1_Float, _OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float);
            float _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float;
            Unity_Saturate_float(_OneMinus_9052aa0a4e7647afbc50ed2e911afdea_Out_1_Float, _Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float);
            float _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float;
            Unity_OneMinus_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float);
            float _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float;
            Unity_Saturate_float(_OneMinus_ce920ee29be6404da140decec91fcf85_Out_1_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float);
            float _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float;
            Unity_Smoothstep_float(_Subtract_ed42c18e75ab43208c1d9b0021aef52e_Out_2_Float, _Add_40e85658b1f6438798e25c11414a3b61_Out_2_Float, _Saturate_85a1ad460969473fb123f5f0c6e7f541_Out_1_Float, _Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float);
            float _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float = _OpacityFalloff;
            float _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float;
            Unity_Power_float(_Saturate_3249953abfa94c0d95c15446d7284804_Out_1_Float, _Property_41b69c74c0154cdeb519baa71d7762bc_Out_0_Float, _Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float);
            float _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float;
            Unity_OneMinus_float(_Power_75192e3466574f45a2eedb38aa12451c_Out_2_Float, _OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float);
            UnityTexture2D _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float2 _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2;
            Unity_PolarCoordinates_float(_ParallaxMapping_5fe44706ea8b4b04bc5d1ab4c712ffb6_ParallaxUVs_0_Vector2, float2 (0.5, 0.5), 1.06, 2, _PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2);
            float2 _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2 = _SandMovement;
            float2 _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_451bf3b95b4144229d2d5d1a6c3e9b28_Out_0_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2);
            float2 _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2;
            Unity_Add_float2(_PolarCoordinates_79356a5cbedd4fdf9eb789f298539a8c_Out_4_Vector2, _Multiply_0a2f50a3e049408d955f2a57515a40f5_Out_2_Vector2, _Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2);
            float4 _UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4 = IN.uv0;
            float4 _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4;
            Unity_DDX_f0e8a7b629e84005987c13441cfc31a6_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4);
            float4 _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4;
            Unity_DDY_de809dde6dc74dc7bec2c5aafe18e59b_float4(_UV_ef9dcbd612864bf09bca0db470ac2b63_Out_0_Vector4, _DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4);
            float4 _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4 = SAMPLE_TEXTURE2D_GRAD(_Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.tex, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.samplerstate, _Property_51b655be292f42698abfe58abb70e6d5_Out_0_Texture2D.GetTransformedUV(_Add_91e8d3e6f5364c539bc8af8cf04199fb_Out_2_Vector2) , (_DDX_f0e8a7b629e84005987c13441cfc31a6_Out_1_Vector4.xy), (_DDY_de809dde6dc74dc7bec2c5aafe18e59b_Out_1_Vector4.xy));
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.r;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_G_5_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.g;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_B_6_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.b;
            float _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_A_7_Float = _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_RGBA_0_Vector4.a;
            float _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _SampleTexture2D_ee21fbf05c41449e9f551453a81a1f9b_R_4_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float);
            float _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_8a118deb11874acbaacce4565264dd5d_Out_1_Float, _Multiply_fdc7237a28c64dc297d9e6008e589832_Out_2_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float);
            float _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_23620a7c2eac498fa6b33d2bead472c4_Out_3_Float, _Multiply_8746f4afff464f90b78decf3a20ac45c_Out_2_Float, _Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float);
            float _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float = _DitherPower;
            float _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float;
            Unity_Multiply_float_float(_Multiply_97a3479f34384232a1434ccfca6c67c8_Out_2_Float, _Property_8e9593ae5d754529a68f50b2d18cd39e_Out_0_Float, _Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float);
            float _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            Unity_Dither_float(_Multiply_f6ac1b50fbf94714b6b697a57c784f28_Out_2_Float, float4(IN.NDCPosition.xy, 0, 0), _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float);
            surface.Alpha = 1;
            surface.AlphaClipThreshold = _Dither_414d48cac8bd4590b68940f01ce62eb5_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.uv0 =                                        input.uv0;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
            // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);
        
            // use bitangent on the fly like in hdrp
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0)* GetOddNegativeScale();
            float3 bitang = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        
            output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;      // we want a unit length Normal Vector node in shader graph
        
            // to pr               eserve mikktspace compliance we use same scale renormFactor as was used on the normal.
            // This                is explained in section 2.2 in "surface gradient based bump mapping framework"
            output.WorldSpaceTangent = renormFactor * input.tangentWS.xyz;
            output.WorldSpaceBiTangent = renormFactor * bitang;
        
            output.WorldSpaceViewDirection = GetWorldSpaceNormalizeViewDir(input.positionWS);
            float3x3 tangentSpaceTransform = float3x3(output.WorldSpaceTangent, output.WorldSpaceBiTangent, output.WorldSpaceNormal);
            output.TangentSpaceViewDirection = mul(tangentSpaceTransform, output.WorldSpaceViewDirection);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}