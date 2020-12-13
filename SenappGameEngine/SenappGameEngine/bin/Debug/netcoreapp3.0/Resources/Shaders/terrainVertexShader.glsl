#version 400

in vec3 position;
in vec2 textureCoords;
in vec3 normal;


out vec2 pass_textureCoords;
out vec3 surfaceNormal;
out vec3 toLightVector;
out vec3 toCameraVector;
out float visibility;

uniform mat4 transformationMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform vec3 lightPosition;

const float density = 0.007;
const float gradient =  1.5;

void main() 
{
     vec4 worldPosition = vec4(position, 1.0) * transformationMatrix;
	 vec4 positionRealtiveToCamera =worldPosition * viewMatrix;
 	 gl_Position =  positionRealtiveToCamera * projectionMatrix;
	 pass_textureCoords = textureCoords;

	 surfaceNormal = (vec4(normal,0) * transformationMatrix).xyz;
	 toLightVector = lightPosition - worldPosition.xyz; 
	 toCameraVector  = (inverse(viewMatrix) * vec4(0,0,0,1)).xyz - worldPosition.xyz;

	  float distance = length(positionRealtiveToCamera.xyz);
	 visibility = exp(-pow((distance*density),gradient));
	 visibility = clamp(visibility,0,1);
}