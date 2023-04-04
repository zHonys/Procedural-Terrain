#version 330 core

layout (location = 0) in vec3 aPos;
//layout (location = 1) in vec2 aTexCoord;

out vec2 Pos;
//out vec2 TexCoord;

uniform sampler2D Perlin;
uniform float PerlinScalar;
uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

void main()
{
	float height = textureLod(Perlin, aPos.xz/(64*8-1), 0).y;
	Pos = vec2(aPos.xz);
	vec4 position = vec4(aPos, 1);
	position.y = height * PerlinScalar;
	gl_Position = Projection * View * Model * position;
}