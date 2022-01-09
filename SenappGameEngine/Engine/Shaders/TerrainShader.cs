using OpenTK;

using Senapp.Engine.Entities;

namespace Senapp.Engine.Shaders
{
    public class TerrainShader : ShaderProgram
    {
        private static readonly string VERTEX_SHADER_FILE = "terrainVertexShader";
        private static readonly string FRAGMENT_SHADER_FILE = "terrainFragmentShader";

        private int location_transformationMatrix;
        private int location_projectionMatrix;
        private int location_viewMatrix;
        private int location_lightPosition;
        private int location_lightColour;
        private int location_shineDamper;
        private int location_reflectivity;
        private int location_backgroundTexture;
        private int location_rTexture;
        private int location_gTexture;
        private int location_bTexture;
        private int location_blendMap;
        
        public TerrainShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
            base.BindAttribute(1, "textureCoords");
            base.BindAttribute(2, "normal");

        }

        protected override void GetAllUniformLocations()
        {
            location_transformationMatrix = base.GetUniformLocation("transformationMatrix");
            location_projectionMatrix = base.GetUniformLocation("projectionMatrix");
            location_viewMatrix = base.GetUniformLocation("viewMatrix");
            location_lightPosition = base.GetUniformLocation("lightPosition");
            location_lightColour = base.GetUniformLocation("lightColour");
            location_shineDamper = base.GetUniformLocation("shineDamper");
            location_reflectivity = base.GetUniformLocation("reflectivity");
            location_backgroundTexture = base.GetUniformLocation("backgroundTexture");
            location_rTexture = base.GetUniformLocation("rTexture");
            location_gTexture = base.GetUniformLocation("gTexture");
            location_bTexture = base.GetUniformLocation("bTexture");
            location_blendMap = base.GetUniformLocation("blendMap");

        }
        public void LoadTextureUnits()
        {
            base.LoadInt(location_backgroundTexture, 0);
            base.LoadInt(location_rTexture, 1);
            base.LoadInt(location_gTexture, 2);
            base.LoadInt(location_bTexture, 3);
            base.LoadInt(location_blendMap, 4);

        }
        public void LoadShineVariables(float damper, float reflectivity)
        {
            base.LoadFloat(location_shineDamper, damper);
            base.LoadFloat(location_reflectivity, reflectivity);

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
        public void LoadLight(Light light)
        {
            base.LoadVector(location_lightPosition, light.gameObject.transform.position);
            base.LoadVector(location_lightColour, light.gameObject.colour);

        }

        public void UpdateCamera(Camera camera)
        {
            LoadProjectionMatrix(camera.GetProjectionMatrix());
            LoadViewMatrix(camera.GetViewMatrix());
        }
    }
}
