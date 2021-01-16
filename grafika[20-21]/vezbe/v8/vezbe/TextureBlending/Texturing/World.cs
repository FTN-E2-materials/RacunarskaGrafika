// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2012.</copyright>
// <author>Srdjan Mihic, Aleksandar Josic</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using SharpGL.SceneGraph.Quadrics;

namespace Texturing
{
    
    /// <summary>
    ///  Nabrojani tip OpenGL rezima stapanja teksture sa materijalom
    /// </summary>
    public enum TextureBlendingMode
    {
        Modulate,
        Decal,
        Replace,
        Blend
    };

    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World
    {
        #region Atributi

        /// <summary>
        ///  Izabrana OpenGL mehanizam za iscrtavanje.
        /// </summary>
        private TextureBlendingMode m_selectedMode = TextureBlendingMode.Replace;

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

        #endregion Atributi

        #region Properties

        /// <summary>
        ///  Izabrani OpenGL rezim stapanja teksture sa materijalom
        /// </summary>
        public TextureBlendingMode SelectedMode
        {
            get { return m_selectedMode; }
            set
            {
                m_selectedMode = value;
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
        public World()
        {
        }

        #endregion Konstruktori

        #region Metode

        private Sphere sfera;
        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            sfera = new Sphere();
            sfera.CreateInContext(gl);
            sfera.Radius = 1f;
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.Enable(OpenGL.GL_CULL_FACE);

            //Bela pozadina
            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            // Ucitaj sliku
            // koristi se alfa komponenta
            //Bitmap image = new Bitmap("..//..//images//stone.png");

            // Neprozirna slika
            Bitmap image = new Bitmap("..//..//images//stone.jpg");

            // rotiramo sliku zbog koordinantog sistema opengl-a
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            // ako je potreban RGB format
             BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                  System.Drawing.Imaging.PixelFormat.Format24bppRgb);
 
            // Kreiraj teksturu sa RBG formatom
             gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB8, imageData.Width, imageData.Height, 0,
                            OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
             

            // RGBA format (ukljucuje alfa kanal)
       /*     BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                  System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Kreiraj teksturu sa RGBA
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA8, imageData.Width, imageData.Height, 0,
                            OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);*/

            // Podesi parametre teksture
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP);

            // Podesi nacin blending teksture
            m_selectedMode = TextureBlendingMode.Modulate;
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            // Predji u rezim rada sa 2D teksturama
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Posto je kreirana tekstura slika nam vise ne treba
            image.UnlockBits(imageData);
            image.Dispose();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int height, int width)
        {
            m_height = height;
            m_width = width;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0, m_width / m_height, 0.1, 40.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();
                gl.Translate(0f, -0.5f, 0f);
                gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
                gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

                gl.Scale(.5f, .5f, .5f);
                //Nacrtaj piramidu
                //uvek je dobro pravilo da se definisu materijali s obzirom da postoji 
                //mogucnost da se ne kreira i pridruzi tekstura objektu
                gl.Color(1f, 0f, 0f, 0.2f);
                gl.Begin(OpenGL.GL_TRIANGLES);
                //Osnova piramide
                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(0.5f, 0.0f, -0.5f);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-0.5f, 0.0f, 0.5f);
                gl.TexCoord(0.0f, 1.0f);
                gl.Vertex(-0.5f, 0.0f, -0.5f);

                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(0.5f, 0.0f, -0.5f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(0.5f, 0.0f, 0.5f);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-0.5f, 0.0f, 0.5f);

                //Prednja stranica
                gl.TexCoord(0.5f, 1.0f);
                gl.Vertex(0.0f, 0.8f, 0.0f);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-0.5f, 0.0f, 0.5f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(0.5f, 0.0f, 0.5f);

                //Leva stranica
                gl.TexCoord(0.5f, 1.0f);
                gl.Vertex(0.0f, 0.8f, 0.0f);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-0.5f, 0.0f, -0.5f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(-0.5f, 0.0f, 0.5f);

                //Zadnja stranica
                gl.TexCoord(0.5f, 1.0f);
                gl.Vertex(0.0f, 0.8f, 0.0f);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(0.5f, 0.0f, -0.5f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(-0.5f, 0.0f, -0.5f);

                //Desna stranica
                gl.TexCoord(0.5f, 1.0f);
                gl.Vertex(0.0f, 0.8f, 0.0f);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(0.5f, 0.0f, 0.5f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(0.5f, 0.0f, -0.5f);
                gl.End();
            gl.PopMatrix();

            //Oznaci kraj iscrtavanja
            gl.Flush();
        }

        /// <summary>
        /// Menja trenutno aktivni mod za blending.
        /// </summary>
        public void ChangeTextureBlendMode(OpenGL gl)
        {
            SelectedMode = (TextureBlendingMode)(((int)m_selectedMode + 1) % Enum.GetNames(typeof(TextureBlendingMode)).Length);
            switch (m_selectedMode)
            {
                case TextureBlendingMode.Modulate:
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
                    break;

                case TextureBlendingMode.Replace:
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_REPLACE);
                    break;

                case TextureBlendingMode.Decal:
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
                    break;

                case TextureBlendingMode.Blend:
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_BLEND);
                    break;

            }
        }
        #endregion

    }
}
