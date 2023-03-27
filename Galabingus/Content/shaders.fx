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

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
	if (color.a == 1)
	{
		float4 maxBright = normalize(color) * 2;//1.732050807568877293527446341505872366;
		color.r = maxBright.r;
		color.b = maxBright.b;
		color.g = maxBright.g;
	}
	else
	{
		float tempA = color.a;
		color.r = color.r;
		color.g = color.g;
		color.b = color.b;
		//color.a = 1;
		float4 maxBright = normalize(color) * 2;//1.732050807568877293527446341505872366;
		//color.r = maxBright.r;// *tempA;
		//color.b = maxBright.b;// *tempA;
		//color.g = maxBright.g;// *tempA;
		//color.a = tempA;
	}
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};