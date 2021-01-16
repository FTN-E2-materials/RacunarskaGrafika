using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Enumerations;


namespace Materials
{
    class World
    {
        #region Atributi

        float triangleRotation = 0;
        float quadRotation = 0;
        private ShadeModel m_selectedModel;

        #endregion

        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        public World()
        {
            m_selectedModel = ShadeModel.Smooth;
        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            float[] whiteLight = { 0.0f, 0.0f, 1.0f, 1.0f };

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);

            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT);

            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.LightModel(LightModelParameter.Ambient, whiteLight);

            gl.ClearColor(0f, 0f, 0f, 1.0f);
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.1f, 500f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);	// Clear The Screen And The Depth Buffer

            gl.LoadIdentity();					// Reset The View

            DrawGrid(gl);

            gl.Translate(-1.5f, 0.0f, -6.0f);               // Move Left And Into The Screen

            gl.Rotate(triangleRotation, 0.0f, 1.0f, 0.0f);              // Rotate The Pyramid On It's Y Axis

            gl.Begin(OpenGL.GL_TRIANGLES);                  // Start Drawing The Pyramid

            gl.Color(1.0f, 0.0f, 0.0f);         // Red
            gl.Vertex(0.0f, 1.0f, 0.0f);            // Top Of Triangle (Front)
            gl.Color(0.0f, 1.0f, 0.0f);         // Green
            gl.Vertex(-1.0f, -1.0f, 1.0f);          // Left Of Triangle (Front)
            gl.Color(0.0f, 0.0f, 1.0f);         // Blue
            gl.Vertex(1.0f, -1.0f, 1.0f);           // Right Of Triangle (Front)

            gl.Color(1.0f, 0.0f, 0.0f);         // Red
            gl.Vertex(0.0f, 1.0f, 0.0f);            // Top Of Triangle (Right)
            gl.Color(0.0f, 0.0f, 1.0f);         // Blue
            gl.Vertex(1.0f, -1.0f, 1.0f);           // Left Of Triangle (Right)
            gl.Color(0.0f, 1.0f, 0.0f);         // Green
            gl.Vertex(1.0f, -1.0f, -1.0f);          // Right Of Triangle (Right)

            gl.Color(1.0f, 0.0f, 0.0f);         // Red
            gl.Vertex(0.0f, 1.0f, 0.0f);            // Top Of Triangle (Back)
            gl.Color(0.0f, 1.0f, 0.0f);         // Green
            gl.Vertex(1.0f, -1.0f, -1.0f);          // Left Of Triangle (Back)
            gl.Color(0.0f, 0.0f, 1.0f);         // Blue
            gl.Vertex(-1.0f, -1.0f, -1.0f);         // Right Of Triangle (Back)

            gl.Color(1.0f, 0.0f, 0.0f);         // Red
            gl.Vertex(0.0f, 1.0f, 0.0f);            // Top Of Triangle (Left)
            gl.Color(0.0f, 0.0f, 1.0f);         // Blue
            gl.Vertex(-1.0f, -1.0f, -1.0f);         // Left Of Triangle (Left)
            gl.Color(0.0f, 1.0f, 0.0f);         // Green
            gl.Vertex(-1.0f, -1.0f, 1.0f);          // Right Of Triangle (Left)
            gl.End();                       // Done Drawing The Pyramid

            gl.LoadIdentity();
            gl.Translate(1.5f, 0.0f, -7.0f);                // Move Right And Into The Screen

            gl.Rotate(quadRotation, 1.0f, 1.0f, 1.0f);          // Rotate The Cube On X, Y & Z

            gl.Begin(OpenGL.GL_QUADS);                  // Start Drawing The Cube

            gl.Color(0.0f, 1.0f, 0.0f);         // Set The Color To Green
            gl.Vertex(1.0f, 1.0f, -1.0f);           // Top Right Of The Quad (Top)
            gl.Vertex(-1.0f, 1.0f, -1.0f);          // Top Left Of The Quad (Top)
            gl.Vertex(-1.0f, 1.0f, 1.0f);           // Bottom Left Of The Quad (Top)
            gl.Vertex(1.0f, 1.0f, 1.0f);            // Bottom Right Of The Quad (Top)

            gl.Color(1.0f, 0.5f, 0.0f);         // Set The Color To Orange
            gl.Vertex(1.0f, -1.0f, 1.0f);           // Top Right Of The Quad (Bottom)
            gl.Vertex(-1.0f, -1.0f, 1.0f);          // Top Left Of The Quad (Bottom)
            gl.Vertex(-1.0f, -1.0f, -1.0f);         // Bottom Left Of The Quad (Bottom)
            gl.Vertex(1.0f, -1.0f, -1.0f);          // Bottom Right Of The Quad (Bottom)

            gl.Color(1.0f, 0.0f, 0.0f);         // Set The Color To Red
            gl.Vertex(1.0f, 1.0f, 1.0f);            // Top Right Of The Quad (Front)
            gl.Vertex(-1.0f, 1.0f, 1.0f);           // Top Left Of The Quad (Front)
            gl.Vertex(-1.0f, -1.0f, 1.0f);          // Bottom Left Of The Quad (Front)
            gl.Vertex(1.0f, -1.0f, 1.0f);           // Bottom Right Of The Quad (Front)

            gl.Color(1.0f, 1.0f, 0.0f);         // Set The Color To Yellow
            gl.Vertex(1.0f, -1.0f, -1.0f);          // Bottom Left Of The Quad (Back)
            gl.Vertex(-1.0f, -1.0f, -1.0f);         // Bottom Right Of The Quad (Back)
            gl.Vertex(-1.0f, 1.0f, -1.0f);          // Top Right Of The Quad (Back)
            gl.Vertex(1.0f, 1.0f, -1.0f);           // Top Left Of The Quad (Back)

            gl.Color(0.0f, 0.0f, 1.0f);         // Set The Color To Blue
            gl.Vertex(-1.0f, 1.0f, 1.0f);           // Top Right Of The Quad (Left)
            gl.Vertex(-1.0f, 1.0f, -1.0f);          // Top Left Of The Quad (Left)
            gl.Vertex(-1.0f, -1.0f, -1.0f);         // Bottom Left Of The Quad (Left)
            gl.Vertex(-1.0f, -1.0f, 1.0f);          // Bottom Right Of The Quad (Left)

            gl.Color(1.0f, 0.0f, 1.0f);         // Set The Color To Violet
            gl.Vertex(1.0f, 1.0f, -1.0f);           // Top Right Of The Quad (Right)
            gl.Vertex(1.0f, 1.0f, 1.0f);            // Top Left Of The Quad (Right)
            gl.Color(0.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);           // Bottom Left Of The Quad (Right)
            gl.Vertex(1.0f, -1.0f, -1.0f);          // Bottom Right Of The Quad (Right)
            gl.End();                       // Done Drawing The Q

            gl.Flush();

            triangleRotation += 3f;// 0.2f;						// Increase The Rotation Variable For The Triangle 
            quadRotation -= 2f;// 0.15f;						// Decrease The Rotation Variable For The Quad 

            gl.Flush();
        }

        public void ChangeShadeModel(OpenGL gl)
        {
            if (m_selectedModel == ShadeModel.Flat)
            {
                gl.ShadeModel(ShadeModel.Smooth);
                m_selectedModel = ShadeModel.Smooth;
            }
            else
            {
                gl.ShadeModel(ShadeModel.Flat);
                m_selectedModel = ShadeModel.Flat;
            }
        }

        /// <summary>
        ///  Iscrtavanje SharpGL primitive grida.
        /// </summary>
        private void DrawGrid(OpenGL gl)
        {
            gl.PushMatrix();
            Grid grid = new Grid();
            gl.Translate(0f, -1f, -10f);
            gl.Rotate(90f, 0f, 0f);
            grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.PopMatrix();
        }

        #endregion

    }
}
