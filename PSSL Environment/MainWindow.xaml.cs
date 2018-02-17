using HelixToolkit.Wpf;
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
using SharpGL;
using Microsoft.Win32;

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

            ModelVisual3D device3D = new ModelVisual3D();
            //device3D.Content = Display3d("C:\\Users\\jamie\\OneDrive\\Individual Project\\Project\\PSSL Environment\\PSSL Environment\\Resources\\Models\\cube.obj");
        }

        //public Model3DGroup load3dModel(string path)
        //{
        //    ObjReader CurrentHelixObjReader = new ObjReader();
        //    // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"D:\3DModel\dinosaur_FBX\dinosaur.fbx");
        //    // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"C:\Users\aaa\Downloads\jlb4kmi4xssg-iphone6model\iphone_6_model.FBX");


        //    Model3DGroup model = null;

        //    string ext = System.IO.Path.GetExtension(path).ToLower();
        //    switch (ext)
        //    {
        //        case ".3ds":
        //            {
        //                var r = new StudioReader();
        //                model = r.Read(path);
        //                break;
        //            }

        //        case ".fbx":
        //            {
        //                var r = new HelixToolkit.Wpf.ObjReader();
        //                model = r.Read(path);
        //                break;
        //            }

        //        case ".lwo":
        //            {
        //                var r = new HelixToolkit.Wpf.LwoReader();
        //                model = r.Read(path);

        //                break;
        //            }

        //        case ".obj":
        //            {
        //                var r = new HelixToolkit.Wpf.ObjReader();
        //                model = r.Read(path);

        //                //Material matty = (MaterialGroup)((GeometryModel3D)model.Children[0]).Material;
        //                //Material myMaterial = MaterialHelper.CreateImageMaterial(@"C:\Users\aaa\Downloads\jlb4kmi4xssg-iphone6model\sam-scrn.jpg", 1);
        //                // Material anotherMaterial = ((GeometryModel3D)model.Children[0]).Material;
        //                //  Newmodel.Children.Add(new GeometryModel3D { Geometry = anotherMaterial, Material = myMaterial });
        //                break;
        //            }

        //        case ".objz":
        //            {
        //                var r = new HelixToolkit.Wpf.ObjReader();
        //                model = r.ReadZ(path);
        //                break;
        //            }

        //        case ".stl":
        //            {
        //                var r = new HelixToolkit.Wpf.StLReader();
        //                model = r.Read(path);
        //                break;
        //            }

        //        case ".off":
        //            {
        //                var r = new HelixToolkit.Wpf.OffReader();
        //                model = r.Read(path);
        //                break;
        //            }

        //        default:
        //            throw new InvalidOperationException("File format not supported.");
        //    }

        //    return model;
        //    //MyModel.Children.Add(MyModel);


        //}

        ///// <summary>
        ///// Display 3D Model
        ///// </summary>
        ///// <param name="model">Path to the Model file</param>
        ///// <returns>3D Model Content</returns>
        //private Model3D Display3d(string model)
        //{
        //    Model3D device = null;
        //    try
        //    {
        //        ////Adding a gesture here
        //        //viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
        //        ////Adding a scale here
        //        //viewPort3d.ZoomGesture = new MouseGesture(MouseAction.RightClick);

        //        //Import 3D model file
        //        ModelImporter import = new ModelImporter();

        //        //Load the 3D model file
        //        device = load3dModel(model);
        //    }
        //    catch (Exception e)
        //    {
        //        // Handle exception in case can not file 3D model
        //        MessageBox.Show("Exception Error : " + e.StackTrace);
        //    }
        //    return device;
        //}

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

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Get the OpenGL instance that's been passed to us.
            OpenGL gl = args.OpenGL;

            //  Clear the color and depth buffers.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Reset the modelview matrix.
            gl.LoadIdentity();

            //  Move the geometry into a fairly central position.
            gl.Translate(-1.5f, 0.0f, -6.0f);

            //  Draw a pyramid. First, rotate the modelview matrix.
            gl.Rotate(rotatePyramid, 0.0f, 1.0f, 0.0f);

            //  Start drawing triangles.
            gl.Begin(OpenGL.GL_TRIANGLES);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 1.0f, 0.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);

            gl.End();

            //  Reset the modelview.
            gl.LoadIdentity();

            //  Move into a more central position.
            gl.Translate(1.5f, 0.0f, -7.0f);

            //  Rotate the cube.
            gl.Rotate(rquad, 1.0f, 1.0f, 1.0f);

            //  Provide the cube colors and geometry.
            gl.Begin(OpenGL.GL_QUADS);

            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);

            gl.Color(1.0f, 0.5f, 0.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);

            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);

            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);

            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, 1.0f);
            gl.Vertex(-1.0f, 1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, -1.0f);
            gl.Vertex(-1.0f, -1.0f, 1.0f);

            gl.Color(1.0f, 0.0f, 1.0f);
            gl.Vertex(1.0f, 1.0f, -1.0f);
            gl.Vertex(1.0f, 1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, 1.0f);
            gl.Vertex(1.0f, -1.0f, -1.0f);

            gl.End();

            //  Flush OpenGL.
            gl.Flush();

            //  Rotate the geometry a bit.
            rotatePyramid += 3.0f;
            rquad -= 3.0f;
        }

        float rotatePyramid = 0;
        float rquad = 0;

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            //  Enable the OpenGL depth testing functionality.
            args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void OpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            // Get the OpenGL instance.
            OpenGL gl = args.OpenGL;

            // Load and clear the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // Perform a perspective transformation
            gl.Perspective(45.0f, (float)gl.RenderContextProvider.Width /
                (float)gl.RenderContextProvider.Height,
                0.1f, 100.0f);

            // Load the modelview.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }


    }


}
