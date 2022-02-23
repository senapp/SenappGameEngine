namespace Senapp.Engine.Shaders
{
    public class FinalShader : ShaderProgram
    {
        private static readonly string VERTEX_SHADER_FILE = "C:\\Users\\albin\\Documents\\Projects\\Other\\senappGameEngine\\SenappGameEngine\\Resources\\Shaders\\" + "finalVS" + ".glsl";
        private static readonly string FRAGMENT_SHADER_FILE = "C:\\Users\\albin\\Documents\\Projects\\Other\\senappGameEngine\\SenappGameEngine\\Resources\\Shaders\\" + "finalFS" + ".glsl";
        private static readonly bool FROM_RESOURCES = false;

        public FinalShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE, FROM_RESOURCES) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations() { }
    }
}
