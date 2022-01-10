using OpenTK;

using Senapp.Engine.Entities;

namespace Senapp.Engine.Shaders
{
    public class SkyboxShader : ShaderProgram
    {
        private static readonly string VERTEX_SHADER_FILE = "skyboxVertexShader";
        private static readonly string FRAGMENT_SHADER_FILE = "skyboxFragmentShader";

        private int location_transformationMatrix;
        private int location_viewMatrix;
        private int location_projectionMatrix;

        public SkyboxShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE) { }
        public SkyboxShader(string vertexShader, string fragmentShader) : base(vertexShader, fragmentShader) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations()
        {
            location_transformationMatrix = base.GetUniformLocation("transformationMatrix");
            location_viewMatrix = base.GetUniformLocation("viewMatrix");
            location_projectionMatrix = base.GetUniformLocation("projectionMatrix");
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
    }
}
