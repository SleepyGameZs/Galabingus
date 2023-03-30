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
	if ((color.r + color.b + color.g) * color.a < 0.3)
	{
		return color;
	}
	if ((color.r + color.b + color.g) * color.a > 1.732050807568877293527446341505872366)
	{
		return newColor + color;
	}
	else
	{
		return normalize(color) * 1.5;
	}
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
	float3 color2 = lerp(colorA, colorB, 0.973 * color.a);
	color.r = color.r * color2.r;
	color.g = color.g * color2.g;
	color.b = color.b * color2.b;
	color.a = color.a * input.Color.a;
	if (color.a == 0)
	{
		color.r = 0;
		color.g = 0;
		color.b = 0;
	}
	//color = color * 0.5;
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};