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

namespace PSSL_Environment
{
    /// <summary>
    /// Interaction logic for Vec3Constant.xaml
    /// </summary>
    public partial class Vec3Constant : UserControl
    {
        public string name = "";
        public float x = 0.0f;
        public float y = 0.0f;
        public float z = 0.0f;
        public Vec3Constant()
        {
            InitializeComponent();
        }
        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox temp = sender as TextBox;
            name = temp.Text;
        }

        private void Vec3X_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox temp = sender as TextBox;
            x = (float)Convert.ToDouble(temp.Text);
        }

        private void Vec3Y_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox temp = sender as TextBox;
            y = (float)Convert.ToDouble(temp.Text);
        }

        private void Vec3Z_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox temp = sender as TextBox;
            z = (float)Convert.ToDouble(temp.Text);
        }
    }
}
