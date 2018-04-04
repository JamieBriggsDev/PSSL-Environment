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
using System.Windows.Media.Animation;
using PSSL_Environment.UserControls;

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
        public List<ConstantsInterface> ConstantsControl = new List<ConstantsInterface>();

        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            //ModelVisual3D device3D = new ModelVisual3D();
            //device3D.Content = Display3d("C:\\Users\\jamie\\OneDrive\\Individual Project\\Project\\PSSL Environment\\PSSL Environment\\Resources\\Models\\cube.obj");
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
            //this.MinHeight = this.Height;
            //this.MaxHeight = this.Height;
            //this.MinWidth = this.Width;
            //this.MaxWidth = this.Width;
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ToolBar_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.DragMove();
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

            ambientColorPicker.SelectedColor = Color.FromRgb(255, 255, 255);
            //ambientColorPicker.SelectedColorChanged();

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

            gl.ClearColor(0.85f, 0.85f, 0.85f, 1f);
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
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            
            //  Draw the axies.
            //axies.Render(gl, RenderMode.Design);
            if(myScene.graphicsSettings == UsingSettings.BASIC)
            {
                if(WaterEnabled.IsChecked == true)
                {
                    myScene.RenderWaterMode(gl);
                }
                else
                {
                    if (UsingTexture.IsChecked == true)
                        myScene.RenderTextureMode(gl, checkBoxUseToonShader.IsChecked.Value);
                    else
                        myScene.RenderColorMode(gl, checkBoxUseToonShader.IsChecked.Value);
                    //myScene.RenderImmediateMode(gl);
                }
            }

            if (myScene.graphicsSettings == UsingSettings.ADVANCED)
            {
                myScene.RenderCustomMode(gl);
                return;
            }



        }


        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.PolygonMode(FaceMode.Front, PolygonMode.Filled);

            myScene.Initialise(gl);

            // Enable transparency
            gl.Enable(OpenGL.GL_BLEND);

            //gl.Enable(OpenGL.GL_DEPTH_TEST);

            //gl.Enable(OpenGL.GL_DEPTH_RENDERABLE);

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            //  A bit of extra initialisation here, we have to enable textures.
            gl.BlendEquation(OpenGL.GL_ADD);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            openGlCtrl.FrameRate = 120;

            openGlCtrl.DrawFPS = true;
            //gl.Enable(OpenGL.GL_FPS)
            //gl.Enable(OpenGL.GL_DOUBLEBUFFER);

            //gl.Enable(OpenGL.GL_TEXTURE_2D);
            //// Enable antialiasing
            //gl.Enable(OpenGL.GL_POINT_SMOOTH);
            //gl.Enable(OpenGL.GL_LINE_SMOOTH);
            //gl.Enable(OpenGL.GL_POLYGON_SMOOTH);
            //// Telling the OpenGL driver how to do polygon smoothing antialiasing
            //gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
            //gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);
            //gl.Hint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_NICEST);

            //gl.LineWidth(0.0f);
            //SharpGL.RenderContextType




            //gl.Enable(OpenGL.GL_MULTISAMPLE);

            //gl.ShadeModel(OpenGL.GL_SMOOTH);

            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Enable(OpenGL.GL_DEPTH_BUFFER);

            gl.FrontFace(OpenGL.GL_CCW);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.ClearDepth(1.0f);
            gl.DepthFunc(OpenGL.GL_LEQUAL);
            gl.DepthMask(1);
            gl.Hint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);
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
            fileOpenDialog.Filter = "Wavefront Files (*.obj)|*.obj";
            if (fileOpenDialog.ShowDialog(this) == true)
            {
                //  Get the path.
                var filePath = fileOpenDialog.FileName;

                //  Load the data into the scene.

                myScene.Load(openGlCtrl, filePath);
                //openGlCtrl.Cursor = Cursors.;

            }

            //}

        }

        /// <summary>
        /// The axies, which may be drawn.
        /// </summary>
        private readonly Axies axies = new Axies();

        private float theta = 0;

        /// <summary>
        /// The scene we're drawing.
        /// </summary>
        public readonly Scene myScene = new Scene();

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

        private void rotationX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)rotationX.Value;
            myScene.modelRotation.x = value;
        }

        private void rotationY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)rotationY.Value;
            myScene.modelRotation.y = value;
        }

        private void rotationZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)rotationZ.Value;
            myScene.modelRotation.z = value;
        }



        private void lightPositionX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)lightPositionX.Value;
            myScene.lightLocation.x = value;
        }

        private void lightPositionY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)lightPositionY.Value;
            myScene.lightLocation.y = value;
        }

        private void lightPositionZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)lightPositionZ.Value;
            myScene.lightLocation.z = value;
        }
        private void SmoothingEnabled_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            var gl = openGlCtrl.OpenGL;
            if(checkBox.IsChecked == true)
            {
                //gl.BlendFunc(OpenGL.GL_SRC_ALPHA_SATURATE, OpenGL.GL_ONE);
                //gl.Enable(OpenGL.GL_POINT_SMOOTH);
                //gl.Enable(OpenGL.GL_LINE_SMOOTH);
                gl.Enable(OpenGL.GL_POLYGON_SMOOTH);
                // Telling the OpenGL driver how to do polygon smoothing antialiasing
                //gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
                //gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);
                gl.Hint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_NICEST);
            }
            else
            {
                gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                gl.Disable(OpenGL.GL_POINT_SMOOTH);
                gl.Disable(OpenGL.GL_LINE_SMOOTH);
                gl.Disable(OpenGL.GL_POLYGON_SMOOTH);
            }
        }

        private void WaterEnabled_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Bitmap myTexture = new System.Drawing.Bitmap(@"Resources\Textures\Water.png");
            myScene.LoadWaterTextures(openGlCtrl.OpenGL, myTexture);
        }

        private void AmplitudeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)AmplitudeSlider.Value;
            myScene.amplitude = value;
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)SpeedSlider.Value;
            myScene.speed = value;
        }

        private void FrequencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)FrequencySlider.Value;
            myScene.frequency = value;
        }

        

        private void CompileShadersAdvButton_Click(object sender, RoutedEventArgs e)
        {
            CompileErrorOutput.Text = myScene.CompileCustomShader(openGlCtrl.OpenGL,
                VertexShaderText.Text, FragmentShaderText.Text);
        }


        private void BasicSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            BasicShaderScroll.Visibility = Visibility.Visible;
            AdvancedShaderCanvas.Visibility = Visibility.Hidden;
            ViewportSettingsCanvas.Visibility = Visibility.Hidden;
            ButtonSelector.SetValue(System.Windows.Controls.Grid.RowProperty, 0);
            myScene.graphicsSettings = UsingSettings.BASIC;
        }

        private void AdvancedSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AdvancedShaderCanvas.Visibility = Visibility.Visible;
            BasicShaderScroll.Visibility = Visibility.Hidden;
            ViewportSettingsCanvas.Visibility = Visibility.Hidden;
            ButtonSelector.SetValue(System.Windows.Controls.Grid.RowProperty, 1);
            myScene.graphicsSettings = UsingSettings.ADVANCED;
        }

        private void ViewportSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewportSettingsCanvas.Visibility = Visibility.Visible;
            BasicShaderScroll.Visibility = Visibility.Hidden;
            AdvancedShaderCanvas.Visibility = Visibility.Hidden;
        }

        private void UsingTexture_Click(object sender, RoutedEventArgs e)
        {
            var temp = sender as CheckBox;
            if(temp.IsChecked == true)
            {
                LoadPictureCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                LoadPictureCanvas.Visibility = Visibility.Hidden;
            }
        }

        private void DockButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleDockButton.IsChecked = !ToggleDockButton.IsChecked;
        }

        public void RemoveConstantControl(UserControl ctl)
        {
            try
            {
                ConstantsDock.Children.Remove(ctl);
            } catch (Exception)
            {
                return;
            }

            ctl = null;
        }
        private void AddFloatConstant_Click(object sender, RoutedEventArgs e)
        {
            FloatConstant newConstant = new FloatConstant();
            ConstantsDock.Children.Add(newConstant);
            DockPanel.SetDock(newConstant, Dock.Top);

            ConstantsControl.Add(newConstant);
        }

        private void AddIntConstant_Click(object sender, RoutedEventArgs e)
        {
            IntConstant newConstant = new IntConstant();
            ConstantsDock.Children.Add(newConstant);
            DockPanel.SetDock(newConstant, Dock.Top);

            ConstantsControl.Add(newConstant);
        }

        private void AddColorConstant_Click(object sender, RoutedEventArgs e)
        {
            ColorConstant newConstant = new ColorConstant();
            ConstantsDock.Children.Add(newConstant);
            DockPanel.SetDock(newConstant, Dock.Top);

            ConstantsControl.Add(newConstant);
        }

        private void AddVec3Constant_Click(object sender, RoutedEventArgs e)
        {
            Vec3Constant newConstant = new Vec3Constant();
            ConstantsDock.Children.Add(newConstant);
            DockPanel.SetDock(newConstant, Dock.Top);

            ConstantsControl.Add(newConstant);
        }

        private void CompileShaders_Click(object sender, RoutedEventArgs e)
        {
            if(myScene.graphicsSettings == UsingSettings.ADVANCED)
            {
                Interpreter.GetInstance().GeneratePSSLAdvanced(FragmentShaderText.Text,
                    VertexShaderText.Text);
            }
            else
            {
                Interpreter.GetInstance().GeneratePSSLBasic();
            }

        }



        

        //private void CompileShaders_Click(object sender, RoutedEventArgs e)
        //{

        //}
        //Interpreter.GetInstance().GenerateConstants(ManifestResourceLoader.LoadTextFile(@"Shaders\Custom\CustomFrag.frag"), 
        //    ManifestResourceLoader.LoadTextFile(@"Shaders\Custom\CustomVertex.vert"));
    }


}
