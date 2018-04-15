using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace PSSL_Environment
{
    public class CustomMath
    {
        static public mat4 TransposeMatrix(mat4 input)
        {
            vec4 col0, col1, col2, col3;
            col0 = new vec4(input[0].x, input[1].x, input[2].x, input[3].x);
            col1 = new vec4(input[0].y, input[1].y, input[2].y, input[3].y);
            col2 = new vec4(input[0].z, input[1].z, input[2].z, input[3].z);
            col3 = new vec4(input[0].w, input[1].w, input[2].w, input[3].w);

            return new mat4(col0, col1, col2, col3);
        }
    }
}
