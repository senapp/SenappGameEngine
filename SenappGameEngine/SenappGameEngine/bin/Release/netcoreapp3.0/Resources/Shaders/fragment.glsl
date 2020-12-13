#version 330
out vec4 out_Colour;
in vec2 uvCoordinate;
in vec4 colour;

uniform sampler2D textureSampler;

void main() {
	vec4 color = texture2D(textureSampler, uvCoordinate.xy);
	 if (color.a<0.5){
		   discard;
	   }
	if(colour != vec4(0,0,0,0) && color != vec4(0,0,0,1))
	{
		out_Colour = (colour * color) / 2;
	}
	else if(colour != vec4(0,0,0,0))
	{
		out_Colour = colour;
	}
	else if (color == vec4(0,0,0,1))
	{
		out_Colour = vec4(1,0,0,1);
	}
	else
	{
		out_Colour = color;	
	}
}