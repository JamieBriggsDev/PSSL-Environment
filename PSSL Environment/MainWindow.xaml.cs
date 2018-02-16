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
    }

    
}


