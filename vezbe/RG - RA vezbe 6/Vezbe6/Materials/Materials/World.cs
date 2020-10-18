using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Assets;

namespace Materials
{
    class World
    {
        #region Atributi

        /// <summary>
        /// Trenutno aktivni shading model.
        /// </summary>
        private ShadeModel m_selectedModel;

        //Primitive SharpGL-a koje iscrtavamo u sceni.
        private Sphere sphere ;
        private Cylinder cyl ;

        #endregion

        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        public World()
        {
            sphere = new Sphere();
            cyl = new Cylinder();
            sphere.Radius = 1f;
            m_selectedModel = ShadeModel.Smooth;
        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);

            SetupLighting(gl);
            
            gl.ClearColor(0f, 0f, 0f, 1.0f);

            sphere.CreateInContext(gl);
            cyl.CreateInContext(gl);

            sphere.Material = new Material();
            sphere.Material.Diffuse = Color.Red;
            sphere.Material.Ambient = Color.Blue;
            sphere.Material.Specular = Color.Green;
            sphere.Material.Shininess = 100f;

            cyl.Material = new Material();
            cyl.Material.Diffuse = Color.Red;
            cyl.Material.Ambient = Color.Green;
            cyl.Material.Specular = Color.White;
            cyl.Material.Shininess = 10f;

            gl.ShadeModel(ShadeModel.Flat);
            m_selectedModel = ShadeModel.Flat;
        }

        /// <summary>
        /// Podesavanje osvetljenja
        /// </summary>
        private void SetupLighting(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 0.0f, 0.0f, -4.0f, 1.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
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

            gl.PushMatrix();

            gl.Translate(-2f, 0f, -5f);

            sphere.Material.Bind(gl);
            /*  Deo izvornog koda SharpGL-a od klase Material
             *  https://github.com/dwmkerr/sharpgl/blob/master/source/SharpGL/Core/SharpGL.SceneGraph/Assets/Material.cs
             *  
             *          public void Bind(OpenGL gl)
                        {
                            //	Set the material properties.
                            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, ambient);
                            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, diffuse);
                            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, specular);
                            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_EMISSION, emission);
                            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, shininess);

                            //  If we have a texture, bind it.
                            //  No need to push or pop it as we do that earlier.
                            if (texture != null)
                                texture.Bind(gl);
                        }
              */
            sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.Translate(4f, -1f, 0f);
            gl.Rotate(-90f, 0f, 0f);

            cyl.Material.Bind(gl);
            cyl.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();

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
