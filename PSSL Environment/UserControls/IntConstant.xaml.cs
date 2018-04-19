﻿using SharpGL.Shaders;
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
    public partial class IntConstant : UserControl, ConstantsInterface
    {
        public string m_ConstantName {
            get; set; }

        public int InputValue { get; set; }

        public bool Remove { get; set; }

        public IntConstant()
        {
            m_ConstantName = "Name";
            InputValue = 0;
            Remove = false;
                
            InitializeComponent();
        }

        private void ConstantName_SelectionChanged(object sender, RoutedEventArgs e)
        {
            m_ConstantName = ConstantName.Text;
        }

        private void Value_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string str = Value.Text.ToString();

            int val;
            bool valid;

            try
            {
                val = Convert.ToInt32(Value.Text);
                valid = int.TryParse(str, out val);
            } catch (Exception)
            {
                return;
            }

            
            if(valid == true)
            {
                InputValue = val;
            }
            else
            {
                InputValue = 0;
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).RemoveConstantControl(this);
            ((MainWindow)Application.Current.MainWindow).ConstantsControl.Remove(this);
        }

        void ConstantsInterface.AddToShaderProgram(SharpGL.OpenGL gl, ShaderProgram shader)
        {
            shader.SetUniform1(gl, m_ConstantName, InputValue);
        }

    }
}
 