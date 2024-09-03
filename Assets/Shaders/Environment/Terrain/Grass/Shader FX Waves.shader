Shader "Unlit/Shader1" {
    Properties { // input data
        _Gradient ("Gradient Texture", 2D) = "white" {}
        _Posterization ("Posterization Texture", 2D) = "white" {}
        _SurfaceNormal ("Surface Normal", Vector) = (0, 0, 0, 0)
        _Color ("Color", Color) = (1, 1, 1, 1)
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
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            #include "Assets/Shaders/Environment/Terrain/Grass/MainLightGrass.hlsl"

            sampler2D _Gradient;
            float4 _Gradient_ST;

            sampler2D _Posterization;
            float4 _Posterization_ST;

            float4 _Color;

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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _SurfaceNormal)
            UNITY_INSTANCING_BUFFER_END(Props)

            Interpolators vert( MeshData v ){
                Interpolators o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = TransformObjectToHClip( v.vertex ); // local space to clip space
                o.normal = TransformObjectToWorldNormal( v.normals );
                o.uv = v.uv0; //(v.uv0 + _Offset) * _Scale; // passthrough
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
            
            float4 frag (Interpolators i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float3 objectPos = mul(GetObjectToWorldMatrix(), float4(0.0, 0.0, 0.0, 1.0));
                float3 surfaceNormal = UNITY_ACCESS_INSTANCED_PROP(Props, _SurfaceNormal).xyz;
                
                float3 mainLightDir;
                float3 mainLightColor;
                float distanceAtten;
                float shadowAtten;
                
                CalculateMainLight_float(objectPos, mainLightDir, mainLightColor, distanceAtten, shadowAtten);
                
                float dirAlignment = saturate(dot(mainLightDir, surfaceNormal));
                float alignAtten = (distanceAtten * shadowAtten) * dirAlignment;

                float3 gradientCol = tex2D(_Gradient, float2(alignAtten, 0.5f)) * mainLightColor;
                
                float smoothness = 0.;
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - objectPos.xyz);
                float mainSpecular = 0;
                
                float diffuse;
                float specular;
                float3 mainColor;
                
                AddAdditionalLights_float(smoothness, objectPos, surfaceNormal, viewDir, alignAtten, mainSpecular, gradientCol, diffuse, specular, mainColor);
                
                
                return float4(mainColor * _Color, 1.);
            }
            
            ENDHLSL
        }
    }
}
