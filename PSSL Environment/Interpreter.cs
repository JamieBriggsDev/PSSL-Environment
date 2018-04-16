using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using SharpGL;

namespace PSSL_Environment
{
    struct ShaderFileNames
    {
        
        public string ShaderOutputStructName;
        public string ShaderConstantsFileName;
        public string ShaderConstantsStructName;
        public string ShaderOutputFileName;
        public string ShaderVertexFilename;
        public string ShaderFragmentOutputName;
    }
    public class Interpreter
    {
        // Instance of the interpreter to make
        //  it a singleton
        private static Interpreter m_instance;

        public static string ShaderName;

        private string FilePath;

        private ShaderFileNames FileNames;
        private Interpreter() { SetShaderName("TEST"); }

        private enum GLSLType { MAT4, VEC2, VEC3, VEC4, FLOAT, SAMPLER2D};
        private enum PSSLType { FLOAT1, FLOAT2, FLOAT3, FLOAT4};

        private List<KeyValuePair<GLSLType, string>> Constants = new List<KeyValuePair<GLSLType, string>>();
        private List<KeyValuePair<PSSLType, string>> InputStruct = new List<KeyValuePair<PSSLType, string>>();
        private List<KeyValuePair<PSSLType, string>> OutputStruct = new List<KeyValuePair<PSSLType, string>>();

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


