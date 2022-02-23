#version 400

in vec2 pass_textureCoords;
in float pass_isMultisample;
in vec3 fragPos;
in vec3 surfaceNormal;

layout(location = 0) out vec4 out_Colour;
layout(location = 1) out vec4 out_Normal;
layout(location = 2) out vec4 out_Position;
layout(location = 3) out vec4 out_Model;
layout(location = 4) out int out_InstanceId;

uniform sampler2D textureSampler;

uniform vec3 colour;
uniform float luminosity;
uniform float shineDamper;
uniform float reflectivity;
uniform int instanceId;

void main() {
		vec4 textureColour = texture(textureSampler, pass_textureCoords);
		if (textureColour.a < 0.5){
			discard;
		}

		if (pass_isMultisample > 0.5) {
			out_Colour = textureColour * vec4(colour, 1);
		} else {
			out_Position = vec4(fragPos, 1);
			out_Normal = vec4(surfaceNormal, 1);
			out_Model = vec4(luminosity, shineDamper, reflectivity, 1);
			out_InstanceId = instanceId;	
		}
}