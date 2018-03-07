#version 130
in vec4 Position;
in vec3 Normal;
in vec2 TexCoord;

uniform mat4 Projection;
uniform mat4 Modelview;
uniform mat3 NormalMatrix;
uniform vec3 DiffuseMaterial;
//uniform float time;

out vec3 EyespaceNormal;
out vec3 Diffuse;
out vec2 TexCoordV;

// simple vertex shader

void main()
{
    EyespaceNormal = NormalMatrix * Normal;
    gl_Position = Projection * Modelview * Position;
    Diffuse = DiffuseMaterial;

	//gl_Position    = gl_ModelViewProjectionMatrix * gl_Vertex;

	TexCoordV = TexCoord;
	//gl_FrontColor  = gl_Color;
	//gl_TexCoord[0] = gl_MultiTexCoord0;
}