#version 130
in vec4 Position;
in vec3 Normal;
in vec2 TexCoord;

uniform mat4 Projection;
uniform mat4 Modelview;
uniform mat3 NormalMatrix;
uniform vec3 DiffuseMaterial;

out vec3 EyespaceNormal;
out vec3 Diffuse;
out vec2 TexCoordV;

void main()
{
    EyespaceNormal = NormalMatrix * Normal;
    gl_Position = Projection * Modelview * Position;
    Diffuse = DiffuseMaterial;

	TexCoordV = TexCoord;
}