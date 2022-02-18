using OpenTK;

using Senapp.Engine.Renderer.PostProcessing;

namespace Senapp.Engine.Shaders.PostProcessing
{
    public class SSAOShader : ShaderProgram
    {
        private static readonly string VERTEX_SHADER_FILE = "SSAOVS";
        private static readonly string FRAGMENT_SHADER_FILE = "SSAOFS";

        private int location_projectionMatrix;
        private readonly int[] location_samples = new int[SSAO.SAMPLE_POINTS];

        public SSAOShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations() 
        {
            location_projectionMatrix = base.GetUniformLocation("projectionMatrix");
            for (int i = 0; i < SSAO.SAMPLE_POINTS; i++)
            {
                location_samples[i] = base.GetUniformLocation($"samples[{i}]");
            }
        }

        public void LoadProjectionMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_projectionMatrix, matrix);
        }
        public void LoadSample(int index, Vector3 sample)
        {
            base.LoadVector(location_samples[index], sample);
        }
    }
}
