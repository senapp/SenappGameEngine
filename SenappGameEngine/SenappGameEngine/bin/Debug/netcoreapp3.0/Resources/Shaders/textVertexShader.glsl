#version 400

in vec3 position;
in vec2 textureCoords;
in vec3 normal;


out vec2 pass_textureCoords;

uniform mat4 transformationMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;


void main() 
{
     vec4 worldPosition = vec4(position, 1.0) * transformationMatrix;
 	 gl_Position =  worldPosition * viewMatrix * projectionMatrix;
	 pass_textureCoords = textureCoords;
}