using OpenTK;

using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Models;

namespace Senapp.Programs.Moba
{
    public class MobaWorld : Component
    {
        public Entity Ground;

        public MobaWorld() 
        {
            Ground = new Entity(RawModel.GenerateTerrain(TerrainSize), "map");
        }

        public override void Awake()
        {
            Ground = new GameObject()
               .WithParent(gameObject)
               .WithName("Moba World Terrain")
               .WithScale(new Vector3(1, 1, MapRatio))
               .WithPosition(new Vector3(TerrainSize / 2, 0, TerrainSize / (2 / MapRatio)))
               .AddComponent(Ground);
        }

        private const int TerrainSize = 125;

        private float MapRatio => 1068f / 2048f;
    }
}
