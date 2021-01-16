// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2012.</copyright>
// <author>Srdjan Mihic, Aleksandar Josic</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using SharpGL;

namespace GenTexCoord
{
    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World
    {
        #region Atributi

        /// <summary>
        ///   Ugao rotacije kamere oko Y ose.
        /// </summary>
        private float m_angle = 0.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        /// <summary>
        ///	 Identifikatori tekstura za bolju citljivost.
        /// </summary>
        private enum TextureObjects { Back = 0, Front, Bottom, Top, Left, Right };

        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Broj tekstura.
        /// </summary>
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        ///	 Referenca na OpenGL instancu u aplikaciji.
        /// </summary>
        private OpenGL gl;

        #endregion Atributi

        #region Properties

        public float Angle
        {
            get { return m_angle; }
            set { m_angle = value; }
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
            m_textures = new uint[m_textureCount];
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize()
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            CreateTextures();
        }

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
            gl.Perspective(45.0, (double)m_width / (double)m_height, 0.1, 500.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Rotate(0f, m_angle, 0f); 
            // nacrtaj okolinu
            DrawEnviroment(0.0f, 0.0f, 0.0f, 400.0f, 200.0f, 400.0f);

            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        /// <summary>
        ///  Iscrtavanje teksturiranog kvadra - okruzenja.
        /// </summary>
        private void DrawEnviroment(float x, float y, float z, float width, float height, float length)
        {
            // BACK teksturu pridruzi zadnjoj stranici kocke
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Back]);

            // kocka je centrirana oko (x,y,z) tacke
            x = x - width / 2;
            y = y - height / 2;
            z = z - length / 2;

            // BACK stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(x + width, y, z);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(x + width, y + height, z);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(x, y + height, z);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(x, y, z);
            gl.End();

            // FRONT teksturu pridruzi prednjoj stranici kocke
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Front]);

            // FRONT stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(x, y, z + length);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(x, y + height, z + length);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(x + width, y + height, z + length);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(x + width, y, z + length);
            gl.End();

            // BOTTOM teksturu pridruzi donjoj stranici kocke
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Bottom]);

            // BOTTOM stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x, y, z);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x, y, z + length);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x + width, y, z + length);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x + width, y, z);
            gl.End();

            // TOP teksturu pridruzi gornjoj stranici
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Top]);

            // TOP stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x + width, y + height, z);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x + width, y + height, z + length);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x, y + height, z + length);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x, y + height, z);
            gl.End();

            // LEFT teksturu pridruzi levoj stranici
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Left]);

            // LEFT stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x, y + height, z);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x, y + height, z + length);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x, y, z + length);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x, y, z);
            gl.End();

            // RIGHT teksturu pridruzi desnoj stranici
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Right]);

            // RIGHT stranica
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x + width, y, z);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x + width, y, z + length);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x + width, y + height, z + length);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x + width, y + height, z);
            gl.End();
        }

        /// <summary>
        ///  Kreiranje teksture.
        /// </summary>
        private void CreateTextures()
        {
            string[] textureFiles = new string[] {  "..//..//images//back.jpg",
                                                    "..//..//images//front.jpg",
                                                    "..//..//images//bottom.jpg",
                                                    "..//..//images//top.jpg",
                                                    "..//..//images//left.jpg",
                                                    "..//..//images//right.jpg" };

            gl.GenTextures(m_textureCount, m_textures);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);

            for (int textureID = 0; textureID < m_textures.Length; textureID++)
            {
                Bitmap image = new Bitmap(textureFiles[textureID]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)textureID]);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
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
            gl.DeleteTextures(m_textureCount, m_textures);
        }

        #endregion IDisposable metode
    }
}
