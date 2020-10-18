using System;
using System.Windows;
using System.Windows.Input;
using SharpGL.SceneGraph;
using System.Runtime.InteropServices;

namespace FPSCamera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        private const int BORDER = 100;

        /// <summary>
        ///  Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        /// <summary>
        ///  Pamti staru poziciju kursora da bi mogli da racunamo pomeraj.
        /// </summary>
        private Point oldPos;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World();
                
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta", "GRESKA", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //Iscrtaj svet
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            SetCursorPos((int)(this.Left + this.Width / 2), (int)(this.Top + this.Height / 2));
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            bool outOfBoundsX = false;
            bool outOfBoundsY = false;
            Point point = e.GetPosition(this);

            if (point.Y <= BORDER || point.Y >= this.Height - BORDER)
            {
                outOfBoundsY = true;
            }
            if (point.X <= BORDER || point.X >= this.Width - BORDER)
            {
                outOfBoundsX = true;
            }

            if (!outOfBoundsY && !outOfBoundsX)
            {
                double deltaX = oldPos.X - point.X;
                double deltaY = oldPos.Y - point.Y;
                m_world.UpdateCameraRotation(deltaX, deltaY);
                oldPos = point;
            }
            else
            {
                if (outOfBoundsX)
                {
                    SetCursorPos((int)this.Left + (int)this.Width / 2, (int)this.Top + (int)point.Y);
                    oldPos.X = this.Width / 2;
                    oldPos.Y = point.Y;
                }
                else
                {
                    SetCursorPos((int)this.Left + (int)point.X, (int)this.Top + (int)this.Height / 2);
                    oldPos.Y = this.Height / 2;
                    oldPos.X = point.X;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W: m_world.UpdateCameraPosition(0, 0, 1); break;
                case Key.S: m_world.UpdateCameraPosition(0, 0, -1); break;
                case Key.A: m_world.UpdateCameraPosition(-1, 0, 0); break;
                case Key.D: m_world.UpdateCameraPosition(1, 0, 0); break;
                case Key.Q: m_world.UpdateCameraPosition(0, 1, 0); break;
                case Key.E: m_world.UpdateCameraPosition(0, -1,0); break;
            }
        }
    }
}
