using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Loaders;

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
            Loader.DisposeModel(rawModel);
            Loader.DisposeTexture(texture);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(TexturedModel)))
            {
                var model = (TexturedModel)obj;

                var equal = (this.texture == model.texture &&
                    this.shineDamper == model.shineDamper &&
                    this.reflectivity == model.reflectivity &&
                    this.luminosity == model.luminosity);

                return equal;
            }

            return base.Equals(obj);
        }
    }
}
