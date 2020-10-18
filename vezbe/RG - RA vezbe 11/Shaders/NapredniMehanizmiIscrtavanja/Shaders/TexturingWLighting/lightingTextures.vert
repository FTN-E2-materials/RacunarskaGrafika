#version 330 core
layout (location = 0) in vec3 in_position;
layout (location = 1) in vec3 in_normal;
layout (location = 2) in vec2 in_texCoords;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

uniform mat4 modelMat;
uniform mat4 viewMat;
uniform mat4 projectionMat;

void main()
{
    gl_Position = projectionMat * viewMat *  modelMat * vec4(in_position, 1.0);
    FragPos = vec3(modelMat * vec4(in_position, 1.0));
    Normal = mat3(transpose(inverse(modelMat))) * in_normal;
    TexCoords = in_texCoords;
}