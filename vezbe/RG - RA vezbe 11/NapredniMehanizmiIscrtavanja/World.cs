using System;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using GlmNet;
using System.Drawing;
using System.Drawing.Imaging;

namespace NapredniMehanizmiIscrtavanja
{
    class World : IDisposable
    {
        #region Attributes

        private float cubesRotationX;
        private float cubesRotationY;

        vec3 camPos = new vec3(0f, 5f, 5f);

        mat4 projectionMatrix;
        mat4 viewMatrix;

        mat4 modelMatrixCube;
        mat4 modelMatrixTexturedCube;
        mat4 modelMatrixSpecularMapCube;
        mat4 modelMatrixLamp;

        const uint attributeIndexPosition = 0;
        const uint attributeIndexColor = 1;

        uint[] textureIds;

        VertexBufferArray vertexBufferArrayCubeLighting;
        VertexBufferArray vertexBufferArrayCubeTextureLighting;
        VertexBufferArray vertexBufferArrayLamp;

        private ShaderProgram shaderProgramCube;
        private ShaderProgram shaderProgramTexturedLightedCube;
        private ShaderProgram shaderProgramTexturedLightedCubeSpecularMap;
        private ShaderProgram shaderProgramLamp;

        private OpenGL gl;

        #region Cube
        float[] cubeVertices =
        {
        // Positions            
        -0.5f, -0.5f, -0.5f,    
        0.5f, -0.5f, -0.5f,     
        0.5f,  0.5f, -0.5f,     
        0.5f,  0.5f, -0.5f,     
        -0.5f,  0.5f, -0.5f,    
        -0.5f, -0.5f, -0.5f,    

        -0.5f, -0.5f,  0.5f,    
        0.5f, -0.5f,  0.5f,     
        0.5f,  0.5f,  0.5f,     
        0.5f,  0.5f,  0.5f,     
        -0.5f,  0.5f,  0.5f,    
        -0.5f, -0.5f,  0.5f,    

        -0.5f,  0.5f,  0.5f,    
        -0.5f,  0.5f, -0.5f,    
        -0.5f, -0.5f, -0.5f,    
        -0.5f, -0.5f, -0.5f,    
        -0.5f, -0.5f,  0.5f,    
        -0.5f,  0.5f,  0.5f,    

        0.5f,  0.5f,  0.5f,     
        0.5f,  0.5f, -0.5f,     
        0.5f, -0.5f, -0.5f,     
        0.5f, -0.5f, -0.5f,     
        0.5f, -0.5f,  0.5f,     
        0.5f,  0.5f,  0.5f,     

        -0.5f, -0.5f, -0.5f,    
        0.5f, -0.5f, -0.5f,     
        0.5f, -0.5f,  0.5f,     
        0.5f, -0.5f,  0.5f,     
        -0.5f, -0.5f,  0.5f,    
        -0.5f, -0.5f, -0.5f,    

        -0.5f,  0.5f, -0.5f,    
        0.5f,  0.5f, -0.5f,     
        0.5f,  0.5f,  0.5f,     
        0.5f,  0.5f,  0.5f,     
        -0.5f,  0.5f,  0.5f,    
        -0.5f,  0.5f, -0.5f,    
    };

        float[] cubeNormals =
        {
          // Normals            
          0.0f,  0.0f, -1.0f,
          0.0f,  0.0f, -1.0f,
          0.0f,  0.0f, -1.0f,
          0.0f,  0.0f, -1.0f,
          0.0f,  0.0f, -1.0f,
          0.0f,  0.0f, -1.0f,

          0.0f,  0.0f,  1.0f,
          0.0f,  0.0f,  1.0f,
          0.0f,  0.0f,  1.0f,
          0.0f,  0.0f,  1.0f,
          0.0f,  0.0f,  1.0f,
          0.0f,  0.0f,  1.0f,

          -1.0f,  0.0f,  0.0f,
          -1.0f,  0.0f,  0.0f,
          -1.0f,  0.0f,  0.0f,
          -1.0f,  0.0f,  0.0f,
          -1.0f,  0.0f,  0.0f,
          -1.0f,  0.0f,  0.0f,

          1.0f,  0.0f,  0.0f,
          1.0f,  0.0f,  0.0f,
          1.0f,  0.0f,  0.0f,
          1.0f,  0.0f,  0.0f,
          1.0f,  0.0f,  0.0f,
          1.0f,  0.0f,  0.0f,

          0.0f, -1.0f,  0.0f,
          0.0f, -1.0f,  0.0f,
          0.0f, -1.0f,  0.0f,
          0.0f, -1.0f,  0.0f,
          0.0f, -1.0f,  0.0f,
          0.0f, -1.0f,  0.0f,

          0.0f,  1.0f,  0.0f,
          0.0f,  1.0f,  0.0f,
          0.0f,  1.0f,  0.0f,
          0.0f,  1.0f,  0.0f,
          0.0f,  1.0f,  0.0f,
          0.0f,  1.0f,  0.0f,
          };

