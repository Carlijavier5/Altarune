Shader "Unlit/Shader1" {
    Properties { // input data
        _Gradient ("Gradient Texture", 2D) = "white" {}
        _Posterization ("Posterization Texture", 2D) = "white" {}
        _SurfaceNormal ("Surface Normal", Vector) = (0, 0, 0, 0)
        _Color ("Color", Color) = (1, 1, 1, 1)
        _AmbientColor ("Color", Color) = (1, 1, 1, 1)
        _CloudSpeed ("Cloud Speed", Vector) = (0, 0, 0, 0)
        _CloudScale ("Cloud Scale", Float) = 1
        _StaticGrassConstant ("Grass Constant", Range(0.0, 1.0)) = 0.
        
        _WindMap ("Wind Map", 2D) = "black" {}
        _WaveSpeed("Wave Speed", float) = 1.0
        _WindStrength("Wind Strength", float) = 1.0
        _WindSpeed ("Wind Speed", Vector) = (0, 0, 0, 0)
        _WindScale ("Wind Scale", Float) = 1.0
        _HeightCutoff("Height Cutoff", float) = 1.0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Assets/Shaders/Environment/Terrain/Grass/MainLightGrass.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

            sampler2D _Gradient;
            float4 _Gradient_ST;

            sampler2D _Posterization;
            float4 _Posterization_ST;

            float4 _Color;
            float4 _AmbientColor;

            float4 _CloudSpeed;
            float _CloudScale;

            sampler2D _WindMap;
            float2 _WindSpeed;
            float _WindScale;
            float _HeightCutoff;
            float _WaveSpeed;
            float _WindStrength;

            // automatically filled out by Unity
            struct MeshData { // per-vertex mesh data
                float4 vertex : POSITION; // local space vertex position
                float3 normals : NORMAL; // local space normal direction
                float4 uv0 : TEXCOORD0; // uv0 diffuse/normal map textures
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // data passed from the vertex shader to the fragment shader
            // this will interpolate/blend across the triangle!
            struct Interpolators {
                float4 vertex : SV_POSITION; // clip space position
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 pos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            //Worley Noise
            float2 Unity_GradientNoise_Deterministic_Dir_float(float2 p)
            {
                float x; Hash_Tchou_2_1_float(p, x);
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            void Unity_GradientNoise_Deterministic_float (float2 UV, float3 Scale, out float Out)
            {
                float2 p = UV * Scale.xy;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Deterministic_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _SurfaceNormal)
                UNITY_DEFINE_INSTANCED_PROP(float, _StaticGrassConstant)
            UNITY_INSTANCING_BUFFER_END(Props)

            Interpolators vert( MeshData v ){
                Interpolators o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = TransformObjectToHClip( v.vertex ); // local space to clip space

                float2 windTilingOffset = TransformObjectToWorld( v.vertex ).xz * _WindScale + (_Time.y * _WindSpeed);
                float wind = tex2Dlod(_WindMap, float4(windTilingOffset, 0, 0));
                wind *= _WindStrength;
                float3 windPos = TransformObjectToWorld( v.vertex );
                windPos.x += wind;
                windPos.z += wind;
                o.vertex = TransformWorldToHClip( windPos );
                
                o.normal = TransformObjectToWorldNormal( v.normals );
                o.uv = v.uv0; //(v.uv0 + _Offset) * _Scale; // passthrough
                o.pos = TransformObjectToWorld( v.vertex );
                return o;
            }

            void AddAdditionalLights_float(float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
                float MainDiffuse, float MainSpecular, float3 MainColor,
                out float Diffuse, out float Specular, out float3 Color) {
                Diffuse = MainDiffuse;
                Specular = MainSpecular;
                Color = 0;

                #ifndef SHADERGRAPH_PREVIEW
                int pixelLightCount = GetAdditionalLightsCount();
                for (int i = 0; i < pixelLightCount; ++i) {
                    Light light = GetAdditionalLight(i, WorldPosition);
                    half NdotL = saturate(dot(WorldNormal, light.direction));
                    half atten = light.distanceAttenuation * light.shadowAttenuation;
                    half thisDiffuse = atten * NdotL;
                    half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
                    Diffuse += thisDiffuse;
                    Specular += thisSpecular;
                    float3 posterizedCol = tex2D(_Posterization, float2(thisDiffuse * 70, 0.5f));
                    if (thisDiffuse > 0)
                    {
                        Color += light.color * posterizedCol;
                    }
                }

                if (Color.x <= 0)
                {
                    Color += (MainColor * (MainDiffuse + MainSpecular));
                } else
                {
                    Color += MainColor;
                }
                #endif
                half total = Diffuse + Specular;
                // If no light touches this pixel, set the color to the main light's color
                Color = total <= 0 ? MainColor : Color / total;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
                        
            float4 frag (Interpolators i) : SV_Target
            {
                
                UNITY_SETUP_INSTANCE_ID(i);
                float3 objectPos = mul(GetObjectToWorldMatrix(), float4(0.0, 0.0, 0.0, 1.0));
                float3 surfaceNormal = UNITY_ACCESS_INSTANCED_PROP(Props, _SurfaceNormal).xyz;

                float time = _Time.y;
                float2 cloudScroll = time * _CloudSpeed.xy;
                float2 cloudTiling;
                Unity_TilingAndOffset_float(objectPos.xz, float2(1., 1.), cloudScroll, cloudTiling);
                float cloudNoise;
                Unity_GradientNoise_Deterministic_float(cloudTiling, _CloudScale, cloudNoise);

                float2 perlin;
                Unity_TilingAndOffset_float(objectPos.xz, float2(1., 1.), float2(0., 0.), perlin);
                float perlinNoise;
                Unity_GradientNoise_Deterministic_float(perlin, _CloudScale, perlinNoise);

                cloudNoise = (perlinNoise + cloudNoise) / 2;
                
                float3 mainLightDir;
                float3 mainLightColor;
                float distanceAtten;
                float shadowAtten;
                
                CalculateMainLight_float(objectPos, mainLightDir, mainLightColor, distanceAtten, shadowAtten);
                
                float dirAlignment = saturate(dot(mainLightDir, surfaceNormal));
                float alignAtten = (distanceAtten * shadowAtten) * dirAlignment;
                
                float3 gradientCol = tex2D(_Gradient, float2(cloudNoise, 0.5f)) * mainLightColor;
                if (UNITY_ACCESS_INSTANCED_PROP(Props, _StaticGrassConstant) > 0)
                {
                    gradientCol = tex2D(_Gradient, float2(UNITY_ACCESS_INSTANCED_PROP(Props, _StaticGrassConstant), 0.5f)) * mainLightColor;
                }

                gradientCol *= _AmbientColor;
                
                float smoothness = 0.;
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - objectPos.xyz);
                float mainSpecular = 0;
                
                float diffuse;
                float specular;
                float3 mainColor;
                
                AddAdditionalLights_float(smoothness, objectPos, surfaceNormal, viewDir, alignAtten, mainSpecular, gradientCol, diffuse, specular, mainColor);
                
                
                return float4(mainColor * _Color * diffuse, 1.);
            }
            
            ENDHLSL
        }
    }
}
