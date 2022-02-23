using OpenTK;
using System.Reflection;

namespace Senapp.Engine.Shaders
{
    public class SkyboxShader : ShaderProgram
    {
        private const string VERTEX_SHADER_FILE = "skyboxVS";
        private const string FRAGMENT_SHADER_FILE = "skyboxFS";
        private const bool FROM_RESOURCES = true;

        private int location_transformationMatrix;
        private int location_viewMatrix;
        private int location_projectionMatrix;
        private int location_isColourPass;

        public SkyboxShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE, FROM_RESOURCES) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations()
        {
            location_transformationMatrix = base.GetUniformLocation("transformationMatrix");
            location_viewMatrix = base.GetUniformLocation("viewMatrix");
            location_projectionMatrix = base.GetUniformLocation("projectionMatrix");
            location_isColourPass = base.GetUniformLocation("isColourPass");
        }

        public void LoadViewMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_viewMatrix, matrix);
        }
        public void LoadProjectionMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_projectionMatrix, matrix);
        }
        public void LoadTransformationMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_transformationMatrix, matrix);
        }
        public void LoadIsColourPass(bool isColourPass)
        {
            base.LoadBoolean(location_isColourPass, isColourPass);
        }
    }
}
