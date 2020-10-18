using System;
using System.Windows;
using System.Windows.Input;
using SharpGL.SceneGraph;

namespace LightSources
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi
        /// <summary>
        ///  Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

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
                m_world = new World(openGLControl.OpenGL);
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
            m_world.Draw();
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize();
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize((int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W: m_world.RotationX -= 2.0f; break;
                case Key.S: m_world.RotationX += 2.0f; break;
                case Key.A: m_world.RotationY -= 2.0f; break;
                case Key.D: m_world.RotationY += 2.0f; break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cb1.ItemsSource = Enum.GetValues(typeof(ShadingMode));
            cb2.ItemsSource = Enum.GetValues(typeof(TesselationLevel));
            cb3.ItemsSource = Enum.GetValues(typeof(LightSourceType));
            cb1.SelectedIndex = 0;
            cb2.SelectedIndex = 0;
            cb3.SelectedIndex = 0;
        }

        private void cb1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.SelectedShadingMode = (ShadingMode)cb1.SelectedIndex;
            openGLControl.Focus();
        }

        private void cb2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.SelectedTesselationLevel = (TesselationLevel)cb2.SelectedIndex;
            openGLControl.Focus();
        }

        private void cb3_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.SelectedLightSourceType = (LightSourceType)cb3.SelectedIndex;
            openGLControl.Focus();
        }
    }
}
