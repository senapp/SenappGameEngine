#version 400

in vec2 pass_textureCoords;

out vec4 out_Colour;

uniform sampler2D textureSampler;
uniform vec4 colour;
uniform vec4 colourEdge;
uniform float colourMixRatio;
uniform float edgeRatio;
uniform float roundnessRatio;
uniform vec2 screenSize;



void main(){
		vec4 colourBase = texture(textureSampler, pass_textureCoords);
		if (colourBase.a < 0.5){
			discard;
		}
		if (false)
		{
		
		vec2 pos = (gl_FragCoord.xy - 0.5) / screenSize; 
		float distanceVal = roundnessRatio;
		bool isInCorner = false;
		if (pos.x < roundnessRatio && pos.y < roundnessRatio)
		{
			distanceVal = distance(pos, vec2(roundnessRatio, roundnessRatio));
			isInCorner = true;
		}
		if (isInCorner){
		float x = roundnessRatio - distanceVal;
		if (x < 0){
			discard;
		}
		}
		}
		out_Colour = mix(colourBase, colour, colourMixRatio); 

}