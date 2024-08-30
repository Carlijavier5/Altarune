Shader "Unlit/InstancedGrass" {
    Properties {
        _Gradient ("Gradient Texture", 2D) = "white" {}
        _SurfaceNormal ("Surface Normal", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members pos)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "Assets/Shaders/Environment/Terrain/MainLight.hlsl"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : UNITY_POSITION();
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _SurfaceNormal)
            UNITY_INSTANCING_BUFFER_END(Props)

            sampler2D _Gradient;
            float4 _Gradient_ST;
 
            v2f vert (MeshData v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Gradient);
                o.pos = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                // float4 objectPos = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
                // float3 surfaceNormal = UNITY_ACCESS_INSTANCED_PROP(Props, _SurfaceNormal);
                //
                // float3 mainLightDir;
                // float3 mainLightColor;
                // float distanceAtten;
                // float shadowAtten;
                //
                // CalculateMainLight_float(objectPos, mainLightDir, mainLightColor, distanceAtten, shadowAtten);
                //
                // float dirAlignment = saturate(dot(mainLightDir, surfaceNormal));
                // float alignAtten = (distanceAtten * shadowAtten) * dirAlignment;
                //
                // float smoothness = 0.;
                // float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.pos.xyz);
                // float mainSpecular = 0;
                //
                // float diffuse;
                // float specular;
                // float3 mainColor;
                //
                // AddAdditionalLights_float(smoothness, i.pos, surfaceNormal, viewDir, alignAtten, mainSpecular, mainLightColor, diffuse, specular, mainColor);
                //
                // fixed3 gradientCol = tex2D(_Gradient, float2(diffuse, 0.5f)) * mainColor;
                //
                // return float4(gradientCol, 1.);
                return float4(1., 1., 1., 1.);
            }
            ENDCG
        }
    }
}
