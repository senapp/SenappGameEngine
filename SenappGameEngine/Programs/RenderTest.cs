using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using Senapp.Engine.Core;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.UI;
using Senapp.Engine.Utilities;

namespace Senapp.Programs
{
    public class RenderTest : Game
    {
        public RenderTest(GraphicsMode GRAPHICS_MODE) : base(WIDTH, HEIGHT, GRAPHICS_MODE, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameUpdatedEvent += Update;
            Run();
        }

        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "Render Test";

        public GameFont font = new();

        private void Initialize(object sender)
        {
            Icon = Resources.GetIcon("new_icon");
            VSync = VSyncMode.On;

            font.LoadFont("opensans");

            var sphere = new GameObject()
                .WithParent(MainScene)
                .WithPosition(new Vector3(0, 0, -5))
                .AddComponent(new Entity(Geometries.Sphere));
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;

            if (Input.GetKeyDown(Key.Q))
            {
                Renderer.finalRenderer.ColourAttachmentId++;
            }
        }
    }
}
