#if HLSL
    #define SAMPLE(texture, sampler, uv) SAMPLE_TEXTURE2D_X(texture, sampler, uv)
#else
    #define SAMPLE(texture, sampler, uv) UNITY_SAMPLE_SCREENSPACE_TEXTURE(texture, uv)
#endif

//CAVITY V
#ifdef CAVITY_SAMPLES_6
	#define CAVITY_SAMPLES 6
#endif
#ifdef CAVITY_SAMPLES_8
	#define CAVITY_SAMPLES 8
#endif
#ifdef CAVITY_SAMPLES_12
	#define CAVITY_SAMPLES 12
#endif
#ifdef CAVITY_SAMPLES_20
    #define CAVITY_SAMPLES 20
#endif
#ifdef SSCC_HDRP
	#define ACTUAL_CAVITY_SAMPLES (CAVITY_SAMPLES+2)
#else
	#define ACTUAL_CAVITY_SAMPLES (CAVITY_SAMPLES)
#endif
float _InterleavedGradientNoise(float2 pixCoord, int frameCount)
{
	const float3 magic = float3(0.06711056f, 0.00583715f, 52.9829189f);
	float2 frameMagicScale = float2(2.083f, 4.867f);
	pixCoord += frameCount * frameMagicScale;
	return frac(magic.z * frac(dot(pixCoord, magic.xy)));
}
float3 PickSamplePoint(float2 uv, float randAddon, int index)
{
	float2 positionSS = uv * _CavityTex_TexelSize.zw;
	float gn = _InterleavedGradientNoise(positionSS, index);
	float u = frac(gn) * 2.0 - 1.0;
	float theta = gn * 6.28318530717958647693;
	float sn, cs;
	sincos(theta, sn, cs);
	return float3(float2(cs, sn) * sqrt(1.0 - u * u), u);
}
float3x3 GetCoordinateConversionParameters(out float2 p11_22, out float2 p13_31)
{
	float3x3 camProj = (float3x3)unity_CameraProjection;
	//float3x3 camProj = (float3x3)/*UNITY_MATRIX_P*/_Projection;
	p11_22 = rcp(float2(camProj._11, camProj._22));
	p13_31 = float2(camProj._13, camProj._23);
	return camProj;
}
float3 ReconstructViewPos(float2 uv, float depth, float2 p11_22, float2 p13_31)
{
	#if ORTHOGRAPHIC_PROJECTION
		float3 viewPos = float3(((uv.xy * 2.0 - 1.0 - p13_31) * p11_22), depth);
	#else
		float3 viewPos = float3(depth * ((uv.xy * 2.0 - 1.0 - p13_31) * p11_22), depth);
	#endif
	return viewPos;
}
void SampleDepthAndViewpos(float2 uv, float2 p11_22, float2 p13_31, out float depth, out float3 vpos)
{
	depth = LinearizeDepth(FetchRawDepth(uv));
	vpos = ReconstructViewPos(uv, depth, p11_22, p13_31);
}
void Cavity(float2 uv, float3 normal, out float cavity, out float edges)
{
	cavity = edges = 0.0;
	float2 p11_22, p13_31;
	float3x3 camProj = GetCoordinateConversionParameters(p11_22, p13_31);

	float depth;
	float3 vpos;
	SampleDepthAndViewpos(uv, p11_22, p13_31, depth, vpos);

	float randAddon = uv.x * 1e-10;
	float rcpSampleCount = rcp(ACTUAL_CAVITY_SAMPLES);

	//UNITY_LOOP
	UNITY_UNROLL
	for (int i = 0; i < int(ACTUAL_CAVITY_SAMPLES); i++)
	{
		#if defined(SHADER_API_D3D11)
			i = floor(1.0001 * i);
		#endif

		#if 0
			float3 v_s1 = PickSamplePoint(uv.yx, randAddon, i);
		#else
			float3 v_s1 = PickSamplePoint(uv, randAddon, i);
		#endif
		v_s1 *= sqrt((i + 1.0) * rcpSampleCount) * _CavityWorldRadius * 0.5;
		float3 vpos_s1 = vpos + v_s1;

		float3 spos_s1 = mul(camProj, vpos_s1);
		#if ORTHOGRAPHIC_PROJECTION
			float2 uv_s1_01 = clamp((spos_s1.xy + 1.0) * 0.5, 0.0, 1.0);
		#else
			float2 uv_s1_01 = clamp((spos_s1.xy * rcp(vpos_s1.z) + 1.0) * 0.5, 0.0, 1.0);
		#endif

		float depth_s1 = LinearizeDepth(FetchRawDepth(uv_s1_01));
		float3 vpos_s2 = ReconstructViewPos(uv_s1_01, depth_s1, p11_22, p13_31);

		float3 dir = vpos_s2 - vpos;
		float len = length(dir);
		float f_dot = dot(dir, normal);
		//float kBeta = 0.002;
		float kBeta = 0.002 * 4;
		float f_cavities = f_dot - kBeta * depth;
		float f_edge = -f_dot - kBeta * depth;
		float f_bias = 0.05 * len + 0.0001;

		if (f_cavities > -f_bias)
		{
			float attenuation = 1.0 / (len * (1.0 + len * len * 3.));
			cavity += f_cavities * attenuation;
		}
		if (f_edge > f_bias)
		{
			float attenuation = 1.0 / (len * (1.0 + len * len * 0.01));
			edges += f_edge * attenuation;
		}
	}

	//cavity *= 1.0 / ACTUAL_CAVITY_SAMPLES;
	//edges *= 1.0 / ACTUAL_CAVITY_SAMPLES;
	cavity *= 1.0 * _CavityWorldRadius * 0.5;
	edges *= 1.0 * _CavityWorldRadius * 0.5;

	float kContrast = 0.6;
	cavity = pow(abs(cavity * rcpSampleCount), kContrast);
	edges = pow(abs(edges * rcpSampleCount), kContrast);

	cavity = clamp(cavity * _CavityDarks, 0.0, 1.0);
	edges = edges * _CavityBrights;
}
//CAVITY ^

