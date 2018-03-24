using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media.Imaging;
using GlmNet;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.Shaders;
using SharpGL.Textures;
using SharpGL.VertexBuffers;
using GLint = System.Int32;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using System.Windows;
using System.Windows.Input;
using SharpGL.WPF;
using System.Text.RegularExpressions;
using System.Text;
using PSSL_Environment.UserControls;
//using SharpGL.SceneGraph.Shaders;

namespace PSSL_Environment
{
    public static class VertexAttributes
    {
        public const uint Position = 0;
        public const uint Normal = 1;
        public const uint TexCoord = 2;
    }

    public enum UsingSettings {  BASIC, ADVANCED};
    
    /// <summary>
    /// A class that represents the scene for this sample.
    /// </summary>
    public class Scene
    {
        public UsingSettings graphicsSettings = UsingSettings.BASIC;

        public string vertexShader = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert");

        public string fragShader = ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.frag");

        private Obj myOBJ;

        public Obj getOBJ => myOBJ;

        //private Mesh myMesh;
        
        private KeyValuePair<Obj, VertexBufferArray> meshVertexBufferArray = new KeyValuePair<Obj, VertexBufferArray>();
        private KeyValuePair<Obj, Texture2D> meshTextures = new KeyValuePair<Obj, Texture2D>();

        //  The shaders we use.
        private SharpGL.Shaders.ShaderProgram shaderPerPixel;
        private SharpGL.Shaders.ShaderProgram shaderToon;
        private SharpGL.Shaders.ShaderProgram shaderTexturedPerPixel;
        private SharpGL.Shaders.ShaderProgram shaderTexturedToon;
        private SharpGL.Shaders.ShaderProgram shaderWater;

        // Advanced Shaders
        private ShaderProgram customShaderProgram;
        private bool ShaderProgramValid = false;
        //private VertexShader customVertShader;
        //private FragmentShader customFragShader;

