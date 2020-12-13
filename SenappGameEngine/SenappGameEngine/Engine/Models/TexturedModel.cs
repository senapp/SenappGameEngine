using OpenTK.Graphics.OpenGL4;
namespace Senapp.Engine.Models
{
    public class TexturedModel
    {
        public RawModel rawModel { get; }
        public Texture texture { get; }
        public float shineDamper { get; set; }
        public float reflectivity { get; set; }
        public float luminosity { get; set; }

        public bool hasTransparency { get; set; }
        public bool useFakeLighting { get; set; }

        public TexturedModel(RawModel model, Texture tex)
        {
            rawModel = model;
            texture = tex;
            texture = tex;
            shineDamper = 10;
            reflectivity = 1;
            luminosity = 0;
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
