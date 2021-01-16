using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.SceneGraph.Quadrics;

namespace Lighting
{
    /// <summary>
    ///  Nabrojani tip OpenGL podrzanih tipova normala
    /// </summary>
    public enum NormalMode
    {
        PerFace,
        PerVertex
    };

    class World
    {
        #region Atributi

        /// <summary>
        ///  Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///  Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        /// <summary>
        ///	 Izabrani tip normala.
        /// </summary>
        private NormalMode m_selectedNormalMode;

        /// <summary>
        ///   Konstanta koja opisuje sa koliko komponenti su zadate pozicije temena.
        /// </summary>
        private const int VERTEX_COMPONENT_COUNT = 3;

        /// <summary>
        ///   Niz sa koordinatama temena aviona
        /// </summary>
        private float[] m_vertices = {
                                      0.0f, 0.0f, 60.0f, -15.0f, 0.0f, 30.0f, 15.0f, 0.0f, 30.0f,
                                      15.0f, 0.0f, 30.0f, 0.0f, 15.0f, 30.0f, 0.0f, 0.0f, 60.0f,
                                      0.0f, 0.0f, 60.0f, 0.0f, 15.0f, 30.0f, -15.0f, 0.0f, 30.0f,
                                      -15.0f, 0.0f, 30.0f, 0.0f, 15.0f, 30.0f, 0.0f, 0.0f, -56.0f,
                                      0.0f, 0.0f, -56.0f, 0.0f, 15.0f, 30.0f, 15.0f, 0.0f, 30.0f,	
                                      15.0f, 0.0f, 30.0f, -15.0f, 0.0f, 30.0f, 0.0f, 0.0f, -56.0f, 
                                      0.0f, 2.0f, 27.0f, -60.0f, 2.0f, -8.0f, 60.0f, 2.0f, -8.0f,
                                      60.0f, 2.0f, -8.0f, 0.0f, 7.0f, -8.0f, 0.0f, 2.0f, 27.0f,
                                      60.0f, 2.0f, -8.0f, -60.0f, 2.0f, -8.0f, 0.0f, 7.0f, -8.0f,
                                      0.0f, 2.0f, 27.0f, 0.0f, 7.0f, -8.0f, -60.0f, 2.0f, -8.0f,
                                      -30.0f, -0.5f, -57.0f, 30.0f, -0.5f, -57.0f, 0.0f, -0.5f, -40.0f,
                                      0.0f, -0.5f, -40.0f, 30.0f, -0.5f, -57.0f, 0.0f, 4.0f, -57.0f, 
                                      0.0f, 4.0f, -57.0f, -30.0f, -0.5f, -57.0f, 0.0f, -0.5f, -40.0f, 
                                      30.0f, -0.5f, -57.0f, -30.0f, -0.5f, -57.0f, 0.0f, 4.0f, -57.0f,
                                      0.0f, 0.5f, -40.0f, 3.0f, 0.5f, -57.0f, 0.0f, 25.0f, -65.0f,
                                      0.0f, 25.0f, -65.0f, -3.0f, 0.5f, -57.0f, 0.0f, 0.5f, -40.0f,
                                      3.0f, 0.5f, -57.0f, -3.0f, 0.5f, -57.0f, 0.0f, 25.0f, -65.0f
                                    };

        /// <summary>
        ///   Niz sa normalama u temenima aviona
        /// </summary>
        private float[] m_normals;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Izabrani tip normala.
        /// </summary>
        public NormalMode SelectedNormalMode
        {
            get { return m_selectedNormalMode; }
            set { m_selectedNormalMode = value; }
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

        #endregion Properties

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
            m_selectedNormalMode = NormalMode.PerFace;
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
        /// Podesavanje osvetljenja
        /// </summary>
        private void SetupLighting(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 0.0f, 10.0f, -10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            // Definisemo belu spekularnu komponentu materijala sa jakim odsjajem
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, light0specular);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128.0f);

            //Uikljuci color tracking mehanizam
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            // Podesi na koje parametre materijala se odnose pozivi glColor funkcije
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);

