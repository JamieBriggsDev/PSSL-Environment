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
    

    
    public partial class ColorConstant : UserControl, ConstantsInterface
    {
        public string m_ConstantName {
            get; set; }

        public vec3 InputValue { get; set; }

        public bool Remove { get; set; }

        public ColorConstant()
        {
            m_ConstantName = "Name";
            InputValue = new vec3(1.0f, 1.0f, 1.0f);
            Remove = false;
                
            InitializeComponent();
        }

        private void ConstantName_SelectionChanged(object sender, RoutedEventArgs e)
        {
            m_ConstantName = ConstantName.Text;
        }

        public vec3 ColorToVec(Color _color)
        {
            vec3 output;
            output.x = _color.R / 255.0f;
            output.y = _color.G / 255.0f;
            output.z = _color.B / 255.0f;

            return output;
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

        private void Value_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Color col = (Color)Value.SelectedColor;
            InputValue = ColorToVec(col);
        }
    }
}
