﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PSSL_Environment
{
    struct ShaderFileNames
    {
        public string ShaderName;
        public string ShaderOutputStructName;
        public string ShaderVertexFilename;
        public string ShaderConstantsFileName;
        public string ShaderOutputFileName;
    }
    public class Interpreter
    {
        // Instance of the interpreter to make
        //  it a singleton
        private static Interpreter m_instance;

        private string FilePath;

        private ShaderFileNames FileNames;
        private Interpreter() { }

        private enum GLSLType { MAT4, VEC2, VEC3, VEC4, FLOAT};
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
            FileNames.ShaderName = name;
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
                        }
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

            SetShaderName("test");
            FillStructs(Frag, Vert);
            GeneratePSSLConstantsFile();
            GeneratePSSLOutputFile();
            GeneratePSSLVertexFile(Vert);
        }

        public void GeneratePSSLConstantsFile()
        {
            string file, ClassName;
            // Comment at the start
            file = "// " + FileNames.ShaderName + " Shader Constants" + Environment.NewLine;

            // find what the name of the file is in preprocessor standar if #ifndefs
            ClassName = FileNames.ShaderName;
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
                if(i.Key == GLSLType.MAT4)
                {
                    file = file + "\tMatrix4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == GLSLType.VEC2)
                {
                    file = file + "\t// The .w and .z values of this vector is not used" + Environment.NewLine;
                    file = file + "\tVector4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == GLSLType.VEC3)
                {
                    file = file + "\t// The .w value of this vector is not used" + Environment.NewLine;
                    file = file + "\tVector4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == GLSLType.VEC4)
                {
                    file = file + "\tVector4Unaligned shc_" + i.Value + ";" + Environment.NewLine;
                }
                else if (i.Key == GLSLType.FLOAT)
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
            string path = FilePath + "\\" + FileNames.ShaderName + "ShaderConstants.h";
            System.IO.File.WriteAllText(path, output);

            FileNames.ShaderConstantsFileName = FileNames.ShaderName + "ShaderConstants.h";

        }
        public void GeneratePSSLOutputFile()
        {
            string OutputFile, ClassName;
            // Comment at the start
            OutputFile = "// " + FileNames.ShaderName + " Shader Output" + Environment.NewLine;

            // find what the name of the file is in preprocessorS #ifndefs
            ClassName = FileNames.ShaderName;
            ClassName = ClassName.ToUpper();
            ClassName = ClassName.Replace(' ', '_');
            string preprocessor = "__" + ClassName + "_VS_OUTPUT__";
            // Start with adding the #ifndefs
            OutputFile = OutputFile + Environment.NewLine + "#ifndef " + preprocessor + Environment.NewLine;
            OutputFile = OutputFile + "#define " + preprocessor + Environment.NewLine;
            // space out preprocessor commands with struct
            OutputFile = OutputFile + Environment.NewLine;

            // Find out what the output struct name is
            FileNames.ShaderOutputStructName = FileNames.ShaderName + "VSOutput";

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
            string path = FilePath + "\\" + FileNames.ShaderName + "VSOutput.hs";
            System.IO.File.WriteAllText(path, output);

            FileNames.ShaderOutputFileName = FileNames.ShaderName + "VSOutput.hs";
        }
        public void GeneratePSSLVertexFile(string vert)
        {
            string PSSLVert = "// " + FileNames.ShaderName + " Vertex Shader" + Environment.NewLine;
            // Add all includes to top of the file
            PSSLVert = PSSLVert + "#include \"" + FileNames.ShaderConstantsFileName 
                + "\"" + Environment.NewLine;
            PSSLVert = PSSLVert + "#include \"" + FileNames.ShaderOutputFileName
                + "\"" + Environment.NewLine;
            PSSLVert = PSSLVert + Environment.NewLine;

            // Add ptVSInput struct to top
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

            // Add space
            PSSLVert = PSSLVert + Environment.NewLine;

            // Start main vertex function
            PSSLVert = PSSLVert + FileNames.ShaderOutputStructName + " main(ptVSInput _input )" + Environment.NewLine;
            PSSLVert = PSSLVert + "{" + Environment.NewLine;
            PSSLVert = PSSLVert + "\t" + FileNames.ShaderOutputStructName + " l_output;"+ Environment.NewLine;

            // Make some space
            PSSLVert = PSSLVert + Environment.NewLine;

            // Split vertex shader into seperate lines
            string[] vertSplit = vert.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            string[] outputVariables = new string[OutputStruct.Count];
            int counter = 0;
            foreach(var i in OutputStruct)
            {
                outputVariables[counter] = "l_output." + i.Value + " =";
                counter++;
            }

            string mainFunc = "";
            bool foundMain = false;
            using (StringReader reader = new StringReader(vert))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        //string[] temp = line.Split(new char[] { ' ', ';' });
                        //line.Trim(new char[] { ' ', ';' });
                        //line.Replace("uniform ", "");
                        if (line.Contains("main()") || foundMain == true)
                        {
                            foundMain = true;
                            mainFunc = mainFunc + line + Environment.NewLine;
                        }

                       
                    }

                } while (line != null);
            }
            // Split main function into seperate lines
            string[] mainSplit = mainFunc.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Go through all output struct variables and check if they are found within the main function
            foreach(var i in OutputStruct)
            {
                for(int j = 0; j < mainSplit.Length; j++)
                {
                    if(mainSplit[j].Contains(i.Value))
                    {
                        PSSLVert = PSSLVert + mainSplit[j].Replace("gl_Position", "Position").Replace(i.Value + " =", "l_output." + i.Value + " =")
                            + Environment.NewLine;
                    }
                }
            }

            // Make some space
            PSSLVert = PSSLVert + Environment.NewLine;

            // Return output
            PSSLVert = PSSLVert + "\treturn l_output" + Environment.NewLine;

            // Close main function
            PSSLVert = PSSLVert + "}" + Environment.NewLine;

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
            string path = FilePath + "\\" + FileNames.ShaderName + "_vv.pssl";
            System.IO.File.WriteAllText(path, output);

            FileNames.ShaderVertexFilename = FileNames.ShaderName + "_vv.pssl";
        }
    }
}
