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

        public Model3DGroup load3dModel(string path)
        {
            ObjReader CurrentHelixObjReader = new ObjReader();
            // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"D:\3DModel\dinosaur_FBX\dinosaur.fbx");
            // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"C:\Users\aaa\Downloads\jlb4kmi4xssg-iphone6model\iphone_6_model.FBX");


            Model3DGroup model = null;

            string ext = System.IO.Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".3ds":
                    {
                        var r = new StudioReader();
                        model = r.Read(path);
                        break;
                    }

                case ".fbx":
                    {
                        var r = new HelixToolkit.Wpf.ObjReader();
                        model = r.Read(path);
                        break;
                    }

                case ".lwo":
                    {
                        var r = new HelixToolkit.Wpf.LwoReader();
                        model = r.Read(path);

                        break;
                    }

                case ".obj":
                    {
                        var r = new HelixToolkit.Wpf.ObjReader();
                        model = r.Read(path);

                        //Material matty = (MaterialGroup)((GeometryModel3D)model.Children[0]).Material;
                        //Material myMaterial = MaterialHelper.CreateImageMaterial(@"C:\Users\aaa\Downloads\jlb4kmi4xssg-iphone6model\sam-scrn.jpg", 1);
                        // Material anotherMaterial = ((GeometryModel3D)model.Children[0]).Material;
                        //  Newmodel.Children.Add(new GeometryModel3D { Geometry = anotherMaterial, Material = myMaterial });
                        break;
                    }

                case ".objz":
                    {
                        var r = new HelixToolkit.Wpf.ObjReader();
                        model = r.ReadZ(path);
                        break;
                    }

                case ".stl":
                    {
                        var r = new HelixToolkit.Wpf.StLReader();
                        model = r.Read(path);
                        break;
                    }

                case ".off":
                    {
                        var r = new HelixToolkit.Wpf.OffReader();
                        model = r.Read(path);
                        break;
                    }

                default:
                    throw new InvalidOperationException("File format not supported.");
            }

            return model;
            //MyModel.Children.Add(MyModel);


        }

        /// <summary>
        /// Display 3D Model
        /// </summary>
        /// <param name="model">Path to the Model file</param>
        /// <returns>3D Model Content</returns>
        private Model3D Display3d(string model)
        {
            Model3D device = null;
            try
            {
                //Adding a gesture here
                viewPort3d.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                //Adding a scale here
                viewPort3d.ZoomGesture = new MouseGesture(MouseAction.RightClick);

                //Import 3D model file
                ModelImporter import = new ModelImporter();

                //Load the 3D model file
                device = load3dModel(model);
            }
            catch (Exception e)
            {
                // Handle exception in case can not file 3D model
                MessageBox.Show("Exception Error : " + e.StackTrace);
            }
            return device;
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
            Helix3D_Viewport.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }
        //    private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        //    {
        //        RadioButton rb = sender as RadioButton;


        //        if (rb != null)
        //        {
        //            string colorName = rb.Tag.ToString();
        //            switch (colorName)
        //            {
        //                case "singleColour":
        //                    PixelShaderGrid.Children.Remove(visualBoard);
        //                    //lgb.
        //                    break;
        //                case "textureColour":
        //                    PixelColourOptionTitle.Text = "Texture";

        //                    break;
        //                case "templateColour":
        //                    PixelColourOptionTitle.Text = "Template";
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            PixelColourOptionTitle.Text = "Error";
        //        }
        //    }

        //    // Visual Board for colours
        //    Rectangle visualBoard = new Rectangle();

        //    public void CreateVisualBrush()
        //    {
        //        // Create a background recntangle



        //        visualBoard.Width = 300;

        //        visualBoard.Height = 300;



        //        // Create a DrawingBrush

        //        VisualBrush vBrush = new VisualBrush();



        //        // Create a StackPanel and add a few controls to it

        //        StackPanel stkPanel = new StackPanel();



        //        // Create a Rectangle and add it to StackPanel

        //        Rectangle Rectangle = new Rectangle();

        //        Rectangle.Height = 100;

        //        Rectangle.Width = 20;

        //        // Linear Gradient Brush Settings!!!
        //        //
        //        //
        //        LinearGradientBrush HueBrush = new LinearGradientBrush();

        //        HueBrush.StartPoint = new Point(0, 0);

        //        HueBrush.EndPoint = new Point(0, 1);


        //        // RedStart Value
        //        GradientStop RedStart = new GradientStop();

        //        RedStart.Color = Color.FromRgb(255, 0, 0);

        //        RedStart.Offset = 0.0;

        //        HueBrush.GradientStops.Add(RedStart);


        //        // Yellow Value
        //        GradientStop Yellow = new GradientStop();

        //        Yellow.Color = Color.FromRgb(255, 255, 0);

        //        Yellow.Offset = 0.167;

        //        HueBrush.GradientStops.Add(Yellow);


        //        // Green Value
        //        GradientStop Green = new GradientStop();

        //        Green.Color = Color.FromRgb(0, 255, 0);

        //        Green.Offset = 0.333;

        //        HueBrush.GradientStops.Add(Green);


        //        // LightBlue Value
        //        GradientStop LightBlue = new GradientStop();

        //        LightBlue.Color = Color.FromRgb(0, 255, 255);

        //        LightBlue.Offset = 0.5;

        //        HueBrush.GradientStops.Add(LightBlue);


        //        // Blue Value
        //        GradientStop Blue = new GradientStop();

        //        Blue.Color = Color.FromRgb(0, 0, 255);

        //        Blue.Offset = 0.667;

        //        HueBrush.GradientStops.Add(Blue);


        //        // Magenta Value
        //        GradientStop Magenta = new GradientStop();

        //        Magenta.Color = Color.FromRgb(255, 0, 255);

        //        Magenta.Offset = 0.833;

        //        HueBrush.GradientStops.Add(Magenta);

        //        // RedFinish Value
        //        GradientStop RedFinish = new GradientStop();

        //        RedFinish.Color = Color.FromRgb(255, 0, 0);

        //        RedFinish.Offset = 1.0;

        //        HueBrush.GradientStops.Add(RedFinish);


        //        Rectangle.Fill = HueBrush;



        //        stkPanel.Children.Add(Rectangle);

        //        ///////////////////////////////////////////////



        //        // Set Viewport and TileMode
        //        vBrush.Viewport = new Rect(0, 0, 0.25, 0.25);
        //        vBrush.TileMode = TileMode.None;



        //        // Set Visual of VisualBrush
        //        vBrush.ca
        //        vBrush.Visual = stkPanel;



        //        // Fill rectangle with a DrawingBrush

        //        visualBoard.Fill = vBrush;



        //        PixelShaderGrid.Children.Add(visualBoard);
        //    }

        //}


    }


}