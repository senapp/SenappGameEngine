using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using Senapp.Engine.Base;
using System;

namespace Senapp.Engine.Entities
{
    public class Entity : Component
    {
        public TexturedModel model;

        public Entity() { }
        public Entity(TexturedModel texturedModel)
        {
            model = texturedModel;

        }
        public Entity(Geometries geometry, string textureFileName = null)
        {
            RawModel rawModel = Loader.LoadToVAO(Geometry.GetVertex(geometry));
            model = new TexturedModel(rawModel, Loader.LoadTexture(textureFileName));
        }
        public Entity(string objectFileName = null, string textureFileName = null)
        {
            RawModel rawModel = Loader.LoadToVAO(OBJLoader.LoadOBJModel(objectFileName));
            model = new TexturedModel(rawModel, Loader.LoadTexture(textureFileName));
        }
        public Entity(RawModel rawModel, string textureFileName = null)
        {
            model = new TexturedModel(rawModel, Loader.LoadTexture(textureFileName));
        }
    }
}
