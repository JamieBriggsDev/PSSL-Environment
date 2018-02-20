using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FileFormatWavefront.Model;
using GlmNet;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.Shaders;
using SharpGL.Textures;
using SharpGL.VertexBuffers;

namespace PSSL_Environment
{
    public static class VertexAttributes
    {
        public const uint Position = 0;
        public const uint Normal = 1;
        public const uint TexCoord = 2;
    }
    /// <summary>
    /// A class that represents the scene for this sample.
    /// </summary>
    public class Scene
    {
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
            var attributeLocations = new Dictionary<uint, string>
            {
                {positionAttribute, "Position"},
                {normalAttribute, "Normal"},
            };

            //  Create the per pixel shader.
            shaderPerPixel = new ShaderProgram();
            shaderPerPixel.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\PerPixel.frag"), attributeLocations);

            //  Create the toon shader.
            shaderToon = new ShaderProgram();
            shaderToon.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.vert"),
                ManifestResourceLoader.LoadTextFile(@"Shaders\Toon.frag"), attributeLocations);

            gl.ClearColor(1.0f, 1.0f, 0.0f, 1.0f);

            //  Immediate mode only features!
            gl.Enable(OpenGL.GL_TEXTURE_2D);
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
            mat4 rotation = glm.rotate(mat4.identity(), rotationAngle, new vec3(0, 1, 0));
            mat4 translation = glm.translate(mat4.identity(), new vec3(-1, -1, -5));
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
        public void RenderImmediateMode(OpenGL gl)
        {
            //  Setup the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.MultMatrix(modelviewMatrix.to_array());

            //  Go through each group.
            foreach (var mesh in meshes)
            {
                var texture = meshTextures.ContainsKey(mesh) ? meshTextures[mesh] : null;
                if (texture != null)
                    texture.Bind(gl);

                uint mode = OpenGL.GL_TRIANGLES;
                if (mesh.indicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (mesh.indicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                //  Render the group faces.
                gl.Begin(mode);
                for (int i = 0; i < mesh.vertices.Length; i++)
                {
                    gl.Vertex(mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z);
                    if (mesh.normals != null)
                        gl.Normal(mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z);
                    if (mesh.uvs != null)
                        gl.TexCoord(mesh.uvs[i].x, mesh.uvs[i].y);
                }
                gl.End();

                if (texture != null)
                    texture.Unbind(gl);
            }

            
        }

        // Get colors from color picker
        public vec3 ambientMaterialColor;
        public vec3 diffuseMaterialColor;
        public vec3 specularMaterialColor;
        public float alphaColor;

        /// <summary>
        /// Renders the scene in retained mode.
        /// </summary>
        /// <param name="gl">The OpenGL instance.</param>
        /// <param name="useToonShader">if set to <c>true</c> use the toon shader, otherwise use a per-pixel shader.</param>
        public void RenderRetainedMode(OpenGL gl, bool useToonShader)
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
            shader.SetUniform3(gl, "LightPosition", 0.25f, 0.25f, 10f);

            //  Set the matrices.
            shader.SetUniformMatrix4(gl, "Projection", projectionMatrix.to_array());
            shader.SetUniformMatrix4(gl, "Modelview", modelviewMatrix.to_array());
            shader.SetUniformMatrix3(gl, "NormalMatrix", normalMatrix.to_array());

            // Set shader alpha
            shader.SetUniform1(gl, "Alpha", alphaColor);

            //  Go through each mesh and render the vertex buffer array.
            foreach (var mesh in meshes)
            {
                //  If we have a material for the mesh, we'll use it. If we don't, we'll use the default material.
                if (mesh.material != null)
                {
                    //shader.SetUniform3(gl, "DiffuseMaterial", mesh.material.Diffuse.r, mesh.material.Diffuse.g, mesh.material.Diffuse.b);
                    //shader.SetUniform3(gl, "AmbientMaterial", mesh.material.Ambient.r, mesh.material.Ambient.g, mesh.material.Ambient.b);
                    //shader.SetUniform3(gl, "SpecularMaterial", mesh.material.Specular.r, mesh.material.Specular.g, mesh.material.Specular.b);
                    shader.SetUniform3(gl, "AmbientMaterial", ambientMaterialColor.x, ambientMaterialColor.y,
                        ambientMaterialColor.z);
                    shader.SetUniform3(gl, "DiffuseMaterial", diffuseMaterialColor.x, diffuseMaterialColor.y,
                        diffuseMaterialColor.z);
                    shader.SetUniform3(gl, "SpecularMaterial", specularMaterialColor.x, specularMaterialColor.y,
                        specularMaterialColor.z);
                    shader.SetUniform1(gl, "Shininess", mesh.material.Shininess);
                }
                else
                {
                    int i = 0;
                    //  TODO: we should really set a default material here.
                }
                var vertexBufferArray = meshVertexBufferArrays[mesh];
                vertexBufferArray.Bind(gl);


                //  IMPORTANT: This is interesting. If you use OpenGL 2.1, you can use quads. If you move to 3.0 or onwards, 
                //  you can only draw the triangle types - cause 3.0 onwards deprecates other types.
                //  see: http://stackoverflow.com/questions/8041361/simple-opengl-clarification
                //  this shows that the OpenGL mode selection works - if I choose 2.1 I can draw quads, otherwise I can't.
                //  There's a good article on tesselating quads to triangles here:
                //  http://prideout.net/blog/?p=49
                //  This should be a sample!


                uint mode = OpenGL.GL_TRIANGLES;
                if (mesh.indicesPerFace == 4)
                    mode = OpenGL.GL_QUADS;
                else if (mesh.indicesPerFace > 4)
                    mode = OpenGL.GL_POLYGON;

                //gl.BufferData(OpenGL.GL_ARRAY_BUFFER, mesh.vertices.Length, mesh.vertices, OpenGL.GL_STATIC_DRAW);
                gl.DrawArrays(mode, 0, mesh.vertices.Length);
            }

            shader.Unbind(gl);
        }

        private void CreateVertexBufferArray(OpenGL gl, Mesh mesh)
        {
            //  Create and bind a vertex buffer array.
            var vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            //  Create a vertex buffer for the vertices.
            var verticesVertexBuffer = new VertexBuffer();
            verticesVertexBuffer.Create(gl);
            verticesVertexBuffer.Bind(gl);
            verticesVertexBuffer.SetData(gl, VertexAttributes.Position,
                                 mesh.vertices.SelectMany(v => v.to_array()).ToArray(),
                                 false, 3);
            if (mesh.normals != null)
            {
                var normalsVertexBuffer = new VertexBuffer();
                normalsVertexBuffer.Create(gl);
                normalsVertexBuffer.Bind(gl);
                normalsVertexBuffer.SetData(gl, VertexAttributes.Normal,
                                            mesh.normals.SelectMany(v => v.to_array()).ToArray(),
                                            false, 3);
            }

            if (mesh.uvs != null)
            {
                var texCoordsVertexBuffer = new VertexBuffer();
                texCoordsVertexBuffer.Create(gl);
                texCoordsVertexBuffer.Bind(gl);
                texCoordsVertexBuffer.SetData(gl, VertexAttributes.TexCoord,
                                              mesh.uvs.SelectMany(v => v.to_array()).ToArray(),
                                              false, 2);
            }
            //  We're done creating the vertex buffer array - unbind it and add it to the dictionary.
            verticesVertexBuffer.Unbind(gl);
            meshVertexBufferArrays[mesh] = vertexBufferArray;
        }

        public void Load(OpenGL gl, string objectFilePath)
        {
            //  TODO: cleanup old files.

            //  Destroy all of the vertex buffer arrays in the meshes.
            foreach (var vertexBufferArray in meshVertexBufferArrays.Values)
                vertexBufferArray.Delete(gl);
            meshes.Clear();
            meshVertexBufferArrays.Clear();

            //  Load the object file.
            var result = FileFormatWavefront.FileFormatObj.Load(objectFilePath, true);

            meshes.AddRange(SceneDenormaliser.Denormalize(result.Model));

            //  Create a vertex buffer array for each mesh.
            meshes.ForEach(m => CreateVertexBufferArray(gl, m));

            //  Create textures for each texture map.
            CreateTextures(gl, meshes);

            //  TODO: handle errors and warnings.

            //  TODO: cleanup

        }

        private void CreateTextures(OpenGL gl, IEnumerable<Mesh> meshes)
        {
            foreach (var mesh in meshes.Where(m => m.material != null && m.material.TextureMapDiffuse != null))
            {
                //  Create a new texture and bind it.
                var texture = new Texture2D();
                texture.Create(gl);
                texture.Bind(gl);
                texture.SetParameter(gl, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                texture.SetParameter(gl, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                texture.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
                texture.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
                texture.SetImage(gl, (Bitmap)mesh.material.TextureMapDiffuse.Image);
                texture.Unbind(gl);
                meshTextures[mesh] = texture;
            }
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


        private readonly List<Mesh> meshes = new List<Mesh>();
        private readonly Dictionary<Mesh, VertexBufferArray> meshVertexBufferArrays = new Dictionary<Mesh, VertexBufferArray>();
        private readonly Dictionary<Mesh, Texture2D> meshTextures = new Dictionary<Mesh, Texture2D>();

        //  The shaders we use.
        private ShaderProgram shaderPerPixel;
        private ShaderProgram shaderToon;

        //  The modelview, projection and normal matrices.
        private mat4 modelviewMatrix = mat4.identity();
        private mat4 projectionMatrix = mat4.identity();
        private mat3 normalMatrix = mat3.identity();

        private float scaleFactor = 1.0f;

        /// <summary>
        /// Sets the scale factor automatically based on the size of the geometry.
        /// Returns the computed scale factor.
        /// </summary>
        /// <returns>The computed scale factor.</returns>
        public float SetScaleFactorAuto()
        {
            //  0.02 good for inet models.

            //  If we have no meshes, just use 1.0f.
            if (!meshes.Any())
            {
                scaleFactor = 1.0f;
                return scaleFactor;
            }

            //  Find the maximum vertex value.
            var maxX = meshes.SelectMany(m => m.vertices).AsParallel().Max(v => Math.Abs(v.x));
            var maxY = meshes.SelectMany(m => m.vertices).AsParallel().Max(v => Math.Abs(v.y));
            var maxZ = meshes.SelectMany(m => m.vertices).AsParallel().Max(v => Math.Abs(v.z));
            var max = (new[] { maxX, maxY, maxZ }).Max();

            //  Set the scale factor accordingly.
            //  sf = max/c
            scaleFactor = 2.0f / max;
            return scaleFactor;
        }
    }


}