using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PSSL_Environment.UserControls
{
    /// <summary>
    /// Interaction logic for CameraController.xaml
    /// </summary>
    public partial class CameraController : UserControl
    {
        public CameraController()
        {
            InitializeComponent();
        }

        public float InOut = 0.0f;
        public float LeftRight = 0.0f;
        public float UpDown = 0.0f;

        private void OutButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).myScene.myCamera.trackInOut(0.05f);
            InOut += 0.05f;
            //Debug.WriteLine(string.Format("InOut = {0}\nLeftRight = {1}\n UpDown = {2}\n", InOut, LeftRight, UpDown));
            InOut_Value.Text = string.Format("{0:f2}", InOut);
        }

        private void InButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).myScene.myCamera.trackInOut(-0.05f);
            InOut -= 0.05f;
            InOut_Value.Text = string.Format("{0:f2}", InOut);
            //Debug.WriteLine(string.Format("InOut = {0}\nLeftRight = {1}\n UpDown = {2}\n", InOut, LeftRight, UpDown));
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).myScene.myCamera.TrackLeftRight(-0.05f);
            LeftRight += 0.05f;
            LeftRight_Value.Text = string.Format("{0:f2}", LeftRight);
            //Debug.WriteLine(string.Format("InOut = {0}\nLeftRight = {1}\n UpDown = {2}\n", InOut, LeftRight, UpDown));
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).myScene.myCamera.TrackLeftRight(0.05f);
            LeftRight -= 0.05f;
            LeftRight_Value.Text = string.Format("{0:f2}", LeftRight);
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).myScene.myCamera.trackUpDown(0.05f);
            UpDown += 0.05f;
            UpDown_value.Text = string.Format("{0:f2}", UpDown);
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).myScene.myCamera.trackUpDown(-0.05f);
            UpDown -= 0.05f;
            UpDown_value.Text = string.Format("{0:f1}", UpDown);

        }
    }
}
