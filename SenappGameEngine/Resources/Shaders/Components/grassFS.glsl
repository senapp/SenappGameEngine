#version 400

layout(location = 0) out vec4 out_Colour;
layout(location = 1) out vec4 out_Normal;
layout(location = 2) out vec4 out_Position;
layout(location = 3) out vec4 out_Model;

uniform vec3 lightPosition;
uniform vec3 cameraPosition;
uniform vec3 lightColour;

in vec2 pass_textureCoords;
in float pass_Height;
in float pass_Width;
in vec3 surfaceNormal;
in vec3 fragPos;

uniform sampler2D textureSampler;
uniform bool isColourPass;

float easeInQuad(float x, float power)
{
   return pow(x, power);
}

void main(){
	    vec4 textureColour = texture(textureSampler, pass_textureCoords);
		if (textureColour.a < 0.5){
			discard;
		}

		if (isColourPass) {
			float luminosity = 0.3;
			float shineDamper = 1;
			float reflectivity = 0.01;
			
			vec3 toLightVector = lightPosition - fragPos; 
			vec3 viewVector = normalize(fragPos - cameraPosition);
			vec3 reflectedVector = reflect(viewVector, surfaceNormal.xyz);
			
			vec3 unitLightVector = normalize(toLightVector);
			
			float nDotl = dot(unitLightVector, surfaceNormal.xyz);
			float ao = mix(0.3, 1.0, pass_Height);
			float brightness = max(nDotl, luminosity * ao);
			vec3 diffuse = brightness * lightColour;	

			vec3 finalSpecular = vec3(0,0,0);
			if (nDotl > 0) {
				float specularFactor = max(nDotl, 0);
				finalSpecular = pow(specularFactor, shineDamper) * reflectivity * lightColour;
			}	
			
			// vec3 bottomColour = vec3(0.5, 0.5, 0.1); //vec3 bottomColour = vec3(0.05, 0.2, 0.01);
			// vec3 topColour = vec3(0.5, 0.5, 0.1);
			// textureColour = vec4(mix(bottomColour, topColour, easeInQuad(pass_Height, 2)), 1);
			out_Colour = textureColour * vec4(diffuse, 1) + vec4(finalSpecular, 1);

		} else {

			float aoForDensity = mix(1.0, 0.25, 0.2);
			float ao = mix(0.0, 1.0, easeInQuad(pass_Height, 2));
			
			out_Normal = vec4(surfaceNormal, 1);
			out_Position = vec4(fragPos, 1);
			out_Model = vec4(0.1 * ao, 0.2, 0.001, 1);
		}
}