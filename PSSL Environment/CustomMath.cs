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

        //static public mat4 MultiplyMatrix(mat4 _input1, mat4 _input2)
        //{
        //    mat4 m = new mat4();
        //    for (int i = 0; i < 4; i++)
        //    {
        //        m[i][0] = (srcA->m[i][0] * srcB->m[0][0]) +
        //        (srcA->m[i][1] * srcB->m[1][0]) +
        //        (srcA->m[i][2] * srcB->m[2][0]) +
        //        (srcA->m[i][3] * srcB->m[3][0]);

        //        m[i][1] = (srcA->m[i][0] * srcB->m[0][1]) +
        //                        (srcA->m[i][1] * srcB->m[1][1]) +
        //                        (srcA->m[i][2] * srcB->m[2][1]) +
        //                        (srcA->m[i][3] * srcB->m[3][1]);

        //        m[i][2] = (srcA->m[i][0] * srcB->m[0][2]) +
        //                        (srcA->m[i][1] * srcB->m[1][2]) +
        //                        (srcA->m[i][2] * srcB->m[2][2]) +
        //                        (srcA->m[i][3] * srcB->m[3][2]);

        //        m[i][3] = (srcA->m[i][0] * srcB->m[0][3]) +
        //                        (srcA->m[i][1] * srcB->m[1][3]) +
        //                        (srcA->m[i][2] * srcB->m[2][3]) +
        //                        (srcA->m[i][3] * srcB->m[3][3]);
        //    }
        //}
    }
}
