// -----------------------------------------------------------------------
// <file>PrimitivesWPF.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2016.</copyright>
// <author>Milan Adamovic</author>
// <summary>Demonstracija iscrtavanja preko OpenGL primitiva</summary>
// -----------------------------------------------------------------------
using System;
using System.Collections;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.Enumerations;

namespace Primitives1
{

    /// <summary>
    ///  Nabrojani tip OpenGL podrzanih primitiva 
    /// </summary>
    public enum OpenGLPrimitive
    {
        Points,
        Lines,
        LineStrip,
        LineLoop,
        Triangles,
        TriangleStrip,
        TriangleFan,
        Quads,
        QuadStrip,
        Polygon,
        Axies,
        Grid,
        Cube,
        Teapot,
        //QUadric objekti, nisu OpenGL primitive
        Cylinder,
        Disk,
        Sphere
    }

    ///<summary> Klasa koja enkapsulira OpenGL programski kod </summary>
    class World
    {
        #region Atributi

        /// <summary>
        ///	 Verteksi tacaka, linija i poligona
        /// </summary>
        private float[] pointLinePolygonVertices = new float[]
                {
                    0, 0f,
                    0.5f, 0.2f,
                    0.4f, 0.3f,
                    0.2f, 0.4f,
                    -0.1f, 0.5f,
                    -0.4f, 0.4f,
                    -0.6f, 0.2f,
                    -0.6f, -0.1f,
                    -0.5f,-0.3f,
                    -0.1f, -0.5f,
                    0.3f, -0.4f,
                    0.4f, -0.1f
                };

        /// <summary>
        ///	 Velicine tacaka
        /// </summary>
        private ArrayList m_pointSizeList = new ArrayList();

        /// <summary>
        ///	 Velicina tacaka
        /// </summary>
        private float m_pointSize = 0;

        /// <summary>
        ///	 Debljine linija
        /// </summary>
        private ArrayList m_lineWidthList = new ArrayList();

        /// <summary>
        ///	 Debljina linija
        /// </summary>
        private float m_lineWidth = 0;
        
        /// <summary>
        /// Isprekidanost linija
        /// </summary>
        private ushort m_pattern = 0xFFFF;

        /// <summary>
        /// razmera isprekidanosti linija
        /// </summary>
        int m_patternFactor = 1;
      
        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        /// <summary>
        ///  Izabrana OpenGL primitiva ili objekat za iscrtavanje.
        /// </summary>
        private OpenGLPrimitive m_selectedPrimitive = OpenGLPrimitive.TriangleFan;

        /// <summary>
        ///  Izabrana strana poligona.
        /// </summary>
        private FaceMode m_selectedFaceMode = FaceMode.FrontAndBack;

        /// <summary>
        ///	 Da li iscrtavamo glatke linije i tacke.
        /// </summary>
        private bool m_smooth;


        /// <summary>
        ///  Izabran nacin iscrtavanja poligona.
        /// </summary>
        private PolygonMode m_selectedPolygonMode = PolygonMode.Filled;

        #endregion

        #region Properties

        /// <summary>
        ///  Izabrana OpenGL primitiva ili GLU quadric objekat za iscrtavanje.
        /// </summary>
        public OpenGLPrimitive SelectedPrimitive
        {
            get { return m_selectedPrimitive; }
            set { m_selectedPrimitive = value; }
        }

        /// <summary>
        ///  Raspon debljina tacaka
        /// </summary>
        public ArrayList PointSizeList
        {
            get { return m_pointSizeList; }
            set { m_pointSizeList = value; }
        }

        /// <summary>
        ///  Trenutna debljina tacaka
        /// </summary>
        public float PointSize
        {
            get { return m_pointSize; }
            set { m_pointSize = value; }
        }

        /// <summary>
        ///  Raspon debljina linija
        /// </summary>
        public ArrayList LineWidthList
        {
            get { return m_lineWidthList; }
            set { m_lineWidthList = value; }
        }

        /// <summary>
        ///  Trenutna debljina tacaka
        /// </summary>
        public float LineWidth
        {
            get { return m_lineWidth; }
            set { m_lineWidth = value; }
        }

        /// <summary>
        /// Sablon isprekidanosti linija
        /// </summary>
        public ushort Pattern
        {
            get { return m_pattern; }
            set { m_pattern = value; }
        }

        /// <summary>
        /// Ucestalost isprekidanosti linija
        /// </summary>
        public int PatternFactor
        {
            get { return m_patternFactor; }
            set { m_patternFactor = value; }
        }

