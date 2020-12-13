#version 400

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector;
in vec3 toCameraVector;

out vec4 out_Colour;

uniform sampler2D textureSampler;
uniform vec3 lightColour;
uniform float shineDamper;
uniform float reflectivity;
uniform float luminosity;

void main() {
		vec3 unitNormal = normalize(surfaceNormal);
		vec3 unitLightVector = normalize(toLightVector);

		float nDotl = dot(unitLightVector, unitNormal);
		float brightness = max(nDotl, luminosity);
		vec3 diffuse = brightness * lightColour;	

		vec3 finalSpecular = vec3(0,0,0);
		if (nDotl > 0) {
			float specularFactor = max(nDotl, 0);
			finalSpecular = pow(specularFactor, shineDamper) * reflectivity * lightColour;
		}	

		vec4 textureColour = texture(textureSampler, pass_textureCoords);
		if (textureColour.a <0.5) {
			discard;
		}

		out_Colour = vec4(diffuse, 1) * textureColour + vec4(finalSpecular, 1);
}