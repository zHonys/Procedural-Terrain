#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform sampler2D Perlin;

struct heightColor{
	float height;
	vec3 color;
};
uniform heightColor heightColors[7];

void main()
{
	vec4 texColor = texture(Perlin, TexCoord);
	vec4 color = texColor;
	for(int i = 0; i < 7; i++){
		if(texColor.x > heightColors[i].height){
			color = vec4(heightColors[i].color, 1);
		}
	}
	FragColor = color;
}