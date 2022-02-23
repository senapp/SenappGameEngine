#version 400

in vec2 textureCoords;

out vec4 out_Colour;

uniform vec3 lightPosition;
uniform vec3 cameraPosition;

uniform vec3 lightColour;

uniform sampler2D colourTexture;
uniform sampler2D normalTexture;
uniform sampler2D positionTexture;
uniform sampler2D modelsTexture;

uniform samplerCube enviroMap;

void main(void){
	vec4 textureColour = texture(colourTexture, textureCoords);
	vec4 worldPosition = texture(positionTexture, textureCoords);
	vec4 surfaceNormal = texture(normalTexture, textureCoords);
	vec4 model = texture(modelsTexture, textureCoords);

	// Is part of skybox
	if (model.w < 0.5) {
		discard;
	}

	float luminosity = model.x;
	float shineDamper = model.y;
	float reflectivity = model.z;
	
	vec3 toLightVector = lightPosition - worldPosition.xyz; 
	vec3 viewVector = normalize(worldPosition.xyz - cameraPosition);
	vec3 reflectedVector = reflect(viewVector, surfaceNormal.xyz);
	
	vec3 unitLightVector = normalize(toLightVector);
	
	float nDotl = dot(unitLightVector, surfaceNormal.xyz);
	float brightness = max(nDotl, luminosity);
	vec3 diffuse = brightness * lightColour;	
	
	vec3 finalSpecular = vec3(0,0,0);
	if (nDotl > 0) {
		float specularFactor = max(nDotl, 0);
		finalSpecular = pow(specularFactor, shineDamper) * reflectivity * lightColour;
	}	
	
	vec4 reflectedColour = texture(enviroMap, reflectedVector);

	out_Colour = textureColour * vec4(diffuse, 1) + vec4(finalSpecular, 1);
}