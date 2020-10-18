using System;
using System.Windows;
using System.Windows.Input;

namespace NapredniMehanizmiIscrtavanja
{
    public partial class MainWindow : Window
    {
        #region Atributi

        World m_world = null;

        #endregion

        #region Konstruktori

        public MainWindow()
        {
            InitializeComponent();

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

        #endregion

        #region Rukovaoci dogadjajima OpenGL kontrole

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL,(int) Width,(int) Height);
        }

        /// <summary>
        /// Rukovalac dogadjaja iscrtavanja OpenGL kontrole
        /// </summary>
        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            m_world.Draw();
        }

        #endregion

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case (Key.I):
                    {
                        m_world.CubesRotationX--;
                        break;
                    }
                case (Key.K):
                    {
                        m_world.CubesRotationX++;
                        break;
                    }
                case (Key.J):
                    {
                        m_world.CubesRotationY--;
                        break;
                    }
                case (Key.L):
                    {
                        m_world.CubesRotationY++;
                        break;
                    }
            }
        }
    }
}