        float[] cubeTextureCoords =
        {
            // Texture Coords
            0.0f,  0.0f, 
            1.0f,  0.0f, 
            1.0f,  1.0f, 
            1.0f,  1.0f, 
            0.0f,  1.0f, 
            0.0f,  0.0f, 

            0.0f,  0.0f, 
            1.0f,  0.0f, 
            1.0f,  1.0f, 
            1.0f,  1.0f, 
            0.0f,  1.0f, 
            0.0f,  0.0f, 

            1.0f,  0.0f, 
            1.0f,  1.0f, 
            0.0f,  1.0f, 
            0.0f,  1.0f, 
            0.0f,  0.0f, 
            1.0f,  0.0f, 

            1.0f,  0.0f, 
            1.0f,  1.0f, 
            0.0f,  1.0f, 
            0.0f,  1.0f, 
            0.0f,  0.0f, 
            1.0f,  0.0f, 

            0.0f,  1.0f, 
            1.0f,  1.0f, 
            1.0f,  0.0f, 
            1.0f,  0.0f, 
            0.0f,  0.0f, 
            0.0f,  1.0f, 

            0.0f,  1.0f, 
            1.0f,  1.0f, 
            1.0f,  0.0f, 
            1.0f,  0.0f, 
            0.0f,  0.0f, 
            0.0f,  1.0f 
        };
        #endregion

        #endregion

        #region Properties

        public float CubesRotationX
        {
            get { return cubesRotationX; }
            set
            {
                cubesRotationX = value;
                modelMatrixCube = glm.rotate(new mat4(1.0f), glm.radians(cubesRotationX), new vec3(1f, 0f, 0f));
                modelMatrixCube = glm.rotate(modelMatrixCube, glm.radians(cubesRotationY), new vec3(0f, 1f, 0f));
                modelMatrixCube = glm.scale(modelMatrixCube, new vec3(3f));

                modelMatrixTexturedCube = glm.translate(new mat4(1.0f), new vec3(-5f, 0f, -3f));
                modelMatrixTexturedCube = glm.rotate(modelMatrixTexturedCube, glm.radians(cubesRotationX), new vec3(1f, 0f, 0f));
                modelMatrixTexturedCube = glm.rotate(modelMatrixTexturedCube, glm.radians(cubesRotationY), new vec3(0f, 1f, 0f));
                modelMatrixTexturedCube = glm.scale(modelMatrixTexturedCube, new vec3(3f));

                modelMatrixSpecularMapCube = glm.translate(new mat4(1.0f), new vec3(5f, 0f, -3f));
                modelMatrixSpecularMapCube = glm.rotate(modelMatrixSpecularMapCube, glm.radians(cubesRotationX), new vec3(1f, 0f, 0f));
                modelMatrixSpecularMapCube = glm.rotate(modelMatrixSpecularMapCube, glm.radians(cubesRotationY), new vec3(0f, 1f, 0f));
                modelMatrixSpecularMapCube = glm.scale(modelMatrixSpecularMapCube, new vec3(3f));
            }
        }

        public float CubesRotationY
        {
            get { return cubesRotationY; }
            set
            {
                cubesRotationY = value;
                modelMatrixCube = glm.rotate(new mat4(1.0f), glm.radians(cubesRotationX), new vec3(1f, 0f, 0f));
                modelMatrixCube = glm.rotate(modelMatrixCube, glm.radians(cubesRotationY), new vec3(0f, 1f, 0f));
                modelMatrixCube = glm.scale(modelMatrixCube, new vec3(3f));

                modelMatrixTexturedCube = glm.translate(new mat4(1.0f), new vec3(-5f, 0f, -3f));
                modelMatrixTexturedCube = glm.rotate(modelMatrixTexturedCube, glm.radians(cubesRotationX), new vec3(1f, 0f, 0f));
                modelMatrixTexturedCube = glm.rotate(modelMatrixTexturedCube, glm.radians(cubesRotationY), new vec3(0f, 1f, 0f));
                modelMatrixTexturedCube = glm.scale(modelMatrixTexturedCube, new vec3(3f));

                modelMatrixSpecularMapCube = glm.translate(new mat4(1.0f), new vec3(5f, 0f, -3f));
                modelMatrixSpecularMapCube = glm.rotate(modelMatrixSpecularMapCube, glm.radians(cubesRotationX), new vec3(1f, 0f, 0f));
                modelMatrixSpecularMapCube = glm.rotate(modelMatrixSpecularMapCube, glm.radians(cubesRotationY), new vec3(0f, 1f, 0f));
                modelMatrixSpecularMapCube = glm.scale(modelMatrixSpecularMapCube, new vec3(3f));
            }
        }

