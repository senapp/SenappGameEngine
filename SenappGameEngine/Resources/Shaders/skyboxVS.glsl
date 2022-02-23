#version 400

in vec3 position;
out vec3 textureCoords;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 transformationMatrix;


void main() 
{
 	 gl_Position = vec4(position, 1.0) * transformationMatrix * viewMatrix * projectionMatrix;
	 textureCoords = position;
}