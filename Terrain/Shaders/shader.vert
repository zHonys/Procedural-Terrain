#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

void main()
{
	TexCoord = aTexCoord;
	gl_Position = Projection * View * Model * vec4(aPos * vec3(1, 1, -1), 1.0);
}