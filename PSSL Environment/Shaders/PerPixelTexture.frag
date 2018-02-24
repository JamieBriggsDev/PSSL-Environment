#version 130
in vec3 EyespaceNormal;
in vec3 Diffuse;
in vec2 TexCoordV;
out vec4 FragColor;

uniform sampler2D Texture;
uniform vec3 LightPosition;
uniform vec3 AmbientMaterial;
uniform vec3 SpecularMaterial;
uniform float Alpha;
uniform float Shininess;

void main()
{
    vec3 N = normalize(EyespaceNormal);
    vec3 L = normalize(LightPosition);
    vec3 E = vec3(0, 0, 1);
    vec3 H = normalize(L + E);

	vec2 invTexCoordV = vec2(TexCoordV.x, 1 - TexCoordV.y);
    
    float df = max(0.0, dot(N, L));
    float sf = max(0.0, dot(N, H));
    sf = pow(sf, Shininess);

    vec3 color = AmbientMaterial + df * Diffuse + sf * SpecularMaterial;
	FragColor = texture2D(Texture, invTexCoordV) * vec4(color, Alpha);
	//FragColor = vec4(1, TexCoordV, Alpha);
    //FragColor = vec4(texture, Alpha);
}
