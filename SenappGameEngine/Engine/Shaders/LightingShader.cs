using OpenTK;

using Senapp.Engine.Entities;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Shaders
{
    public class LightingShader : ShaderProgram
    {
        private const string VERTEX_SHADER_FILE = "lightingVS";
        private const string FRAGMENT_SHADER_FILE = "lightingFS";
        private const bool FROM_RESOURCES = true;

        private int location_colourTexture;
        private int location_normalTexture;
        private int location_positionTexture;
        private int location_modelsTexture;
        private int location_lightPosition;
        private int location_lightColour;
        private int location_enviroMap;
        private int location_cameraPosition;

        public LightingShader() : base(VERTEX_SHADER_FILE, FRAGMENT_SHADER_FILE, FROM_RESOURCES) { }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations()
        {
            // FS
            location_lightPosition = base.GetUniformLocation("lightPosition");
            location_lightColour = base.GetUniformLocation("lightColour");
            location_cameraPosition = base.GetUniformLocation("cameraPosition");
            location_enviroMap = base.GetUniformLocation("enviroMap");
            location_colourTexture = base.GetUniformLocation("colourTexture");
            location_normalTexture = base.GetUniformLocation("normalTexture");
            location_positionTexture = base.GetUniformLocation("positionTexture");
            location_modelsTexture = base.GetUniformLocation("modelsTexture");
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
        public void LoadEnviromentMap(int textureID)
        {
            base.LoadInt(location_enviroMap, textureID);
        }
        public void UpdateCamera(Camera camera)
        {
            LoadCameraPosition(camera.gameObject.transform.GetWorldPosition());
        }
        public void LoadTextureUnits()
        {
            base.LoadInt(location_colourTexture, 0);
            base.LoadInt(location_normalTexture, 1);
            base.LoadInt(location_positionTexture, 2);
            base.LoadInt(location_modelsTexture, 3);
        }
    }
}
