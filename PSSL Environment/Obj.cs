using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSSL_Environment.ObjTypes;

// debug TEST
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GlmNet;

namespace PSSL_Environment
{
    public class Obj
    {
        public static int totalElements = 0;
        public static int currentElement = 0;
        public static float progressLoaded = 0;
        public List<Vertex> VertexList;
        public List<Normal> NormalList;
        public List<Face> FaceList;
        public List<TextureVertex> TextureList;

        public List<uint> VertexIndices;
        public List<uint> NormalIndices;
        public List<uint> UVIndices;

        public mat4 m_modelWorldMx = new mat4();

        public int IndicesPerFace = 3;
        private bool validObject = false;
        /// <summary>
        /// Sees if there is a valid object
        /// </summary>
        /// <returns></returns>
        public bool GetValidObject()
        {
            return validObject;
        }

        /// <summary>
        /// Constructor of the OBJ
        /// </summary>
        public Obj()
        {
            m_modelWorldMx = glm.translate(mat4.identity(), new vec3(0.0f, 0.0f, -10.0f));
        }

        /// <summary>
        /// Initilise function for the obj class
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<int> Initialise(string path)
        {
            
            VertexList = new List<Vertex>();
            FaceList = new List<Face>();
            TextureList = new List<TextureVertex>();         
            NormalList = new List<Normal>();                 
                                                             
            VertexIndices = new List<uint>();                
            NormalIndices = new List<uint>();                
            UVIndices = new List<uint>();

            bool indexWorking = false;
            await Task.Run(() =>
            {
                //totalElements = TotalElementsFound(File.ReadAllLines(path));
                LoadObj(path);
                indexWorking = FixIndexing();
                //System.Threading.Thread.Sleep(5000);
                if (indexWorking == true)
                    validObject = true;
            });



            return 0;
        }

