using Senapp.Engine.Loaders;
using Senapp.Engine.Models;

namespace Senapp.Engine.Terrains
{
    public class TerrainTexture
    {
        public Texture backgroundTexture;
        public Texture rTexture;
        public Texture gTexture;
        public Texture bTexture;
        public Texture blendMap;

        public TerrainTexture(Texture backgroundTexture, Texture rTexture, Texture gTexture, Texture bTexture, Texture blendMap)
        {
            this.backgroundTexture = backgroundTexture;
            this.rTexture = rTexture;
            this.gTexture = gTexture;
            this.bTexture = bTexture;
            this.blendMap = blendMap;
        }
        public TerrainTexture(string backgroundTexture, string rTexture, string gTexture, string bTexture, string blendMap)
        {
            this.backgroundTexture = Loader.LoadTexture(backgroundTexture);
            this.rTexture = Loader.LoadTexture(rTexture);
            this.gTexture = Loader.LoadTexture(gTexture);
            this.bTexture = Loader.LoadTexture(bTexture);
            this.blendMap = Loader.LoadTexture(blendMap);
        }
    }
}
