using System;
using System.Drawing;
using System.Drawing.Imaging;
using SharpGL;

namespace GenTexCoord
{
    /// <summary>
    ///  Nabrojani tip OpenGL rezima filtriranja tekstura
    /// </summary>
    public enum TextureGenMode
    {
        ObjectLinear,
        EyeLinear,
        SphereMap
    };

    class World
    {
        #region Atributi

        /// <summary>
        ///  Identifikator teksture
        /// </summary>
        uint[] textureIDs;

        /// <summary>
        ///  Izabrana OpenGL mehanizam za iscrtavanje.
        /// </summary>
        private TextureGenMode m_selectedMode = TextureGenMode.SphereMap;

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
        ///	 Referenca na OpenGL instancu unutar aplikacije
        /// </summary>
        private OpenGL gl;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///  Izabrani OpenGL rezim stapanja teksture sa materijalom
        /// </summary>
        public TextureGenMode SelectedMode
        {
            get { return m_selectedMode; }
            set
            {
                m_selectedMode = value;

                // Projekciona ravan
                float[] zPlane = { 0.0f, 0.0f, 1.0f, 1.0f };

                switch (m_selectedMode)
                {
                    case TextureGenMode.ObjectLinear:
                        // Object Linear
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_OBJECT_LINEAR);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_OBJECT_LINEAR);
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_OBJECT_PLANE, zPlane);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_OBJECT_PLANE, zPlane);
                        break;

                    case TextureGenMode.EyeLinear:
                        // Eye Linear
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_EYE_LINEAR);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_EYE_LINEAR);
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_EYE_PLANE, zPlane);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_EYE_PLANE, zPlane);
                        break;

                    case TextureGenMode.SphereMap:
                        // Sphere Map
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
                        break;
                }
            }
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

        #region Konstruktori

        /// <summary>
        ///  Konstruktor.
        /// </summary>
        public World(OpenGL gl)
        {
            this.gl = gl;
            textureIDs = new uint[1];
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0, (double)m_width / m_height, 1.0, 225.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize()
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);

            gl.GenTextures(1, textureIDs);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIDs[0]);
            // Ucitaj sliku i podesi parametre teksture
            Bitmap image = new Bitmap("..//..//images//stripes.jpg");
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA8, image.Width, image.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

            image.UnlockBits(imageData);
            image.Dispose();

            // Ukljuci generisanje koord. teksture
            gl.Enable(OpenGL.GL_TEXTURE_GEN_S);
            gl.Enable(OpenGL.GL_TEXTURE_GEN_T);

            // sferno podrazumevani nacin generisanja koord. teksture
     /*       gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
            gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);*/

            gl.Hint(OpenGL.GL_GENERATE_MIPMAP_SGIS, OpenGL.GL_NICEST);
            gl.Hint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);
            gl.Hint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_NICEST);
            gl.Hint(OpenGL.GL_TEXTURE_COMPRESSION_HINT, OpenGL.GL_NICEST);
            gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);
            gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIDs[0]);

            // Nacrtaj torus
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -2.0f);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            gltDrawTorus(0.35f, 0.15f, 128, 128);

            gl.PopMatrix();

            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        /// <summary>
        ///  Iscrtavanje torusa u XY ravni.
        /// </summary>
        private void gltDrawTorus(float majorRadius, float minorRadius, int numMajor, int numMinor)
        {
            float[] vNormal = new float[3];
            float vLength;
            double majorStep = 2.0f * Math.PI / numMajor;
            double minorStep = 2.0f * Math.PI / numMinor;
            int i, j;

            for (i = 0; i < numMajor; ++i)
            {
                double a0 = i * majorStep;
                double a1 = a0 + majorStep;
                float x0 = (float)Math.Cos(a0);
                float y0 = (float)Math.Sin(a0);
                float x1 = (float)Math.Cos(a1);
                float y1 = (float)Math.Sin(a1);

                gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
                for (j = 0; j <= numMinor; ++j)
                {
                    double b = j * minorStep;
                    float c = (float)Math.Cos(b);
                    float r = minorRadius * c + majorRadius;
                    float z = minorRadius * (float)Math.Sin(b);

                    // First point
                    //gl.TexCoord((float)(i)/(float)(numMajor), (float)(j)/(float)(numMinor));
                    vNormal[0] = x0 * c;
                    vNormal[1] = y0 * c;
                    vNormal[2] = z / minorRadius;
                    // normalizacija vektora
                    vLength = (float)Math.Sqrt(vNormal[0] * vNormal[0] + vNormal[1] * vNormal[1] + vNormal[2] * vNormal[2]);
                    vNormal[0] *= 1.0f / vLength;
                    vNormal[1] *= 1.0f / vLength;
                    vNormal[2] *= 1.0f / vLength;
                    gl.Normal(vNormal);
                    gl.Vertex(x0 * r, y0 * r, z);

                    //gl.TexCoord((float)(i+1)/(float)(numMajor), (float)(j)/(float)(numMinor));
                    vNormal[0] = x1 * c;
                    vNormal[1] = y1 * c;
                    vNormal[2] = z / minorRadius;
                    // normalizacija vektora
                    vLength = (float)Math.Sqrt(vNormal[0] * vNormal[0] + vNormal[1] * vNormal[1] + vNormal[2] * vNormal[2]);
                    vNormal[0] *= 1.0f / vLength;
                    vNormal[1] *= 1.0f / vLength;
                    vNormal[2] *= 1.0f / vLength;
                    gl.Normal(vNormal);
                    gl.Vertex(x1 * r, y1 * r, z);
                }
                gl.End();
            }
        }

        /// <summary>
        /// Menja trenutno aktivni mod za filtering.
        /// </summary>
        public void ChangeTextureGenMode()
        {
            SelectedMode = (TextureGenMode)(((int)m_selectedMode + 1) % Enum.GetNames(typeof(TextureGenMode)).Length);
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

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Terminate();
            }
        }

        /// <summary>
        ///  Korisnicko oslobadjanje OpenGL resursa.
        /// </summary>
        private void Terminate()
        {
            gl.DeleteTextures(1, textureIDs);
        }

        #endregion IDisposable metode
    }
}
