using OpenTK;

using Senapp.Engine.Entities;
using Senapp.Engine.Utilities;
using System.Reflection;

namespace Senapp.Engine.Shaders.Components
{
    public class EntityShader : ShaderProgram
    {
        private const string VERTEX_SHADER_FILE = "entityVS";
        private const string FRAGMENT_SHADER_FILE = "entityFS";
        private const bool FROM_RESOURCES = true;

        private int location_transformationMatrix;
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int location_shineDamper;
        private int location_reflectivity;
        private int location_luminosity;
        private int location_useFakeLighting;
        private int location_colour;
        private int location_isMultisample;
        private int location_instanceId;

        public EntityShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE, FROM_RESOURCES) { }

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
            location_useFakeLighting = base.GetUniformLocation("useFakeLighting");
            location_isMultisample = base.GetUniformLocation("isMultisample");

            // FS
            location_shineDamper = base.GetUniformLocation("shineDamper");
            location_reflectivity = base.GetUniformLocation("reflectivity");
            location_luminosity = base.GetUniformLocation("luminosity");
            location_colour = base.GetUniformLocation("colour");
            location_instanceId = base.GetUniformLocation("instanceId");
        }

        public void LoadColour(Vector3 colour)
        {
            base.LoadVector(location_colour, colour);
        }
        public void LoadUseFakeLightingVariable(bool useFake)
        {
            base.LoadBoolean(location_useFakeLighting, useFake);
        }
        public void LoadShineVariables(float damper, float reflectivity, float luminosity)
        {
            base.LoadFloat(location_shineDamper, damper);
            base.LoadFloat(location_reflectivity, reflectivity);
            base.LoadFloat(location_luminosity, luminosity);
        }
        public void LoadTransformationMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_transformationMatrix, matrix);
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
        public void LoadIsMultisample(bool isMultisample)
        {
            base.LoadBoolean(location_isMultisample, isMultisample);
        }
        public void LoadInstanceId(int instanceId)
        {
            base.LoadInt(location_instanceId, instanceId);
        }
    }
}
