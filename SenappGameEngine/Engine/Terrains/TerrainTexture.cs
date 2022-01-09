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

        public TerrainTexture(Texture _backgroundTexture, Texture _rTexture, Texture _gTexture, Texture _bTexture, Texture _blendMap)
        {
            backgroundTexture = _backgroundTexture;
            rTexture = _rTexture;
            gTexture = _gTexture;
            bTexture = _bTexture;
            blendMap = _blendMap;
        }
        public TerrainTexture(string _backgroundTexture, string _rTexture, string _gTexture, string _bTexture, string _blendMap)
        {
            backgroundTexture = Loader.LoadTexture(_backgroundTexture);
            rTexture = Loader.LoadTexture(_rTexture);
            gTexture = Loader.LoadTexture(_gTexture);
            bTexture = Loader.LoadTexture(_bTexture);
            blendMap = Loader.LoadTexture(_blendMap);
        }
    }
}
