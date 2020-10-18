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

namespace MipMaps{
    /// <summary>
    ///  Nabrojani tip OpenGL rezima filtriranja tekstura
    /// </summary>
    public enum TextureFilterMode
    {
        Nearest,
        Linear,
        NearestMipmapNearest,
        NearestMipmapLinear,
        LinearMipmapNearest,
        LinearMipmapLinear
    };

    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Identifikatori tekstura za jednostavniji pristup teksturama
        /// </summary>
        private enum TextureObjects { Brick = 0, Floor, Ceiling };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//images//brick.jpg", "..//..//images//floor.jpg", "..//..//images//ceiling.jpg" };

        /// <summary>
        ///  Izabrana OpenGL mehanizam za iscrtavanje.
        /// </summary>
        private TextureFilterMode m_selectedMode = TextureFilterMode.Nearest;

        /// <summary>
        ///  Pomeraj po Z osi
        /// </summary>
        private float m_zPosition = -60.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        /// <summary>
        ///	 Referenca na OpenGL instancu unutar aplikacije.
        /// </summary>
        private OpenGL gl;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///  Izabrani OpenGL rezim stapanja teksture sa materijalom
        /// </summary>
        public TextureFilterMode SelectedMode
        {
            get { return m_selectedMode; }
            set
            {
                m_selectedMode = value;

                foreach (uint textureId in m_textures)
                {
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

                    switch (m_selectedMode)
                    {
                        case TextureFilterMode.Nearest:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                            break;

                        case TextureFilterMode.Linear:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                            break;

                        case TextureFilterMode.NearestMipmapNearest:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_NEAREST);
                            break;

                        case TextureFilterMode.NearestMipmapLinear:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_LINEAR);
                            break;

                        case TextureFilterMode.LinearMipmapNearest:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_NEAREST);
                            break;

                        case TextureFilterMode.LinearMipmapLinear:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float PositionZ
        {
            get { return m_zPosition; }
            set { m_zPosition = value; }
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
            // Crna pozadina
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Ukljuci depth testing i back face culling i podesi da je front = CW
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CW);

            // Teksture se primenjuju sa parametrom decal
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering

                image.UnlockBits(imageData);
                image.Dispose();
            }
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
            gl.Perspective(90.0, (double)m_width / m_height, 1.0, 120.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            // Pomeraj objekat po z-osi
            gl.Translate(0.0f, 0.0f, m_zPosition);

            for (float z = 60.0f; z >= 0.0f; z -= 10.0f)
            {
                // Pod tunela
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Floor]);
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-10.0f, -10.0f, z);
                gl.TexCoord(0.0f, 1.0f);
                gl.Vertex(-10.0f, -10.0f, z - 10.0f);
                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(10.0f, -10.0f, z - 10.0f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(10.0f, -10.0f, z);
                gl.End();

                // Plafon tunela
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Ceiling]);
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-10.0f, 10.0f, z);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(10.0f, 10.0f, z);
                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(10.0f, 10.0f, z - 10.0f);
                gl.TexCoord(0.0f, 1.0f);
                gl.Vertex(-10.0f, 10.0f, z - 10.0f);
                gl.End();

                // Levi zid tunela
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(-10.0f, -10.0f, z);
                gl.TexCoord(0.0f, 1.0f);
                gl.Vertex(-10.0f, 10.0f, z);
                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(-10.0f, 10.0f, z - 10.0f);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(-10.0f, -10.0f, z - 10.0f);
                gl.End();

                // Desni zid tunela
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 0.0f);
                gl.Vertex(10.0f, -10.0f, z);
                gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(10.0f, -10.0f, z - 10.0f);
                gl.TexCoord(1.0f, 1.0f);
                gl.Vertex(10.0f, 10.0f, z - 10.0f);
                gl.TexCoord(0.0f, 1.0f);
                gl.Vertex(10.0f, 10.0f, z);
                gl.End();
            }

            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        /// <summary>
        /// Menja trenutno aktivni mod za filtering.
        /// </summary>
        public void ChangeTextureFilterMode()
        {
            SelectedMode = (TextureFilterMode)(((int)m_selectedMode + 1) % Enum.GetNames(typeof(TextureFilterMode)).Length);
        }

        #endregion Metode

        #region IDisposable metode

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

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                gl.DeleteTextures(m_textureCount, m_textures);
            }
        }

        #endregion IDisposable metode
    }
}
