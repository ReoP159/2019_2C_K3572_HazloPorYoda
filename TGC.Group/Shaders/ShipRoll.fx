/*
* Shader con tecnicas varias utilizadas por diversas herramientas del framework,
* como: TgcBox, TgcArrow, TgcPlaneWall, TgcBoundingBox, TgcBoundingSphere, etc.
* Hay varias Techniques, una para cada combinacion utilizada en el framework de formato de vertice:
*	- PositionColoredTextured
*	- PositionTextured
*	- PositionColored
*	- PositionColoredAlpha
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float time = 0;

struct VS_INPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float2 RealPos : TEXCOORD1;
    float4 Color : COLOR0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float4 Color : COLOR0;
};

/**************************************************************************************/
/* Inmune */
/**************************************************************************************/

//Vertex Shader
VS_OUTPUT vs( VS_INPUT Input )
{
    VS_OUTPUT Output;
	//Proyectar posicion
	Output.Position = mul( Input.Position, matWorldViewProj);
	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;
	//Propago el color x vertice
	Output.Color = Input.Color;
	return( Output );
}

//Pixel Shader
float4 ps_Inmune(VS_OUTPUT Input) : COLOR0
{
    float4 fvBaseColor = tex2D(diffuseMap, Input.Texcoord);
	float4 yellow = float4(fvBaseColor.r + 1, fvBaseColor.g + 0.85, fvBaseColor.b, 0) / 2;
	float4 middle = yellow / 2;
	return middle + abs(middle * cos(time * 10));
}

/**************************************************************************************/
/* Normal */
/**************************************************************************************/

//Pixel Shader
float4 ps_Normal(VS_OUTPUT Input) : COLOR0
{
    float4 fvBaseColor = tex2D(diffuseMap, Input.Texcoord);
    return fvBaseColor;
}

/*
* Technique Inmune
*/
technique Inmune
{
	pass Pass_0
	{
	    VertexShader = compile vs_3_0 vs();
		PixelShader = compile ps_3_0 ps_Inmune();
	}
}

/*
* Technique Normal
*/
technique Normal
{
	pass Pass_0
	{
	    VertexShader = compile vs_3_0 vs();
		PixelShader = compile ps_3_0 ps_Normal();
	}
}