        public string StripControlChars(string s)
        {
            //string temp = s;
            //char newLine = '\n';
            //string sNewLine = "" + newLine;
            //Regex.Replace(temp, Environment.NewLine, sNewLine);
            //return temp;

            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            //string input = "Auspuffanlage \"Century\" f├╝r";
            return ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(s)));
        }

        public string CompileCustomShader(OpenGL gl, string vertexShader, string fragShader)
        {
            ShaderProgramValid = false;
            //  We're going to specify the attribute locations for the position and normal, 
            //  so that we can force both shaders to explicitly have the same locations.
            const uint positionAttribute = 0;
            const uint normalAttribute = 1;
            const uint texCoordAttribute = 2;
            var attributeLocations = new Dictionary<uint, string>
            {
                {positionAttribute, "Position"},
                {normalAttribute, "Normal"},
                {texCoordAttribute, "TexCoord" },
            };

            // Destroy old shader program first
            try
            {
                customShaderProgram.Delete(gl);

            } catch (Exception e)
            {
                // Do nothing as this means there is no context yet
            }

            // Save shader files first
            string x = StripControlChars(vertexShader);
            System.IO.File.WriteAllText(@"Shaders\Custom\CustomVertex.vert", x);



            string y = StripControlChars(fragShader);
            System.IO.File.WriteAllText(@"Shaders\Custom\CustomFrag.frag", y);



            customShaderProgram = new ShaderProgram();
            try
            {
                customShaderProgram.Create(gl, 
                    ManifestResourceLoader.LoadTextFile(@"Shaders\Custom\CustomVertex.vert"),
                    ManifestResourceLoader.LoadTextFile(@"Shaders\Custom\CustomFrag.frag"), 
                    attributeLocations);
            } catch (SharpGL.Shaders.ShaderCompilationException e)
            {
                return e.CompilerOutput;
            }

            ShaderProgramValid = true;
            return "Shaders Compiled Succesfully!";
        }

        //  The modelview, projection and normal matrices.
        private mat4 modelviewMatrix = mat4.identity();
        private mat4 projectionMatrix = mat4.identity();
        private mat3 normalMatrix = mat3.identity();

        private float scaleFactor = 1.0f;

        private float time = 0.0f;

        public bool usingTexture;
        public vec3 modelLocation;
        public vec3 modelRotation;
        public vec3 lightLocation;
        // Get colors from color picker
        public vec3 ambientMaterialColor;
        public vec3 diffuseMaterialColor;
        public vec3 specularMaterialColor;
        public float alphaColor;
        public Texture2D meshTexture;

        public Texture2D[] waterTextures = new Texture2D[2];

        public float frequency = 1;
        public float speed = 1;
        public float amplitude = 1;
        //public SharpGL.GL

        /// <summary>
        /// Initialises the Scene.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        public void Initialise(OpenGL gl)
        {
            //  We're going to specify the attribute locations for the position and normal, 
            //  so that we can force both shaders to explicitly have the same locations.
            const uint positionAttribute = 0;
            const uint normalAttribute = 1;
            const uint texCoordAttribute = 2;
            var attributeLocations = new Dictionary<uint, string>
            {
                {positionAttribute, "Position"},
                {normalAttribute, "Normal"},
                {texCoordAttribute, "TexCoord" },
            };

            // Create non texture shaders first
            //  Create the per pixel shader.tr
            shaderPerPixel = new SharpGL.Shaders.ShaderProgram();
            shaderPerPixel.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.frag"), attributeLocations);

            //  Create the toon shader.
            shaderToon = new SharpGL.Shaders.ShaderProgram();
            shaderToon.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.frag"), attributeLocations);

            // Creat tecture shaders second
            //  Create the per pixel shader.tr
            shaderTexturedPerPixel = new SharpGL.Shaders.ShaderProgram();
            shaderTexturedPerPixel.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixelTexture.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixelTexture.frag"), attributeLocations);

            //  Create the toon shader.
            shaderTexturedToon = new SharpGL.Shaders.ShaderProgram();
            shaderTexturedToon.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\ToonTexture.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\ToonTexture.frag"), attributeLocations);

            //  Create the water shader.
            shaderWater = new SharpGL.Shaders.ShaderProgram();
            shaderWater.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\Water\Water.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\Water\Water.frag"), attributeLocations);

            

            //  Needed to render textures in viewport
            //gl.Enable(OpenGL.GL_TEXTURE_2D);


            // Set up any variables
            modelLocation = new vec3(-1, -1, -10);
            modelRotation = new vec3(0, 1, 0);
            lightLocation = new vec3(0.25f, 0.25f, 10f);

            ((MainWindow)Application.Current.MainWindow).VertexShaderText.Text = vertexShader;
            ((MainWindow)Application.Current.MainWindow).FragmentShaderText.Text = fragShader;

            ((MainWindow)Application.Current.MainWindow).CompileErrorOutput.Text = CompileCustomShader(gl,
                vertexShader, fragShader);

        }

        /// <summary>
        /// Creates the projection matrix for the given screen size.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        /// <param name="screenWidth">Width of the screen.</param>
        /// <param name="screenHeight">Height of the screen.</param>
        public void CreateProjectionMatrix(OpenGL gl, float screenWidth, float screenHeight)
        {
            //  Create the projection matrix for our screen size.
            const float S = 0.46f;
            float H = S * screenHeight / screenWidth;
            projectionMatrix = glm.frustum(-S, S, -H, H, 1, 100);

            //  When we do immediate mode drawing, OpenGL needs to know what our projection matrix
            //  is, so set it now.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MultMatrix(projectionMatrix.to_array());
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        /// <summary>
        /// Creates the modelview and normal matrix. Also rotates the sceen by a specified amount.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle, in radians.</param>
        public void CreateModelviewAndNormalMatrix(float rotationAngle)
        {
            //  Create the modelview and normal matrix. We'll also rotate the scene
            //  by the provided rotation angle, which means things that draw it 
            //  can make the scene rotate easily.
            mat4 rotation = glm.rotate(mat4.identity(), rotationAngle, modelRotation);
            mat4 translation = glm.translate(mat4.identity(), modelLocation);
            mat4 scale = glm.scale(mat4.identity(), new vec3(scaleFactor, scaleFactor, scaleFactor));
            modelviewMatrix = scale * rotation * translation;
            normalMatrix = modelviewMatrix.to_mat3();
        }

        // Adjust the viewport to scale properly to avoid distorition
        public void ResizeViewport(OpenGL gl, int screenWidth, int screenHeight)
        {
            gl.Viewport(0, 0, (int)screenWidth, (int)screenHeight);
            //gl.Frustum()
        }

        /// <summary>
        /// Renders the scene in immediate mode.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        //public void RenderImmediateMode(OpenGL gl)
        //{
        //    //  Setup the modelview matrix.
        //    gl.MatrixMode(OpenGL.GL_MODELVIEW);
        //    gl.LoadIdentity();
        //    gl.MultMatrix(modelviewMatrix.to_array());


        //    //var texture = meshTextures.ContainsKey(meshes) ? meshTextures[meshes] : null;
        //    var texture = meshTextures.Value;
        //    if (texture != null)
        //        texture.Bind(gl);

        //    uint mode = OpenGL.GL_TRIANGLES;
        //    if (myMesh.indicesPerFace == 4)
        //        mode = OpenGL.GL_QUADS;
        //    else if (myMesh.indicesPerFace > 4)
        //        mode = OpenGL.GL_POLYGON;

        //    //  Render the group faces.
        //    gl.Begin(mode);
        //    for (int i = 0; i < myMesh.vertices.Length; i++)
        //    {
        //        gl.Vertex(myMesh.vertices[i].x, myMesh.vertices[i].y, myMesh.vertices[i].z);
        //        if (myMesh.normals != null)
        //            gl.Normal(myMesh.normals[i].x, myMesh.normals[i].y, myMesh.normals[i].z);
        //        if (myMesh.uvs != null)
        //            gl.TexCoord(myMesh.uvs[i].x, myMesh.uvs[i].y);
        //    }
        //    gl.End();

        //    if (texture != null)
        //        texture.Unbind(gl);
            
            
            
        //}

        public void LoadTexture(OpenGL gl, Bitmap newTexture)
        {
            //meshTexture = new Texture2D();
            meshTexture = new Texture2D();
            meshTexture.SetImage(gl, newTexture);
            AddTexture(gl, newTexture);
            //  Go through each mesh and give texture.
            //foreach (var mesh in meshes)
            //{
            //    mesh.
            //}
        }

        public void LoadWaterTextures(OpenGL gl, Bitmap newTexture)
        {
            //meshTexture = new Texture2D();
            waterTextures[0] = new Texture2D();
            waterTextures[0].SetImage(gl, newTexture);
            AddTexture(gl, newTexture);

            waterTextures[1] = new Texture2D();
            waterTextures[1].SetImage(gl, newTexture);
            AddTexture(gl, newTexture);
            //  Go through each mesh and give texture.
            //foreach (var mesh in meshes)
            //{
            //    mesh.
            //}
        }

        // Renders the scene with colour input for each of ADS
        public void RenderColorMode(OpenGL gl, bool useToonShader)
        {
            if(myOBJ != null)
            {
                if(myOBJ.GetValidObject() == true)
            {
                vec3 defaultValues;
                defaultValues.x = 0.0f;
                defaultValues.y = 0.0f;
                defaultValues.z = 0.0f;

                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).ambientColorPicker.SelectedColor == null)
                {
                    ambientMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).diffuseColorPicker.SelectedColor == null)
                {
                    diffuseMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).specularColorPicker.SelectedColor == null)
                {
                    specularMaterialColor = defaultValues;
                }



                //  Get a reference to the appropriate shader.
                var shader = useToonShader ? shaderToon : shaderPerPixel;

                //  Use the shader program.
                shader.Bind(gl);

                //  Set the light position.
                shader.SetUniform3(gl, "LightPosition", lightLocation.x, lightLocation.y, lightLocation.z);

                //  Set the matrices.
                shader.SetUniformMatrix4(gl, "Projection", projectionMatrix.to_array());
                shader.SetUniformMatrix4(gl, "Modelview", modelviewMatrix.to_array());
                shader.SetUniformMatrix3(gl, "NormalMatrix", normalMatrix.to_array());

                // Set shader alpha
                shader.SetUniform1(gl, "Alpha", alphaColor);



                //shader.SetUniform3(gl, "DiffuseMaterial", mesh.material.Diffuse.r, mesh.material.Diffuse.g, mesh.material.Diffuse.b);
                //shader.SetUniform3(gl, "AmbientMaterial", mesh.material.Ambient.r, mesh.material.Ambient.g, mesh.material.Ambient.b);
                //shader.SetUniform3(gl, "SpecularMaterial", mesh.material.Specular.r, mesh.material.Specular.g, mesh.material.Specular.b);
                shader.SetUniform3(gl, "AmbientMaterial", ambientMaterialColor.x, ambientMaterialColor.y,
                    ambientMaterialColor.z);
                shader.SetUniform3(gl, "DiffuseMaterial", diffuseMaterialColor.x, diffuseMaterialColor.y,
                    diffuseMaterialColor.z);
                shader.SetUniform3(gl, "SpecularMaterial", specularMaterialColor.x, specularMaterialColor.y,
                    specularMaterialColor.z);
                shader.SetUniform1(gl, "Shininess", (float)((MainWindow)System.Windows.Application.Current.MainWindow).shininessValue.Value);

                var vertexBufferArray = meshVertexBufferArray;
                //vertexBufferArray.Bind(gl);
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBufferArray.Value.VertexBufferArrayObject);

                

                uint mode = OpenGL.GL_TRIANGLES;
                if (myOBJ.IndicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (myOBJ.IndicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);
                    //gl.DrawElements(mode, myOBJ.VertexIndices.Count, type, myOBJ.VertexIndices);
                //gl.DrawArrays(mode, 0, myOBJ.NormalList.Count);

                //IntPtr[] vertices = new IntPtr[myOBJ.VertexList.Count];



                //gl.BufferData(OpenGL.GL_ARRAY_BUFFER, myOBJ.VertexList.Count, myOBJ.VertexList., OpenGL.GL_STATIC_DRAW);


                shader.Unbind(gl);
            }
            }
            
        }
        
        // Renders the scene with a texture along with colours for ADS
        public void RenderTextureMode(OpenGL gl, bool useToonShader)
        {
            if(myOBJ == null)
            {
                return;
            }

            if (myOBJ.GetValidObject() == true)
            {
                vec3 defaultValues;
                defaultValues.x = 0.0f;
                defaultValues.y = 0.0f;
                defaultValues.z = 0.0f;


                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).ambientColorPicker.SelectedColor == null)
                {
                    ambientMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).diffuseColorPicker.SelectedColor == null)
                {
                    diffuseMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).specularColorPicker.SelectedColor == null)
                {
                    specularMaterialColor = defaultValues;
                }



                //  Get a reference to the appropriate shader.
                var shader = useToonShader ? shaderTexturedToon : shaderTexturedPerPixel;

                //  Use the shader program.
                shader.Bind(gl);

                //  Set the light position.
                shader.SetUniform3(gl, "LightPosition", lightLocation.x, lightLocation.y, lightLocation.z);

                //  Set the matrices.
                shader.SetUniformMatrix4(gl, "Projection", projectionMatrix.to_array());
                shader.SetUniformMatrix4(gl, "Modelview", modelviewMatrix.to_array());
                shader.SetUniformMatrix3(gl, "NormalMatrix", normalMatrix.to_array());

                // Set shader alpha
                shader.SetUniform1(gl, "Alpha", alphaColor);

                //shader.SetUniform3(gl, "DiffuseMaterial", mesh.material.Diffuse.r, mesh.material.Diffuse.g, mesh.material.Diffuse.b);
                //shader.SetUniform3(gl, "AmbientMaterial", mesh.material.Ambient.r, mesh.material.Ambient.g, mesh.material.Ambient.b);
                //shader.SetUniform3(gl, "SpecularMaterial", mesh.material.Specular.r, mesh.material.Specular.g, mesh.material.Specular.b);
                shader.SetUniform3(gl, "AmbientMaterial", ambientMaterialColor.x, ambientMaterialColor.y,
                    ambientMaterialColor.z);
                shader.SetUniform3(gl, "DiffuseMaterial", diffuseMaterialColor.x, diffuseMaterialColor.y,
                    diffuseMaterialColor.z);
                shader.SetUniform3(gl, "SpecularMaterial", specularMaterialColor.x, specularMaterialColor.y,
                    specularMaterialColor.z);
                shader.SetUniform1(gl, "Shininess", (float)((MainWindow)System.Windows.Application.Current.MainWindow).shininessValue.Value);

                var vertexBufferArray = meshVertexBufferArray;
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBufferArray.Value.VertexBufferArrayObject);
                //vertexBufferArray.Bind(gl);


                //uint mode = OpenGL.GL_TRIANGLES;
                //if (mesh.indicesPerFace == 4)
                //    mode = OpenGL.GL_QUADS;
                //else if (mesh.indicesPerFace > 4)
                //    mode = OpenGL.GL_POLYGON;

                //gl.BufferData(OpenGL.GL_BUFFER, mesh.vertices.Length, mesh.vertices, OpenGL.GL_STATIC_DRAW);
                //gl.DrawArrays(mode, 0, mesh.vertices.Length);
                //gl.draw

                // Set shader texture
                //if (meshTexture != null)
                //{

                //}

                // Bind texture
                //var texture = meshTextures(meshes) ? meshTextures[meshes] : null;
                var texture = meshTextures.Value;
                if (texture != null)
                {
                    GLint texLoc;
                    texLoc = shader.GetUniformLocation(gl, "Texture");
                    gl.Uniform1(texLoc, 0);

                    gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.textureObject);

                    //texture.Bind(gl);
                    //gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.textureObject);
                    //shader.SetUniform1(gl, "Texture", 0);

                    ((MainWindow)System.Windows.Application.Current.MainWindow).UsingTexture.IsChecked = true;
                    //gl.

                }

                //uint mode = OpenGL.GL_TRIANGLES;
                //if (myMesh.indicesPerFace == 4)
                //    mode = OpenGL.GL_QUADS;
                //else if (myMesh.indicesPerFace > 4)
                //    mode = OpenGL.GL_POLYGON;

                //gl.DrawArrays(OpenGL.GL_QUADS, 0, myOBJ.VertexList.Count * 3);


                uint mode = OpenGL.GL_TRIANGLES;
                if (myOBJ.IndicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (myOBJ.IndicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);


                //gl.BufferData(OpenGL.GL_ARRAY_BUFFER, myOBJ.VertexList.Count * 
                //    System.Runtime.InteropServices.Marshal.SizeOf(vec3), &)

                //  Render the group faces.
                //gl.Begin(OpenGL.GL_QUADS);
                //for (int i = 0; i < myOBJ.VertexList.Count; i++)
                //{
                //    gl.Vertex(myOBJ.VertexList.ElementAt(i).X, myOBJ.VertexList.ElementAt(i).Y, myOBJ.VertexList.ElementAt(i).Z);
                //    if (myOBJ.NormalList.Count > 0)
                //        gl.Normal(myOBJ.NormalList.ElementAt(i).NX, myOBJ.NormalList.ElementAt(i).NY, myOBJ.NormalList.ElementAt(i).NZ);
                //    if (myOBJ.TextureList.Count > 0)
                //        gl.TexCoord(myOBJ.TextureList.ElementAt(i).X, myOBJ.TextureList.ElementAt(i).Y);
                //}
                //gl.End();
                //gl.Flush();


                if (texture != null)
                    texture.Unbind(gl);

                shader.Unbind(gl);
            }
            
        }

        // Renders the scene with a texture along with colours for ADS
        public void RenderWaterMode(OpenGL gl)
        {
            if(myOBJ == null)
            {
                return;
            }

            if (myOBJ.GetValidObject() == true)
            {
                vec3 defaultValues;
                defaultValues.x = 0.0f;
                defaultValues.y = 0.0f;
                defaultValues.z = 0.0f;

                time += 1.0f / 60.0f;


                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).ambientColorPicker.SelectedColor == null)
                {
                    ambientMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).diffuseColorPicker.SelectedColor == null)
                {
                    diffuseMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).specularColorPicker.SelectedColor == null)
                {
                    specularMaterialColor = defaultValues;
                }



                //  Get a reference to the appropriate shader.
                var shader = shaderWater;

                //  Use the shader program.
                shader.Bind(gl);

                //  Set the light position.
                shader.SetUniform3(gl, "LightPosition", lightLocation.x, lightLocation.y, lightLocation.z);

                //  Set the matrices.
                shader.SetUniformMatrix4(gl, "Projection", projectionMatrix.to_array());
                shader.SetUniformMatrix4(gl, "Modelview", modelviewMatrix.to_array());
                shader.SetUniformMatrix3(gl, "NormalMatrix", normalMatrix.to_array());

                // Set shader alpha
                shader.SetUniform1(gl, "Alpha", alphaColor);

                // Set shader time
                shader.SetUniform1(gl, "Time", time);

                // Set speed, amplitude and frequency
                shader.SetUniform1(gl, "Speed", speed);
                shader.SetUniform1(gl, "Amplitude", amplitude);
                shader.SetUniform1(gl, "Frequency", frequency);

                //shader.SetUniform3(gl, "DiffuseMaterial", mesh.material.Diffuse.r, mesh.material.Diffuse.g, mesh.material.Diffuse.b);
                //shader.SetUniform3(gl, "AmbientMaterial", mesh.material.Ambient.r, mesh.material.Ambient.g, mesh.material.Ambient.b);
                //shader.SetUniform3(gl, "SpecularMaterial", mesh.material.Specular.r, mesh.material.Specular.g, mesh.material.Specular.b);
                shader.SetUniform3(gl, "AmbientMaterial", ambientMaterialColor.x, ambientMaterialColor.y,
                    ambientMaterialColor.z);
                shader.SetUniform3(gl, "DiffuseMaterial", diffuseMaterialColor.x, diffuseMaterialColor.y,
                    diffuseMaterialColor.z);
                shader.SetUniform3(gl, "SpecularMaterial", specularMaterialColor.x, specularMaterialColor.y,
                    specularMaterialColor.z);
                shader.SetUniform1(gl, "Shininess", (float)((MainWindow)System.Windows.Application.Current.MainWindow).shininessValue.Value);

                var vertexBufferArray = meshVertexBufferArray;
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBufferArray.Value.VertexBufferArrayObject);
                //vertexBufferArray.Bind(gl);


                //uint mode = OpenGL.GL_TRIANGLES;
                //if (mesh.indicesPerFace == 4)
                //    mode = OpenGL.GL_QUADS;
                //else if (mesh.indicesPerFace > 4)
                //    mode = OpenGL.GL_POLYGON;

                //gl.BufferData(OpenGL.GL_BUFFER, mesh.vertices.Length, mesh.vertices, OpenGL.GL_STATIC_DRAW);
                //gl.DrawArrays(mode, 0, mesh.vertices.Length);
                //gl.draw

                // Set shader texture
                //if (meshTexture != null)
                //{

                //}

                // Bind texture
                //var texture = meshTextures(meshes) ? meshTextures[meshes] : null;
                var texture = meshTextures.Value;
                if (texture != null)
                {
                    GLint texLocOne;
                    texLocOne = shader.GetUniformLocation(gl, "tex");
                    gl.Uniform1(texLocOne, 0);
                    gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.textureObject);

                    GLint texLocTwo;
                    texLocTwo = shader.GetUniformLocation(gl, "tex2");
                    gl.Uniform1(texLocTwo, 1);
                    gl.ActiveTexture(OpenGL.GL_TEXTURE1);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.textureObject);


                    //texture.Bind(gl);
                    //gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.textureObject);
                    //shader.SetUniform1(gl, "Texture", 0);

                    //((MainWindow)System.Windows.Application.Current.MainWindow).UsingTexture.IsChecked = true;
                    //gl.

                }

                //uint mode = OpenGL.GL_TRIANGLES;
                //if (myMesh.indicesPerFace == 4)
                //    mode = OpenGL.GL_QUADS;
                //else if (myMesh.indicesPerFace > 4)
                //    mode = OpenGL.GL_POLYGON;

                //gl.DrawArrays(OpenGL.GL_QUADS, 0, myOBJ.VertexList.Count * 3);


                uint mode = OpenGL.GL_TRIANGLES;
                if (myOBJ.IndicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (myOBJ.IndicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);


                //gl.BufferData(OpenGL.GL_ARRAY_BUFFER, myOBJ.VertexList.Count * 
                //    System.Runtime.InteropServices.Marshal.SizeOf(vec3), &)

                //  Render the group faces.
                //gl.Begin(OpenGL.GL_QUADS);
                //for (int i = 0; i < myOBJ.VertexList.Count; i++)
                //{
                //    gl.Vertex(myOBJ.VertexList.ElementAt(i).X, myOBJ.VertexList.ElementAt(i).Y, myOBJ.VertexList.ElementAt(i).Z);
                //    if (myOBJ.NormalList.Count > 0)
                //        gl.Normal(myOBJ.NormalList.ElementAt(i).NX, myOBJ.NormalList.ElementAt(i).NY, myOBJ.NormalList.ElementAt(i).NZ);
                //    if (myOBJ.TextureList.Count > 0)
                //        gl.TexCoord(myOBJ.TextureList.ElementAt(i).X, myOBJ.TextureList.ElementAt(i).Y);
                //}
                //gl.End();
                //gl.Flush();


                if (texture != null)
                    texture.Unbind(gl);

                shader.Unbind(gl);
            }

        }

        public void RenderCustomMode(OpenGL gl)
        {
            if (myOBJ == null)
                return;

            if (myOBJ.GetValidObject() == true && ShaderProgramValid == true)
            {
                vec3 defaultValues;
                defaultValues.x = 0.0f;
                defaultValues.y = 0.0f;
                defaultValues.z = 0.0f;

                time += 1.0f / 60.0f;


                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).ambientColorPicker.SelectedColor == null)
                {
                    ambientMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).diffuseColorPicker.SelectedColor == null)
                {
                    diffuseMaterialColor = defaultValues;
                }
                // Checks if colour pickers has a colour yet or not and sets a value if it doesnt
                if (((MainWindow)System.Windows.Application.Current.MainWindow).specularColorPicker.SelectedColor == null)
                {
                    specularMaterialColor = defaultValues;
                }



                //  Get a reference to the appropriate shader.
                var shader = customShaderProgram;

                //  Use the shader program.
                shader.Bind(gl);

                //  Set the light position.
                shader.SetUniform3(gl, "LightPosition", lightLocation.x, lightLocation.y, lightLocation.z);

                //  Set the matrices.
                shader.SetUniformMatrix4(gl, "Projection", projectionMatrix.to_array());
                shader.SetUniformMatrix4(gl, "Modelview", modelviewMatrix.to_array());
                shader.SetUniformMatrix3(gl, "NormalMatrix", normalMatrix.to_array());

                // Set shader alpha
                shader.SetUniform1(gl, "Alpha", alphaColor);

                // Set shader time
                shader.SetUniform1(gl, "Time", time);

                //shader.SetUniform3(gl, "DiffuseMaterial", mesh.material.Diffuse.r, mesh.material.Diffuse.g, mesh.material.Diffuse.b);
                //shader.SetUniform3(gl, "AmbientMaterial", mesh.material.Ambient.r, mesh.material.Ambient.g, mesh.material.Ambient.b);
                //shader.SetUniform3(gl, "SpecularMaterial", mesh.material.Specular.r, mesh.material.Specular.g, mesh.material.Specular.b);
                shader.SetUniform3(gl, "AmbientMaterial", ambientMaterialColor.x, ambientMaterialColor.y,
                    ambientMaterialColor.z);
                shader.SetUniform3(gl, "DiffuseMaterial", diffuseMaterialColor.x, diffuseMaterialColor.y,
                    diffuseMaterialColor.z);
                shader.SetUniform3(gl, "SpecularMaterial", specularMaterialColor.x, specularMaterialColor.y,
                    specularMaterialColor.z);
                shader.SetUniform1(gl, "Shininess", (float)((MainWindow)System.Windows.Application.Current.MainWindow).shininessValue.Value);

                var vertexBufferArray = meshVertexBufferArray;
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBufferArray.Value.VertexBufferArrayObject);
                //vertexBufferArray.Bind(gl);


                //uint mode = OpenGL.GL_TRIANGLES;
                //if (mesh.indicesPerFace == 4)
                //    mode = OpenGL.GL_QUADS;
                //else if (mesh.indicesPerFace > 4)
                //    mode = OpenGL.GL_POLYGON;

                //gl.BufferData(OpenGL.GL_BUFFER, mesh.vertices.Length, mesh.vertices, OpenGL.GL_STATIC_DRAW);
                //gl.DrawArrays(mode, 0, mesh.vertices.Length);
                //gl.draw

                // Set shader texture
                //if (meshTexture != null)
                //{

                //}

                // Bind texture
                //var texture = meshTextures(meshes) ? meshTextures[meshes] : null;
                var texture = meshTextures.Value;
                if (texture != null)
                {
                    GLint texLoc;
                    texLoc = shader.GetUniformLocation(gl, "Texture");
                    gl.Uniform1(texLoc, 0);

                    gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.textureObject);

                    //texture.Bind(gl);
                    //gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.textureObject);
                    //shader.SetUniform1(gl, "Texture", 0);

                    ((MainWindow)System.Windows.Application.Current.MainWindow).UsingTexture.IsChecked = true;
                    //gl.

                }

                // Custom constants
                // Float Constants
                List<ConstantsInterface> ConstantList = ((MainWindow)System.Windows.Application.Current.MainWindow).ConstantsControl;
                foreach(var e in ConstantList)
                {
                    e.AddToShaderProgram(gl, shader);
                }

                //uint mode = OpenGL.GL_TRIANGLES;
                //if (myMesh.indicesPerFace == 4)
                //    mode = OpenGL.GL_QUADS;
                //else if (myMesh.indicesPerFace > 4)
                //    mode = OpenGL.GL_POLYGON;

                //gl.DrawArrays(OpenGL.GL_QUADS, 0, myOBJ.VertexList.Count * 3);


                uint mode = OpenGL.GL_TRIANGLES;
                if (myOBJ.IndicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (myOBJ.IndicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);


                //gl.BufferData(OpenGL.GL_ARRAY_BUFFER, myOBJ.VertexList.Count * 
                //    System.Runtime.InteropServices.Marshal.SizeOf(vec3), &)

                //  Render the group faces.
                //gl.Begin(OpenGL.GL_QUADS);
                //for (int i = 0; i < myOBJ.VertexList.Count; i++)
                //{
                //    gl.Vertex(myOBJ.VertexList.ElementAt(i).X, myOBJ.VertexList.ElementAt(i).Y, myOBJ.VertexList.ElementAt(i).Z);
                //    if (myOBJ.NormalList.Count > 0)
                //        gl.Normal(myOBJ.NormalList.ElementAt(i).NX, myOBJ.NormalList.ElementAt(i).NY, myOBJ.NormalList.ElementAt(i).NZ);
                //    if (myOBJ.TextureList.Count > 0)
                //        gl.TexCoord(myOBJ.TextureList.ElementAt(i).X, myOBJ.TextureList.ElementAt(i).Y);
                //}
                //gl.End();
                //gl.Flush();


                if (texture != null)
                    texture.Unbind(gl);

                shader.Unbind(gl);
            }

        }


        private int CreateVertexBufferArray(OpenGL gl, Obj newObj)
        {
            //  Create and bind a vertex buffer array.
            var vertexBufferArray = new VertexBufferArray();
            try
            {
                vertexBufferArray.Create(gl);
                vertexBufferArray.Bind(gl);
            } catch(Exception e)
            {
                System.Windows.MessageBox.Show("Failed to create and bind vertexBufferArray", "Error loading OBJ.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }



            //  Create a vertex buffer for the vertices.
            var verticesVertexBuffer = new VertexBuffer();
            verticesVertexBuffer.Create(gl);
            verticesVertexBuffer.Bind(gl);
            verticesVertexBuffer.SetData(gl, VertexAttributes.Position,
                                 newObj.ToFloatArrayVertex(),
                                 false, 3);
            
            //((MainWindow)System.Windows.Application.Current.MainWindow).PositionDebug.Text = "Position = " + (int)newObj.VertexList.Count;
            // Fills the Model Properties window
            //int pIndex = 0;
            //((MainWindow)System.Windows.Application.Current.MainWindow).PositionDataList.Items.Clear();
            //foreach (var i in newObj.VertexList)
            //{
            //    string Pos = "Position[" + pIndex + "] = ( " + i.X + ", " + i.Y + ", " + i.Z + ")";
            //    ((MainWindow)System.Windows.Application.Current.MainWindow).PositionDataList.Items.Add(Pos);
            //    pIndex++;
            //}
            //((MainWindow)System.Windows.Application.Current.MainWindow).PositionDataList.ad(_items);

            if (newObj.NormalList.Count > 0)
            {
                var normalsVertexBuffer = new VertexBuffer();
                normalsVertexBuffer.Create(gl);
                normalsVertexBuffer.Bind(gl);
                normalsVertexBuffer.SetData(gl, VertexAttributes.Normal,
                                            newObj.ToFloatArrayNormal(),
                                            false, 3);

                //((MainWindow)System.Windows.Application.Current.MainWindow).NormalDebug.Text = "Normal = " + (int)newObj.NormalList.Count;
                // Fills the Model Properties window
                //int nIndex = 0;
                //((MainWindow)System.Windows.Application.Current.MainWindow).NormalDataList.Items.Clear();
                //foreach (var i in newObj.NormalList)
                //{
                //    string Nor = "Normal[" + nIndex + "] = ( " + i.NX + ", " + i.NY + ", " + i.NZ + ")";
                //    ((MainWindow)System.Windows.Application.Current.MainWindow).NormalDataList.Items.Add(Nor);
                //    nIndex++;
                //}
            }

            if (newObj.TextureList.Count > 0)
            {
                var texCoordsVertexBuffer = new VertexBuffer();
                texCoordsVertexBuffer.Create(gl);
                texCoordsVertexBuffer.Bind(gl);
                texCoordsVertexBuffer.SetData(gl, VertexAttributes.TexCoord,
                                              newObj.ToFloatArrayTexture(),
                                              false, 2);
                //((MainWindow)System.Windows.Application.Current.MainWindow).TexCoordDebug.Text = "Texcood = " + (int)newObj.TextureList.Count;
                // Fills the Model Properties window
                //int tIndex = 0;
                //((MainWindow)System.Windows.Application.Current.MainWindow).TexCoordDataList.Items.Clear();
                //foreach (var i in newObj.TextureList)
                //{
                //    string Tex = "Normal[" + tIndex + "] = ( " + i.X + ", " + i.Y + ")";
                //    ((MainWindow)System.Windows.Application.Current.MainWindow).TexCoordDataList.Items.Add(Tex);
                //    tIndex++;
                //}
            }

            verticesVertexBuffer.Unbind(gl);
            //gl.
            meshVertexBufferArray = new KeyValuePair<Obj, VertexBufferArray>(meshVertexBufferArray.Key, vertexBufferArray);

           
            return 0;
        }

        public async void Load(OpenGLControl openGLContext, string objectFilePath)
        {
            OpenGL gl = openGLContext.OpenGL;
            openGLContext.Cursor = Cursors.Wait;
            myOBJ = new Obj();
            int result = await myOBJ.Initialise(objectFilePath);

            CreateVertexBufferArray(gl, myOBJ);
            //  Auto scale.
            SetScaleFactorAuto();

            openGLContext.Cursor = Cursors.Arrow;
        }

        //private void CreateTextures(OpenGL gl, Obj mesh)
        //
        //    //  Create a new texture and bind it.
        //    var texture = new Texture2D();
        //    texture.Create(gl);
        //    texture.Bind(gl);
        //    texture.SetParameter(gl, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
        //    texture.SetParameter(gl, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
        //    texture.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
        //    texture.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
        //    texture.SetImage(gl, (Bitmap)mesh.material.TextureMapDiffuse.Image);
        //    texture.Unbind(gl);
        //    meshTextures = new KeyValuePair<Obj, Texture2D>(meshTextures.Key, texture);
        //}

        private void AddTexture(OpenGL gl, Bitmap image)
        {

            //meshTextures[meshes] = null;
            //  Create a new texture and bind it.
            var texture = new Texture2D();
            texture.Create(gl);
            texture.Bind(gl);
            texture.SetImage(gl, image);
            texture.SetParameter(gl, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            texture.SetParameter(gl, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            texture.Unbind(gl);
            meshTextures = new KeyValuePair<Obj, Texture2D>(meshTextures.Key, texture);

        }

        /// <summary>
        /// Gets or sets the scale factor.
        /// </summary>
        public float ScaleFactor
        {
            get { return scaleFactor; }
            set { scaleFactor = value; }
        }

        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        public mat4 ProjectionMatrix
        {
            get { return projectionMatrix; }
        }


        /// <summary>
        /// Sets the scale factor automatically based on the size of the geometry.
        /// Returns the computed scale factor.
        /// </summary>
        /// <returns>The computed scale factor.</returns>
        public float SetScaleFactorAuto()
        {
            //  0.02 good for inet models.

            //  If we have no meshes, just use 1.0f.
            if (myOBJ == null)
            {
                scaleFactor = 1.0f;
                return scaleFactor;
            }

            //  Find the maximum vertex value.
            double max = 0.0;
            try
            {
                var maxX = myOBJ.VertexList.AsParallel().Max(v => Math.Abs(v.X));
                var maxY = myOBJ.VertexList.AsParallel().Max(v => Math.Abs(v.Y));
                var maxZ = myOBJ.VertexList.AsParallel().Max(v => Math.Abs(v.Z));
                max = (new[] { maxX, maxY, maxZ }).Max();
            } catch (Exception i)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("No vertices were found.", "Error loading OBJ.", MessageBoxButton.OK, MessageBoxImage.Error);
                scaleFactor = 1.0f;
                return scaleFactor;
            }



            //  Set the scale factor accordingly.
            //  sf = max/c
            scaleFactor = 2.0f / (float)max;
            return scaleFactor;
        }


    }

    
}