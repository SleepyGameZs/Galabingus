#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

uniform float fade;
uniform float redShade;
uniform float shadeFadeTime;
uniform bool fadeIn;
uniform bool fadeOut;
uniform bool bossEffect;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float3 normalizeSaturation(float4 color)
{
	float4 newColor = normalize(color) * 1.732050807568877293527446341505872366 * -1.732050807568877293527446341505872366 + color * 1.732050807568877293527446341505872366;
	float adjstColor = color;

	if (color.r == color.g)
	{
		color.r = color.r * 0.8;
		color.g = color.g * 0.8;
	}

	if ((color.r + color.b + color.g) * color.a < 0.3)
	{
		color = color * 1.27;
	}

	if ((color.r + color.b + color.g) * color.a < 1.9 && (color.r + color.b + color.g) * color.a > 0.5)
	{
		color = ((newColor * 0.5f) + color);
	}

	if ((color.r + color.b + color.g) * color.a > 2)
	{
		color = ((newColor * 0.5f) + color) * 1.4;
	}
	else
	{
		color = color * 1.27 * 0.8;
	}

	return  color * 1.3f;
}

float4 BossEffect(float4 inColor)
{
	if (bossEffect)// && inColor.a == 1)
	{
		return float4(inColor.r, inColor.g * redShade * shadeFadeTime * 0.8f, inColor.b * redShade * shadeFadeTime * 0.8f, inColor.a);
	}
	return float4(inColor.r, inColor.g, inColor.b, inColor.a);
}

float4 FadeIn(float4 inColor)
{
	if (fadeIn)
	{
		inColor.r *= (1 - fade);
		inColor.g *= (1 - fade);
		inColor.b *= (1 - fade);
		return float4(inColor.r, inColor.g, inColor.b, inColor.a * (1 - fade));
	}
	else
	{
		return inColor;
	}
}

float4 FadeOut(float4 inColor)
{
	if (fadeOut)
	{
		inColor.r *= fade;
		inColor.g *= fade;
		inColor.b *= fade;
		return float4(inColor.r, inColor.g, inColor.b, inColor.a * fade);
	}
	else
	{
		return inColor;
	}
}



float4 MainPS(VertexShaderOutput input) : COLOR
{
	float weights[11];
	float2 offsets[11];

	// Calculate the texel size
	float2 texelSize = 1.0f/ float2(1600, 1600);

	// Calculate the Gaussian weights and offsets
	float sigma = 10 / 3.0f;
	float twoSigma2 = 2.0f * sigma * sigma;
	float weightSum = 0;
	for (int i = 0; i < 11; i++)
	{
		float offset = float(i - 5);
		weights[i] = exp(-(offset * offset) / twoSigma2);
		weightSum += weights[i];
		offsets[i] = float2(offset,0);
	}
	for (int i = 0; i < 11; i++)
	{
		weights[i] /= weightSum;
	}

	// Blur horizontally
	float4 halation = float4(0, 0, 0, 0);
	for (int i = 0; i < 11; i++)
	{
		float2 offset = float2(offsets[i].x, 0) * texelSize;
		float4 texColor = tex2D(SpriteTextureSampler, input.TextureCoordinates + offset);
		halation += weights[i] * texColor;
	}

	// Blur vertically
	for (int i = 0; i < 11; i++)
	{
		float2 offset = float2(0, offsets[i].x) * texelSize;
		float4 texColor = tex2D(SpriteTextureSampler, input.TextureCoordinates + offset);
		halation += weights[i] * texColor;
	}

	// Clamp the maximum brightness value of the red color channel to create a red hue halation effect
	const float4 maxHalationColor = float4(0.5f, 0.5f, 0.5f, 1.0f);
	//halation.r = min(halation.r, maxColor.r);
	//halation.g = min(halation.g, maxColor.g);
	//halation.b = min(halation.b, maxHalationColor.b);


	const float gamma = 0.41f;//1.26795f;
	float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	float4 colorBefore = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	float4 correctedColor = exp(log(colorBefore / input.Color * (1.5f)) *  (1 / gamma) ) * input.Color;
	float4 colorTrue = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	float4 colorP = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	float3 colorA = color.rgb;
	float3 colorB = input.Color.rgb;
	float3 color2 = lerp(colorA, colorB, 0.973);
	color.r = color.r * color2.r;
	color.g = color.g * color2.g;
	color.b = color.b * color2.b;
	color.r = color.r * 0.125 + color.r * 0.875 * input.Color.a * 1.0;
	color.g = color.g * 0.125 + color.g * 0.875 * input.Color.a * 1.0;
	color.b = color.b * 0.125 + color.b * 0.875 * input.Color.a * 1.0;
	color.a = color.a * input.Color.a;
	color = color * input.Color;

	if (color.a == 0)
	{
		color.r = 0;
		color.g = 0;
		color.b = 0;
	}

	float4 lerpPixels = colorTrue;

	if ((colorTrue.a == 1 && ((color.r+color.b+color.g) > 0.1f) || colorTrue.a < 0.1))
	{
		lerpPixels = lerp(lerp(color, colorTrue, 0.059875), colorP, 0.5);
		lerpPixels = lerp(lerpPixels, lerpPixels * correctedColor, 0.7);
		if (length(lerpPixels.rgb) < 2.0f)
		{
			lerpPixels.rgb = lerp(lerpPixels.rgb, lerpPixels.rgb * 0.21, 0.5);
		}

		//lerpPixel.rgb = lerp(lerpPixels.rgb, colorTrue.rgb, 0.9875);
	}

	if (colorTrue.a == 1 && length(lerpPixels.rgb) < 0.5f)
	{
		lerpPixels.rgb = lerpPixels.rgb * 2.5f;
		if (colorTrue.r > 0.5f && colorTrue.b < 0.2f && colorTrue.g < 0.2f)
		{
			//lerpPixels.r *= 1.0f;
			//lerpPixels.g *= 1.125f;
		}
	}



	halation.r = halation.r * 0.75;
	halation.g = halation.g * 0.75;
	halation.b = halation.b * 0.75;

	float4 halationLerp;

	if (lerpPixels.a == 0 && lerpPixels.b == 0 && lerpPixels.r == 0 && lerpPixels.g == 0)
	{
		halationLerp = lerp(halation, lerpPixels, 0.75) * 1.5;
	}
	else
	{
		halationLerp = lerpPixels;
	}

	halationLerp.a = lerpPixels.a;

	return BossEffect(FadeIn(FadeOut(halationLerp)));
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};