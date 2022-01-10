using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using Senapp.Engine.Base;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.Physics;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Renderer;
using Senapp.Engine.Terrains;
using Senapp.Engine.UI;
using Senapp.Engine.Utilities;
using Senapp.Programs.Moba;

namespace Senapp.Programs
{
    public class MobaGame : Game
    {
        public MobaGame(GraphicsMode gMode) : base(WIDTH, HEIGHT, gMode, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameUpdatedEvent += Update;
            Run();
        }

        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "League Of Legends";

        public MobaUI MobaUI;
        public GameFont font = new GameFont();

        private void Initialize(object sender)
        {
            Icon = Resources.GetIcon("new_icon");
            VSync = VSyncMode.On;

            font.LoadFont("opensans");

            MobaUI = new GameObject()
                .WithName("Moba UI")
                .WithParent(MainScene)
                .AddComponent(new MobaUI(font));
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;
        }
    }
}
