using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using Senapp.Engine.Base;

namespace Senapp.Engine.Entities
{
    public class Entity : Component
    {
        public TexturedModel model;

        public Entity() { }
        public Entity(TexturedModel model)
        {
            this.model = model;
        }
        public Entity(Geometries geometry, string textureFileName = null, string ext = ".png")
        {
            RawModel rawModel = Loader.LoadToVAO(Geometry.GetVertex(geometry), Geometry.GetVertexName(geometry));
            model = new TexturedModel(rawModel, Loader.LoadTexture(textureFileName, ext));
        }
        public Entity(string objectFileName = null, string textureFileName = null, string ext = ".png")
        {
            var vertex = OBJLoader.LoadOBJModel(objectFileName);
            RawModel rawModel = Loader.LoadToVAO(vertex, objectFileName);
            model = new TexturedModel(rawModel, Loader.LoadTexture(textureFileName, ext));
        }
        public Entity(RawModel rawModel, string textureFileName = null, string ext = ".png")
        {
            model = new TexturedModel(rawModel, Loader.LoadTexture(textureFileName, ext));
        }
    }
}
