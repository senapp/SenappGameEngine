using OpenTK;
using Senapp.Engine.Entities;
using Senapp.Engine.UI;

namespace Senapp.Engine.Shaders
{
    public class UIShader : ShaderProgram
    {
        private static readonly string VERTEX_SHADER_FILE = "Resources/Shaders/UIvertexShader.glsl";
        private static readonly string FRAGMENT_SHADER_FILE = "Resources/Shaders/UIfragmentShader.glsl";

        private int location_transformationMatrix;
        private int location_viewMatrix;
        private int location_projectionMatrix;
        private int location_colour;
        private int location_colourEdge;
        private int location_colourMixRatio;
        private int location_edgeRatio;
        private int location_roundnessRatio;
        private int location_screenSize;



        public UIShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE) { }
        public UIShader(string vertexShader, string fragmentShader) : base(vertexShader, fragmentShader) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
            base.BindAttribute(1, "textureCoords");
        }

        protected override void GetAllUniformLocations()
        {
            location_transformationMatrix = base.GetUniformLocation("transformationMatrix");
            location_viewMatrix = base.GetUniformLocation("viewMatrix");
            location_projectionMatrix = base.GetUniformLocation("projectionMatrix");
            location_colour = base.GetUniformLocation("colour");
            location_colourEdge = base.GetUniformLocation("colourEdge");
            location_colourMixRatio = base.GetUniformLocation("colourMixRatio");
            location_edgeRatio = base.GetUniformLocation("edgeRatio");
            location_roundnessRatio = base.GetUniformLocation("roundnessRatio");
            location_screenSize = base.GetUniformLocation("screenSize");
        }

        public void LoadTransformationMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_transformationMatrix, matrix);
        }
        public void LoadSprite(Sprite sprite)
        {
            base.LoadVector(location_colour, sprite.colour);
            base.LoadVector(location_colourEdge, sprite.colourEdge);
            base.LoadFloat(location_colourMixRatio, sprite.colourMixRatio);
            base.LoadFloat(location_edgeRatio, sprite.edgeRatio);
            base.LoadFloat(location_roundnessRatio, sprite.roundnessRatio);
            base.LoadVector(location_screenSize, new Vector2(Game.Instance.Width, Game.Instance.Height));
        }

        public void LoadViewMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_viewMatrix, matrix);
        }
        public void LoadProjectionMatrix(Matrix4 matrix)
        {
            base.LoadMatrix(location_projectionMatrix, matrix);
        }


        public void UpdateCamera(Camera camera, Transform transform)
        {
            LoadViewMatrix(camera.GetViewMatrixUI(transform));
            LoadProjectionMatrix(camera.GetProjectionMatrixUI());
        }
    }
}
