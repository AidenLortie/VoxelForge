#version 330 core

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoord;

out vec4 FragColor;

void main()
{
    // Use UV coordinates to determine vertex color (as per user request)
    // This creates a colorful pattern based on texture coordinates
    vec3 color = vec3(TexCoord.x, TexCoord.y, 0.5);
    
    // Simple directional lighting
    vec3 lightDir = normalize(vec3(0.5, 1.0, 0.3));
    float diff = max(dot(normalize(Normal), lightDir), 0.0);
    
    // Ambient + diffuse
    vec3 ambient = vec3(0.3);
    vec3 lighting = ambient + diff * vec3(0.7);
    
    FragColor = vec4(color * lighting, 1.0);
}
