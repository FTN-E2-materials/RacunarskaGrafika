using System;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using System.Windows.Threading;

namespace FPSCamera
{
    class World
    {
        #region Atributi

        //Atribut za unosenje nasumicnosti
        private Random rand;

        // Atributi koji uticu na ponasanje FPS kamere
        private LookAtCamera lookAtCam;
        private float walkSpeed = 0.1f;
        float mouseSpeed = 0.005f;
        double horizontalAngle = 0f;
        double verticalAngle = 0.0f;

        //Pomocni vektori preko kojih definisemo lookAt funkciju
        private Vertex direction;
        private Vertex right;
        private Vertex up;

        // Parametri za animaciju
        private float cubeHeight;
        private bool cubesGoingUp;
        private static int cubeNumber = 12;
        private GLColor[] cubeColors;
        private float sphereSize;
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;

        //Primitive sa kojima iscrtavamo scenu
        Cube cube;
        Sphere sphere;

        #endregion

        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        public World()
        {
            cube = new Cube();
            sphere = new Sphere();
        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            SetupLighting(gl);

            SetupFog(gl);

            SetupAnimation(gl);

            SetupCamera(gl);
        }

        /// <summary>
        /// Podesavanje inicijalnih parametara kamere
        /// </summary>
        private void SetupCamera(OpenGL gl)
        {
            lookAtCam = new LookAtCamera();
            lookAtCam.Position = new Vertex(0f, 0f, 0f);
            lookAtCam.Target = new Vertex(0f, 0f, -10f);
            lookAtCam.UpVector = new Vertex(0f, 1f, 0f);
            right = new Vertex(1f, 0f, 0f);
            direction = new Vertex(0f, 0f, -1f);
            lookAtCam.Target = lookAtCam.Position + direction;
            lookAtCam.Project(gl);
        }

        /// <summary>
        /// Podesavanje parametara animacije
        /// </summary>
        private void SetupAnimation(OpenGL gl)
        {
            // Definisanje tajmera za animaciju
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            timer1.Tick += new EventHandler(UpdateAnimation1);
            timer1.Start();
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(3f);
            timer2.Tick += new EventHandler(UpdateAnimation2);
            timer2.Start();

            cubeHeight = 0f;
            sphereSize = 0f;
            cubesGoingUp = true;
            sphere.CreateInContext(gl);
            sphere.Slices = 100;
            sphere.Stacks = 100;
            cubeColors = new GLColor[cubeNumber];
            rand = new Random();

            for (int i = 0; i < cubeNumber; i++)
            {
                cubeColors[i] = new GLColor((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), 1f);
            }
        }

        /// <summary>
        /// Podesavanje osvetljenja
        /// </summary>
        private void SetupLighting(OpenGL gl)
        {
            float[] blackLight = { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] grayLight = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] whiteLight = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

