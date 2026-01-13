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
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;
using Senapp.Engine.Utilities.Testing;
using System;
using System.Drawing;
using System.Linq;

namespace Senapp.Programs
{
    public class GrassTest : Game
    {
        public GrassTest(GraphicsMode GRAPHICS_MODE) : base(WIDTH, HEIGHT, GRAPHICS_MODE, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameUpdatedEvent += Update;
            Run();
        }

        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "Grass Test";
        private void Initialize(object sender)
        {
            Icon = Resources.GetIcon("new_icon");
            VSync = VSyncMode.On;
            MainLight.gameObject.transform.SetPosition(new Vector3(0, 20f, 20f));

            MainCamera.gameObject.transform.SetPosition(new Vector3(0, 5, 5));
            MainCamera.gameObject.transform.SetRotation(new Vector3(-35, 0, 0));

            var sd = new GameObject().WithParent(MainScene).AddComponent(new Entity(Geometries.Sphere));

            int size = 20;
            terrain = new GameObject()
                .WithName("Terrain")
                .WithParent(MainScene)
                .WithPosition(new Vector3(-size / 2, 0, -size / 2))
                .AddComponent(new Entity(Terrain.GenerateTerrain(size, tiling: 3), "terrain"));

            float grassSize = 10f;
            GrassBed = new GameObject()
                .WithName("GrassBed")
                .WithParent(MainScene)
                .WithPosition(new Vector3(-grassSize / 2f, 0.01f, -grassSize / 2f))
                .WithColour(Color.FromArgb((int)1 * 255, (int)(0.025f * 255), (int)(0.1f * 255), (int)(0.005f * 255)))
                .AddComponent(new Entity(Terrain.GenerateTerrain(grassSize, tiling: 1)))
                .WithLuminosity(0)
                .WithReflectivity(0)
                .WithShineDamper(1)
                .gameObject;

            new GameObject().WithParent(MainScene).WithName("ProfilerContainer").AddComponent(new Profiler());
        }

        Entity terrain;

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            Title = FrameRate.FPS.ToString();
            if (!Focused)
                return;
        }
    }
}
