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
    public enum ProjectionType
    {
        Orthogonal,
        Perspective,
        PerspectiveAlternative,
        OrthogonalFixed,
        PerspectiveFixed
    }

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

        private float m_eyeX = 0.0f;
        private float m_eyeY = 0.0f;
        private float m_eyeZ = 0.0f;

        private float m_centerX = 0.0f;
        private float m_centerY = 0.0f;
        private float m_centerZ = -1.0f;

        private float m_upX = 0.0f;
        private float m_upY = 1.0f;
        private float m_upZ = 0.0f;

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
        Cube cube;

        /// <summary>
        ///   Odabrani tip projekcije.
        /// </summary>
        private ProjectionType m_currentProjectionType = ProjectionType.Perspective;

        /// <summary>
        ///   Trenutna udaljenost objekta po z-osi;
        /// </summary>
        private float m_zDistance;


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

        /// <summary>
        ///	 X pozicija kamere.
        /// </summary>
        public float EyeX
        {
            get { return m_eyeX; }
            set { m_eyeX = value; }
        }

        /// <summary>
        ///	 Y pozicija kamere.
        /// </summary>
        public float EyeY
        {
            get { return m_eyeY; }
            set { m_eyeY = value; }
        }

        /// <summary>
        ///	 Z pozicija kamere.
        /// </summary>
        public float EyeZ
        {
            get { return m_eyeZ; }
            set { m_eyeZ = value; }
        }

        /// <summary>
        ///   Odabrani tip projekcije.
        /// </summary>
        public ProjectionType CurrentProjectionType
        {
            get { return m_currentProjectionType; }
            set { m_currentProjectionType = value; }
        }

        /// <summary>
        ///   Trenutna udaljenost objekta po z-osi;
        /// </summary>
        public float ZDistance
        {
            get { return m_zDistance; }
            set { m_zDistance = value; }
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
            gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);
            m_zDistance = -250;
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // podesi boju za brisanje ekrana na crnu
            grid = new Grid();
            cube = new Cube();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height); // kreiraj viewport po celom prozoru
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();			              // resetuj Projection Matrix

            if (m_currentProjectionType == ProjectionType.Orthogonal)
            {
                if (m_width <= m_height)
                {
                    gl.Ortho(-400.0f, 400.0f, -400.0f * m_height / m_width, 400.0f * m_height / m_width, 100.0f, 1000.0f);
                }
                else
                {
                    gl.Ortho(-400.0f * m_width / m_height, 400.0f * m_width / m_height, -400.0f, 400.0f, 100.0f, 1000.0f);
                }
            }
            else if (m_currentProjectionType == ProjectionType.Perspective)
            {
                if (m_width <= m_height)
                {
                    gl.Frustum(-400.0f, 400.0f, -400.0f * m_height / m_width, 400.0f * m_height / m_width, 100.0f, 1000.0f);
                }
                else
                {
                    gl.Frustum(-400.0f * m_width / m_height, 400.0f * m_width / m_height, -400.0f, 400.0f, 100.0f, 1000.0f);
                }
            }
            else if (m_currentProjectionType == ProjectionType.PerspectiveAlternative)
            {
                    gl.Perspective(152, (float)m_width / m_height,100, 1000);
            }
            else if (m_currentProjectionType == ProjectionType.OrthogonalFixed)
            {
                    gl.Ortho(-400.0f, 400.0f, -400.0f, 400.0f, 100.0f, 1000.0f);
            }
            else if (m_currentProjectionType == ProjectionType.PerspectiveFixed)
            {
                    gl.Frustum(-400.0f, 400.0f, -400.0f, 400.0f, 100.0f, 1000.0f);
            }


            gl.MatrixMode(OpenGL.GL_MODELVIEW);   // selektuj ModelView Matrix
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();

            gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);

            gl.Translate(0, 0, m_zDistance);
            gl.Rotate(m_xRotation, 1, 0, 0);
            gl.Rotate(m_yRotation, 0, 1, 0);

            gl.Scale(5, 5, 5);
            gl.Color(255, 255, 255);
            grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.Color(255,255,255);
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);

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
