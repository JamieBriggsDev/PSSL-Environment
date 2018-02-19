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

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            // Get color canvas colours
            byte r = singleColorCanvas.R;
            byte g = singleColorCanvas.G;
            byte b = singleColorCanvas.B;
            byte a = singleColorCanvas.A;
            Viewport.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            //scene.
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
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
                imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
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
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            myScene.RenderRetainedMode(gl, checkBoxUseToonShader.IsChecked.Value);
            //myScene.RenderImmediateMode(gl);
        }


        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            myScene.Initialise(gl);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
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
    }


}
