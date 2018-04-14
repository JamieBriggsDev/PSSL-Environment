#version 130
in vec3 EyespaceNormal;
in vec3 Diffuse;
in vec2 TexCoordV;
out vec4 FragColor;

uniform float Time;
uniform vec3 LightPosition;
uniform vec3 AmbientMaterial;
uniform vec3 SpecularMaterial;
uniform float Alpha;
uniform float Shininess;
uniform sampler2D tex;
uniform sampler2D tex2;

float radius = .5;

void main()
{
    vec3 N = normalize(EyespaceNormal);
    vec3 L = normalize(LightPosition);
    vec3 E = vec3(0, 0, 1);
    vec3 H = normalize(L + E);
    
    float df = max(0.0, dot(N, L));
    float sf = max(0.0, dot(N, H));
    sf = pow(sf, Shininess);

    vec3 color = AmbientMaterial + df * Diffuse + sf * SpecularMaterial;

	float t = clamp(Time / 6., 0., 1.);

	//vec2 coords = gl_TexCoord[0].st;
	vec2 dir = TexCoordV - vec2(.5);
	
	float dist = distance(TexCoordV, vec2(.5));
	vec2 offset = dir * (sin(dist * 80. - Time*15.) + .5) / 30.;

	vec2 texCoord = TexCoordV + offset;
	vec4 diffuseT = texture2D(tex, texCoord);

	vec4 mixin = texture2D(tex2, texCoord);

 	FragColor = mixin * t + diffuseT * (1. - t) * vec4(color, Alpha);;
}
