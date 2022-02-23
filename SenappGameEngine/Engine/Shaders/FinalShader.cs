namespace Senapp.Engine.Shaders
{
    public class FinalShader : ShaderProgram
    {
        private const string VERTEX_SHADER_FILE = "finalVS";
        private const string FRAGMENT_SHADER_FILE = "finalFS";
        private const bool FROM_RESOURCES = true;

        public FinalShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE, FROM_RESOURCES) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations() { }
    }
}
