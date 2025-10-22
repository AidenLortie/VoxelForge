#version 330 core

in vec2 vTexCoord;
in vec3 vForegroundColor;
in vec3 vBackgroundColor;

out vec4 FragColor;
uniform sampler2D uTexture;

void main()
{
    float alpha = texture(uTexture, vTexCoord).a;
    vec3 color = mix(vBackgroundColor, vForegroundColor, alpha);
    FragColor = vec4(color, 1.0);
}