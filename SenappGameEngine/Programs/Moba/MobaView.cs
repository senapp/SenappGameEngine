using System.Drawing;

using OpenTK;

using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Loaders.Models;
using Senapp.Engine.Renderer.Helper;

namespace Senapp.Programs.Moba
{
    public class MobaView : Component
    {
        public MobaWorld MobaWorld;
        public Entity Lux;
        public GameObject player;
        public GameObject targetMarker;

        public MobaView() 
        {
            MobaWorld = new MobaWorld();
            Lux = new Entity("lux", ModelTypes.OBJ, "lux_tex");
        }

        public override void Awake()
        {
            MobaWorld = new GameObject()
                .WithParent(gameObject)
                .WithName("Moba World")
                .AddComponent(MobaWorld);

            targetMarker = new GameObject()
                .WithParent(gameObject)
                .WithName("Marker")
                .WithScale(new Vector3(0.4f))
                .WithRotation(new Vector3(-90, 0, 0))
                .WithColour(Color.LightYellow)
                .WithComponent(new Entity(Geometries.Quad, "marker").WithLuminosity(1));


            Lux = new GameObject()
                .WithParent(gameObject)
                .WithName("Lux")
                .WithComponent(new MobaPlayerController(targetMarker))
                .AddComponent(Lux);
        }
    }
}
