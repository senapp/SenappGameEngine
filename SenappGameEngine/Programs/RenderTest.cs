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
        private void Initialize(object sender)
        {
            Icon = Resources.GetIcon("new_icon");
            VSync = VSyncMode.Off;

            var imageList = Resources.GetAllImages().Where(x => x.Contains("Square")).ToList();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int num = (i + j * 10);
                    var gameObject = new GameObjectUI()
                        .WithParent(MainScene)
                        .WithName("image_" + num)
                        .WithScale(new Vector3(0.2f, 0.2f, 0.4f))
                        .WithPosition(new Vector3(i * 20 - 100, j * 20 - 100, 0))
                        .AddComponent(new Sprite(imageList[Math.Min(num, imageList.Count - 1)]));
                }
            }

            MainLight.gameObject.transform.SetPosition(new Vector3(0, 2.5f, 2.5f));

            new GameObject().WithParent(MainScene).WithName("ProfilerContainer").AddComponent(new Profiler());

            var newImage = new Sprite(""); // Replace "your_new_image" with the actual image key

            // Create a new GameObjectUI for the new image
            var topLeftImage = new GameObjectUI()
                .WithParent(MainScene)
                .WithName("top_left_image")
                .WithScale(new Vector3(0.2f, 0.2f, 0.4f))
                .WithPosition(new Vector3(-WIDTH / 2f + 20, HEIGHT / 2f - 20, 0)) // Position in the top-left corner
                .AddComponent(newImage);
        }


        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;
        }
    }
}
