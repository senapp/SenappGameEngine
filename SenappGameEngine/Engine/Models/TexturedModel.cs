using OpenTK.Graphics.OpenGL4;

namespace Senapp.Engine.Models
{
    public class TexturedModel
    {
        public RawModel rawModel;
        public Texture texture;
        public float shineDamper = 1;
        public float reflectivity = 0.01f;
        public float luminosity = 0.3f;

        public bool hasTransparency = false;
        public bool useFakeLighting = false;

        public TexturedModel(RawModel model, Texture tex)
        {
            rawModel = model;
            texture = tex;
        }
        public void BindTexture(TextureUnit textureUnit)
        {
            texture.Bind(textureUnit);
        }
        public void Dispose()
        {
            Loader.DisposeModelAndTexture(this);
        }
    }
}