        #endregion

        #region Methods
        public void Initialize(OpenGL gl, float width, float height)
        {
            this.gl = gl;
            gl.ClearColor(0.2f, 0.3f, 0.2f, 0.0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            //  Create the shader programs.
            var vertexShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/Lighting/lighting.vert");
            var fragmentShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/Lighting/lighting.frag");
            shaderProgramCube = new ShaderProgram();
            shaderProgramCube.Create(gl, vertexShaderSource, fragmentShaderSource, null);
            shaderProgramCube.BindAttributeLocation(gl, attributeIndexPosition, "in_Position");
            shaderProgramCube.BindAttributeLocation(gl, attributeIndexColor, "in_Color");
            shaderProgramCube.AssertValid(gl);

            vertexShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/Lamp/Lampshader.vert");
            fragmentShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/Lamp/Lampshader.frag");
            shaderProgramLamp = new ShaderProgram();
            shaderProgramLamp.Create(gl, vertexShaderSource, fragmentShaderSource, null);
            shaderProgramLamp.BindAttributeLocation(gl, attributeIndexPosition, "in_Position");
            shaderProgramLamp.AssertValid(gl);

            vertexShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/TexturingWLighting/lightingTextures.vert");
            fragmentShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/TexturingWLighting/lightingTextures.frag");
            shaderProgramTexturedLightedCube = new ShaderProgram();
            shaderProgramTexturedLightedCube.Create(gl, vertexShaderSource, fragmentShaderSource, null);
            shaderProgramTexturedLightedCube.BindAttributeLocation(gl, attributeIndexPosition, "in_Position");
            shaderProgramTexturedLightedCube.AssertValid(gl);

            vertexShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/TexturingWLightingSpecularMaps/lightingSpecular.vert");
            fragmentShaderSource = ManifestResourceLoader.LoadTextFile("Shaders/TexturingWLightingSpecularMaps/lightingSpecular.frag");
            shaderProgramTexturedLightedCubeSpecularMap = new ShaderProgram();
            shaderProgramTexturedLightedCubeSpecularMap.Create(gl, vertexShaderSource, fragmentShaderSource, null);
            shaderProgramTexturedLightedCubeSpecularMap.BindAttributeLocation(gl, attributeIndexPosition, "in_Position");
            shaderProgramTexturedLightedCubeSpecularMap.AssertValid(gl);

            //  Definisanje matrice za projekciju
            projectionMatrix = glm.perspective(45, width / height, 0.1f, 100.0f);

            //  Definisanje parametara kamere i View matrice
            vec3 camTarget = new vec3(0f, 0f, -3f);
            vec3 camUpVec = new vec3(0f, 1f, 0f);
            viewMatrix = glm.lookAt(camPos, camTarget , camUpVec);

            // Definisanje Model matrica za svaki objekat u sceni
            modelMatrixCube = glm.scale(new mat4(1.0f), new vec3(3f));

            modelMatrixTexturedCube = glm.translate(new mat4(1.0f), new vec3(-5f, 0f, -3f));
            modelMatrixTexturedCube = glm.scale(modelMatrixTexturedCube, new vec3(3f));

            modelMatrixSpecularMapCube = glm.translate(new mat4(1.0f), new vec3(5f, 0f, -3f));
            modelMatrixSpecularMapCube = glm.scale(modelMatrixSpecularMapCube, new vec3(3f));
            
            modelMatrixLamp = glm.translate(new mat4(1.0f), new vec3(0f, 3f, -3f));

            LoadTextures();
            CreateVerticesForTexturedCube();

            CreateVerticesForLightedCube();
            CreateVerticesForLamp();
        }

        public void Draw()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT );

            DrawTexturedCubeWSpecularMap();

            DrawTexturedCube();

            DrawRedCube();

            DrawLamp();
        }

