
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GlmNet;


namespace PSSL_Environment
{
    public class Camera
    {
        public mat4 ProjectionMx;
        public mat4 WorldViewMx;
        public float Angle;
        public float AspectRatio;
        public float ZNear;
        public float ZFar;

        const float PI_OVER_2 = (float)Math.PI / 2; //PI / 2
        public Camera()
        {
            var temp = ((MainWindow)Application.Current.MainWindow).openGlCtrl;
            WorldViewMx = mat4.identity();
            UpdatePerspective(1.0f, (float)(temp.ActualWidth / temp.ActualHeight),1f, 100.0f);
        }

        public void UpdatePerspective(float angle, float aspectRatio, float zNear, float zFar)
        {
            Angle = angle;
            AspectRatio = aspectRatio;
            ZNear = zNear;
            ZFar = zFar;

            ProjectionMx = GeneratePerspectiveMatrix(angle, aspectRatio, zNear, zFar);
        }

        public void UpdatePerspective(mat4 proj)
        {
            ProjectionMx = proj;
        }

        private mat4 GeneratePerspectiveMatrix(float angle, float aspectRatio, float zNear, float zFar)
        {
            //// Makes a mat4 perspective matrix

            float f, rangeInv;
            f = glm.tan(PI_OVER_2 - (0.5f * angle));
            rangeInv = (1.0f / (zNear / zFar));

            return new mat4(
                new vec4((f / aspectRatio), 0.0f, 0.0f, 0.0f),
                new vec4(0.0f, f, 0.0f, 0.0f),
                new vec4(0.0f, 0.0f, ((zNear + zFar) * rangeInv), -1.0f),
                new vec4(0.0f, 0.0f, (((zNear + zFar) * rangeInv) * 2.0f), 0.0f));
        } 


    }
}
