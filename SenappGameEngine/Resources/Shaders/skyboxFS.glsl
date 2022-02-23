#version 400

in vec3 textureCoords;
layout(location = 0) out vec4 out_Colour;
layout(location = 3) out vec4 out_Model;

uniform samplerCube cubeMap;
uniform bool isColourPass;

void main()
{
	if (isColourPass) {
		out_Colour = texture(cubeMap, textureCoords);
	} else {
		out_Model = vec4(0);
	}
}