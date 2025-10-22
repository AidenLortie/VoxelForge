#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec4 aForegroundColor;
layout (location = 3) in vec4 aBackgroundColor;

uniform mat4 u_projection;
uniform mat4 u_model;

out vec2 vTexCoord;
out vec4 vForegroundColor;
out vec4 vBackgroundColor;

void main()
{
    gl_Position = projection * model * vec4(aPos, 1.0);
    TexCoord = aTexCoord;
    ForegroundColor = aForegroundColor;
    BackgroundColor = aBackgroundColor;
}