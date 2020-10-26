#if !defined(LOOKING_THROUGH_WATER_INCLUDED)
#define LOOKING_THROUGH_WATER_INCLUDED

sampler2D _CameraDepthTexture, _WaterBackground;
float4 _CameraDepthTexture_TexelSize;

float3 _WaterFogColor;
float3 _WaterBackgroundColor;
float _WaterFogDensity;
float _ShoreSize;

float3 ColorBelowWater(float4 screenPos) {
	float2 uv = screenPos.xy / screenPos.w;

#if UNITY_UV_STARTS_AT_TOP
	if (_CameraDepthTexture_TexelSize.y < 0)
	{
		uv.y = 1 - uv.y;
	}
#endif

	float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
	float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
	float depthDifference = backgroundDepth - surfaceDepth;

	float3 backgroundColor = tex2D(_WaterBackground, uv).rgb * _WaterBackgroundColor;
	float fogFactor = exp2(-_WaterFogDensity * depthDifference);

	return lerp(_WaterFogColor, backgroundColor, fogFactor);
}

float3 ShoreLine(float4 screenPos) {
	float2 uv = screenPos.xy / screenPos.w;

#if UNITY_UV_STARTS_AT_TOP
	if (_CameraDepthTexture_TexelSize.y < 0)
	{
		uv.y = 1 - uv.y;
	}
#endif

	float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
	float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
	float depthDifference = backgroundDepth - surfaceDepth;
	
	return step((1-_ShoreSize), 1 - depthDifference);
}

float3 ShoreLineSmooth(float4 screenPos) {
	float2 uv = screenPos.xy / screenPos.w;

#if UNITY_UV_STARTS_AT_TOP
	if (_CameraDepthTexture_TexelSize.y < 0)
	{
		uv.y = 1 - uv.y;
	}
#endif

	float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
	float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);
	float depthDifference = backgroundDepth - surfaceDepth;

	return smoothstep(0, (1 - _ShoreSize), 1 - depthDifference);
}

#endif