#version 400

in vec3 position;
in vec2 textureCoords;
in vec3 normal;


out vec2 pass_textureCoords;
out vec3 surfaceNormal;
out vec3 toLightVector;
out vec3 toCameraVector;

uniform mat4 transformationMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform vec3 lightPosition;
uniform float useFakeLighting;

void main() 
{
     vec4 worldPosition = vec4(position, 1.0) * transformationMatrix;
	 vec4 positionRealtiveToCamera = worldPosition * viewMatrix;
 	 gl_Position =  positionRealtiveToCamera * projectionMatrix;
	 pass_textureCoords = textureCoords;

	 vec3 actualNormal = normal;
	 if (useFakeLighting > 0.5)
	 {
		actualNormal = vec3(0,1,0);
	 }

	 surfaceNormal = actualNormal * mat3(transpose(inverse(transformationMatrix)));
	 toLightVector = lightPosition - worldPosition.xyz; 
	 toCameraVector  = (inverse(viewMatrix) * vec4(0,0,0,1)).xyz - worldPosition.xyz;
}