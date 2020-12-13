﻿#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec2 textureCoordinate;
out vec2 uvCoordinate;

uniform mat4 transformationMatrix;
uniform vec4 uniColour;
out vec4 colour;

uniform mat4 view;
uniform mat4 projection;


void main() 
{
    colour = uniColour;
	uvCoordinate = textureCoordinate;
	gl_Position = vec4(position, 1.0) * transformationMatrix * view * projection;
	

}