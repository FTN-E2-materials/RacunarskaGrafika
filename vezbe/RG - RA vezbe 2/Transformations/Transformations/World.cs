using System;
using System.Collections;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Core;
using System.Diagnostics;

namespace Transformations
{
    ///<summary> Klasa koja enkapsulira OpenGL programski kod </summary>
    class World
    {
        #region Atributi

        /// <summary>
        ///	 Rotacija kocke
        /// </summary>
        private float m_cubeRotationY = 0;

        /// <summary>
        ///	 Translacija kocke po X
        /// </summary>
        private float m_cubeTransX = 2;

        /// <summary>
        ///	 Translacija cajnika po X
        /// </summary>
        private float m_teapotTransX = -2;

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;


        #endregion

        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        public World()
        {

        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST | OpenGL.GL_CULL_FACE);
            SetupLighting(gl);  
        }

        /// <summary>
        /// Podesavanje osvetljenja. Više informacija u nastavku kursa, za sada samo koristiti
        /// </summary>
        private void SetupLighting(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_LIGHTING | OpenGL.GL_LIGHT0);
            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.ShadeModel(OpenGL.GL_SMOOTH);	
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
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();

            DrawGrid(gl);
            
            DrawSolarSystem(gl);

            //   DrawTeapotsAndCubes(gl);
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


        public void DrawSolarSystem(OpenGL gl)
        {
            Sphere sfera = new Sphere();
            sfera.CreateInContext(gl);
            sfera.Radius = 20f;

            gl.PushMatrix();
            // Transliraj scenu tako da se vidi unutar prozora	
            gl.Translate(0.0f, -50f, -250.0f);
            sfera.Render(gl, RenderMode.Render);

            // Rotiraj koordinatni sistem da bi iscrtali Zemlju
            gl.Rotate(m_earthRotation, 0.0f, 1.0f, 0.0f);
            // Zemlja
            gl.Translate(100.0f, 0.0f, 0.0f);
            sfera.Radius = 10f;
            sfera.Render(gl, RenderMode.Render);

            // Rotiraj za ugao Meseca i nacrtaj Mesec
            gl.Rotate(m_moonRotation, 0.0f, 1.0f, 0.0f);
            gl.Translate(30.0f, 0.0f, 0.0f);
            sfera.Radius = 5f;
            sfera.Render(gl, RenderMode.Render);

            // Restauriraj stanje ModelView matrice pre crtanja
            gl.PopMatrix();

            m_earthRotation += 2.0f;
            if (m_earthRotation > 360.0f)
            {
                m_earthRotation = 0.0f;
            }

            m_moonRotation += 5.0f;
            if (m_moonRotation > 360.0f)
            {
                m_moonRotation = 0.0f;
            }
        }

        private void Zadatak(OpenGL gl)
        {
        }

        public void DrawTeapotsAndCubes(OpenGL gl)
        {
            gl.PushMatrix();
            // Pomeranje desno, skaliranje i iscrtavanje kocke
            gl.Translate(m_cubeTransX, 0f, -10f);
            gl.Rotate(m_cubeRotationY, 0f, 1f, 0f);
            gl.Scale(2f, 1f, 1f);
            Cube cube = new Cube();
            cube.Render(gl, RenderMode.Render);
       //     gl.PopMatrix();

            // Pomeranje levo i iscrtavanje cajnika
       //     gl.PushMatrix();
            gl.Translate(0, 2f, 0f);
            gl.Scale(.5f, 1f, 1f);
            Teapot teapot = new Teapot();
            teapot.Render(gl, RenderMode.Render);
      //      gl.PopMatrix();

            m_cubeRotationY++;
            gl.Flush();
        }


        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Destruktor.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion

        #region IDisposable Metode

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    //Oslobodi managed resurse
            //}
        }

        #endregion
    }
}
