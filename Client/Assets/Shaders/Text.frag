#version 330 core

in vec2 vTexCoords;
in vec4 vForegroundColor;
in vec4 vBackgroundColor;

out vec4 FragColor;
uniform sampler2D uTexture;

void main()
{
    float alpha = texture(uTexture, vTexCoords).a;
    FragColor = mix(vBackgroundColor, vForegroundColor, alpha);
}