//CURVATURE V
float CurvatureSoftClamp(float curvature, float control)
{
	if (curvature < 0.5 / control) 
		return curvature * (1.0 - curvature * control);
	return 0.25 / control;
}
float Curvature(float2 uv, float3 P)
{
	float3 offset = float3(_Input_TexelSize.xy, 0.0) * (_CurvaturePixelRadius);

	float normal_up = FetchViewNormals(P, uv + offset.zy).g;
	float normal_down = FetchViewNormals(P, uv - offset.zy).g;
	float normal_right = FetchViewNormals(P, uv + offset.xz).r;
	float normal_left = FetchViewNormals(P, uv - offset.xz).r;

	float normal_diff = (normal_up - normal_down) + (normal_right - normal_left);

	//if (abs(normal_diff) <= 0.1) return 0; //slight low pass filter to remove noise from camera normals precision
    //new and improved low pass filter:
    //if (uv.x < 0.5)
    {
        if (normal_diff > 0.0) normal_diff = sign(normal_diff) * pow(normal_diff, 2.0);
        _CavityBrights += 0.5;
    }

	if (normal_diff >= 0.0)
		return 2.0 * CurvatureSoftClamp(normal_diff, _CurvatureBrights);
	else
		return -2.0 * CurvatureSoftClamp(-normal_diff, _CurvatureDarks);
}
//CURVATURE ^

float invLerp(float from, float to, float value) {
	return (value - from) / (to - from);
}
float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value) {
	float rel = invLerp(origFrom, origTo, value);
	return lerp(targetFrom, targetTo, rel);
}

float4 Cavity_Frag(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    #ifdef UnityStereoTransformScreenSpaceTex
    float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);
    #else
    float2 uv = input.uv;
    #endif
    
    float3 P = FetchViewPos(uv);
    float3 N = FetchViewNormals(P, uv);

    float cavity = 0.0, edges = 0.0;
    Cavity(uv, N, cavity, edges);
    return float4(cavity, edges, FetchRawDepth(uv), 1.0);
}

float2 GaussianBlur(float2 uv, float2 pixelOffset)
{
    const float gWeights[2] =
    {
        0.44908,
        0.05092
    };
    const float gOffsets[2] =
    {
        0.53805,
        2.06278
    };
    float2 colOut = 0.0;
    UNITY_UNROLL
    for(int i = 0; i < 2; i++)
    {
        float2 texCoordOffset = pixelOffset * gOffsets[i];
        float2 p1 = SAMPLE(_MainTex, sampler_LinearClamp, uv + texCoordOffset).xy;
        float2 p2 = SAMPLE(_MainTex, sampler_LinearClamp, uv - texCoordOffset).xy;
        colOut += gWeights[i] * (p1 + p2);
    }
    return colOut;
}
float4 HorizontalBlur_Frag(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    return float4
    (
        GaussianBlur(input.uv, float2(_CavityTex_TexelSize.x, 0.0)),
        SAMPLE(_MainTex, sampler_LinearClamp, input.uv).z,
        1.0
    );
}
float4 VerticalBlur_Frag(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    return float4
    (
        GaussianBlur(input.uv, float2(0.0, _CavityTex_TexelSize.y)),
        SAMPLE(_MainTex, sampler_LinearClamp, input.uv).z,
        1.0
    );
}

