using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Quadrics;

namespace LightSources
{
    /// <summary>
    ///  Nabrojani tip OpenGL podrzanih tipova sencenja
    /// </summary>
    public enum ShadingMode
    {
        Flat,
        Smooth
    };

    /// <summary>
    ///  Nabrojani tip OpenGL nivoa teselacije
    /// </summary>
    public enum TesselationLevel
    {
        Low,
        Medium,
        High
    };

    /// <summary>
    ///  Nabrojani tip OpenGL nivoa teselacije
    /// </summary>
    public enum LightSourceType
    {
        Point,
        Spot,
        Directional
    };

       
    class World
    {

        #region Konstruktori

        public World(OpenGL gl)
        {
            this.gl = gl;
        }

        #endregion

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
        ///	 Izabrani tip teselacije.
        /// </summary>
        private ShadingMode m_selectedShadingMode;

        /// <summary>
        ///	 Izabrani tip sencenja.
        /// </summary>
        private TesselationLevel m_selectedTesselationLevel;

        /// <summary>
        ///	 Izabrani tip svetlosnog izvora.
        /// </summary>
        private LightSourceType m_selectedLightSourceType;

        /// <summary>
        ///	 Broj preseka po X osi.
        /// </summary>
        private int m_slices;

        /// <summary>
        ///	 Broj preseka po Z osi.
        /// </summary>
        private int m_stacks;

        /// <summary>
        ///	 Smer reflektorskog svetlosnog izvora.
        /// </summary>
        private float[] m_spotDirection = { 0.0f, 0.0f, -1.0f };

        /// <summary>
        ///	 Pozicija reflektorskog svetlosnog izvora.
        /// </summary>
        private float[] m_spotPosition = { 0.0f, 0.0f, 75.0f, 1.0f };

        /// <summary>
        ///	 Promenljiva za iscrtavanje sfere
        /// </summary>
        private Sphere shadedSphere;

        /// <summary>
        ///	 Promenljive za iscrtavanje lampe
        /// </summary>
        private Sphere sphereLamp;
        private Cylinder cylinderLamp;

        private OpenGL gl;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Izabrani tip sencenja.
        /// </summary>
        public ShadingMode SelectedShadingMode
        {
            get { return m_selectedShadingMode; }
            set
            {
                m_selectedShadingMode = value;

                if (m_selectedShadingMode == ShadingMode.Flat)
                {
                    gl.ShadeModel(OpenGL.GL_FLAT);
                }
                else
                {
                    gl.ShadeModel(OpenGL.GL_SMOOTH);
                }
            }
        }

        /// <summary>
        ///	 Izabrani tip teselacije.
        /// </summary>
        public TesselationLevel SelectedTesselationLevel
        {
            get { return m_selectedTesselationLevel; }
            set
            {
                m_selectedTesselationLevel = value;

                switch (m_selectedTesselationLevel)
                {
                    case TesselationLevel.Low:
                        m_slices = 8; m_stacks = 8;
                        break;

                    case TesselationLevel.Medium:
                        m_slices = 64; m_stacks = 64;
                        break;

                    case TesselationLevel.High:
                        m_slices = 120; m_stacks = 120;
                        break;
                };
            }
        }

        /// <summary>
        ///	 Izabrani tip svetlosnog izvora.
        /// </summary>
        public LightSourceType SelectedLightSourceType
        {
            get { return m_selectedLightSourceType; }
            set
            {
                m_selectedLightSourceType = value; // Specifikuj novu poziciju i smer svetlosnog izvora
                switch (m_selectedLightSourceType)
                {
                    case LightSourceType.Spot:
                        m_spotPosition[3] = 1.0f;
                        gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 60.0f);
                        gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_EXPONENT, 5.0f);
                        break;

                    case LightSourceType.Point:
                        m_spotPosition[3] = 1.0f;
                        gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
                        break;

                    case LightSourceType.Directional:
                        m_spotPosition[3] = 0.0f;
                        gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, m_spotPosition);
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

        /// <summary>
        ///	 Referenca na OpenGL instancu unutar kontrole aplikacije.
        /// </summary>
        public OpenGL GL
        {
            get;
            set;
        }

        #endregion Properties

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize()
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);

            SetupLighting();

            shadedSphere = new Sphere();
            shadedSphere.CreateInContext(gl);
            shadedSphere.Radius = 50f;
            cylinderLamp = new Cylinder();
            cylinderLamp.CreateInContext(gl);
            sphereLamp = new Sphere();
            sphereLamp.CreateInContext(gl);
            sphereLamp.Radius = 3f;

            m_slices = 8; m_stacks = 8;
            shadedSphere.Material = new SharpGL.SceneGraph.Assets.Material();
            shadedSphere.Material.Ambient = Color.Blue;
            shadedSphere.Material.Diffuse = Color.White;
            shadedSphere.Material.Specular = Color.White;
            shadedSphere.Material.Shininess = 128;
            shadedSphere.Material.Bind(gl);

            cylinderLamp.Material = new SharpGL.SceneGraph.Assets.Material();
            cylinderLamp.Material.Ambient = Color.Gray;
            sphereLamp.Material = new SharpGL.SceneGraph.Assets.Material();
            sphereLamp.Material.Emission = Color.Yellow;
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(int width, int height)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.1f, 500f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        /// Podesavanje osvetljenja
        /// </summary>
        private void SetupLighting()
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

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -250.0f);
            gl.PushMatrix();

            shadedSphere.Slices = m_slices;
            shadedSphere.Stacks = m_stacks;

            //Rotiraj koord. sistem – simulacija kretanja svetlosnog izvora  
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            //Pomeri se u tacku gde je svetlo pozicionirano i nacrtaj kupu
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, m_spotPosition);

            gl.Translate(m_spotPosition[0], m_spotPosition[1], m_spotPosition[2]);

            gl.PushMatrix();
            gl.Scale(10f, 10f, 10f);
            cylinderLamp.Material.Bind(gl);
            cylinderLamp.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //Nacrtaj malu sferu (sijalica)
            sphereLamp.Material.Bind(gl);
            sphereLamp.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //Nacrtaj sferu nad kojom ce se manifestovati efekat osvetljenja
            shadedSphere.Material.Bind(gl);
            shadedSphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
            
            //Oznaci kraj iscrtavanja
            gl.Flush();
        }

        #endregion

    }
}
