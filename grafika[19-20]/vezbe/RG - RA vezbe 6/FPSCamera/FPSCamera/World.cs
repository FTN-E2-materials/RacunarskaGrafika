using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Assets;
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
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;

        //Primitive preko kojih definisemo scenu
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
            SetupLighting(gl);

            // Podesavanje inicijalnih parametara kamere
            lookAtCam = new LookAtCamera();
            lookAtCam.Position = new Vertex(0f, 0f, 0f);
            lookAtCam.Target = new Vertex(0f, 0f, -10f);
            lookAtCam.UpVector = new Vertex(0f, 1f, 0f);
            right = new Vertex(1f, 0f, 0f);
            direction = new Vertex(0f, 0f, -1f);
            lookAtCam.Target = lookAtCam.Position + direction;
            lookAtCam.Project(gl);
            
            // Definisanje tajmera za animaciju
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            timer1.Tick += new EventHandler(UpdateAnimation1);
            timer1.Start();
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(6f);
            timer2.Tick += new EventHandler(UpdateAnimation2);
            timer2.Start();

            cubeHeight = 0f;
            cubesGoingUp = true;
            sphere.CreateInContext(gl);
            sphere.Slices = 100;
            sphere.Stacks = 100;
        }

        /// <summary>
        /// Definiše offset kocki
        /// </summary>
        private void UpdateAnimation1(object sender, EventArgs e)
        {
            if (cubesGoingUp)
                cubeHeight += 0.01f;
            else
                cubeHeight -= 0.02f;
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
            gl.ShadeModel(OpenGL.GL_SMOOTH);
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
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();

            //Unosi transformacije u ModelView matricu koristeći svoje trenutno podešene parametre
            lookAtCam.Project(gl);

            DrawGrid(gl);

            DrawCubes(gl);
        }


        private void DrawCubes(OpenGL gl)
        {
            gl.PushMatrix();

            float specificCubeHeight;

            gl.Translate(0f, 0f, -10f);
            gl.PushMatrix();
                gl.Scale(1f + cubeHeight, 1f + cubeHeight, 1f + cubeHeight);
                sphere.Render(gl, RenderMode.Render);

            gl.PopMatrix();
            gl.PushMatrix();
            for (int i = 0; i < 8; i++)
            {
                gl.PopMatrix();
                gl.PushMatrix();
                gl.Rotate(0f, 45f * i, 0f);
                specificCubeHeight = i * cubeHeight;
                specificCubeHeight = Clamp(specificCubeHeight, 0f, 3f);
                gl.Translate(5f, specificCubeHeight, 0f);
                gl.Scale(.5f, .5f, .5f);
                cube.Render(gl, RenderMode.Render);
            }
            gl.PopMatrix();
            gl.PopMatrix();
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

        /// <summary>
        ///  Iscrtavanje SharpGL primitive grida.
        /// </summary>
        private void DrawGrid(OpenGL gl)
        {
            gl.PushMatrix();
            Grid grid = new Grid();
            gl.Translate(0f, -1f, -10f);
            gl.Rotate(90f, 0f, 0f);
            grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
            gl.PopMatrix();
        }


        #endregion

        #region IDisposable Metode
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
            timer1.Stop();
            timer2.Stop();
        }

        #endregion
    }
}
