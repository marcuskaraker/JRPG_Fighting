Shader "Custom/ToonWater"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

		_NoiseTexture ("Noise Texture", 2D) = "white" {}

		_WaterFogColor("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterBackgroundColor("Water Background Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity("Water Fog Density", Range(0, 2)) = 0.1
		_ShoreSize("Shore Size", Range(0, 1)) = 0.1

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

		GrabPass { "_WaterBackground" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha finalcolor:ResetAlpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		#include "LookingThroughWater.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_NoiseTexture;
			float4 screenPos;
        };

		sampler2D _NoiseTexture;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void ResetAlpha(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			color.a = 1;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

			float2 noiseUV = IN.uv_NoiseTexture + _Time * 0.05 * -1;
			fixed4 noiseTex = tex2D(_NoiseTexture, noiseUV);

			float noise = 1 - step(ShoreLineSmooth(IN.screenPos), noiseTex.r);
			o.Emission = noise + ColorBelowWater(IN.screenPos) * (1 - c.a);
        }
        ENDCG
    }
    //FallBack "Diffuse"
}