        /// <summary>
        ///  Izabrana OpenGL primitiva ili GLU quadric objekat za iscrtavanje.
        /// </summary>
        public FaceMode SelectedFaceMode
        {
            get { return m_selectedFaceMode; }
            set { m_selectedFaceMode = value; }
        }

        /// <summary>
        ///  Izabrana OpenGL primitiva ili GLU quadric objekat za iscrtavanje.
        /// </summary>
        public PolygonMode SelectedPolygonMode
        {
            get { return m_selectedPolygonMode; }
            set { m_selectedPolygonMode = value; }
        }

        /// <summary>
        /// Glatkoca
        /// </summary>
        public bool Smooth
        {
            get { return m_smooth; }
            set { m_smooth = value; }
        }

        #endregion

        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        /// <param name="width">Visina OpenGL kontrole u pikselima.</param>
        /// <param name="height">Sirina OpenGL kontrole u pikselima.</param>
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

            ///Parametri vezani za tacke
            float[] pointSizeRange = new float[2];
            float[] pointSizeGranularity = new float[1];

            // Ocitaj iz promenljive stanja GL_POINT_SIZE_RANGE
            // minimalnu i maksimalnu velicinu (min = prvi el. niza, max = drugi el. niza)
            gl.GetFloat(OpenGL.GL_POINT_SIZE_RANGE, pointSizeRange);
            gl.GetFloat(OpenGL.GL_POINT_SIZE_GRANULARITY, pointSizeGranularity);

            int range = (int)(Math.Round((pointSizeRange[1] - pointSizeRange[0]) / pointSizeGranularity[0]));

            for (int i = 0; i <= range; i++)
            {
                m_pointSizeList.Add(pointSizeRange[0] + i * pointSizeGranularity[0]);
            }

            //Parametri vezani za linije
            float[] lineWidthRange = new float[2];
            float[] lineWidthGranularity = new float[1];

            gl.GetFloat(OpenGL.GL_LINE_WIDTH_RANGE, lineWidthRange);
            gl.GetFloat(OpenGL.GL_LINE_WIDTH_GRANULARITY, lineWidthGranularity);

            range = (int)(Math.Round((lineWidthRange[1] - lineWidthRange[0]) / lineWidthGranularity[0]));

            for (int i = 0; i <= range; i++)
            {
                m_lineWidthList.Add(lineWidthRange[0] + i * lineWidthGranularity[0]);
            }

            gl.Enable(OpenGL.GL_LINE_STIPPLE);
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu, viŠe informacija u nastavku kursa
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            if (m_width >= m_height)
                gl.Ortho(-((float)m_width / (float)m_height), (float)m_width / (float)m_height, -1, 1, -100, 100);
            else
                gl.Ortho(-1, 1, -((float)m_height / (float)m_width), (float)m_height / (float)m_width, -100, 100);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            
            // Podesi velicinu tacke
            gl.PointSize(m_pointSize);

            // Podesi debljinu linije
            gl.LineWidth(m_lineWidth);

            // Podesi vrstu i razmeru isprekidanosti linije
            gl.LineStipple(m_patternFactor, m_pattern);

            // Podesi nacin iscrtavanja poligona
            gl.PolygonMode(m_selectedFaceMode, m_selectedPolygonMode);

            if (m_smooth)
            {
                // Podesi da tacka se iscrtava kao krug, a ne kao kvadrat
                gl.Enable(OpenGL.GL_POINT_SMOOTH);
            }
            else
            {
                gl.Disable(OpenGL.GL_POINT_SMOOTH);
            }


