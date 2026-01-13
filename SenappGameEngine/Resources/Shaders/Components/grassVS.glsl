#version 400

in vec3 position;
in vec2 textureCoords;
in vec3 normal;

out vec2 pass_textureCoords;
out float pass_Height;
out float pass_Width;
out vec3 surfaceNormal;
out vec3 fragPos;

uniform mat4 transformationMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

uniform int grassX;
uniform int grassY;
uniform float time;

float rand(vec2 co)
{
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

mat3 rotateX(float theta) {
    float c = cos(theta);
    float s = sin(theta);
    return mat3(
        vec3(1, 0, 0),
        vec3(0, c, -s),
        vec3(0, s, c)
    );
}

// Rotation matrix around the Y axis.
mat3 rotateY(float theta) {
    float c = cos(theta);
    float s = sin(theta);
    return mat3(
        vec3(c, 0, s),
        vec3(0, 1, 0),
        vec3(-s, 0, c)
    );
}

// Rotation matrix around the Z axis.
mat3 rotateZ(float theta) {
    float c = cos(theta);
    float s = sin(theta);
    return mat3(
        vec3(c, -s, 0),
        vec3(s, c, 0),
        vec3(0, 0, 1)
    );
}

mat3 rotateAxis(vec3 axis, float angle) {
  axis = normalize(axis);
  float s = sin(angle);
  float c = cos(angle);
  float oc = 1.0 - c;

  return mat3(
    oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
    oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
    oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c
  );
}

float saturate (float val) {
    return clamp(val, 0, 1);
}

float easeOutQuad(float x)
{
  return 1.0 - (1.0-x) * (1.0 -x );
}

float easeInQuad(float x, float power)
{
   return pow(x, power);
}
// float noise(vec2 n) {
// 	const vec2 d = vec2(0.0, 1.0);
//   vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
// 	return mix(mix(rand(b), rand(b + d.yx), f.x), mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
// }

uint murmurHash12(uvec2 src) {
  const uint M = 0x5bd1e995u;
  uint h = 1190494759u;
  src *= M; src ^= src>>24u; src *= M;
  h *= M; h ^= src.x; h *= M; h ^= src.y;
  h ^= h>>13u; h *= M; h ^= h>>15u;
  return h;
}

float hash12(vec2 src) {
  uint h = murmurHash12(floatBitsToUint(src));
  return uintBitsToFloat(h & 0x007fffffu | 0x3f800000u) - 1.0;
}

float noise(vec2 p) {
  vec2 i = floor(p);

  vec2 f = fract(p);
  vec2 u = smoothstep(vec2(0.0), vec2(1.0), f);

	float val = mix( mix( hash12( i + vec2(0.0, 0.0) ), 
                        hash12( i + vec2(1.0, 0.0) ), u.x),
                   mix( hash12( i + vec2(0.0, 1.0) ), 
                        hash12( i + vec2(1.0, 1.0) ), u.x), u.y);
  return val * 2.0 - 1.0;
}

float remap(float value, float min1, float max1, float min2, float max2) {
  return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}


vec3 bezier(vec3 A, vec3 B, vec3 C, vec3 D, float t) {
  vec3 E = mix(A, B, t);
  vec3 F = mix(B, C, t);
  vec3 G = mix(C, D, t);

  vec3 H = mix(E, F, t);
  vec3 I = mix(F, G, t);

  vec3 P = mix(H, I, t);

  return P;
}

void main() 
{
     float xCoord = gl_InstanceID - (grassX * floor(gl_InstanceID/grassX));
     float zCoord = floor(gl_InstanceID / grassY);

     float rand_1 = hash12(vec2(xCoord, zCoord));
     float rand_2 = hash12(vec2(-zCoord, -xCoord));
     float rand_3 = hash12(vec2(-xCoord, zCoord));
     float rand_4 = hash12(vec2(xCoord, -zCoord));
     
     float rand_5 = hash12(vec2(rand_1, rand_2));
     float rand_6 = hash12(vec2(rand_3, rand_4));

     float PI = 3.14159;

     float height = mix(0.0, 2.0, rand_1);
     float heightPercentage = position.y / 4.5; // Based on vertice max y
     float widthPercentage = (position.x + 0.3) / 0.6; // Based on vertice max x

     // vec3 viewDir = vec3(viewMatrix[2][0], viewMatrix[2][1], viewMatrix[2][2]);
     // float viewDotNormal = saturate(dot(newNormal.xz, viewDir.xz));
     // 
     // float viewSpaceThickenFactor = easeOutQuad(1.0 - viewDotNormal); // 4.0);?
     // viewSpaceThickenFactor *= smoothstep(0.0, 0.2, viewDotNormal);
     // 
     // vec4 myPosition = viewMatrix * vec4(position, 1);
     // myPosition.x += viewSpaceThickenFactor * vec4(1, 0, 0, 1) * 0.8; 

     vec3 rotatedNormal1 = rotateY(PI * 0.3) * normal;
     vec3 rotatedNormal2 = rotateY(PI * -0.3) * normal;
     
     float normalMixFactor = widthPercentage;
     vec3 newNormal = mix(rotatedNormal1, rotatedNormal2, normalMixFactor);
     newNormal = normalize(newNormal);
           
     vec3 curveAmount = bezier(vec3(0), vec3(0.2, 0.5, 0), vec3(0.6, 0.8, 0), vec3(1, 0.8, 0), heightPercentage) * mix(0.6, 0.9, rand_2);


     float windDir = noise(vec2(xCoord, zCoord) * 0.05 + 0.05 * time);
     float windNoiseSample = noise(vec2(xCoord, zCoord) * 0.25 + time);
     float windLeanAngle = remap(windNoiseSample, -1.0, 1.0, 0.25, 1.0);
     windLeanAngle = easeInQuad(windLeanAngle, 2.0) * 1.25;
     vec3 windAxis = vec3(sin(windDir), 0.0, -cos(windDir));
     windLeanAngle *= heightPercentage;

     mat3 turnMat = rotateX(curveAmount.x);

     float randomAngle = rand_3 * PI * 2;
     mat3 rotation = rotateY(randomAngle);

     mat3 grassMat = rotateAxis(windAxis, windLeanAngle) * rotation;

     vec3 basePos = turnMat * vec3(position.x, position.y, position.z);
     basePos = grassMat * basePos;

     float x = (xCoord - grassX / 2) * 1000 / grassX; //+ rand_5 * 4 - 1;
     float z = (zCoord - grassY / 2) * 1000 / grassY; // + rand_6 * 4 - 1;
     vec4 pos = vec4(basePos.x + x, basePos.y + height * heightPercentage, basePos.z + z, 1);

     vec4 worldPosition = pos * transformationMatrix;
 	 gl_Position = worldPosition * viewMatrix * projectionMatrix;

     pass_textureCoords = textureCoords;
     pass_Height = heightPercentage;
     pass_Width = widthPercentage;

	 surfaceNormal = newNormal;
	 fragPos = vec3(worldPosition);
}
