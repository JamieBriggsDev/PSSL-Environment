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

        public Obj myOBJ = new Obj();

        public Obj getOBJ => myOBJ;

        public Camera myCamera = new Camera();

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

        /// <summary>
        /// Takes out any char in the string which shouldnt be passed into generation
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string StripControlChars(string s)
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding ascii = Encoding.ASCII;

            //string input = "Auspuffanlage \"Century\" f├╝r";
            return ascii.GetString(Encoding.Convert(utf8, ascii, utf8.GetBytes(s)));
        }

        /// <summary>
        /// This compiles the custom shader which comes from the advanced settings
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="vertexShader"></param>
        /// <param name="fragShader"></param>
        /// <returns></returns>
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

            } catch (Exception)
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
                ManifestResourceLoader.LoadTextFile(@"Shaders\Ripple.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\Ripple.frag"), attributeLocations);



            //  Needed to render textures in viewport
            //gl.Enable(OpenGL.GL_TEXTURE_2D);


            // Set up any variables
            modelLocation = new vec3(-0.5f, -1f, -10f);
            modelRotation = new vec3(0, 1, 0);
            lightLocation = new vec3(0.25f, 0.25f, 10f);


            ((MainWindow)Application.Current.MainWindow).VertexShaderText.Text = vertexShader;
            ((MainWindow)Application.Current.MainWindow).FragmentShaderText.Text = fragShader;

            ((MainWindow)Application.Current.MainWindow).CompileErrorOutput.Text = CompileCustomShader(gl,
                vertexShader, fragShader);


            // Load default sphere obj into viewport ADD SPHERE BUTTON SHOULD BE MADE!!!!!
            Load(((MainWindow)Application.Current.MainWindow).openGlCtrl, @"Resources\Models\Sphere.obj");


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
            //const float S = 0.46f;
            const float S = 0.46f;
            float H = S * screenHeight / screenWidth;
            projectionMatrix = glm.frustum(-S, S, -H, H, 1, 100);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MultMatrix(myCamera.SetPerspective(projectionMatrix).to_array());
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

        }

        /// <summary>
        /// Creates the modelview and normal matrix. Also rotates the sceen by a specified amount.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle, in radians.</param>
        public void CreateModelviewAndNormalMatrix(vec2 rotate)
        {
            //  Create the modelview and normal matrix. We'll also rotate the scene
            //  by the provided rotation angle, which means things that draw it 
            //  can make the scene rotate easily.
            mat4 rotationx = glm.rotate(mat4.identity(), rotate.x / 100.0f, new vec3(0.0f, 1.0f, 0.0f));
            mat4 rotationy = glm.rotate(mat4.identity(), rotate.y / -100.0f, new vec3(1.0f, 0.0f, 0.0f));
            mat4 translation = glm.translate(mat4.identity(), ((MainWindow)Application.Current.MainWindow).newPosition + modelLocation);
            mat4 scale = glm.scale(mat4.identity(), new vec3(scaleFactor, scaleFactor, scaleFactor));
            modelviewMatrix = scale * rotationx * rotationy * translation;
            myOBJ.m_modelWorldMx = modelviewMatrix;
            normalMatrix = modelviewMatrix.to_mat3();
        }

        /// <summary>
        //  Adjust the viewport to scale properly to avoid distorition
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void ResizeViewport(OpenGL gl, int screenWidth, int screenHeight)
        {
            gl.Viewport(0, 0, (int)screenWidth, (int)screenHeight);
        }

        /// <summary>
        /// Loads a single shader into the viewport
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="newTexture"></param>
        public void LoadTexture(OpenGL gl, Bitmap newTexture)
        {
            //meshTexture = new Texture2D();
            meshTexture = new Texture2D();
            meshTexture.SetImage(gl, newTexture);
            AddTexture(gl, newTexture);
        }

        /// <summary>
        /// Loads two textures into the viewport for use with the water shader
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="newTexture"></param>
        public void LoadWaterTextures(OpenGL gl, Bitmap newTexture)
        {
            //meshTexture = new Texture2D();
            waterTextures[0] = new Texture2D();
            waterTextures[0].SetImage(gl, newTexture);
            AddTexture(gl, newTexture);

            waterTextures[1] = new Texture2D();
            waterTextures[1].SetImage(gl, newTexture);
            AddTexture(gl, newTexture);
        }

        /// <summary>
        /// Renders the scene with colour input for each of ADS
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="useToonShader"></param>
        public void RenderColorMode(OpenGL gl, bool useToonShader)
        {
            if (myOBJ != null)
            {
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
                    var shader = useToonShader ? shaderToon : shaderPerPixel;

                    //  Use the shader program.
                    shader.Bind(gl);

                    //  Set The light Position.
                    shader.SetUniform3(gl, "LightPosition", lightLocation.x, lightLocation.y, lightLocation.z);

                    // Set up projection and model view matrices
                    shader.SetUniformMatrix4fv(gl, "Projection", (myCamera.m_projectionMx * myCamera.m_worldViewMx).to_array());
                    shader.SetUniformMatrix4(gl, "Modelview", myOBJ.m_modelWorldMx.to_array());

                    // Set up normal matrix
                    shader.SetUniformMatrix4(gl, "NormalMatrix", CustomMath.TransposeMatrix(glm.inverse(myCamera.m_worldViewMx)).to_array());

                    // Set Shader Alpha
                    shader.SetUniform1(gl, "Alpha", alphaColor);

                    // Set Material Colors
                    shader.SetUniform3(gl, "AmbientMaterial", ambientMaterialColor.x, ambientMaterialColor.y,
                        ambientMaterialColor.z);
                    shader.SetUniform3(gl, "DiffuseMaterial", diffuseMaterialColor.x, diffuseMaterialColor.y,
                        diffuseMaterialColor.z);
                    shader.SetUniform3(gl, "SpecularMaterial", specularMaterialColor.x, specularMaterialColor.y,
                        specularMaterialColor.z);

                    // Set Shader Shininess
                    shader.SetUniform1(gl, "Shininess", (float)((MainWindow)Application.Current.MainWindow).shininessValue.Value);

                    // Set up vertex buffer array
                    var vertexBufferArray = meshVertexBufferArray;
                    gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBufferArray.Value.VertexBufferArrayObject);

                    // Find drawing mode by total indices to a face
                    uint mode = OpenGL.GL_TRIANGLES;
                    if (myOBJ.IndicesPerFace == 4)
                        mode = OpenGL.GL_QUADS;
                    else if (myOBJ.IndicesPerFace > 4)
                        mode = OpenGL.GL_POLYGON;

                    // Draw the arrays of data for the model
                    gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);

                    // Unbind the shader from OpenGL
                    shader.Unbind(gl);
                }
            }
        }

        /// <summary>
        /// Renders the scene with a texture along with colours for ADS
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="useToonShader"></param>
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
                shader.SetUniformMatrix4(gl, "Projection", myCamera.m_projectionMx.to_array());
                shader.SetUniformMatrix4(gl, "Modelview", (myCamera.m_worldViewMx * myOBJ.m_modelWorldMx).to_array());

                // Set up normal matrix
                shader.SetUniformMatrix4(gl, "NormalMatrix", CustomMath.TransposeMatrix(glm.inverse(myCamera.m_worldViewMx)).to_array());

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

                    ((MainWindow)System.Windows.Application.Current.MainWindow).UsingTexture.IsChecked = true;

                }

                uint mode = OpenGL.GL_TRIANGLES;
                if (myOBJ.IndicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (myOBJ.IndicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);

                if (texture != null)
                    texture.Unbind(gl);

                shader.Unbind(gl);
            }
            
        }

        /// <summary>
        /// Renders the scene with a texture along with colours for ADS
        /// </summary>
        /// <param name="gl"></param>
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

                // Set up normal matrix
                shader.SetUniformMatrix4(gl, "NormalMatrix", CustomMath.TransposeMatrix(glm.inverse(myCamera.m_worldViewMx)).to_array());


                // Set shader alpha
                shader.SetUniform1(gl, "Alpha", alphaColor);

                // Set shader time
                shader.SetUniform1(gl, "Time", time);

                // Set speed, amplitude and frequency
                shader.SetUniform1(gl, "Speed", speed);
                shader.SetUniform1(gl, "Amplitude", amplitude);
                shader.SetUniform1(gl, "Frequency", frequency);

                shader.SetUniform3(gl, "AmbientMaterial", ambientMaterialColor.x, ambientMaterialColor.y,
                    ambientMaterialColor.z);
                shader.SetUniform3(gl, "DiffuseMaterial", diffuseMaterialColor.x, diffuseMaterialColor.y,
                    diffuseMaterialColor.z);
                shader.SetUniform3(gl, "SpecularMaterial", specularMaterialColor.x, specularMaterialColor.y,
                    specularMaterialColor.z);
                shader.SetUniform1(gl, "Shininess", (float)((MainWindow)System.Windows.Application.Current.MainWindow).shininessValue.Value);

                var vertexBufferArray = meshVertexBufferArray;
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vertexBufferArray.Value.VertexBufferArrayObject);

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

                }

                uint mode = OpenGL.GL_TRIANGLES;
                if (myOBJ.IndicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (myOBJ.IndicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);


                if (texture != null)
                    texture.Unbind(gl);

                shader.Unbind(gl);
            }

        }

        /// <summary>
        /// Renders the scene using the custom settings within the application
        /// </summary>
        /// <param name="gl"></param>
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

                time += 5.0f / (float)((MainWindow)Application.Current.MainWindow).openGlCtrl.FrameRate; /* 30.0f*/;


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

                // Set up normal matrix
                shader.SetUniformMatrix4(gl, "NormalMatrix", CustomMath.TransposeMatrix(glm.inverse(myCamera.m_worldViewMx)).to_array());

                // Set shader alpha
                shader.SetUniform1(gl, "Alpha", alphaColor);

                // Set shader time
                shader.SetUniform1(gl, "Time", time);

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

                uint mode = OpenGL.GL_TRIANGLES;
                if (myOBJ.IndicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (myOBJ.IndicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                gl.DrawArrays(mode, 0, myOBJ.VertexList.Count);

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
            } catch(Exception)
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
            

            if (newObj.NormalList.Count > 0)
            {
                var normalsVertexBuffer = new VertexBuffer();
                normalsVertexBuffer.Create(gl);
                normalsVertexBuffer.Bind(gl);
                normalsVertexBuffer.SetData(gl, VertexAttributes.Normal,
                                            newObj.ToFloatArrayNormal(),
                                            false, 3);

            }

            
            if (newObj.TextureList.Count > 0)
            {
                var texCoordsVertexBuffer = new VertexBuffer();
                texCoordsVertexBuffer.Create(gl);
                texCoordsVertexBuffer.Bind(gl);
                texCoordsVertexBuffer.SetData(gl, VertexAttributes.TexCoord,
                                              newObj.ToFloatArrayTexture(),
                                              false, 2);

            }

            verticesVertexBuffer.Unbind(gl);
            //gl.
            meshVertexBufferArray = new KeyValuePair<Obj, VertexBufferArray>(meshVertexBufferArray.Key, vertexBufferArray);

           
            return 0;
        }

        /// <summary>
        /// Loads an .obj files into the scene
        /// </summary>
        /// <param name="openGLContext"></param>
        /// <param name="objectFilePath"></param>
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

        /// <summary>
        /// Adds a texture to the gl context and the program
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="image"></param>
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
            } catch (Exception)
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