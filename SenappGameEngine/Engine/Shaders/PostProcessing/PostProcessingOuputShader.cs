namespace Senapp.Engine.Shaders.PostProcessing
{
    public class PostProcessingOuputShader : ShaderProgram
    {
        private static readonly string VERTEX_SHADER_FILE = "postProcessingOutputVS";
        private static readonly string FRAGMENT_SHADER_FILE = "postProcessingOutputFS";

        public PostProcessingOuputShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations() { }
    }
}
