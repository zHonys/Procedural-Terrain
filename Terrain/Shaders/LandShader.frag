#version 330 core
out vec4 FragColor;

in vec2 Pos;
//in vec2 TexCoord;

//uniform vec2 planeSize;
uniform sampler2D Perlin;

void main()
{
	//vec4 texColor = texture(Perlin, TexCoord);
	vec2 TexCoord = Pos/(64*8-1);
	FragColor = texture(Perlin, TexCoord);
}