using OpenTK;
using Senapp.Engine.Entities;
using Senapp.Engine.Utilities;
using System;

namespace Senapp.Engine.Shaders.Components
{
    public class GrassShader : ShaderProgram
    {
        private const string VERTEX_SHADER_FILE = "grassVS";
        private const string FRAGMENT_SHADER_FILE = "grassFS";
        private const bool FROM_RESOURCES = true;

        private int location_transformationMatrix;
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int location_grassX;
        private int location_grassY;
        private int location_isColourPass;
        private int location_time;
        private int location_lightPosition;
        private int location_lightColour;
        private int location_cameraPosition;
        public GrassShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE, FROM_RESOURCES) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
            base.BindAttribute(1, "textureCoords");
            base.BindAttribute(2, "normal");
        }

        protected override void GetAllUniformLocations()
        {
            // VS
            location_transformationMatrix = base.GetUniformLocation("transformationMatrix");
            location_projectionMatrix = base.GetUniformLocation("projectionMatrix");
            location_viewMatrix = base.GetUniformLocation("viewMatrix");
            location_grassX = base.GetUniformLocation("grassX");
            location_grassY = base.GetUniformLocation("grassY");
            location_time = base.GetUniformLocation("time");

            // FS
            location_isColourPass = base.GetUniformLocation("isColourPass");
            location_lightPosition = base.GetUniformLocation("lightPosition");
            location_lightColour = base.GetUniformLocation("lightColour");
            location_cameraPosition = base.GetUniformLocation("cameraPosition");
        }
        public void LoadLight(Light light)
        {
            base.LoadVector(location_lightPosition, light.gameObject.transform.GetWorldPosition());
            base.LoadVector(location_lightColour, light.gameObject.colour.ToVector());
        }
        public void LoadCameraPosition(Vector3 pos)
        {
            base.LoadVector(location_cameraPosition, pos);
        }
        public void LoadTransformationMatrix(Matrix4 transformationMatrix)
        {
            base.LoadMatrix(location_transformationMatrix, transformationMatrix);
        }
        public void LoadProjectionMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_projectionMatrix, matrix);
        }
        public void LoadViewMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_viewMatrix, matrix);
        }
        public void LoadCameraMatrix(Camera camera)
        {
            LoadProjectionMatrix(camera.GetProjectionMatrix());
            LoadViewMatrix(camera.GetViewMatrix());
        }
        public void LoadGrassGrid(int gridX, int gridY)
        {
            base.LoadInt(location_grassX, gridX);
            base.LoadInt(location_grassY, gridY);
        }
        public void LoadIsColourPass(bool isColourPass)
        {
            base.LoadBoolean(location_isColourPass, isColourPass);
        }
        public void LoadTime(float time)
        {
            base.LoadFloat(location_time, time);
        }
    }
}