        private void DrawLamp()
        {
            //  Bind the shader, set the matrices.
            shaderProgramLamp.Bind(gl);
            shaderProgramLamp.SetUniformMatrix4(gl, "projectionMat", projectionMatrix.to_array());
            shaderProgramLamp.SetUniformMatrix4(gl, "viewMat", viewMatrix.to_array());
            shaderProgramLamp.SetUniformMatrix4(gl, "modelMat", modelMatrixLamp.to_array());

            //  Bind the out vertex array.
            vertexBufferArrayLamp.Bind(gl);

            //  Draw the square.
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, cubeVertices.Length / 3);

            //  Unbind our vertex array and shader.
            vertexBufferArrayLamp.Unbind(gl);
            shaderProgramLamp.Unbind(gl);
        }

        private void DrawTexturedCube()
        {
            //  Bind the shader, set the matrices.
            shaderProgramTexturedLightedCube.Bind(gl);
            shaderProgramTexturedLightedCube.SetUniform1(gl, "material.diffuse", 0);
            shaderProgramTexturedLightedCube.SetUniform1(gl, "material.shininess", 32.0f);

            shaderProgramTexturedLightedCube.SetUniform3(gl, "light.position", 3f, 3f, -3f);

            shaderProgramTexturedLightedCube.SetUniform3(gl, "light.ambient", 0.4f, 0.4f, 0.4f);
            shaderProgramTexturedLightedCube.SetUniform3(gl, "light.diffuse", 0.5f, 0.5f, 0.5f);
            shaderProgramTexturedLightedCube.SetUniform3(gl, "light.specular", 1f, 1f, 1f);

            shaderProgramTexturedLightedCube.SetUniform1(gl, "light.constant", 1f);
            shaderProgramTexturedLightedCube.SetUniform1(gl, "light.linear", 0.09f);
            shaderProgramTexturedLightedCube.SetUniform1(gl, "light.quadratic", 0.032f);

            shaderProgramTexturedLightedCube.SetUniform3(gl, "viewPos", camPos.x, camPos.y, camPos.z);
            shaderProgramTexturedLightedCube.SetUniformMatrix4(gl, "projectionMat", projectionMatrix.to_array());
            shaderProgramTexturedLightedCube.SetUniformMatrix4(gl, "viewMat", viewMatrix.to_array());
            shaderProgramTexturedLightedCube.SetUniformMatrix4(gl, "modelMat", modelMatrixTexturedCube.to_array());

            // Bind diffuse map
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIds[0]);

            //  Bind the out vertex array.
            vertexBufferArrayCubeTextureLighting.Bind(gl);

            //  Draw the square.
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, cubeVertices.Length / 3);

            //  Unbind our vertex array and shader.
            vertexBufferArrayCubeTextureLighting.Unbind(gl);
            shaderProgramTexturedLightedCube.Unbind(gl);
        }

        private void DrawTexturedCubeWSpecularMap()
        {
            //  Bind the shader, set the matrices.
            shaderProgramTexturedLightedCubeSpecularMap.Bind(gl);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform1(gl, "material.diffuse", 0);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform1(gl, "material.specular", 1);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform1(gl, "material.shininess", 32.0f);
            
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform3(gl, "light.position", 3f, 3f, -3f);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform3(gl, "light.ambient", 0.4f, 0.4f, 0.4f);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform3(gl, "light.diffuse", 0.5f, 0.5f, 0.5f);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform3(gl, "light.specular", .5f, .5f, .5f);

            shaderProgramTexturedLightedCubeSpecularMap.SetUniform1(gl, "light.constant", 1f);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform1(gl, "light.linear", 0.09f);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniform1(gl, "light.quadratic", 0.032f);

            shaderProgramTexturedLightedCubeSpecularMap.SetUniform3(gl, "viewPos", camPos.x, camPos.y, camPos.z);
            shaderProgramTexturedLightedCubeSpecularMap.SetUniformMatrix4(gl, "projectionMat", projectionMatrix.to_array());
            shaderProgramTexturedLightedCubeSpecularMap.SetUniformMatrix4(gl, "viewMat", viewMatrix.to_array());
            shaderProgramTexturedLightedCubeSpecularMap.SetUniformMatrix4(gl, "modelMat", modelMatrixSpecularMapCube.to_array());

            // Bind diffuse map
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIds[1]);

            gl.ActiveTexture(OpenGL.GL_TEXTURE1);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIds[2]);

            //  Bind the out vertex array.
            vertexBufferArrayCubeTextureLighting.Bind(gl);

            //  Draw the square.
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, cubeVertices.Length / 3);

            //  Unbind our vertex array and shader.
            vertexBufferArrayCubeTextureLighting.Unbind(gl);
            shaderProgramTexturedLightedCubeSpecularMap.Unbind(gl);
        }

        private void DrawRedCube()
        {
            //  Bind the shader, set the matrices.
            shaderProgramCube.Bind(gl);
            shaderProgramCube.SetUniform3(gl, "objectColor", 1f, 0f, 0f);
            shaderProgramCube.SetUniform3(gl, "lightColor", 1.0f, 1f, 1f);
            shaderProgramCube.SetUniform3(gl, "lightPos", 3f, 3f, -3f);
            shaderProgramCube.SetUniform3(gl, "viewPos", camPos.x, camPos.y, camPos.z);
            shaderProgramCube.SetUniformMatrix4(gl, "projectionMat", projectionMatrix.to_array());
            shaderProgramCube.SetUniformMatrix4(gl, "viewMat", viewMatrix.to_array());
            shaderProgramCube.SetUniformMatrix4(gl, "modelMat", modelMatrixCube.to_array());

            //  Bind the out vertex array.
            vertexBufferArrayCubeLighting.Bind(gl);

            //  Draw the square.
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, cubeVertices.Length / 3);

            //  Unbind our vertex array and shader.
            vertexBufferArrayCubeLighting.Unbind(gl);
            shaderProgramCube.Unbind(gl);
        }

        private void CreateVerticesForLightedCube()
        {
            //  Create the vertex array object.
            vertexBufferArrayCubeLighting = new VertexBufferArray();
            vertexBufferArrayCubeLighting.Create(gl);
            vertexBufferArrayCubeLighting.Bind(gl);

            //  Create a vertex buffer for the vertex data.
            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, cubeVertices, false, 3);

            //  Create a vertex buffer for the vertex normals.
            var vertexNormalBuffer = new VertexBuffer();
            vertexNormalBuffer.Create(gl);
            vertexNormalBuffer.Bind(gl);
            vertexNormalBuffer.SetData(gl, 1, cubeNormals, false, 3);

            //  Unbind the vertex array, we've finished specifying data for it.
            vertexBufferArrayCubeLighting.Unbind(gl);
        }

        private void CreateVerticesForTexturedCube()
        {
            //  Create the vertex array object.
            vertexBufferArrayCubeTextureLighting = new VertexBufferArray();
            vertexBufferArrayCubeTextureLighting.Create(gl);
            vertexBufferArrayCubeTextureLighting.Bind(gl);

            //  Create a vertex buffer for the vertex data.
            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, cubeVertices, false, 3);

            //  Create a vertex buffer for the vertex normals.
            var vertexNormalBuffer = new VertexBuffer();
            vertexNormalBuffer.Create(gl);
            vertexNormalBuffer.Bind(gl);
            vertexNormalBuffer.SetData(gl, 1, cubeNormals, false, 3);

            //  Create a vertex buffer for the vertex normals.
            var vertexTextureCoordBuffer = new VertexBuffer();
            vertexTextureCoordBuffer.Create(gl);
            vertexTextureCoordBuffer.Bind(gl);
            vertexTextureCoordBuffer.SetData(gl, 2, cubeTextureCoords, false, 2);

            //  Unbind the vertex array, we've finished specifying data for it.
            vertexBufferArrayCubeTextureLighting.Unbind(gl);
        }

        private void CreateVerticesForLamp()
        {
            //  Create the vertex array object.
            vertexBufferArrayLamp = new VertexBufferArray();
            vertexBufferArrayLamp.Create(gl);
            vertexBufferArrayLamp.Bind(gl);

            //  Create a vertex buffer for the vertex data.
            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, cubeVertices, false, 3);

            //  Unbind the vertex array, we've finished specifying data for it.
            vertexBufferArrayLamp.Unbind(gl);
        }

        public void LoadTextures()
        {
            textureIds = new uint[3];
            gl.GenTextures(3, textureIds);

            string[] imageURIs =
            {
                "..//..//Textures//box.jpg",
                "..//..//Textures//box2.png",
                "..//..//Textures//box2_specular.png"
            };

            for (int i = 0; i < imageURIs.Length; i++)
            {
                // Neprozirna slika
                Bitmap image = new Bitmap(imageURIs[i]);

                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                // ako je potreban RGB format
                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIds[i]);

                // Kreiraj teksturu sa RBG formatom
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB8, imageData.Width, imageData.Height, 0,
                               OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

                // Podesi parametre teksture
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP);

                // Posto je kreirana tekstura slika nam vise ne treba
                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        public void Dispose()
        {
            gl.DeleteTextures(3, textureIds);
            vertexBufferArrayCubeTextureLighting.Delete(gl);
            vertexBufferArrayCubeLighting.Delete(gl);
            vertexBufferArrayLamp.Delete(gl);
        }

        #endregion
    }
}
