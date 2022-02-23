using Senapp.Engine.Models;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Loaders;
using Senapp.Engine.Loaders.Models;
using Senapp.Engine.Renderer.Helper;

namespace Senapp.Engine.Entities
{
    public class Entity : Component
    {
        public TexturedModel model;

        public Entity(Geometries geometry, string texturePath = "", bool fromResources = true) 
            : this(Loader.LoadGeometry(geometry, texturePath, fromResources)) { }
        public Entity(string modelPath = "", ModelTypes modelType = ModelTypes.OBJ, string texturePath = "", bool fromResources = true) 
            : this(Loader.LoadModel(modelPath, modelType, texturePath, fromResources)) { }
        public Entity(RawModel rawModel, string textureFileName = "", bool loadFromResources = true) 
            : this(new TexturedModel(rawModel, Loader.LoadTexture(textureFileName, loadFromResources))) { }
        public Entity(TexturedModel model)
        {
            this.model = model;
        }
        public Entity() { }

        public Entity WithLuminosity(float luminosity)
        {
            model.luminosity = luminosity;
            return this;
        }
        public Entity WithShineDamper(float shineDamper)
        {
            model.shineDamper = shineDamper;
            return this;
        }
        public Entity WithReflectivity(float reflectivity)
        {
            model.reflectivity = reflectivity;
            return this;
        }
        public Entity WithLightable(bool lightable)
        {
            model.lightable = lightable;
            return this;
        }
    }
}
