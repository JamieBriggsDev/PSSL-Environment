using GlmNet;
using SharpGL.Shaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Interaction logic for FloatConstant.xaml
    /// </summary>
    public partial class Vec3Constant : UserControl, ConstantsInterface
    {
        public string m_ConstantName {
            get; set; }

        public vec3 InputValue { get; set; }

        public bool Remove { get; set; }

        public Vec3Constant()
        {
            m_ConstantName = "Name";
            InputValue = new vec3(0.0f);
            Remove = false;
                
            InitializeComponent();
        }

        private void ConstantName_SelectionChanged(object sender, RoutedEventArgs e)
        {
            m_ConstantName = ConstantName.Text;
        }

        private void ValueX_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string str = ValueX.Text.ToString();

            float val;
            bool valid;

            try
            {
                val = (float)Convert.ToDouble(ValueX.Text);
                valid = float.TryParse(str, out val);
            } catch (Exception)
            {
                return;
            }

            
            if(valid == true)
            {
                InputValue = new vec3(val, InputValue.y, InputValue.z);
            }
            else
            {
                InputValue = new vec3(0.0f, InputValue.y, InputValue.z);
            }
        }

        private void ValueY_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string str = ValueY.Text.ToString();

            float val;
            bool valid;

            try
            {
                val = (float)Convert.ToDouble(ValueY.Text);
                valid = float.TryParse(str, out val);
            }
            catch (Exception)
            {
                return;
            }


            if (valid == true)
            {
                InputValue = new vec3(InputValue.x, val, InputValue.z);
            }
            else
            {
                InputValue = new vec3(InputValue.x, 0.0f, InputValue.z);
            }
        }

        private void ValueZ_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string str = ValueZ.Text.ToString();

            float val;
            bool valid;

            try
            {
                val = (float)Convert.ToDouble(ValueZ.Text);
                valid = float.TryParse(str, out val);
            }
            catch (Exception)
            {
                return;
            }


            if (valid == true)
            {
                InputValue = new vec3(InputValue.x, InputValue.y, val);
            }
            else
            {
                InputValue = new vec3(InputValue.x, InputValue.y, 0.0f);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).RemoveConstantControl(this);
            ((MainWindow)Application.Current.MainWindow).ConstantsControl.Remove(this);
        }

        void ConstantsInterface.AddToShaderProgram(SharpGL.OpenGL gl, ShaderProgram shader)
        {
            shader.SetUniform3(gl, m_ConstantName, InputValue.x, InputValue.y, InputValue.z);
        }

    }
}
