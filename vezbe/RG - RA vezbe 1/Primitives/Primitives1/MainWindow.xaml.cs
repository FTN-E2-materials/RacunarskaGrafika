// -----------------------------------------------------------------------
// <file>PrimitivesWPF.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2016.</copyright>
// <author>Milan Adamovic</author>
// <summary>Demonstracija iscrtavanja preko OpenGL primitiva</summary>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;
using SharpGL.WPF;
using SharpGL.Enumerations;


namespace Primitives1
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

        #region Konstruktori

        public MainWindow()
        {
            //Inicijalizacija komponenti
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

        #endregion

        #region Rukovaoci dogadjajima OpenGL kontrole

        /// <summary>
        /// Rukovalac dogadjaja iscrtavanja OpenGL kontrole
        /// </summary>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Rukovalac dogadja izmene dimenzija OpenGL kontrole
        /// </summary>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

    #endregion


        private void primitiveTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.SelectedPrimitive = (OpenGLPrimitive)primitiveTypeComboBox.SelectedItem;
            OpenGLPrimitive tempSelectedPrimitive = (OpenGLPrimitive)primitiveTypeComboBox.SelectedItem;
            switch (tempSelectedPrimitive)
            {
                case OpenGLPrimitive.Points:
                    {
                        smooth_CheckBox.Visibility = Visibility.Visible;
                        pointSizeLabel.Visibility = Visibility.Visible;
                        pointSizeComboBox.Visibility = Visibility.Visible;
                        lineWidthLabel.Visibility = Visibility.Collapsed;
                        lineWidthComboBox.Visibility = Visibility.Collapsed;
                        lineStippleLabel.Visibility = Visibility.Collapsed;
                        lineStippleMaskedTextBox.Visibility = Visibility.Collapsed;
                        lineStippleFactorLabel.Visibility = Visibility.Collapsed;
                        lineStippleFactorMaskedTextBox.Visibility = Visibility.Collapsed;
                        polygonFaceModeLabel.Visibility = Visibility.Collapsed;
                        polygonFaceModeComboBox.Visibility = Visibility.Collapsed;
                        polygonModeLabel.Visibility = Visibility.Collapsed;
                        polygonModeComboBox.Visibility = Visibility.Collapsed;
                        break;
                    }
                case OpenGLPrimitive.Lines:
                case OpenGLPrimitive.LineStrip:
                case OpenGLPrimitive.LineLoop:
                    {
                        smooth_CheckBox.Visibility = Visibility.Collapsed;
                        pointSizeLabel.Visibility = Visibility.Collapsed;
                        pointSizeComboBox.Visibility = Visibility.Collapsed;
                        lineWidthLabel.Visibility = Visibility.Visible;
                        lineWidthComboBox.Visibility = Visibility.Visible;
                        lineStippleLabel.Visibility = Visibility.Visible;
                        lineStippleMaskedTextBox.Visibility = Visibility.Visible;
                        lineStippleFactorLabel.Visibility = Visibility.Visible;
                        lineStippleFactorMaskedTextBox.Visibility = Visibility.Visible;
                        polygonFaceModeLabel.Visibility = Visibility.Collapsed;
                        polygonFaceModeComboBox.Visibility = Visibility.Collapsed;
                        polygonModeLabel.Visibility = Visibility.Collapsed;
                        polygonModeComboBox.Visibility = Visibility.Collapsed;
                        break;
                    }
                case OpenGLPrimitive.Triangles:
                case OpenGLPrimitive.TriangleStrip:
                case OpenGLPrimitive.TriangleFan:
                case OpenGLPrimitive.Quads:
                case OpenGLPrimitive.QuadStrip:
                case OpenGLPrimitive.Polygon:
                case OpenGLPrimitive.Cube:
                case OpenGLPrimitive.Teapot:
                case OpenGLPrimitive.Disk:
                case OpenGLPrimitive.Cylinder:
                case OpenGLPrimitive.Sphere:
                    {
                        smooth_CheckBox.Visibility = Visibility.Collapsed;
                        if ((PolygonMode)polygonModeComboBox.SelectedItem == PolygonMode.Points)
                        {
                            pointSizeLabel.Visibility = Visibility.Visible;
                            pointSizeComboBox.Visibility = Visibility.Visible;
                            lineWidthLabel.Visibility = Visibility.Collapsed;
                            lineWidthComboBox.Visibility = Visibility.Collapsed;
                            lineStippleLabel.Visibility = Visibility.Collapsed;
                            lineStippleMaskedTextBox.Visibility = Visibility.Collapsed;
                            lineStippleFactorLabel.Visibility = Visibility.Collapsed;
                            lineStippleFactorMaskedTextBox.Visibility = Visibility.Collapsed;

                        }
                        else if ((PolygonMode)polygonModeComboBox.SelectedItem == PolygonMode.Lines)
                        {
                            pointSizeLabel.Visibility = Visibility.Collapsed;
                            pointSizeComboBox.Visibility = Visibility.Collapsed;
                            lineWidthLabel.Visibility = Visibility.Visible;
                            lineWidthComboBox.Visibility = Visibility.Visible;
                            lineStippleLabel.Visibility = Visibility.Visible;
                            lineStippleMaskedTextBox.Visibility = Visibility.Visible;
                            lineStippleFactorLabel.Visibility = Visibility.Visible;
                            lineStippleFactorMaskedTextBox.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            pointSizeLabel.Visibility = Visibility.Collapsed;
                            pointSizeComboBox.Visibility = Visibility.Collapsed;
                            lineWidthLabel.Visibility = Visibility.Collapsed;
                            lineWidthComboBox.Visibility = Visibility.Collapsed;
                            lineStippleLabel.Visibility = Visibility.Collapsed;
                            lineStippleMaskedTextBox.Visibility = Visibility.Collapsed;
                            lineStippleFactorLabel.Visibility = Visibility.Collapsed;
                            lineStippleFactorMaskedTextBox.Visibility = Visibility.Collapsed;
                        }

                        polygonFaceModeLabel.Visibility = Visibility.Visible;
                        polygonFaceModeComboBox.Visibility = Visibility.Visible;
                        polygonModeLabel.Visibility = Visibility.Visible;
                        polygonModeComboBox.Visibility = Visibility.Visible;
                        break;
                    }
                case OpenGLPrimitive.Axies:
                case OpenGLPrimitive.Grid:
                    {
                        pointSizeLabel.Visibility = Visibility.Collapsed;
                        pointSizeComboBox.Visibility = Visibility.Collapsed;
                        lineWidthLabel.Visibility = Visibility.Collapsed;
                        lineWidthComboBox.Visibility = Visibility.Collapsed;
                        lineStippleLabel.Visibility = Visibility.Visible;
                        lineStippleMaskedTextBox.Visibility = Visibility.Visible;
                        lineStippleFactorLabel.Visibility = Visibility.Visible;
                        lineStippleFactorMaskedTextBox.Visibility = Visibility.Visible;
                        polygonFaceModeLabel.Visibility = Visibility.Collapsed;
                        polygonFaceModeComboBox.Visibility = Visibility.Collapsed;
                        polygonModeLabel.Visibility = Visibility.Collapsed;
                        polygonModeComboBox.Visibility = Visibility.Collapsed;
                        break;
                    }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            primitiveTypeComboBox.ItemsSource = Enum.GetValues(typeof(OpenGLPrimitive));
            pointSizeComboBox.ItemsSource = m_world.PointSizeList;
            lineWidthComboBox.ItemsSource = m_world.LineWidthList;
            polygonFaceModeComboBox.ItemsSource = Enum.GetValues(typeof(FaceMode));
            polygonFaceModeComboBox.SelectedItem = FaceMode.FrontAndBack;
            polygonModeComboBox.ItemsSource = Enum.GetValues(typeof(PolygonMode));
            polygonModeComboBox.SelectedItem = PolygonMode.Filled;
        }

        private void pointSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_world.PointSize = (float)pointSizeComboBox.SelectedItem;
        }

        private void lineWidthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_world.LineWidth = (float)lineWidthComboBox.SelectedItem;
        }

        private void lineStippleMaskedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tempString = lineStippleMaskedTextBox.Text;
            if (tempString.Length == 16)
            {
                foreach (char tempChar in tempString)
                {
                    if (tempChar != '0' && tempChar != '1')
                        return;
                }
                if (m_world != null)
                    m_world.Pattern = Convert.ToUInt16(tempString, 2);
            }
        }

        private void lineStippleFactorMaskedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string tempString = lineStippleFactorMaskedTextBox.Text;
            if (tempString.Length == 3)
            {
                foreach (char tempChar in tempString)
                {
                    if (!Char.IsDigit(tempChar))
                        return;
                }
                UInt16 tempInt = Convert.ToUInt16(tempString, 10);
                if (tempInt > 255)
                {
                    tempInt = 255;
                    lineStippleFactorMaskedTextBox.Text = "255";
                }
                else if (tempInt < 1)
                {
                    tempInt = 1;
                    lineStippleFactorMaskedTextBox.Text = "001";
                }
                if (m_world != null)
                    m_world.PatternFactor = tempInt;
            }
        }

        private void polygonFaceModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_world.SelectedFaceMode = (FaceMode)polygonFaceModeComboBox.SelectedItem;
        }

        private void smooth_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            m_world.Smooth = (bool)smooth_CheckBox.IsChecked;
        }

        private void polygonModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_world.SelectedPolygonMode = (PolygonMode)polygonModeComboBox.SelectedItem;

            if (polygonModeComboBox.Visibility == Visibility.Visible)
                if ((PolygonMode)polygonModeComboBox.SelectedItem == PolygonMode.Points)
                {
                    pointSizeLabel.Visibility = Visibility.Visible;
                    pointSizeComboBox.Visibility = Visibility.Visible;
                    smooth_CheckBox.Visibility = Visibility.Visible;
                    lineWidthLabel.Visibility = Visibility.Collapsed;
                    lineWidthComboBox.Visibility = Visibility.Collapsed;
                    lineStippleLabel.Visibility = Visibility.Collapsed;
                    lineStippleMaskedTextBox.Visibility = Visibility.Collapsed;
                    lineStippleFactorLabel.Visibility = Visibility.Collapsed;
                    lineStippleFactorMaskedTextBox.Visibility = Visibility.Collapsed;

                }
                else if ((PolygonMode)polygonModeComboBox.SelectedItem == PolygonMode.Lines)
                {
                    pointSizeLabel.Visibility = Visibility.Collapsed;
                    pointSizeComboBox.Visibility = Visibility.Collapsed;
                    smooth_CheckBox.Visibility = Visibility.Collapsed;
                    lineWidthLabel.Visibility = Visibility.Visible;
                    lineWidthComboBox.Visibility = Visibility.Visible;
                    lineStippleLabel.Visibility = Visibility.Collapsed;
                    lineStippleMaskedTextBox.Visibility = Visibility.Collapsed;
                    lineStippleFactorLabel.Visibility = Visibility.Collapsed;
                    lineStippleFactorMaskedTextBox.Visibility = Visibility.Collapsed;
                }

                else
                {
                    smooth_CheckBox.Visibility = Visibility.Collapsed;
                    pointSizeLabel.Visibility = Visibility.Collapsed;
                    pointSizeComboBox.Visibility = Visibility.Collapsed;
                    lineWidthLabel.Visibility = Visibility.Collapsed;
                    lineWidthComboBox.Visibility = Visibility.Collapsed;
                    lineStippleLabel.Visibility = Visibility.Collapsed;
                    lineStippleMaskedTextBox.Visibility = Visibility.Collapsed;
                    lineStippleFactorLabel.Visibility = Visibility.Collapsed;
                    lineStippleFactorMaskedTextBox.Visibility = Visibility.Collapsed;
                }
        }
    }
}
