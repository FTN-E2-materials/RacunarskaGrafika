using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;

namespace TriangleFanKupa
{

    ///<summary> Klasa koja enkapsulira OpenGL programski kod </summary>
    class World
    {
        #region Atributi

        /// <summary>
        ///  Ugao rotacije sveta oko X ose.
        /// </summary>
        float m_xRotation = 0.0f;

        /// <summary>
        ///  Ugao rotacije sveta oko Y ose.
        /// </summary>
        float m_yRotation = 0.0f;

        /// <summary>
        ///  Indikator stanja mehanizma sakrivanja nevidljivih povrsina.
        /// </summary>
        bool m_culling = false;

        /// <summary>
        ///  Indikator stanja mehanizma iscrtvanja poligona kao linija.
        /// </summary>
        bool m_outline = false;

        /// <summary>
        ///  Indikator stanja mehanizma za testiranje dubine.
        /// </summary>
        bool m_depthTesting = false;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion

        #region Properties

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
        ///  Indikator stanja mehanizma sakrivanja nevidljivih povrsina.
        /// </summary>
        public bool Culling
        {
            get { return m_culling; }
            set { m_culling = value; }
        }

        /// <summary>
        ///  Indikator stanja mehanizma iscrtvanja poligona kao linija.
        /// </summary>
        public bool Outline
        {
            get { return m_outline; }
            set { m_outline = value; }
        }

        /// <summary>
        ///  Indikator stanja mehanizma za testiranje dubine.
        /// </summary>
        public bool DepthTesting
        {
            get { return m_depthTesting; }
            set { m_depthTesting = value; }
        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            // Crna pozadina i zuta boja za crtanje
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 1f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            // Podrazumevano
            gl.FrontFace(OpenGL.GL_CW);
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl,int width, int height)
        {
            float nRange = 100.0f;
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            if (m_width <= m_height)
                gl.Ortho(-nRange, nRange, -nRange * m_height / m_width, nRange * m_height / m_width, -nRange, nRange);
            else
                gl.Ortho(-nRange * m_width / m_height, nRange * m_width / m_height, -nRange, nRange, -nRange, nRange);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            // Ocisti sadrzaj kolor bafera i bafera dubine
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Ako je izabrano back face culling ukljuci ga i obratno
            if (m_culling == true)
            {
                gl.Enable(OpenGL.GL_CULL_FACE);                
            }
            else
            {
                gl.Disable(OpenGL.GL_CULL_FACE);
            }

            // Ako je izabrano testiranje dubine ukljuci ga i obratno
            if (m_depthTesting == true)
            {
                gl.Enable(OpenGL.GL_DEPTH_TEST);
            }
            else
            {
                gl.Disable(OpenGL.GL_DEPTH_TEST);
            }

            // Ako je izabran rezim iscrtavanja objekta kao wireframe, ukljuci ga i obratno
            if (m_outline == true)
            {
                // Iscrtati ceo objekat kao zicani model.
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            }
            else
            {
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            }

            double step = Math.Round(Math.PI / 8.0, 15); // Korak za segment kruga/omotaca
            const double radius = 50.0; // radijus osnove kupe
            const double height = 75.0; // visina kupe
            double angle; // Ugao rotacije
            int pivot = 0; // Naizmenicna promena boje poligona

            // Sacuvaj stanje ModelView matrice i primeni rotacije
            gl.PushMatrix();
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            // Zapocni iscrtavanje omotaca kupe
            gl.Begin(OpenGL.GL_TRIANGLE_FAN);

            // Da bismo dobili omotac a ne krug, postavljamo vrednost z koordinate na visinu kupe
            gl.Vertex(0.0, 0.0, height);

            // Dodaj temena omotaca
            for (angle = 2 * Math.PI; angle > 0; angle -= step)
            {
                // Menjaj boju trouglova naizmenicno (zuta/plava)
                if ((pivot % 2) == 0)
                {
                    gl.Color(1.0f, 1.0f, 0.0f);
                }
                else
                {
                    gl.Color(0.0f, 0.0f, 1.0f);
                }

                // Azuriraj stanje boje trouglova
                ++pivot;

                gl.Vertex(radius * Math.Cos(angle), radius * Math.Sin(angle));
            }

            // Gotovo iscrtavanje omotaca kupe
            gl.End();

            // Zapocni iscrtavanje osnove kupe
            gl.Begin(OpenGL.GL_TRIANGLE_FAN);

            // Centar osnove je u (0,0)
            gl.Vertex(0.0, 0.0);

            for (angle = 0.0; angle < 2 * Math.PI; angle += step)
            {
                // Menjaj boju trouglova (crvena/zelena)
                if ((pivot % 2) == 0)
                {
                    gl.Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    gl.Color(1.0f, 0.0f, 0.0f);
                }

                // Azuriraj stanje boje trouglova
                ++pivot;

                // Specifikuj teme na osnovu izracunatog
                gl.Vertex(radius * Math.Cos(angle), radius * Math.Sin(angle));
            }

            // Gotovo iscrtavanje osnove kupe
            gl.End();

            // Restauriraj stanje ModelView matrice na ono pre crtanja kupe
            gl.PopMatrix();

            // Oznaci kraj iscrtavanja
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
