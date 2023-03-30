#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

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

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(SpriteTextureSampler,input.TextureCoordinates);
	if (color.a == 1)
	{
		float3 maxBright = normalizeSaturation(color) * 1.0;
		color.r = maxBright.r;
		color.b = maxBright.b;
		color.g = maxBright.g;
	}

	float3 colorA = color.rgb;// / max(max(color.r, color.g), color.b);
	float3 colorB = input.Color.rgb;// / max(max(input.Color.r, input.Color.g), input.Color.b);
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
	//color = color * 0.5;

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

	// Combine the glow color with the original color
	return color;

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