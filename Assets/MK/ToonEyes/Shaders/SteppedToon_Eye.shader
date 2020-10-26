Shader "Unlit/SteppedToonEye" {
    //show values to edit in inspector
    Properties {
        [Header(Color Parameters)]
        _Color ("Base Color", Color) = (0, 0, 0, 1)
		_EyeLidColor ("Eye Lid Color", Color) = (0.5, 0.5, 0.5, 1)
		_PupilColor ("Pupil Color", Color) = (0, 0, 0, 1)

		[Header(Size Parameters)]
		_EyeSize ("Eye Size", Range(0, 1)) = 0.5
		_PupilSize ("Pupil Size", Range(0, 1)) = 0.2
		_EyeLidSizeTop ("Eye Lid Size Top", Range(0, 0.5)) = 0.2
		_EyeLidSizeBot("Eye Lid Size Bot", Range(0, 0.5)) = 0.2

		[Header(Realtime Parameters)]
		_EyeLidRecede ("Eye Lid Recede", Float) = -500
		_BlinkSpeed("Blink Speed", Float) = 10
		_TimeOffset("Time Offset", Float) = 0

        [Header(Lighting Parameters)]
        _ShadowTint ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
    }
    SubShader {
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType"="Transparent" "Queue"="AlphaTest"}

        CGPROGRAM

        //the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
        //our surface shader function is called surf and we use our custom lighting model
        //fullforwardshadows makes sure unity adds the shadow passes the shader might need
        #pragma surface surf Stepped fullforwardshadows alpha:fade
        #pragma target 3.0

        fixed4 _Color;
		float4 _EyeLidColor;
		float4 _PupilColor;

		float _EyeSize;
		float _PupilSize;
		float _EyeLidSizeTop;
		float _EyeLidSizeBot;

		float _EyeLidRecede;
		float _BlinkSpeed;
		float _TimeOffset;

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
        };

		float Remap(float value, float from1, float to1, float from2, float to2) {
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}

        //the surface shader function which sets parameters the lighting function then uses
        void surf (Input i, inout SurfaceOutput o) {
            //sample and tint albedo texture

			// Get distance from center to create mask and pupil
			fixed4 mask = 1 - step(_EyeSize, distance(i.uv_MainTex, float2(0.5, 0.5)));
            float dist = distance(i.uv_MainTex, float2(0.5, 0.5));
            
			float4 eyeWhiteMask = step(_PupilSize, dist);
            float4 pupilMask = (1 - eyeWhiteMask);
			float4 eyeComposite = (pupilMask * _PupilColor) + (eyeWhiteMask * _Color);

			float eyeLidBlinkTimeValue = Remap(sin((_Time + _TimeOffset) * _BlinkSpeed), -1, 1, _EyeLidRecede, 0.6);
			float eyeLidBlinkValue = max(eyeLidBlinkTimeValue, 0);

			float4 noLidMask = step(_EyeLidSizeBot + eyeLidBlinkValue, i.uv_MainTex.yyyy) * step(_EyeLidSizeTop + eyeLidBlinkValue, 1 - i.uv_MainTex.yyyy);
			float4 eyeLidMask = (1 - noLidMask);
			float4 eyeLidComposite = eyeLidMask * _EyeLidColor;

			float4 completeEye = (eyeComposite * noLidMask) + eyeLidComposite;

			o.Albedo = completeEye.rgb;
            o.Alpha = mask;

            o.Emission = 0;
        }
        ENDCG
    }
    FallBack "Standard"
}