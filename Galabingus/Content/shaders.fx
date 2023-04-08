#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

uniform float fade;
uniform bool fadeIn;
uniform bool fadeOut;

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

	if (color.b > 0.5f && color.g > 0.25f && color.g < 0.5f)
	{
		//color.b = color.b * 2;
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

float hash(float2 p)
{
	return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
}

float2 randomValues(float2 uv, float2 scale)
{
	// Generate two random values for the given texture coordinate
	float2 noise = frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);

	// Scale and offset the random values to the desired range
	return noise * scale - (scale / 2.0);
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
	const float gamma = 1.26795f;
	float pixelize = floor(input.TextureCoordinates.x * (640)) / (640);
	float4 color = tex2D(SpriteTextureSampler, pixelize);
	float4 colorBefore = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	float4 correctedColor = exp(log(colorBefore / input.Color * (1.5f)) *  (1 / gamma) ) * input.Color;
	float2 pixelizeM = floor(input.TextureCoordinates * (640)) / (640);
	float4 colorTrue = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	float4 colorP = tex2D(SpriteTextureSampler, pixelizeM) * input.Color;

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
	if (color.a == 0)
	{
		color.r = 0;
		color.g = 0;
		color.b = 0;
	}

	// Calculate the blur strength
	float blurStrength = 0.01 * 1280.0;

	// Apply horizontal blur
	float4 blurColor = 0;
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-0.0004, 0) * blurStrength);
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-0.003, 0) * blurStrength);
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-0.002, 0) * blurStrength);
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-0.001, 0) * blurStrength);
	blurColor += color;
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.0001, 0) * blurStrength);
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.0002, 0) * blurStrength);
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.0003, 0) * blurStrength);
	blurColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0.0004, 0) * blurStrength);
	blurColor /= 9;

	// Apply vertical blur
	float4 glowColor = 0;
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -0.0004) * blurStrength);
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -0.0003) * blurStrength);
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -0.0002) * blurStrength);
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -0.0001) * blurStrength);
	glowColor += blurColor;
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 0.0001) * blurStrength);
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 0.0002) * blurStrength);
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 0.0003) * blurStrength);
	glowColor += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, 0.0004) * blurStrength);
	glowColor /= 9;

	float4 lerpGlow = lerp(color, glowColor, 0.05);
	color.r = lerpGlow.r;
	color.b = lerpGlow.b;

	float4 lerpPixels = lerp(lerp(color, colorTrue, 0.9875), colorP,0.5);
	lerpPixels = lerpPixels * correctedColor;

	return FadeIn(FadeOut(lerpPixels));

	//return color;
	//return color * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};