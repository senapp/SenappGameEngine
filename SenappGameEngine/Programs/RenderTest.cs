using OpenTK;
using OpenTK.Graphics;

using Senapp.Engine.Core;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Events;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Components;
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

            var text1 = new GameObjectUI()
                .WithName("Test text1")
                .WithParent(MainScene)
                .AddComponent(new Text("Test text1", font));

            var text2 = new GameObjectUI()
              .WithName("Test text2")
              .WithPosition(new Vector3(25))
              .WithParent(MainScene)
              .AddComponent(new Text("Test text2", font));
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;
        }
    }
}
