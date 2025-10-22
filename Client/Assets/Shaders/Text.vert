#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aForegroundColor;
layout (location = 3) in vec3 aBackgroundColor;

uniform mat4 u_projection;
uniform mat4 u_model;

out vec2 vTexCoord;
out vec3 vForegroundColor;
out vec3 vBackgroundColor;

void main()
{
    gl_Position = u_projection * u_model * vec4(aPos, 1.0);
    vTexCoord = aTexCoord;
    vForegroundColor = aForegroundColor;
    vBackgroundColor = aBackgroundColor;
}