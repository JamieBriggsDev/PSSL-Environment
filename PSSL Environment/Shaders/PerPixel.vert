#version 440
in vec4 Position;
in vec3 Normal;

uniform mat4 Projection;
uniform mat4 Modelview;

uniform mat4 NormalMatrix;

out vec3 EyespaceNormal;

void main()
{
    EyespaceNormal = vec3(NormalMatrix * vec4(Normal, 1.0f));

    gl_Position = Projection * Modelview * Position;
}