            switch (m_selectedPrimitive)
            {
                case OpenGLPrimitive.Points:
                    {
                        // Iscrtaj 12 tacaka
                        gl.Begin(OpenGL.GL_POINTS);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.Lines:
                    {
                        gl.Begin(OpenGL.GL_LINES);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                             gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.LineStrip:
                    {
                        gl.Begin(OpenGL.GL_LINE_STRIP);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.LineLoop:
                    {
                        gl.Begin(OpenGL.GL_LINE_LOOP);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.Polygon:
                    {
                        gl.Begin(OpenGL.GL_POLYGON);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.Triangles:
                    {
                        gl.Begin(OpenGL.GL_TRIANGLES);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.TriangleStrip:
                    {
                        gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.TriangleFan:
                    {
                        gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.Quads:
                    {
                        gl.Begin(OpenGL.GL_QUADS);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();
                        break;
                    }
                case OpenGLPrimitive.QuadStrip:
                    {
                        gl.Begin(OpenGL.GL_QUAD_STRIP);
                        for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                        {
                            gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1]);
                        }
                        gl.End();

                        break;
                    }
                case OpenGLPrimitive.Cylinder:
                    {
                        gl.PushMatrix();
                        gl.Rotate(-90f, 0f, 0f);
                        gl.Enable(OpenGL.GL_LIGHTING);
                        gl.Enable(OpenGL.GL_LIGHT0);
                        Cylinder cil = new Cylinder();
                        cil.CreateInContext(gl);
                        cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                        gl.Disable(OpenGL.GL_LIGHT0);
                        gl.Disable(OpenGL.GL_LIGHTING);
                        gl.PopMatrix();
                        break;
                    }
                case OpenGLPrimitive.Disk:
                    {
                        gl.PushMatrix();
                        gl.Translate(0f, 0f, -5f);
                        gl.Scale(0.3f, 0.3f, 0.3f);
                        gl.Rotate(30f, 0f, 0f);
                        gl.Enable(OpenGL.GL_LIGHTING);
                        gl.Enable(OpenGL.GL_LIGHT0);
                        Disk disk = new Disk();
                        disk.Loops = 120;
                        disk.Slices = 10;
                        disk.InnerRadius = 1.5f;
                        disk.OuterRadius = 2f;
                        disk.CreateInContext(gl);
                        disk.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                        gl.Disable(OpenGL.GL_LIGHT0);
                        gl.Disable(OpenGL.GL_LIGHTING);
                        gl.PopMatrix();
                        break;
                    }
                case OpenGLPrimitive.Sphere:
                    {
                        gl.PushMatrix();
                        gl.Rotate(90f, 0f, 0f);
                        gl.Scale(0.5f, 0.5f, 0.5f);
                        gl.Enable(OpenGL.GL_LIGHTING);
                        gl.Enable(OpenGL.GL_LIGHT0);
                        Sphere sp = new Sphere();
                        sp.CreateInContext(gl);
                        sp.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                        gl.Disable(OpenGL.GL_LIGHT0);
                        gl.Disable(OpenGL.GL_LIGHTING);
                        gl.PopMatrix();
                        break;
                    }
                case OpenGLPrimitive.Axies:
                    {
                        gl.PushMatrix();
                        Axies ax = new Axies();
                        ax.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
                        gl.PopMatrix();
                        break;
                    }
                case OpenGLPrimitive.Grid:
                    {
                        gl.PushMatrix();
                        Grid grid = new Grid();
                        grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
                        gl.PopMatrix();
                        break;
                    }
                case OpenGLPrimitive.Cube:
                    {
                        gl.PushMatrix();
                        Cube cube = new Cube();
                        cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                        gl.PopMatrix();
                        break;
                    }
                case OpenGLPrimitive.Teapot:
                    {
                        gl.PushMatrix();
                        gl.Enable(OpenGL.GL_LIGHTING);
                        gl.Enable(OpenGL.GL_LIGHT0);
                        Teapot teapot = new Teapot();
                        teapot.Draw(gl, 8, 0.5, OpenGL.GL_FILL);
                        gl.Disable(OpenGL.GL_LIGHT0);
                        gl.Disable(OpenGL.GL_LIGHTING);
                        gl.PopMatrix();
                        break;
                    }
                default:
                    {
                        System.Console.Error.WriteLine("Greska! Odabrani mod iscrtavanja ne postoji");
                        break;
                    }
            }

      /*      if (m_selectedPrimitive == OpenGLPrimitive.Points ||
                m_selectedPrimitive == OpenGLPrimitive.Lines ||
                m_selectedPrimitive == OpenGLPrimitive.LineStrip ||
                m_selectedPrimitive == OpenGLPrimitive.LineLoop ||
                m_selectedPrimitive == OpenGLPrimitive.Triangles ||
                m_selectedPrimitive == OpenGLPrimitive.TriangleStrip ||
                m_selectedPrimitive == OpenGLPrimitive.TriangleFan ||
                m_selectedPrimitive == OpenGLPrimitive.Quads ||
                m_selectedPrimitive == OpenGLPrimitive.QuadStrip ||
                m_selectedPrimitive == OpenGLPrimitive.Polygon)
                for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 2)
                {
                    gl.PushMatrix();
                    gl.PolygonMode(FaceMode.Front, PolygonMode.Filled);
                    gl.Color(1.0, 1.0, 1.0);
                    gl.Translate(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1], 0);
                    gl.Scale(0.1, 0.1, 0.1);
                    gl.Translate(0.25, 0.25, 0.0);
                    gl.DrawText3D("Arial", 12, 0, 0, (i / 2 + 1).ToString());
                    gl.PopMatrix();
                }*/
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
