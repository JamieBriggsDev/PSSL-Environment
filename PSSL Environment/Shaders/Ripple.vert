#version 130
in vec4 Position;
in vec3 Normal;
in vec2 TexCoord;

uniform mat4 Modelview;
uniform mat4 Projection;
uniform mat4 NormalMatrix;
uniform vec3 DiffuseMaterial;

uniform float Time;
uniform float Speed;
uniform float Frequency;
uniform float Amplitude;

out vec3 EyespaceNormal;
out vec3 Diffuse;
out vec2 TexCoordV;

// simple vertex shader

void main()
{
    EyespaceNormal = vec3(NormalMatrix * vec4(Normal, 1.0f));

    Diffuse = DiffuseMaterial;

	vec3 offset = normalize(
		vec3(0.0) - vec3(Position)) * 
		(Amplitude * sin( Speed * Time + Position.y * Frequency));
		
	vec3 newPosition = vec3(Position) + vec3(offset.x, 0.0, offset.z);

	gl_Position = Projection * Modelview * vec4(newPosition, 1.0);

	//gl_Position    = gl_ModelViewProjectionMatrix * gl_Vertex;

	TexCoordV = TexCoord;
	//gl_FrontColor  = gl_Color;
	//gl_TexCoord[0] = gl_MultiTexCoord0;
}