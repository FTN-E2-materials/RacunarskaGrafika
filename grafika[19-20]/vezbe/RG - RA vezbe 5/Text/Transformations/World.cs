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
        ///	 Ugao rotacije sveta oko X ose
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width = 0;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height = 0;

        /// <summary>
        ///	 Mreza za iscrtavanje
        /// </summary>
        Grid grid;

        /// <summary>
        ///	 Mreza za iscrtavanje
        /// </summary>
        Teapot teapot;

        #endregion

        #region Properties

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

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
            // Ukljuci depth testing, back face culling i podesiti orijentaciju poligona lica
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CW);
            
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // podesi boju za brisanje ekrana na crnu
            grid = new Grid();
            teapot = new Teapot();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(35.0, (double)m_width / (double)m_height, 1.0, 40.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // perspektiva
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(35.0, m_width / (double)m_height, 0.5, 40.0);
            // Sacuvaj stanje modelview matrice i primeni transformacije
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();
            // Primeni transformacije
            gl.Translate(0.0f, 0.0f, -5.0f);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.PushMatrix();
            gl.Translate(-1f, 1f, 0f);
            gl.DrawText3D("Arial", 25f, 1f, 0.1f, "teapot");
            gl.PopMatrix();
//            gl.Rotate(45.0f, 0.0f, -1.0f, 0.0f); // scena je prikazana zarotirana za -45 stepeni oko y-ose
            grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.Color(1.0f,0.0f,0.0f);
            teapot.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            gl.DrawText(10, 30, 0.0f, 1.0f, 0.0f, "Courier New", 12, "Perspektiva");
            gl.PopMatrix();

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