            // Siva pozadina
            gl.ClearColor(grayLight[0], grayLight[1], grayLight[2], grayLight[3]);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, blackLight);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, grayLight);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, grayLight);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, whiteLight);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            //Ukljuci i podesi color tracking
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128);
        }

        /// <summary>
        /// Podesavanje efekta magle
        /// </summary>
        public void SetupFog(OpenGL gl)
        {
            float[] grayLight = { 0.2f, 0.2f, 0.2f, 1.0f }; // Boja magle = boja pozadine

            //Kreiranje efekta magle
            gl.Enable(OpenGL.GL_FOG);                  // ukljuci efekat magle
            gl.Fog(OpenGL.GL_FOG_COLOR, grayLight);   // boja magle = boja pozadine
            gl.Fog(OpenGL.GL_FOG_START, 1.0f);        // magla pocinje od 5 jedinica
            gl.Fog(OpenGL.GL_FOG_END, 20.0f);         // magla prestaje nakon 50 jedinica
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR); // jednacina za proracun efekta magle
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.1f, 1000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            float[] lightPosition = { -100.0f, 100.0f, 50.0f, 1.0f };
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();

            // Pozicioniraj svetlosni izvor pre transformacija
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, lightPosition);

            //Unosi transformacije u ModelView matricu koristeći svoje trenutno podešene parametre
            lookAtCam.Project(gl);

            DrawFloor(gl);

            DrawCubes(gl);
        }

        /// <summary>
        ///  Iscrtavanje elemenata u sceni.
        /// </summary>
        private void DrawCubes(OpenGL gl)
        {
            gl.PushMatrix();

            float specificCubeHeight;

            gl.Translate(0f, 0f, -10f);
            gl.PushMatrix();
            gl.Translate(0f, 1f, 0f);
            gl.Scale(sphereSize, sphereSize, sphereSize);
            sphere.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            for (int i = 0; i < cubeNumber; i++)
            {
                gl.PopMatrix();
                gl.PushMatrix();
                gl.Rotate(0f, (360f / cubeNumber) * i, 0f);
                specificCubeHeight = i * cubeHeight;
                specificCubeHeight = Clamp(specificCubeHeight, 0f, 3f);
                gl.Translate(5f, specificCubeHeight, 0f);
                gl.Scale(.5f, .5f, .5f);
                gl.Color(cubeColors[i]);
                cube.Render(gl, RenderMode.Render);
            }
            gl.PopMatrix();
            gl.PopMatrix();
        }

        /// <summary>
        ///  Iscrtavanje podloge.
        /// </summary>
        private void DrawFloor(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(0f, -1f, 0f);
            gl.Color(0.60f, 0.40f, 0.10f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Vertex(-100.0f, 0.0f, 100.0f);
            gl.Vertex(100.0f, 0.0f, 100.0f);
            gl.Vertex(100.0f, 0.0f, -100.0f);
            gl.Vertex(-100.0f, 0.0f, -100.0f);
            gl.End();
            gl.PopMatrix();
        }

        /// <summary>
        /// Definiše offset kocki
        /// </summary>
        private void UpdateAnimation1(object sender, EventArgs e)
        {
            if (cubesGoingUp)
            {
                sphereSize += 0.02f;
                cubeHeight += 0.01f;
            }
            else
            {
                sphereSize -= 0.02f;
                cubeHeight -= 0.02f;
            }
        }

        /// <summary>
        /// Obrće smer pomeranja kocki
        /// </summary>
        private void UpdateAnimation2(object sender, EventArgs e)
        {
            if (!cubesGoingUp)
            {
                cubeHeight = 0f;
            }
            cubesGoingUp = !cubesGoingUp;
        }

        /// <summary>
        ///  Funkcija ograničava vrednost na opseg min - max
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        ///  Azurira rotaciju kamere preko pomeraja misa
        /// </summary>
        public void UpdateCameraRotation(double deltaX, double deltaY)
        {
            horizontalAngle += mouseSpeed * deltaX;
            verticalAngle += mouseSpeed * deltaY;

            direction.X = (float)(Math.Cos(verticalAngle) * Math.Sin(horizontalAngle));
            direction.Y = (float)(Math.Sin(verticalAngle));
            direction.Z = (float)(Math.Cos(verticalAngle) * Math.Cos(horizontalAngle));

            right.X = (float)Math.Sin(horizontalAngle - (Math.PI / 2));
            right.Y = 0f;
            right.Z = (float)Math.Cos(horizontalAngle - (Math.PI / 2));

            up = right.VectorProduct(direction);

            lookAtCam.Target = lookAtCam.Position + direction;
            lookAtCam.UpVector = up;
        }

        /// <summary>
        ///  Azurira poziciju kamere preko tipki tastature
        /// </summary>
        public void UpdateCameraPosition(int deltaX, int deltaY, int deltaZ)
        {
            Vertex deltaForward = direction * deltaZ;
            Vertex deltaStrafe = right * deltaX;
            Vertex deltaUp = up * deltaY;
            Vertex delta = deltaForward + deltaStrafe + deltaUp;
            lookAtCam.Position += (delta * walkSpeed);
            lookAtCam.Target = lookAtCam.Position + direction;
            lookAtCam.UpVector = up;
        }

        #endregion

    }
}
