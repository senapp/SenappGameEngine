#version 400

in vec3 position;
in vec2 textureCoords;
in vec3 normal;

out float pass_isMultisample;
out vec2 pass_textureCoords;
out vec3 fragPos;
out vec3 surfaceNormal;

uniform mat4 transformationMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform bool useFakeLighting;
uniform bool isMultisample;

void main() 
{
     vec4 worldPosition = vec4(position, 1.0) * transformationMatrix;
	 vec4 positionRealtiveToCamera = worldPosition * viewMatrix;
 	 gl_Position = positionRealtiveToCamera * projectionMatrix;

	 pass_isMultisample = int(isMultisample);
	 if (isMultisample) {
		 pass_textureCoords = textureCoords;
	 } else {
		vec3 actualNormal = normal;
		if (useFakeLighting)
		{
			actualNormal = vec3(0,1,0);
		}

		fragPos = vec3(worldPosition);
		surfaceNormal = normalize(actualNormal * mat3(transpose(inverse(transformationMatrix))));
	 }
}