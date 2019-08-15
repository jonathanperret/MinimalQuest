// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified VertexLit shader, optimized for high-poly meshes. Differences from regular VertexLit one:
// - less per-vertex work compared with Mobile-VertexLit
// - supports only DIRECTIONAL lights and ambient term, saves some vertex processing power
// - no per-material color
// - no specular
// - no emission

Shader "Custom/FlatShaded" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 80

    Pass {
        Name "FORWARD"
        Tags { "LightMode" = "ForwardBase" }

CGPROGRAM
        #pragma vertex vert_surf
        #pragma fragment frag_surf
        #pragma target 2.0

        #include "HLSLSupport.cginc"
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"

        inline float3 LightingLambertVS (float3 normal, float3 lightDir)
        {
            fixed diff = max (0, dot (normal, lightDir));
            return _LightColor0.rgb * diff;
        }

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float2 pack0 : TEXCOORD0;
            fixed3 normal : TEXCOORD1;
            fixed3 vlight : TEXCOORD2;
            float3 world : TEXCOORD3;
        };

        float4 _MainTex_ST;

        v2f vert_surf (appdata_full v)
        {
            v2f o;

            o.pos = UnityObjectToClipPos(v.vertex);
            o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

            float3 worldN = UnityObjectToWorldNormal(v.normal);

            o.normal = worldN;

            o.vlight = LightingLambertVS (worldN, _WorldSpaceLightPos0.xyz);

            o.world = mul(unity_ObjectToWorld, v.vertex).xyz;

            return o;
        }

        fixed4 frag_surf (v2f IN) : SV_Target
        {
            float2 uv_MainTex = IN.pack0.xy;

            half4 c = tex2D (_MainTex, uv_MainTex);

            //c.rgb = c.rgb * IN.vlight;

            // c.rgb = IN.world;

            float3 ddxPos = ddx(IN.world);
            float3 ddyPos = ddy(IN.world);

            float3 normal = normalize(cross(ddyPos, ddxPos));

            float3 vlight = LightingLambertVS (normal, _WorldSpaceLightPos0.xyz);

            c.rgb = vlight;

            c.a = 1.0;

            return c;
        }
ENDCG
    }
}

FallBack "Mobile/VertexLit"
}
