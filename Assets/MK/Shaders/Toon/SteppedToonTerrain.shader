Shader "Unlit/SteppedToonTerrain" {
    //show values to edit in inspector
    Properties {
        [Header(Base Parameters)]
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _CliffTexture ("Cliff Texture", 2D) = "white" {}
		_CliffThresholdMin ("Cliff Threshold Min", Range (0.0, 1.0)) = 0.8
		_CliffThresholdMax ("Cliff Threshold Max", Range (0.0, 1.0)) = 0.805

        [HDR] _Emission ("Emission", color) = (0 ,0 ,0 , 1)

        [Header(Lighting Parameters)]
        _ShadowTint ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
    }
    SubShader {
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        CGPROGRAM

        //the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
        //our surface shader function is called surf and we use our custom lighting model
        //fullforwardshadows makes sure unity adds the shadow passes the shader might need
        #pragma surface surf Stepped fullforwardshadows
        #pragma target 3.0

        sampler2D _CliffTexture;
        float4 _CliffTexture_ST;
        float _CliffThresholdMin;
        float _CliffThresholdMax;

        fixed4 _Color;
        half3 _Emission;

        sampler2D _Control;

        // Textures
        sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
        float4 _Splat0_ST, _Splat1_ST, _Splat2_ST, _Splat3_ST;

        float3 _ShadowTint;

        //our lighting function. Will be called once per light
        float4 LightingStepped(SurfaceOutput s, float3 lightDir, half3 viewDir, float shadowAttenuation){
            //how much does the normal point towards the light?
            float towardsLight = dot(s.Normal, lightDir);
            // make the lighting a hard cut
            float towardsLightChange = fwidth(towardsLight);
            float lightIntensity = smoothstep(0, towardsLightChange, towardsLight);

        #ifdef USING_DIRECTIONAL_LIGHT
            //for directional lights, get a hard vut in the middle of the shadow attenuation
            float attenuationChange = fwidth(shadowAttenuation) * 0.5;
            float shadow = smoothstep(0.5 - attenuationChange, 0.5 + attenuationChange, shadowAttenuation);
        #else
            //for other light types (point, spot), put the cutoff near black, so the falloff doesn't affect the range
            float attenuationChange = fwidth(shadowAttenuation);
            float shadow = smoothstep(0, attenuationChange, shadowAttenuation);
        #endif
            lightIntensity = lightIntensity * shadow;

            //calculate shadow color and mix light and shadow based on the light. Then taint it based on the light color
            float3 shadowColor = s.Albedo * _ShadowTint;
            float4 color;
            color.rgb = lerp(shadowColor, s.Albedo, lightIntensity) * _LightColor0.rgb;
            color.a = s.Alpha;
            return color;
        }


        //input struct which is automatically filled by unity
        struct Input {
            float2 uv_MainTex;
            float2 uv_Control;
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        //the surface shader function which sets parameters the lighting function then uses
        void surf (Input IN, inout SurfaceOutput o) {

            fixed4 splatControl = tex2D(_Control, IN.uv_Control);
            fixed4 col = splatControl.r * tex2D(_Splat0, IN.uv_Control * _Splat0_ST.xy);
            col += splatControl.g * tex2D(_Splat1, IN.uv_Control * _Splat1_ST.xy);
            col += splatControl.b * tex2D(_Splat2, IN.uv_Control * _Splat2_ST.xy);
            col += splatControl.a * tex2D(_Splat3, IN.uv_Control * _Splat3_ST.xy);

            float3 vec = abs(WorldNormalVector(IN, o.Normal));
            float threshold = smoothstep(_CliffThresholdMin, _CliffThresholdMax, abs(dot(vec, float3(0, 1, 0))));
            fixed4 cliffColorXY = tex2D(_CliffTexture, IN.worldPos.xy * _CliffTexture_ST.xy);
            fixed4 cliffColorYZ = tex2D(_CliffTexture, IN.worldPos.yz * _CliffTexture_ST.xy);
            fixed4 cliffColor = vec.x * cliffColorYZ + vec.z * cliffColorXY;

            col = lerp(cliffColor, col, threshold);

            //sample and tint albedo texture
            col *= _Color;
            o.Albedo = col.rgb;

            o.Emission = _Emission;
        }
        ENDCG
    }
    FallBack "Standard"
}