using SharpGL;
using SharpGL.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PSSL_Environment.UserControls
{
    public interface ConstantsInterface
    {
        void AddToShaderProgram(OpenGL gl, ShaderProgram shader);
    }
}