float4 Composite_Frag(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
	#ifdef UnityStereoTransformScreenSpaceTex
		float2 uv = UnityStereoTransformScreenSpaceTex(input.uv);
	#else
		float2 uv = input.uv;
	#endif
    
	float3 P = FetchViewPos(uv);
	float3 N = FetchViewNormals(P, uv);
	float4 col = FetchSceneColor(uv);
	float4 untouchedCol = FetchSceneColor(uv);
	//float depth01 = FetchRawDepth(uv);
	//if (depth01 == 1.0 || depth01 == 0.0) return col;

	float curvature = 0.0;
	curvature = Curvature(uv, P);
    
	//float cavity = 0.0, edges = 0.0;
	//Cavity(uv, N, cavity, edges);

    float2 cavityTex;
    #if UPSCALE_CAVITY
        float2 LowResTexelSize = _CavityTex_TexelSize.xy;
        float2 LowResBufferSize = _CavityTex_TexelSize.zw;
        float2 Corner00UV = floor(uv * LowResBufferSize - .5f) / LowResBufferSize + .5f * LowResTexelSize;
        float2 BilinearWeights = (uv - Corner00UV) * LowResBufferSize;

        //xy is signal, z is depth it used
        float3 TextureValues00 = SAMPLE(_CavityTex, sampler_LinearClamp, Corner00UV).xyz;
        float3 TextureValues10 = SAMPLE(_CavityTex, sampler_LinearClamp, Corner00UV + float2(LowResTexelSize.x, 0)).xyz;
        float3 TextureValues01 = SAMPLE(_CavityTex, sampler_LinearClamp, Corner00UV + float2(0, LowResTexelSize.y)).xyz;
        float3 TextureValues11 = SAMPLE(_CavityTex, sampler_LinearClamp, Corner00UV + LowResTexelSize).xyz;

        float4 CornerWeights = float4(
         (1 - BilinearWeights.y) * (1 - BilinearWeights.x),
         (1 - BilinearWeights.y) * BilinearWeights.x,
         BilinearWeights.y * (1 - BilinearWeights.x),
         BilinearWeights.y * BilinearWeights.x);

        float Epsilon = .0001f/*-.0001f*//*0.0f*/;

        float4 CornerDepths = abs(float4(TextureValues00.z, TextureValues10.z, TextureValues01.z, TextureValues11.z));
        float SceneDepth = FetchRawDepth(uv);
        float4 DepthWeights = 1.0f / (abs(CornerDepths - SceneDepth.xxxx) + Epsilon);
        float4 FinalWeights = CornerWeights * DepthWeights;

        cavityTex = (FinalWeights.x*TextureValues00.xy + FinalWeights.y*TextureValues10.xy + FinalWeights.z*TextureValues01.xy + FinalWeights.w*TextureValues11.xy) / dot(FinalWeights, 1);
    #else
        cavityTex = SAMPLE(_CavityTex, sampler_LinearClamp, uv).xy;
    #endif
    float cavity = cavityTex.r, edges = cavityTex.g;

	if (uv.x < _Input_TexelSize.x * 2 || uv.y < _Input_TexelSize.y * 2 || 1 - uv.x < _Input_TexelSize.x * 2 || 1 - uv.y < _Input_TexelSize.y * 2) { curvature = cavity = edges = 0; };

	col.rgb += (curvature * 0.4);

	#if SATURATE_CAVITY
        //float3 extra = col.rgb - saturate(col.rgb);
		col.rgb = pow(saturate(col.rgb), 1 - (edges * 0.5));
		col.rgb = pow(saturate(col.rgb), 1 + (cavity * 1));
        //col.rgb += extra;
	#else
		col.rgb += (edges * 0.2);
		col.rgb -= (cavity * 0.2);
	#endif

	//Uncomment this block of code for on/off back and forth effect preview
	//if (uv.x < sin(_Time.y+(3.1415/2)) * 0.5 + 0.5) 
	//{
	//	if (uv.x > (sin(_Time.y+(3.1415/2)) * 0.5 + 0.5) - 0.002) return 0;
	//	return untouchedCol;
	//}


    #if DEBUG_EFFECT
		return ((1.0 - cavity) * (1.0 + edges) * (1.0 + curvature)) * 0.25;
	#elif DEBUG_NORMALS
		return float4(N * 0.5 + 0.5, 1);
	#endif

	#if OUTPUT_TO_TEXTURE
		float r = curvature * 0.4;
		float g = (edges * 0.2) - (cavity * 0.2);
		return float4(r * rcp(max(1, P.z * _DistanceFade)) * _EffectIntensity, g * rcp(max(1, P.z * _DistanceFade)) * _EffectIntensity, 1, 1);
		//Values rescaled so they're more consistent to work with, if you just +curvature+edges it should match 'screen' output
	#else
		return lerp(untouchedCol, col, rcp(max(1, P.z * _DistanceFade)) * _EffectIntensity);
	#endif
}
