using System;
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

        enum Type { MAT4, VEC3, FLOAT};

        List<KeyValuePair<Type, string>> Constants = new List<KeyValuePair<Type, string>>();

        // Get instance to the interpreter class
        public static Interpreter GetInstance()
        {

            if(m_instance == null)
            {
                m_instance = new Interpreter(); 
            }
            return m_instance;
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
            
        }
    }
}