        private void FillStructs(string Frag, string Vert)
        {

            // Fill Constants ListFirst
            // Read through the vertex shader first
            FillConstantsStruct(Frag, Vert);

            // Fill Input Struct
            FillInputsStruct(Vert);

            // Fill output Struct
            FillOutputsStruct(Vert);


        }
        private void FillInputsStruct(string Vert)
        {
            InputStruct = new List<KeyValuePair<PSSLType, string>>();
            using (StringReader reader = new StringReader(Vert))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null && line.Contains("in"))
                    {
                        string[] temp = line.Split(new char[] { ' ', ';' });
                        //line.Trim(new char[] { ' ', ';' });
                        //line.Replace("uniform ", "");
                        if (temp[1].Contains("float"))
                        {
                            InputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT1, temp[2]));
                        }
                        else if (temp[1].Contains("vec2"))
                        {
                            InputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT2, temp[2]));
                        }
                        else if (temp[1].Contains("vec3"))
                        {
                            InputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT3, temp[2]));
                        }
                        else if (temp[1].Contains("vec4"))
                        {
                            InputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT4, temp[2]));
                        }
                    }

                } while (line != null);
            }
        }
        private void FillOutputsStruct(string Vert)
        {
            bool PositionOutputFound = false;
            OutputStruct = new List<KeyValuePair<PSSLType, string>>();
            using (StringReader reader = new StringReader(Vert))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null && line.Contains("out"))
                    {
                        string[] temp = line.Split(new char[] { ' ', ';' });
                        //line.Trim(new char[] { ' ', ';' });
                        //line.Replace("uniform ", "");
                        if (temp[2].Contains("Position") && PositionOutputFound == false)
                        {
                            OutputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT4, temp[2]));
                            PositionOutputFound = true;
                        }
                        else
                        {
                            if (temp[1].Contains("float"))
                            {
                                OutputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT1, temp[2]));
                            }
                            else if (temp[1].Contains("vec2"))
                            {
                                OutputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT2, temp[2]));
                            }
                            else if (temp[1].Contains("vec3"))
                            {
                                OutputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT3, temp[2]));
                            }
                            else if (temp[1].Contains("vec4"))
                            {
                                OutputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT4, temp[2]));
                            } 
                        }
                    }

                } while (line != null);
            }

            // If output position was not found then force a one
            if(PositionOutputFound == false)
            {
                OutputStruct.Add(new KeyValuePair<PSSLType, string>(PSSLType.FLOAT4, "Position"));
            }
        }
        private void FillConstantsStruct(string Frag, string Vert)
        {
            Constants = new List<KeyValuePair<GLSLType, string>>();
            // Read through the vertex shader first
            using (StringReader reader = new StringReader(Vert))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null && line.Contains("uniform"))
                    {
                        string[] temp = line.Split(new char[] { ' ', ';' });
                        //line.Trim(new char[] { ' ', ';' });
                        //line.Replace("uniform ", "");
                        if (temp[1].Contains("vec2"))
                        {
                            Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.VEC2, temp[2]));
                        }
                        else if (temp[1].Contains("vec3"))
                        {
                            Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.VEC3, temp[2]));
                        }
                        else if (temp[1].Contains("vec4"))
                        {
                            Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.VEC4, temp[2]));
                        }
                        else if (temp[1].Contains("mat4"))
                        {
                            //line.Replace("vec3", "" );
                            Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.MAT4, temp[2]));
                        }
                        else if (temp[1].Contains("float"))
                        {
                            //line.Replace("vec3", "" );
                            Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.FLOAT, temp[2]));
                        }
                        else if (temp[1].Contains("sampler2D"))
                        {
                            //line.Replace("vec3", "" );
                            Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.SAMPLER2D, temp[2]));
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
                        foreach (KeyValuePair<GLSLType, string> pair in Constants)
                        {
                            if (temp[2] == pair.Value)
                            {
                                clonedConstant = true;
                                break;
                            }
                        }

                        if (clonedConstant == false)
                        {
                            //line.Trim(new char[] { ' ', ';' });
                            //line.Replace("uniform ", "");
                            if (temp[1].Contains("vec2"))
                            {
                                Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.VEC2, temp[2]));
                            }
                            else if (temp[1].Contains("vec3"))
                            {
                                Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.VEC3, temp[2]));
                            }
                            else if (temp[1].Contains("vec4"))
                            {
                                Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.VEC4, temp[2]));
                            }
                            else if (temp[1].Contains("mat4"))
                            {
                                //line.Replace("vec3", "" );
                                Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.MAT4, temp[2]));
                            }
                            else if (temp[1].Contains("float"))
                            {
                                //line.Replace("vec3", "" );
                                Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.FLOAT, temp[2]));
                            }
                        
                            else if (temp[1].Contains("sampler2D"))
                            {
                                //line.Replace("vec3", "" );
                                Constants.Add(new KeyValuePair<GLSLType, string>(GLSLType.SAMPLER2D, temp[2]));
                            }

                        }
                    }
                    // While we are finding variables, find the output variable 
                    //  at the top of the file
                    if(line != null && line.Contains("out vec4") && 
                        FileNames.ShaderFragmentOutputName == null)
                    {
                        string[] temp = line.Split(new char[] { ' ', ';' });

                        FileNames.ShaderFragmentOutputName = temp[2];
                    }
                } while (line != null);
            }
        }


        public void GeneratePSSLAdvanced(string Frag, string Vert)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Desktop";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                FilePath = dialog.FileName;
            }

            //string fragmentShader = Frag;
            //string vertexShader = Vert;
            //Constants = new List<KeyValuePair<Type, string>>();

            FillStructs(Frag, Vert);
            GeneratePSSLConstantsFile();
            GeneratePSSLOutputFile();
            GeneratePSSLVertexFile(Vert);
            GeneratePSSLFragFile(Frag);
        }
        public void GeneratePSSLBasic()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Desktop";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //MessageBox.Show("You selected: " + dialog.FileName);
                FilePath = dialog.FileName;
            }

            string Frag;
            string Vert;

            switch(((MainWindow)Application.Current.MainWindow).GetViewType())
            {
                case MainWindow.ViewType.COLOR:
                    Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.frag");
                    Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert");
                    break;
                case MainWindow.ViewType.TOON:
                    Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.frag");
                    Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.vert");
                    break;
                case MainWindow.ViewType.TEXTURE:
                    Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixelTexture.frag");
                    Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixelTexture.vert");
                    break;
                case MainWindow.ViewType.TOONTEXTURE:
                    Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\ToonTexture.frag");
                    Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\ToonTexture.vert");
                    break;
                case MainWindow.ViewType.RIPPLE:
                    Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\Ripple.frag");
                    Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\Ripple.vert");
                    break;
                default:
                    Frag = "";
                    Vert = "";
                    break;
                    
            }

            // Swap gl_position function version to PSSL version
            Vert = Vert.Replace("uniform mat4 Projection;\nuniform mat4 Modelview", "uniform mat4 ModelProjection\n");
            Vert = Vert.Replace("gl_Position = Projection * Modelview", "Position = ModelProjection");


            FillStructs(Frag, Vert);
            GeneratePSSLConstantsFile();
            GeneratePSSLOutputFile();
            GeneratePSSLVertexFile(Vert);
            GeneratePSSLFragFile(Frag);

            //// GET ALREADY EXISTING SHADER FILES
            //if (((MainWindow)Application.Current.MainWindow).WaterEnabled.IsChecked == true)
            //{
            //    // Generate Water Shaders
            //    Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert");
            //    Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert");
            //}
            //else
            //{
            //    // Generate Texture Mode
            //    if (((MainWindow)Application.Current.MainWindow).UsingTexture.IsChecked == true)
            //    {
            //        if (((MainWindow)Application.Current.MainWindow).checkBoxUseToonShader.IsChecked == true)
            //        {
            //            Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.frag");
            //            Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert");
            //        }
            //        else
            //        {
            //            Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixelTexture.frag");
            //            Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixelTexture.vert");
            //        }

            //    }
            //    // Generate Color Mode
            //    else
            //    {
            //        if(((MainWindow)Application.Current.MainWindow).checkBoxUseToonShader.IsChecked == true)
            //        {
            //            Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.frag");
            //            Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.vert");
            //        }
            //        else
            //        {
            //            Frag = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.frag");
            //            Vert = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert");
            //        }
            //    }
            //    //myScene.RenderImmediateMode(gl);
            //}

            // Generate PSSL



        }

        public void GeneratePSSLConstantsFile()
        {
            string file, ClassName;
            // Comment at the start
            file = "// " + ShaderName + " Shader Constants" + Environment.NewLine;

            FileNames.ShaderConstantsFileName = ShaderName + "ShaderConstants.h";
            FileNames.ShaderConstantsStructName = ShaderName + "ShaderConstants";

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
            file = file + "unistruct " + FileNames.ShaderConstantsStructName + Environment.NewLine + "{" + Environment.NewLine;

            int padCounter = 0;
            
            // Go through each constant in the constants list
            foreach(var i in Constants)
            {
                if(i.Key == GLSLType.MAT4)
                {
                    file = file + "\tMatrix4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == GLSLType.VEC2)
                {
                    file = file + "\t// Adding padding due to Vector2 taking half the data required for gpu" + Environment.NewLine;
                    file = file + "\tVector2Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                    file = file + "\tint pad" + padCounter + "[2];" + Environment.NewLine;
                    padCounter++;
                }
                else if (i.Key == GLSLType.VEC3)
                {
                    file = file + "\t// Adding padding due to Vector3 taking 3/4 the data required for gpu" + Environment.NewLine;
                    file = file + "\tVector3Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                    file = file + "\tint pad" + padCounter + "[1];" + Environment.NewLine;
                    padCounter++;
                }
                else if (i.Key == GLSLType.VEC4)
                {
                    file = file + "\tVector4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == GLSLType.FLOAT)
                {
                    file = file + "\t// Adding padding due to float taking 1/4 the data required for gpu" + Environment.NewLine;
                    file = file + "\tfloat shc_" + i.Value + ";" + Environment.NewLine;
                    file = file + "\tint pad" + padCounter + "[3];" + Environment.NewLine;
                    padCounter++;
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
            string path = FilePath + "\\" + FileNames.ShaderConstantsFileName;
            System.IO.File.WriteAllText(path, output);



        }
        public void GeneratePSSLOutputFile()
        {
            string OutputFile, ClassName;
            // Comment at the start
            OutputFile = "// " + ShaderName + " Shader Output" + Environment.NewLine;

            // find what the name of the file is in preprocessorS #ifndefs
            ClassName = ShaderName;
            ClassName = ClassName.ToUpper();
            ClassName = ClassName.Replace(' ', '_');
            string preprocessor = "__" + ClassName + "_VS_OUTPUT__";
            // Start with adding the #ifndefs
            OutputFile = OutputFile + Environment.NewLine + "#ifndef " + preprocessor + Environment.NewLine;
            OutputFile = OutputFile + "#define " + preprocessor + Environment.NewLine;
            // space out preprocessor commands with struct
            OutputFile = OutputFile + Environment.NewLine;

            // Find out what the output struct name is
            FileNames.ShaderOutputStructName = ShaderName + "VSOutput";

            // Write out stuct
            OutputFile = OutputFile + "struct " + FileNames.ShaderOutputStructName + Environment.NewLine;
            OutputFile = OutputFile + "{" + Environment.NewLine;
            OutputFile = OutputFile + "\tfloat4 Position\t: S_POSITION;" + Environment.NewLine;
            foreach (var i in OutputStruct)
            {
                if (i.Value != "Position")
                {
                    switch (i.Key)
                    {
                        case PSSLType.FLOAT1:
                            OutputFile = OutputFile + string.Format("\tfloat1 {0}\t: {1};\n",
                                i.Value, i.Value.ToUpper().Replace("M_", ""));
                            break;
                        case PSSLType.FLOAT2:
                            OutputFile = OutputFile + string.Format("\tfloat2 {0}\t: {1};\n",
                                i.Value, i.Value.ToUpper().Replace("M_", ""));
                            break;
                        case PSSLType.FLOAT3:
                            OutputFile = OutputFile + string.Format("\tfloat3 {0}\t: {1};\n",
                                i.Value, i.Value.ToUpper().Replace("M_", ""));
                            break;
                        case PSSLType.FLOAT4:
                            OutputFile = OutputFile + string.Format("\tfloat4 {0}\t: {1};\n",
                                i.Value, i.Value.ToUpper().Replace("M_", ""));
                            break;
                    } 
                }
            }
            //file = file + "\tfloat4 m_position\t\t: S_POSITION;" + Environment.NewLine;
            ////file = file + "\tfloat3 m_positionWorld\t\t\t: POS_WORLD;" + Environment.NewLine;
            //file = file + "\tfloat3 m_normal\t\t\t: TEX_COORD;" + Environment.NewLine;
            //file = file + "\tfloat2 m_texcoord\t\t: NORMAL;" + Environment.NewLine;
            OutputFile = OutputFile + "};" + Environment.NewLine + Environment.NewLine;

            // End file
            OutputFile = OutputFile + "#endif";

            // WRITE TO FILE
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            string output = ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(OutputFile)));
            //System.IO.File.WriteAllText(@"Shaders\Generated\" + ClassName + "ShaderConstents.h", output);
            string path = FilePath + "\\" + ShaderName + "VSOutput.hs";
            System.IO.File.WriteAllText(path, output);

            FileNames.ShaderOutputFileName = ShaderName + "VSOutput.hs";
        }


        public void GeneratePSSLVertexFile(string vert)
        {
            string PSSLVert = "// " + ShaderName + " Vertex Shader" + Environment.NewLine;
            // Add all includes to top of the file
            PSSLVert = PSSLVert + "#include \"" + FileNames.ShaderConstantsFileName 
                + "\"" + Environment.NewLine;
            PSSLVert = PSSLVert + "#include \"" + FileNames.ShaderOutputFileName
                + "\"" + Environment.NewLine;
            PSSLVert = PSSLVert + Environment.NewLine;

            // Add ptVSInput struct to top
            PSSLVert = PSSLVert + "// Due to how the PS4 works, insert your own inputs struct which\n follows your own programs vertex inputs" + Environment.NewLine;
            PSSLVert = PSSLVert + "struct ptVSInput" + Environment.NewLine;
            PSSLVert = PSSLVert + "{" + Environment.NewLine;
            // Generate input struct at the top of the shader
            foreach(var i in InputStruct)
            {
                switch(i.Key)
                {
                    case PSSLType.FLOAT1:
                        PSSLVert = PSSLVert + "\tfloat1 " + i.Value + "\t: " +
                            i.Value.ToUpper().Replace("M_", "") + ";" + Environment.NewLine;
                        break;
                    case PSSLType.FLOAT2:
                        PSSLVert = PSSLVert + "\tfloat2 " + i.Value + "\t: " +
                            i.Value.ToUpper().Replace("M_", "") + ";" + Environment.NewLine;
                        break;
                    case PSSLType.FLOAT3:
                        PSSLVert = PSSLVert + "\tfloat3 " + i.Value + "\t: " +
                            i.Value.ToUpper().Replace("M_", "") + ";" + Environment.NewLine;
                        break;
                    case PSSLType.FLOAT4:
                        PSSLVert = PSSLVert + "\tfloat4 " + i.Value + "\t: " +
                            i.Value.ToUpper().Replace("M_", "") + ";" + Environment.NewLine;
                        break;
                }
            }

            PSSLVert = PSSLVert + "};" + Environment.NewLine;

            // Split vertex shader into seperate lines
            string[] vertSplit = vert.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string[] outputVariables = new string[OutputStruct.Count];
            int counter = 0;
            foreach(var i in OutputStruct)
            {
                outputVariables[counter] = "l_output." + i.Value + " =";
                counter++;
            }

            string body = "";
            string line = "";
            bool insideMain = false;
            int mainCurlyCounter = 0;
            using (StringReader reader = new StringReader(vert))
            {
                do
                {
                    if (line.Contains("void main()"))
                    {
                        line = reader.ReadLine();
                        continue;
                    }
                    line = string.Empty;
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        //string[] temp = line.Split(new char[] { ' ', ';' });
                        //line.Trim(new char[] { ' ', ';' });
                        //line.Replace("uniform ", "");

                        if (!line.Contains("in ") && !line.Contains("out ") && !line.Contains("uniform ") &&
                            !line.Contains("#version "))
                        {
                            // Checks for inside of main to place return

                            if (line.Contains("void main()"))
                            {
                                body = body + FileNames.ShaderOutputStructName + " main(ptVSInput _input)" + Environment.NewLine;
                                body = body + "{" + Environment.NewLine;
                                body = body + "\t" + ShaderName + "VSOutput l_output;" + Environment.NewLine;
                                insideMain = true;
                                mainCurlyCounter = 1;
                            }
                            else
                            {
                                if (insideMain == true)
                                {
                                    foreach (char i in line)
                                    {
                                        if (i == '{')
                                            mainCurlyCounter++;
                                        else if (i == '}')
                                            mainCurlyCounter--;

                                        if (mainCurlyCounter <= 0)
                                        {
                                            insideMain = false;
                                            line = "\treturn l_output;\n}";
                                        }
                                    }
                                }

                                body = body + line + Environment.NewLine;
                            }
                        }

                       
                    }

                } while (line != null);
            }
            // Split main function into seperate lines
            string[] mainSplit = body.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Go through all output struct variables and check if they are found within the main function
            foreach (var i in OutputStruct)
            {
                for(int j = 0; j < mainSplit.Length; j++)
                {
                    if(mainSplit[j].Contains(i.Value))
                    {
                        mainSplit[j] = mainSplit[j].Replace(i.Value + " =", "l_output." + i.Value + " =");
                    }
                }
            }
            foreach (var i in Constants)
            {
                if (i.Key == GLSLType.SAMPLER2D)
                    continue;

                for (int j = 0; j < mainSplit.Length; j++)
                {
                    string pattern = "(?<!\\w)" + i.Value + "(?!\\w)";
                    string replace = "shc_" + i.Value;
                    mainSplit[j] = Regex.Replace(mainSplit[j], pattern, replace);
                    //line = line.Replace(i.Value, "t_" + i.Value); 
                }

            }
            foreach (var i in InputStruct)
            {
                if (i.Value == "Position")
                    continue;
                for (int j = 0; j < mainSplit.Length; j++)
                {
                    string pattern = "(?<!\\w)" + i.Value + "(?!\\w)";
                    string replace = "_input." + i.Value;
                    mainSplit[j] = Regex.Replace(mainSplit[j], pattern, replace);
                    //line = line.Replace(i.Value, "t_" + i.Value); 
                }
            }

            foreach (var i in mainSplit)
            {
                PSSLVert = PSSLVert + i + Environment.NewLine;
            }

            // Make some space
            PSSLVert = PSSLVert + Environment.NewLine;

            //// Return output
            //PSSLVert = PSSLVert + "\treturn l_output;" + Environment.NewLine;

            //// Close main function
            //PSSLVert = PSSLVert + "}" + Environment.NewLine;

            // Replace any last minute function calls or class names
            //  (vec2 becomes float2)
            PSSLVert = PSSLVert.Replace("vec2", "float2");
            PSSLVert = PSSLVert.Replace("vec3", "float3");
            PSSLVert = PSSLVert.Replace("vec4", "float4");

            // WRITE TO FILE
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            string output = ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(PSSLVert)));
            //System.IO.File.WriteAllText(@"Shaders\Generated\" + ClassName + "ShaderConstents.h", output);
            string path = FilePath + "\\" + ShaderName + "_vv.pssl";
            System.IO.File.WriteAllText(path, output);

            FileNames.ShaderVertexFilename = ShaderName + "_vv.pssl";
        }

        public void GeneratePSSLFragFile(string frag)
        {
            string PSSLFrag = "// " + ShaderName + " Pixel Shader" + Environment.NewLine;
            // Add all includes to top of the file
            PSSLFrag = PSSLFrag + "#include \"" + FileNames.ShaderConstantsFileName
                + "\"" + Environment.NewLine;
            PSSLFrag = PSSLFrag + "#include \"" + FileNames.ShaderOutputFileName
                + "\"" + Environment.NewLine;
            // Add space
            PSSLFrag = PSSLFrag + Environment.NewLine;


            List<KeyValuePair<string, string>> samplerList = new List<KeyValuePair<string, string>>();

            // Check to see if samplers were found within the uniforms in both files
            //  then include the necasary globals into the frag shader in placement
            foreach (var i in Constants)
            {
                if(i.Key == GLSLType.SAMPLER2D)
                {
                    string samplerName = String.Format("SamplerState l_{0} : register(s0)", i.Value + samplerList.Count.ToString());
                    string texture2DName = String.Format("Texture2D l_{0} : register(t0)", i.Value + "ColorMap");
                    PSSLFrag = PSSLFrag + samplerName + Environment.NewLine;
                    PSSLFrag = PSSLFrag + texture2DName + Environment.NewLine;
                    samplerList.Add(new KeyValuePair<string, string>(i.Value + samplerList.Count.ToString(), i.Value + "ColorMap"));
                }
            }

            // Split vertex shader into seperate lines
            string[] fragSplit = frag.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Make some space
            PSSLFrag = PSSLFrag + Environment.NewLine;


            string body = "";
            string line = "";
            using (StringReader reader = new StringReader(frag))
            {
                line = string.Empty;

                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        //string[] temp = line.Split(new char[] { ' ', ';' });
                        //line.Trim(new char[] { ' ', ';' });
                        //line.Replace("uniform ", "");


                        if (!line.Contains("in ") && !line.Contains("out ") && !line.Contains("uniform ") &&
                            !line.Contains("#version "))
                        {
                            if (line.Contains("void main()"))
                            {
                                body = body + "float4 main(" + ShaderName + 
                                    "VSOutput _input) : S_TARGET_OUTPUT" + Environment.NewLine;
                                body = body + Environment.NewLine;
                            }
                            else
                            {
                                body = body + line + Environment.NewLine;
                            }
                        }

                    }

                } while (line != null);
            }
            // Split main function into seperate lines
            string[] mainSplit = body.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var i in Constants)
            {
                if (i.Key == GLSLType.SAMPLER2D)
                    continue;

                for (int j = 0; j < mainSplit.Length; j++)
                {
                    string pattern = "(?<!\\w)" + i.Value + "(?!\\w)";
                    string replace = "shc_" + i.Value;
                    mainSplit[j] = Regex.Replace(mainSplit[j], pattern, replace);
                    //line = line.Replace(i.Value, "t_" + i.Value); 
                }

            }
            foreach (var i in OutputStruct)
            {
                for (int j = 0; j < mainSplit.Length; j++)
                {
                    string pattern = "(?<!\\w)" + i.Value + "(?!\\w)";
                    string replace = "_input." + i.Value;
                    mainSplit[j] = Regex.Replace(mainSplit[j], pattern, replace);
                    //line = line.Replace(i.Value, "t_" + i.Value); 
                }
            }
            foreach (var i in samplerList)
            {
                for (int j = 0; j < mainSplit.Length; j++)
                {
                    //string pattern = "(?<!\\w)texture(" + i.Key + "(?!\\w)";
                    //string replace = "_input." + i.Value;
                    //mainSplit[j] = Regex.Replace(mainSplit[j], pattern, replace);
                    string replace = Regex.Replace(i.Key, "[^A-Za-z]+", "");
                    mainSplit[j] = mainSplit[j].Replace(string.Format("texture({0}", replace),
                        string.Format("l_{0}.Sample(l_{1}", i.Key, i.Value));
                }
            }


            // Write main function start with temp variables
            //PSSLFrag = PSSLFrag +
            //    string.Format("float4 main({0} _input) : S_TARGET_OUTPUT\n{{\n\n",
            //    FileNames.ShaderOutputStructName);
            //foreach(var localCon in localConstants)
            //{
            //    switch(localCon.Key)
            //    {
            //        case GLSLType.FLOAT:
            //            PSSLFrag = PSSLFrag + string.Format("\tfloat t_{0} = shc_{0}.x;\n", (string)localCon.Value);
            //            break;
            //        case GLSLType.VEC2:
            //            PSSLFrag = PSSLFrag + string.Format("\tfloat2 t_{0} = float2(shc_{0});\n", (string)localCon.Value);
            //            break;
            //        case GLSLType.VEC3:
            //            PSSLFrag = PSSLFrag + string.Format("\tfloat3 t_{0} = float3(shc_{0});\n", (string)localCon.Value);
            //            break;
            //        case GLSLType.VEC4:
            //            PSSLFrag = PSSLFrag + string.Format("\tfloat4 t_{0} = shc_{0};\n", (string)localCon.Value);
            //            break;
            //    }
            //}
            // Make some space
            //PSSLFrag = PSSLFrag + Environment.NewLine;

            // Go through all constant variables and replace how they should be in mainSplit
            //foreach (var i in localConstants)
            //{
            //    for (int j = 0; j < mainSplit.Length; j++)
            //    {
            //        if (mainSplit[j].Contains(i.Value))
            //        {
            //            mainSplit[j].Replace(i.Value, "t_" + i.Value);
            //        }
            //    }
            //}

            // Add the necasary main split lines (Miss the first line as this is already added
            for (int i = 2; i < mainSplit.Count(); i++)
            {
                PSSLFrag = PSSLFrag + mainSplit[i] + Environment.NewLine;
            }

            // Replace any last minute function calls or class names
            //  (vec2 becomes float2)
            PSSLFrag = PSSLFrag.Replace("vec2", "float2");
            PSSLFrag = PSSLFrag.Replace("vec3", "float3");
            PSSLFrag = PSSLFrag.Replace("vec4", "float4");
            PSSLFrag = PSSLFrag.Replace(string.Format("{0} =", FileNames.ShaderFragmentOutputName), "return ");

            // WRITE TO FILE
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            string output = ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(PSSLFrag)));
            //System.IO.File.WriteAllText(@"Shaders\Generated\" + ClassName + "ShaderConstents.h", output);
            string path = FilePath + "\\" + ShaderName + "_p.pssl";
            System.IO.File.WriteAllText(path, output);

            FileNames.ShaderVertexFilename = ShaderName + "_p.pssl";
        }
    }
}