            m_normals = LightingUtilities.ComputeVertexNormals(m_vertices);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();

            gl.Translate(0.0f, 0.0f, -150.0f);
            gl.Rotate(m_yRotation, 0f, 1f, 0f);
            gl.Rotate(m_xRotation, 1f, 0f, 0f);

            //Podesavanje boje za anbient i diffuse komponentu osvetljenja 
            gl.Color(0.4f, 0.4f, 0.4f);

            switch (m_selectedNormalMode)
            {
                case (NormalMode.PerFace):
                    {
                        DrawPlanePerFaceNormals(gl);
                        break;
                    }
                case (NormalMode.PerVertex):
                    {
                        DrawPlanePerVertexNormals(gl);
                        break;
                    }
            }
        }


        private void DrawPlanePerVertexNormals(OpenGL gl)
        {
            // Ukljucivanje rada sa vertex array (VA) mehanizmom 
            gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);

            //Ukljucivanje rada sa normal array mehanizmom.
            //Niz sa normalama se zadaje na isti nacin kao i niz sa koordinatama temena.
            gl.EnableClientState(OpenGL.GL_NORMAL_ARRAY);

            // namesti pointer na nizove temena i normala
            gl.VertexPointer(VERTEX_COMPONENT_COUNT, 0, m_vertices);
            gl.NormalPointer(OpenGL.GL_FLOAT, 0, m_normals);

            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, m_vertices.Length / VERTEX_COMPONENT_COUNT);

            //  Iskljucivanje postojeceg pokazivaca na niz sa pozicijama temena i niz sa normalama
            gl.VertexPointer(VERTEX_COMPONENT_COUNT, OpenGL.GL_FLOAT, 0, IntPtr.Zero);
            gl.NormalPointer(OpenGL.GL_FLOAT, 0, IntPtr.Zero);

            //Iskljucivanje rada sa vertex array i normal array mehanizmom
            gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.DisableClientState(OpenGL.GL_NORMAL_ARRAY);
        }


        /// <summary>
        ///  Iscrtavanje aviona.
        /// </summary>
        private void DrawPlanePerFaceNormals(OpenGL gl)
        {
            // Nose Cone – gleda na dole
            gl.Begin(OpenGL.GL_TRIANGLES);
            
            //nekad je moguce direktno odrediti normalu
            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 60.0f);
            gl.Vertex(-15.0f, 0.0f, 30.0f);
            gl.Vertex(15.0f, 0.0f, 30.0f);
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLES);
            gl.Normal(LightingUtilities.FindFaceNormal(15.0f, 0.0f, 30.0f, 0.0f, 15.0f, 30.0f, 0.0f, 0.0f, 60.0f));
            gl.Vertex(15.0f, 0.0f, 30.0f);
            gl.Vertex(0.0f, 15.0f, 30.0f);
            gl.Vertex(0.0f, 0.0f, 60.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, 0.0f, 60.0f, 0.0f, 15.0f, 30.0f, -15.0f, 0.0f, 30.0f));
            gl.Vertex(0.0f, 0.0f, 60.0f);
            gl.Vertex(0.0f, 15.0f, 30.0f);
            gl.Vertex(-15.0f, 0.0f, 30.0f);

            // Body of the Plane
            gl.Normal(LightingUtilities.FindFaceNormal(-15.0f, 0.0f, 30.0f, 0.0f, 15.0f, 30.0f, 0.0f, 0.0f, -56.0f));
            gl.Vertex(-15.0f, 0.0f, 30.0f);
            gl.Vertex(0.0f, 15.0f, 30.0f);
            gl.Vertex(0.0f, 0.0f, -56.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, 0.0f, -56.0f, 0.0f, 15.0f, 30.0f, 15.0f, 0.0f, 30.0f));
            gl.Vertex(0.0f, 0.0f, -56.0f);
            gl.Vertex(0.0f, 15.0f, 30.0f);
            gl.Vertex(15.0f, 0.0f, 30.0f);

            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.Vertex(15.0f, 0.0f, 30.0f);
            gl.Vertex(-15.0f, 0.0f, 30.0f);
            gl.Vertex(0.0f, 0.0f, -56.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, 2.0f, 27.0f, -60.0f, 2.0f, -8.0f, 60.0f, 2.0f, -8.0f));
            gl.Vertex(0.0f, 2.0f, 27.0f);
            gl.Vertex(-60.0f, 2.0f, -8.0f);
            gl.Vertex(60.0f, 2.0f, -8.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(60.0f, 2.0f, -8.0f, 0.0f, 7.0f, -8.0f, 0.0f, 2.0f, 27.0f));
            gl.Vertex(60.0f, 2.0f, -8.0f);
            gl.Vertex(0.0f, 7.0f, -8.0f);
            gl.Vertex(0.0f, 2.0f, 27.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(60.0f, 2.0f, -8.0f, -60.0f, 2.0f, -8.0f, 0.0f, 7.0f, -8.0f));
            gl.Vertex(60.0f, 2.0f, -8.0f);
            gl.Vertex(-60.0f, 2.0f, -8.0f);
            gl.Vertex(0.0f, 7.0f, -8.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, 2.0f, 27.0f, 0.0f, 7.0f, -8.0f, -60.0f, 2.0f, -8.0f));
            gl.Vertex(0.0f, 2.0f, 27.0f);
            gl.Vertex(0.0f, 7.0f, -8.0f);
            gl.Vertex(-60.0f, 2.0f, -8.0f);

            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.Vertex(-30.0f, -0.50f, -57.0f);
            gl.Vertex(30.0f, -0.50f, -57.0f);
            gl.Vertex(0.0f, -0.50f, -40.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, -0.5f, -40.0f, 30.0f, -0.5f, -57.0f, 0.0f, 4.0f, -57.0f));
            gl.Vertex(0.0f, -0.5f, -40.0f);
            gl.Vertex(30.0f, -0.5f, -57.0f);
            gl.Vertex(0.0f, 4.0f, -57.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, 4.0f, -57.0f, -30.0f, -0.5f, -57.0f, 0.0f, -0.5f, -40.0f));
            gl.Vertex(0.0f, 4.0f, -57.0f);
            gl.Vertex(-30.0f, -0.5f, -57.0f);
            gl.Vertex(0.0f, -0.5f, -40.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(30.0f, -0.5f, -57.0f, -30.0f, -0.5f, -57.0f, 0.0f, 4.0f, -57.0f));
            gl.Vertex(30.0f, -0.5f, -57.0f);
            gl.Vertex(-30.0f, -0.5f, -57.0f);
            gl.Vertex(0.0f, 4.0f, -57.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, 0.5f, -40.0f, 3.0f, 0.5f, -57.0f, 0.0f, 25.0f, -65.0f));
            gl.Vertex(0.0f, 0.5f, -40.0f);
            gl.Vertex(3.0f, 0.5f, -57.0f);
            gl.Vertex(0.0f, 25.0f, -65.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(0.0f, 25.0f, -65.0f, -3.0f, 0.5f, -57.0f, 0.0f, 0.5f, -40.0f));
            gl.Vertex(0.0f, 25.0f, -65.0f);
            gl.Vertex(-3.0f, 0.5f, -57.0f);
            gl.Vertex(0.0f, 0.5f, -40.0f);

            gl.Normal(LightingUtilities.FindFaceNormal(3.0f, 0.5f, -57.0f, -3.0f, 0.5f, -57.0f, 0.0f, 25.0f, -65.0f));
            gl.Vertex(3.0f, 0.5f, -57.0f);
            gl.Vertex(-3.0f, 0.5f, -57.0f);
            gl.Vertex(0.0f, 25.0f, -65.0f);
            gl.End();
        }

        public void SwitchNormalMode()
        {
            if (m_selectedNormalMode == NormalMode.PerFace)
            {
                m_selectedNormalMode = NormalMode.PerVertex;
            }
            else
            {
                m_selectedNormalMode = NormalMode.PerFace;
            }
        }

        #endregion
    }
}
