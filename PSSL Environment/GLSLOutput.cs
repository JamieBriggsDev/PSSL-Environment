using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PSSL_Environment
{
    class GLSLOutput
    {
        private static GLSLOutput m_instance;
        // Get instance to the output class
        public static GLSLOutput GetInstance()
        {
            if(m_instance == null)
            {
                m_instance = new GLSLOutput();
            }
            return m_instance;
        }
        static private string ShaderName;
        private string FilePath;

        public void SetShaderName(string name)
        {
            ShaderName = name;
        }

        public void OutputGLSL()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Desktop";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                FilePath = dialog.FileName;
            }

            // Throws error if files already exist
            try
            {

                switch (((MainWindow)Application.Current.MainWindow).GetViewType())
                {
                    //                ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert"),
                    //ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.frag"), attributeLocations);
                    case MainWindow.ViewType.COLOR:
                            File.Copy(@"Shaders\PerPixel.vert", FilePath + "\\" + ShaderName + ".vert");
                            File.Copy(@"Shaders\PerPixel.frag", FilePath + "\\" + ShaderName + ".frag");
                        break;
                    case MainWindow.ViewType.TEXTURE:
                        File.Copy(@"Shaders\PerPixelTexture.vert", FilePath + "\\" + ShaderName + ".vert");
                        File.Copy(@"Shaders\PerPixelTexture.frag", FilePath + "\\" + ShaderName + ".frag");
                        break;
                    case MainWindow.ViewType.TOON:
                        File.Copy(@"Shaders\Toon.vert", FilePath + "\\" + ShaderName + ".vert");
                        File.Copy(@"Shaders\Toon.frag", FilePath + "\\" + ShaderName + ".frag");
                        break;
                    case MainWindow.ViewType.TOONTEXTURE:
                        File.Copy(@"Shaders\ToonTexture.vert", FilePath + "\\" + ShaderName + ".vert");
                        File.Copy(@"Shaders\ToonTexture.frag", FilePath + "\\" + ShaderName + ".frag");
                        break;
                    case MainWindow.ViewType.RIPPLE:
                        File.Copy(@"Shaders\Ripple.vert", FilePath + "\\" + ShaderName + ".vert");
                        File.Copy(@"Shaders\Ripple.frag", FilePath + "\\" + ShaderName + ".frag");
                        break;
                    case MainWindow.ViewType.CUSTOM:
                        string path = FilePath + "\\" + ShaderName;
                        File.WriteAllText(path + ".vert", 
                            ((MainWindow)Application.Current.MainWindow).VertexShaderText.Text);
                        File.WriteAllText(path + ".frag",
                            ((MainWindow)Application.Current.MainWindow).VertexShaderText.Text);
                        break;
                }

            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show(ShaderName + " files already exist at " + FilePath, "Error loading OBJ.",
                    MessageBoxButton.OK, MessageBoxImage.Error);

            }
}

    }
}
