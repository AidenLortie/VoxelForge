#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

out vec4 FragColor;

uniform sampler2D blockTexture;
uniform vec3 blockColor; // Vertex color for different block types

void main()
{
    // Sample texture
    vec4 texColor = texture(blockTexture, TexCoord);
    
    // Apply block color tint
    vec3 color = texColor.rgb * blockColor;
    
    // Simple directional lighting
    vec3 lightDir = normalize(vec3(0.5, 1.0, 0.3));
    float diff = max(dot(normalize(Normal), lightDir), 0.0);
    
    // Ambient + diffuse
    vec3 ambient = vec3(0.3);
    vec3 lighting = ambient + diff * vec3(0.7);
    
    FragColor = vec4(color * lighting, texColor.a);
}
