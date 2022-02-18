#version 400

in vec2 textureCoords;

out float fragColour;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;

uniform vec3 samples[64];

int kernalSize = 64;
float radius = 0.5;
float bias = 0.025;

const vec2 noiseScale = vec2(1280.0/4.0, 720.0/4.0);

uniform mat4 projectionMatrix;

uniform sampler2D colourTexture;

void main(void){
	vec3 fragPos = texture(gPosition, textureCoords).xyz;
	vec3 normal = normalize(texture(gNormal, textureCoords).rgb);
	vec3 randomVec = normalize(texture(texNoise, textureCoords * noiseScale).xyz);

	vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
	vec3 bitangent = cross(normal, tangent);
	mat3 TBN = mat3(tangent, bitangent, normal);

	float occlusion = 0.0;
	for (int i = 0; i < kernalSize; ++i)
	{
		vec3 samplePoint = TBN * samples[i];
		samplePoint = fragPos + samplePoint * radius;

		vec4 offset = vec4(samplePoint, 1.0);
		offset = projectionMatrix * offset;
		offset.xyz /= offset.w;
		offset.xyz = offset.xyz * 0.5 + 0.5;

		vec3 occluderPos = texture(gPosition, offset.xy).rgb;

		float rangeCheck = smoothstep(0.0, 1.0, radius / length(fragPos - occluderPos));

		occlusion += (occluderPos.z >= samplePoint.z + bias ? 1.0 : 0.0);
	}

	fragColour = 1.0 - (occlusion / kernalSize);
}