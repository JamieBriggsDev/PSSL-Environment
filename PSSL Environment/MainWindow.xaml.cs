using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using GlmNet;
using Microsoft.Win32;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;

//The main PSSL_Enironment namespace
namespace PSSL_Environment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    // Path to the model file
    //private const string MODEL_PATH = "Resources\\PlayerArwing.obj";

    public partial class MainWindow : Window
    {
        public const UInt32 SPI_GETMOUSESPEED = 0x0070;


        const UInt32 SPIF_UPDATEINIFILE = 0x01;
        const UInt32 SPIF_SENDWININICHANGE = 0x02;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            IntPtr pvParam,
            UInt32 fWinIni);

        public MainWindow()
        {
            InitializeComponent();

            //ModelVisual3D device3D = new ModelVisual3D();
            //device3D.Content = Display3d("C:\\Users\\jamie\\OneDrive\\Individual Project\\Project\\PSSL Environment\\PSSL Environment\\Resources\\Models\\cube.obj");
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;


            if (rb != null)
            {
                string colorName = rb.Tag.ToString();
                switch (colorName)
                {
                    case "singleColour":
                        // Change Title under options
                        PixelColourOptionTitle.Text = "Single Colour";
                        //lgb.
                        break;
                    case "textureColour":
                        PixelColourOptionTitle.Text = "Texture";

                        break;
                    case "templateColour":
                        PixelColourOptionTitle.Text = "Template";
                        break;
                }
            }
            else
            {
                PixelColourOptionTitle.Text = "Error";
            }
        }


        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            // File dialog for opening an image file
            OpenFileDialog op = new OpenFileDialog();
            // Set title
            op.Title = "Select a Picture:";
            // Chose which kind of files are only allowed
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true)
            {
                UsingTexture.IsChecked = true;
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
                System.Drawing.Bitmap myTexture = new System.Drawing.Bitmap(op.FileName);
                myScene.LoadTexture(openGlCtrl.OpenGL, myTexture);
            }

        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;

            //  Add a bit to theta (how much we're rotating the scene) and create the modelview
            //  and normal matrices.
            theta += 0.01f;
            myScene.CreateModelviewAndNormalMatrix(theta);

            //  Clear the color and depth buffer.

            gl.ClearColor(1f, 1f, 1f, 1f);
            //if(singleColorCanvas.SelectedColor == null)
            //{
            //}
            //else
            //{
            //    //glColorx = singleColorCanvas.SelectedColor.Value.R / 255;
            //    //glColory = singleColorCanvas.SelectedColor.Value.G / 255;
            //    //glColorz = singleColorCanvas.SelectedColor.Value.B / 255;
            //    //glColorw = singleColorCanvas.SelectedColor.Value.A / 255;

            //    gl.ClearColor((float)singleColorCanvas.SelectedColor.Value.R / 255.0f,
            //                  (float)singleColorCanvas.SelectedColor.Value.G / 255.0f,
            //                  (float)singleColorCanvas.SelectedColor.Value.B / 255.0f,
            //                  (float)singleColorCanvas.SelectedColor.Value.A / 255.0f);

            //}
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            //  Draw the axies.
            //axies.Render(gl, RenderMode.Design);

            if (UsingTexture.IsChecked == true)
                myScene.RenderTextureMode(gl, checkBoxUseToonShader.IsChecked.Value);
            else
                myScene.RenderColorMode(gl, checkBoxUseToonShader.IsChecked.Value);
            //myScene.RenderImmediateMode(gl);
        }


        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            myScene.Initialise(gl);

            //gl.Enable(OpenGL.GL_DEPTH_TEST);
            //gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Enable transparency
            gl.Enable(OpenGL.GL_BLEND);

            gl.Enable(OpenGL.GL_POLYGON_SMOOTH);

            //gl.Enable(OpenGL.GL_CULL_FACE);

            //  A bit of extra initialisation here, we have to enable textures.
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            //gl.BlendFunc(OpenGL.GL_SRC_ALPHA_SATURATE, OpenGL.GL_ONE);
        }

        private void OpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {


            ////  Get the OpenGL instance.
            //var gl = args.OpenGL;

            ////  Create the projection matrix for the screen size.
            //myScene.CreateProjectionMatrix(gl, (float)ActualWidth, (float)ActualHeight);


            //  Get the OpenGL instance.
            var gl = args.OpenGL;

            //  Create a projection matrix for the scene with the screen size.
            myScene.ResizeViewport(gl, (int)ActualWidth, (int)ActualHeight);
            myScene.CreateProjectionMatrix(gl, (float)ActualWidth, (float)ActualHeight);


            //  When we do immediate mode drawing, OpenGL needs to know what our projection matrix
            //  is, so set it now.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MultMatrix(myScene.ProjectionMatrix.to_array());
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }


        private void fileOpenItem_Click(object sender, RoutedEventArgs e)
        {
            // Create a file open dialog
            var fileOpenDialog = new OpenFileDialog();
            fileOpenDialog.Filter = "Wavefront Files (*.obj)|*.obj|All Files (*.*)|*.*";
            if(fileOpenDialog.ShowDialog(this) == true)
            {
                //  Get the path.
                var filePath = fileOpenDialog.FileName;

                //  Load the data into the scene.
                myScene.Load(openGlCtrl.OpenGL, filePath);

                //  Auto scale.
                textBoxScale.Text = myScene.SetScaleFactorAuto().ToString();
            }

        }



        /// <summary>
        /// The axies, which may be drawn.
        /// </summary>
        private readonly Axies axies = new Axies();

        private float theta = 0;

        /// <summary>
        /// The scene we're drawing.
        /// </summary>
        private readonly Scene myScene = new Scene();

        private void ambientColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Get color canvas colours
            Color ambientColor;
            ambientColor = (Color)ambientColorPicker.SelectedColor;

            vec3 rgbValue;
            rgbValue.x = (float)ambientColor.R / 255;
            rgbValue.y = (float)ambientColor.G / 255;
            rgbValue.z = (float)ambientColor.B / 255;

            myScene.ambientMaterialColor = rgbValue;
        }

        private void diffuseColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Get color canvas colours
            Color diffuseColor;
            diffuseColor = (Color)diffuseColorPicker.SelectedColor;

            vec3 rgbValue;
            rgbValue.x = (float)diffuseColor.R / 255;
            rgbValue.y = (float)diffuseColor.G / 255;
            rgbValue.z = (float)diffuseColor.B / 255;

            myScene.diffuseMaterialColor = rgbValue;
        }

        private void specularColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Get color canvas colours
            Color specularColor;
            specularColor = (Color)specularColorPicker.SelectedColor;

            vec3 rgbValue;
            rgbValue.x = (float)specularColor.R / 255;
            rgbValue.y = (float)specularColor.G / 255;
            rgbValue.z = (float)specularColor.B / 255;

            myScene.specularMaterialColor = rgbValue;
        }

        private void colorAlphaValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Get alpha value from the colorAlpha slider
            float alpha;
            alpha = (float)colorAlphaValue.Value;

            // Safety, may not be needed but for sanity reasons it is
            if(alpha > 1)
            {
                alpha = 1.0f;
            }
            if(alpha < 0)
            {
                alpha = 0.0f;
            }

            // Give the scene that alpha value
            myScene.alphaColor = alpha;
        }

        private void positionX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)positionX.Value;
            myScene.modelLocation.x = value;
        }
        private void positionY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)positionY.Value;
            myScene.modelLocation.y = value;
        }
        private void positionZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)positionZ.Value;
            myScene.modelLocation.z = value;
        }
    }


}
