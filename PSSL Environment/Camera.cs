using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlmNet;
using System.Threading.Tasks;

namespace PSSL_Environment
{
    public class Camera
    {
        public mat4 m_projectionMx;
        public mat4 m_worldViewMx;

        private float m_angle;
        private float m_aspectRatio;
        private float m_zNear;
        private float m_zFar;

        

        public Camera()
        {
            // Set up world view matrix to be an identity matrix
            m_worldViewMx = mat4.identity();
            m_worldViewMx = glm.lookAt(new vec3(0.0f, 0.0f, 0.0f), new vec3(0.0f, 0.0f, -10.0f), new vec3(0.0f, 1.0f, 0.0f));
        }

        public mat4 SetPerspective(float angle, float aspectRatio, float zNear, float zFar)
        {
            m_angle = angle;
            m_aspectRatio = aspectRatio;
            m_zNear = zNear;
            m_zFar = zFar;

            // Apply generated perspective matrix
            m_projectionMx = glm.perspective(angle, aspectRatio, zNear, zFar);

            return m_projectionMx;
        }

        public mat4 SetPerspective(mat4 input)
        {

            // Apply generated perspective matrix
            m_projectionMx = input;

            return m_projectionMx;
        }

        public void TrackLeftRight(float _amount)
        {
            m_worldViewMx = glm.translate(m_worldViewMx, new vec3(-_amount, 0.0f, 0.0f));
        }

        public void trackUpDown(float _amount)
        {
            m_worldViewMx = glm.translate(m_worldViewMx, new vec3(0.0f, -_amount, 0.0f));         
        }

        public void trackInOut(float _amount)
        {
            m_worldViewMx = glm.translate(m_worldViewMx, new vec3(0.0f, 0.0f, -_amount));
        }

        public void pitch(float _amount)
        {
            m_worldViewMx = glm.rotate(m_worldViewMx, _amount, new vec3(1.0f, 0.0f, 0.0f));
            //m_worldViewMx = Matrix4::rotationX(_amount) * m_worldViewMx;
        }

        public void roll(float _amount)
        {
            m_worldViewMx = glm.rotate(m_worldViewMx, _amount, new vec3(0.0f, 0.0f, 1.0f));
            //m_worldViewMx = Matrix4::rotationZ(_amount) * m_worldViewMx;
        }

        public void yaw(float _amount)
        {
            m_worldViewMx = glm.rotate(m_worldViewMx, _amount, new vec3(0.0f, 1.0f, 0.0f));
            //m_worldViewMx = Matrix4::rotationY(_amount) * m_worldViewMx;
        }

    }
}