        /// <summary>
        /// Checks through all the elements within the obj file passed through
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int TotalElementsFound(IEnumerable<string> data)
        {
            int output = 0;
            foreach (var line in data)
            {
                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "v":
                            output++;
                            break;
                        case "vn":
                            output++;
                            break;
                        case "f":
                            output++;
                            break;
                        case "vt":
                            output++;
                            break;

                    }
                }
            }
            return output;
        }

        /// <summary>
        /// Fix the indexing of all the model data
        /// </summary>
        /// <returns></returns>
        public bool FixIndexing()
        {


            List<Vertex> tempVertexList = new List<Vertex>();
            List<Normal> tempNormalList = new List<Normal>();
            //List<Face> tempFaceList = new List<Face>();
            List<TextureVertex> tempTextureList = new List<TextureVertex>();

            for (int i = 0; i < FaceList.Count; i++)
            {
                if(FaceList.ElementAt(i).IndicesPerFace == 3)
                {
                    foreach (var j in FaceList.ElementAt(i).VertexIndexList)
                    {
                        VertexIndices.Add((uint)j);
                    }
                    foreach (var j in FaceList.ElementAt(i).NormalIndexList)
                    {
                        NormalIndices.Add((uint)j);
                    }
                    foreach (var j in FaceList.ElementAt(i).TextureVertexIndexList)
                    {
                        UVIndices.Add((uint)j);
                    }
                }
                else if(FaceList.ElementAt(i).IndicesPerFace == 4)
                {
                    int[] indices = new int[]{ 0, 1, 2, 0, 2, 3 };
                    for(int j = 0; j < 6; j++)
                    {
                        VertexIndices.Add((uint)FaceList.ElementAt(i).
                            VertexIndexList.ElementAt(indices[j]));
                        NormalIndices.Add((uint)FaceList.ElementAt(i).
                            NormalIndexList.ElementAt(indices[j]));
                        UVIndices.Add((uint)FaceList.ElementAt(i).
                            TextureVertexIndexList.ElementAt(indices[j]));
                    }
                }
            }

            string vertexCount = VertexIndices.Count.ToString();
            string NormalCount = NormalIndices.Count.ToString();
            string TextureCount = UVIndices.Count.ToString();

            Debug.Write("Vertex Count: " + vertexCount);
            Debug.Write("Normal Count: " + NormalCount);
            Debug.Write("UV Count: " + TextureCount);


            for (int i = 0; i < VertexIndices.Count; i++)
            {
                uint vertexIndex = VertexIndices.ElementAt(i);
                try
                {
                    Vertex vertexOut = VertexList.ElementAt((int)vertexIndex - 1);
                    tempVertexList.Add(vertexOut);
                } catch (Exception)
                {
                    System.Windows.MessageBox.Show("Failed to fix OBJ vertex indices", "Error loading OBJ.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            for (int i = 0; i < NormalIndices.Count; i++)
            {
                uint normalIndex = NormalIndices.ElementAt(i);
                try
                {
                    Normal normalOut = NormalList.ElementAt((int)normalIndex - 1);
                    tempNormalList.Add(normalOut);
                } catch (Exception)
                {
                    System.Windows.MessageBox.Show("Failed to fix OBJ normal indices", "Error loading OBJ.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }

            try
            {
                for (int i = 0; i < UVIndices.Count; i++)
                {
                    uint uvIndex = UVIndices.ElementAt(i);

                    TextureVertex uvOut = TextureList.ElementAt((int)uvIndex - 1);
                    tempTextureList.Add(uvOut);
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("OBJ loaded doesn't contain UV data, textures will not load!", "OBJ Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }


            if (tempVertexList == VertexList)
                Debug.Write("Vertex List is in order!");
            if (tempNormalList == NormalList)
                Debug.Write("Normal List is in order!");
            if (tempTextureList == TextureList)
                Debug.Write("Texture List is in order!");


            // Make templists the real lists
            VertexList = tempVertexList;
            NormalList = tempNormalList;
            TextureList = tempTextureList;

            return true;
        }

        public Extent Size { get; set; }

        public string UseMtl { get; set; }
        public string Mtl { get; set; }

        /// <summary>
        /// converts the vertex list to an array
        /// </summary>
        /// <returns></returns>
        public float[] ToFloatArrayVertex()
        {
            int counter = 0;
            // Make array 3 times the size of vertex list for all coordinates
            float[] output = new float[VertexList.Count * 3];
            for (int i = 0; i < VertexList.Count; i++)
            {
                output[counter++] = (float)VertexList.ElementAt(i).X;
                output[counter++] = (float)VertexList.ElementAt(i).Y;
                output[counter++] = (float)VertexList.ElementAt(i).Z;
            }

            return output;
        }

        /// <summary>
        /// Converts the normal list to an array
        /// </summary>
        /// <returns></returns>
        public float[] ToFloatArrayNormal()
        {
            int counter = 0;
            // Make array 3 times the size of vertex list for all coordinates
            float[] output = new float[NormalList.Count * 3];
            for (int i = 0; i < NormalList.Count; i++)
            {
                output[counter++] = (float)NormalList.ElementAt(i).NX;
                output[counter++] = (float)NormalList.ElementAt(i).NY;
                output[counter++] = (float)NormalList.ElementAt(i).NZ;
            }

            return output;
        }

        /// <summary>
        /// Converts the texture list to an array
        /// </summary>
        /// <returns></returns>
        public float[] ToFloatArrayTexture()
        {
            int counter = 0;
            // Make array 2 times the size of UV list for all coordinates
            float[] output = new float[TextureList.Count * 2];
            for (int i = 0; i < VertexList.Count; i++)
            {
                output[counter++] = (float)TextureList.ElementAt(i).X;
                output[counter++] = (float)TextureList.ElementAt(i).Y;
            }

            return output;
        }

        /// <summary>
        /// Loads the obj within the path passed in the parameter
        /// </summary>
        /// <param name="path"></param>
        private void LoadObj(string path)
        {
            LoadObj(File.ReadAllLines(path));
        }

        /// <summary>
        /// Loads the obj using an IENumerable
        /// </summary>
        /// <param name="data">data of the obj file</param>
        public void LoadObj(IEnumerable<string> data)
        {
            foreach (var line in data)
            {
                processLine(line);
                //progressLoaded = (currentElement / totalElements);
            }

            currentElement = 0;

            //IndicesPerFace = Face.IndicesPerFace;

            updateSize();
        }

        /// <summary>
        /// Updates the size of the obj
        /// </summary>
        private void updateSize()
        {
            // If there are no vertices then size should be 0.
            if (VertexList.Count == 0)
            {
                Size = new Extent
                {
                    XMax = 0,
                    XMin = 0,
                    YMax = 0,
                    YMin = 0,
                    ZMax = 0,
                    ZMin = 0
                };

                // Avoid an exception below if VertexList was empty.
                return;
            }

            Size = new Extent
            {
                XMax = VertexList.Max(v => v.X),
                XMin = VertexList.Min(v => v.X),
                YMax = VertexList.Max(v => v.Y),
                YMin = VertexList.Min(v => v.Y),
                ZMax = VertexList.Max(v => v.Z),
                ZMin = VertexList.Min(v => v.Z)
            };
        }

        /// <summary>
        /// Processes a line of data from the obj file
        /// </summary>
        /// <param name="line">Line from obj. file to be processed</param>
        private void processLine(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "usemtl":
                        UseMtl = parts[1];
                        //currentElement++;
                        break;
                    case "mtllib":
                        Mtl = parts[1];
                        //currentElement++;
                        break;
                    case "v":
                        Vertex v = new Vertex();
                        v.LoadFromStringArray(parts);
                        VertexList.Add(v);
                        v.Index = VertexList.Count();
                        currentElement++;
                        break;
                    case "vn":
                        Normal vn = new Normal();
                        vn.LoadFromStringArray(parts);
                        NormalList.Add(vn);
                        vn.Index = VertexList.Count();
                        currentElement++;
                        break;
                    case "f":
                        Face f = new Face();
                        f.LoadFromStringArray(parts);
                        f.UseMtl = UseMtl;
                        FaceList.Add(f);
                        currentElement++;
                        break;
                    case "vt":
                        TextureVertex vt = new TextureVertex();
                        vt.LoadFromStringArray(parts);
                        TextureList.Add(vt);
                        vt.Index = TextureList.Count();
                        currentElement++;
                        break;

                }
            }
        }
    }
}
