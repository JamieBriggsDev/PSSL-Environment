using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PSSL_Environment
{ 
    public class Interpreter
    {
        // Instance of the interpreter to make
        //  it a singleton
        private static Interpreter m_instance;

        private string filePath;

        private string outputStructName;

        private string ShaderName;

        private string ShaderVertexFilename;
        private string ShaderConstantsFileName;
        private string ShaderOutputFileName;

        private Interpreter() { }

        private enum Type { MAT4, VEC3, VEC4, FLOAT};

        private List<KeyValuePair<Type, string>> Constants = new List<KeyValuePair<Type, string>>();

        // Get instance to the interpreter class
        public static Interpreter GetInstance()
        {

            if(m_instance == null)
            {
                m_instance = new Interpreter(); 
            }
            return m_instance;
        }

        public void SetShaderName(string name)
        {
            ShaderName = name;
        }


        public void GeneratePSSLAdvanced(string Frag, string Vert)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Desktop";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                filePath = dialog.FileName;
            }

            //string fragmentShader = Frag;
            //string vertexShader = Vert;
            Constants = new List<KeyValuePair<Type, string>>();

            // Read through the vertex shader first
            using (StringReader reader = new StringReader(Vert))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null && line.Contains("uniform"))
                    {
                        string[] temp = line.Split(new char[] { ' ', ';'});
                        //line.Trim(new char[] { ' ', ';' });
                        //line.Replace("uniform ", "");
                        if(temp[1].Contains("vec3"))
                        {
                            //line.Replace("vec3", "" );
                            Constants.Add(new KeyValuePair<Type, string>(Type.VEC3, temp[2]));
                        }
                        else if (temp[1].Contains("mat4"))
                        {
                            //line.Replace("vec3", "" );
                            Constants.Add(new KeyValuePair<Type, string>(Type.MAT4, temp[2]));
                        }
                        else if (temp[1].Contains("float"))
                        {
                            //line.Replace("vec3", "" );
                            Constants.Add(new KeyValuePair<Type, string>(Type.FLOAT, temp[2]));
                        }
                    }
                    
                } while (line != null);
            }

            // Read through the fragment shader second
            using (StringReader reader = new StringReader(Frag))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null && line.Contains("uniform"))
                    {
                        string[] temp = line.Split(new char[] { ' ', ';' });

                        bool clonedConstant = false;

                        // Checks to see if constant was found from vertex shader first
                        //  and doesnt add it if it is already contained
                        foreach(KeyValuePair<Type, string> pair in Constants)
                        {
                            if(temp[2] == pair.Value)
                            {
                                clonedConstant = true;
                                break;
                            }
                        }

                        if (clonedConstant == false)
                        {
                            //line.Trim(new char[] { ' ', ';' });
                            //line.Replace("uniform ", "");
                            if (temp[1].Contains("vec3"))
                            {
                                //line.Replace("vec3", "" );
                                Constants.Add(new KeyValuePair<Type, string>(Type.VEC3, temp[2]));
                            }
                            else if (temp[1].Contains("mat4"))
                            {
                                //line.Replace("vec3", "" );
                                Constants.Add(new KeyValuePair<Type, string>(Type.MAT4, temp[2]));
                            }
                            else if (temp[1].Contains("float"))
                            {
                                //line.Replace("vec3", "" );
                                Constants.Add(new KeyValuePair<Type, string>(Type.FLOAT, temp[2]));
                            } 
                        }
                    }

                } while (line != null);
            }
            SetShaderName("test");
            GeneratePSSLConstantsFile();
            GeneratePSSLOutputFile();
            GeneratePSSLVertexFile(Vert);
        }

        public void GeneratePSSLConstantsFile()
        {
            string file, ClassName;
            // Comment at the start
            file = "// " + ShaderName + " Shader Constants" + Environment.NewLine;

            // find what the name of the file is in preprocessor standar if #ifndefs
            ClassName = ShaderName;
            ClassName = ClassName.ToUpper();
            ClassName = ClassName.Replace(' ', '_');
            string preprocessor = "__" + ClassName + "_SHADER_CONSTANTS__";
            // Start with adding the #ifndefs
            file = file + Environment.NewLine + "#ifndef " + preprocessor + Environment.NewLine;
            file = file + "#define " + preprocessor + Environment.NewLine;

            // Space out start of constants file
            file = file + Environment.NewLine;

            // Start of unistruct
            file = file + "unistruct " + ClassName + Environment.NewLine + "{" + Environment.NewLine;
            
            // Go through each constant in the constants list
            foreach(var i in Constants)
            {
                if(i.Key == Type.MAT4)
                {
                    file = file + "\tMatrix4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == Type.VEC3 || i.Key == Type.VEC4)
                {
                    file = file + "\tVector4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == Type.FLOAT)
                {
                    file = file + "\t// Only the .x value of this vector is used" + Environment.NewLine;
                    file = file + "\tVector4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }

            }

            // End unistruct and file
            file = file + "};" + Environment.NewLine + "#endif";



            // WRITE TO FILE
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            //string input = "Auspuffanlage \"Century\" f├╝r";
            //return ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(s)));

            string output = ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(file)));
            //System.IO.File.WriteAllText(@"Shaders\Generated\" + ClassName + "ShaderConstents.h", output);
            string path = filePath + "\\" + ShaderName + "ShaderConstants.h";
            System.IO.File.WriteAllText(path, output);

            ShaderConstantsFileName = ShaderName + "ShaderConstants.h";

        }

        public void GeneratePSSLOutputFile()
        {
            string file, ClassName;
            // Comment at the start
            file = "// " + ShaderName + " Shader Output" + Environment.NewLine;

            // find what the name of the file is in preprocessorS #ifndefs
            ClassName = ShaderName;
            ClassName = ClassName.ToUpper();
            ClassName = ClassName.Replace(' ', '_');
            string preprocessor = "__" + ClassName + "_VS_OUTPUT__";
            // Start with adding the #ifndefs
            file = file + Environment.NewLine + "#ifndef " + preprocessor + Environment.NewLine;
            file = file + "#define " + preprocessor + Environment.NewLine;
            // space out preprocessor commands with struct
            file = file + Environment.NewLine;

            // Find out what the output struct name is
            outputStructName = ShaderName + "Output";

            // Write out stuct
            file = file + "struct " + outputStructName + Environment.NewLine;
            file = file + "{" + Environment.NewLine;
            file = file + "\tfloat4 m_position\t\t: S_POSITION;" + Environment.NewLine;
            //file = file + "\tfloat3 m_positionWorld\t\t\t: POS_WORLD;" + Environment.NewLine;
            file = file + "\tfloat3 m_normal\t\t\t: TEX_COORD;" + Environment.NewLine;
            file = file + "\tfloat2 m_texcoord\t\t: NORMAL;" + Environment.NewLine;
            file = file + "};" + Environment.NewLine + Environment.NewLine;

            // End file
            file = file + "#endif";

            // WRITE TO FILE
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            string output = ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(file)));
            //System.IO.File.WriteAllText(@"Shaders\Generated\" + ClassName + "ShaderConstents.h", output);
            string path = filePath + "\\" + ShaderName + "VSOutput.hs";
            System.IO.File.WriteAllText(path, output);

            ShaderOutputFileName = ShaderName + "VSOutput.hs";
        }

        public void GeneratePSSLVertexFile(string vert)
        {
            string PSSLVert = "// " + ShaderName + " Vertex Shader" + Environment.NewLine;
            
            // Add all includes to top of the file
            PSSLVert = PSSLVert + "#include \"" + ShaderConstantsFileName 
                + "\"" + Environment.NewLine;
            PSSLVert = PSSLVert + "#include \"" + ShaderOutputFileName
                + "\"" + Environment.NewLine;
            PSSLVert = PSSLVert + Environment.NewLine;

            // Add ptVSInput struct to top
            PSSLVert = PSSLVert + "struct ptVSInput" + Environment.NewLine;
            PSSLVert = PSSLVert + "{" + Environment.NewLine;
            PSSLVert = PSSLVert + "\tfloat3 m_position\t\t: POSITION0;" + Environment.NewLine;
            PSSLVert = PSSLVert + "\tfloat3 m_color\t\t\t: COLOR0;" + Environment.NewLine;
            PSSLVert = PSSLVert + "\tfloat3 m_normal\t\t: NORMAL0;" + Environment.NewLine;
            PSSLVert = PSSLVert + "\tfloat2 m_texcoord\t\t: TEXCOORD0;" + Environment.NewLine;
            PSSLVert = PSSLVert + "};" + Environment.NewLine;

            // Add space
            PSSLVert = PSSLVert + Environment.NewLine;

            //using (StringReader reader = new StringReader(vert))
            //{
            //    string line = string.Empty;
            //    do
            //    {
            //        // Ensures to skip any line containing parameters passed in as they are
            //        //  already dealt with
            //        line = reader.ReadLine();
            //        if (line != null && !line.Contains("in")
            //            && !line.Contains("out") && !line.Contains("uniform") 
            //            && !line.Contains("varying") && !line.Contains("precision")
            //            && !line.Contains("#"))
            //        {
            //            string[] temp = line.Split(new char[] { ' ', ';' });
            //            //line.Trim(new char[] { ' ', ';' });
            //            //line.Replace("uniform ", "");
            //            if (temp[1].Contains("vec3"))
            //            {
            //                //line.Replace("vec3", "" );
            //                Constants.Add(new KeyValuePair<Type, string>(Type.VEC3, temp[2]));
            //            }
            //            else if (temp[1].Contains("mat4"))
            //            {
            //                //line.Replace("vec3", "" );
            //                Constants.Add(new KeyValuePair<Type, string>(Type.MAT4, temp[2]));
            //            }
            //            else if (temp[1].Contains("float"))
            //            {
            //                //line.Replace("vec3", "" );
            //                Constants.Add(new KeyValuePair<Type, string>(Type.FLOAT, temp[2]));
            //            }
            //        }

            //    } while (line != null);
            //}

            // WRITE TO FILE
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            string output = ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(PSSLVert)));
            //System.IO.File.WriteAllText(@"Shaders\Generated\" + ClassName + "ShaderConstents.h", output);
            string path = filePath + "\\" + ShaderName + "_vv.pssl";
            System.IO.File.WriteAllText(path, output);

            ShaderVertexFilename = ShaderName + "_vv.pssl";
        }
    }
}
