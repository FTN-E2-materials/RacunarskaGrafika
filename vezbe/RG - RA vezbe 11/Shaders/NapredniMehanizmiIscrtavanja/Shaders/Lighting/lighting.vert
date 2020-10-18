#version 330 core
layout (location = 0) in vec3 in_position;
layout (location = 1) in vec3 in_normal;

out vec3 Normal;
out vec3 FragPos;

uniform mat4 modelMat;
uniform mat4 viewMat;
uniform mat4 projectionMat;

void main()
{
    gl_Position = projectionMat * viewMat *  modelMat * vec4(in_position, 1.0f);
    FragPos = vec3(modelMat * vec4(in_position, 1.0f));
    Normal = mat3(transpose(inverse(modelMat))) * in_normal;
}