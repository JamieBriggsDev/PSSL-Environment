﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSL_Environment
{ 
    public class Interpreter
    {
        // Instance of the interpreter to make
        //  it a singleton
        private static Interpreter m_instance;



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

        private string ShaderName;

        public void SetShaderName(string name)
        {
            ShaderName = name;
        }


        public void GenerateConstants(string Frag, string Vert)
        {
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
            SetShaderName("New PSSL Shader");
            GeneratePSSLConstantsFile();
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
            string preprocessor = "__" + ClassName + "__";
            // Start with adding the #ifndefs
            file = file + Environment.NewLine + "#ifndef " + preprocessor + ";" + Environment.NewLine;
            file = file + "#define " + preprocessor + ";" + Environment.NewLine;

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
            System.IO.File.WriteAllText(@"Shaders\Generated\" + ClassName + "ShaderConstents.h", output);

        }
    }
}
