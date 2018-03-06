using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSSL_Environment.ObjTypes;

// debug
using System.Diagnostics;

namespace PSSL_Environment
{
    class Obj
    {
        public List<Vertex> VertexList;
        public List<Normal> NormalList;
        public List<Face> FaceList;
        public List<TextureVertex> TextureList;

        public List<uint> VertexIndices;
        public List<uint> NormalIndices;
        public List<uint> UVIndices;

        public int IndicesPerFace = 3;
        public Obj(string path)
        {
            VertexList = new List<Vertex>();
            FaceList = new List<Face>();
            TextureList = new List<TextureVertex>();
            NormalList = new List<Normal>();

            VertexIndices = new List<uint>();
            NormalIndices = new List<uint>();
            UVIndices = new List<uint>();


            LoadObj(path);
            FixIndexing();
        }

        public void FixIndexing()
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

            //for (int i = 0; i < FaceList.Count * 3; i++)
            //{
            //    tempVertexList.Add(VertexList.ElementAt((int)VertexIndices.ElementAt(i)));
            //    tempNormalList.Add(NormalList.ElementAt((int)NormalIndices.ElementAt(i)));
            //    tempTextureList.Add(TextureList.ElementAt((int)UVIndices.ElementAt(i)));
            //}


            string vertexCount = VertexIndices.Count.ToString();
            string NormalCount = NormalIndices.Count.ToString();
            string TextureCount = UVIndices.Count.ToString();

            Debug.Write("Vertex Count: " + vertexCount);
            Debug.Write("Normal Count: " + NormalCount);
            Debug.Write("UV Count: " + TextureCount);


            for (int i = 0; i < VertexIndices.Count; i++)
            {
                uint vertexIndex = VertexIndices.ElementAt(i);
                Vertex vertexOut = VertexList.ElementAt((int)vertexIndex - 1);
                tempVertexList.Add(vertexOut);
            }

            for (int i = 0; i < NormalIndices.Count; i++)
            {
                uint normalIndex = NormalIndices.ElementAt(i);
                Normal normalOut = NormalList.ElementAt((int)normalIndex - 1);
                tempNormalList.Add(normalOut);
            }

            for (int i = 0; i < UVIndices.Count; i++)
            {
                uint uvIndex = UVIndices.ElementAt(i);
                TextureVertex uvOut = TextureList.ElementAt((int)uvIndex - 1);
                tempTextureList.Add(uvOut);
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
        }

        //public int indicesPerFace;

        public Extent Size { get; set; }

        public string UseMtl { get; set; }
        public string Mtl { get; set; }

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

        public float[] ToFloatArrayNormal()
        {
            int counter = 0;
            // Make array 3 times the size of vertex list for all coordinates
            float[] output = new float[FaceList.Count * 3];
            for (int i = 0; i < FaceList.Count; i++)
            {
                output[counter++] = (float)NormalList.ElementAt(i).NX;
                output[counter++] = (float)NormalList.ElementAt(i).NY;
                output[counter++] = (float)NormalList.ElementAt(i).NZ;
            }

            return output;
        }

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



        private void LoadObj(string path)
        {
            LoadObj(File.ReadAllLines(path));
        }

        public void LoadObj(Stream data)
        {
            using (var reader = new StreamReader(data))
            {
                LoadObj(reader.ReadToEnd().Split(Environment.NewLine.ToCharArray()));
            }
        }

        public void LoadObj(IEnumerable<string> data)
        {
            foreach (var line in data)
            {
                processLine(line);
            }

            //IndicesPerFace = Face.IndicesPerFace;

            updateSize();
        }

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

        private void processLine(string line)
        {
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "usemtl":
                        UseMtl = parts[1];
                        break;
                    case "mtllib":
                        Mtl = parts[1];
                        break;
                    case "v":
                        Vertex v = new Vertex();
                        v.LoadFromStringArray(parts);
                        VertexList.Add(v);
                        v.Index = VertexList.Count();
                        break;
                    case "vn":
                        Normal vn = new Normal();
                        vn.LoadFromStringArray(parts);
                        NormalList.Add(vn);
                        vn.Index = VertexList.Count();
                        break;
                    case "f":
                        Face f = new Face();
                        f.LoadFromStringArray(parts);
                        f.UseMtl = UseMtl;
                        FaceList.Add(f);
                        break;
                    case "vt":
                        TextureVertex vt = new TextureVertex();
                        vt.LoadFromStringArray(parts);
                        TextureList.Add(vt);
                        vt.Index = TextureList.Count();
                        break;

                }
            }
        }

    }
}
