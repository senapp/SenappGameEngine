#version 400

in vec3 textureCoords;
out vec4 out_Colour;

uniform samplerCube cubeMap;

void main(){
	out_Colour = texture(cubeMap, textureCoords);
}