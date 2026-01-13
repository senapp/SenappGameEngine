using OpenTK.Input;
using OpenTK;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities.Testing;
using Senapp.Engine.Entities;
using Senapp.Engine.Core.GameObjects;
using System.Drawing;
using Senapp.Engine.Core;
using System.Text;
using System.IO;
using System.Linq;

namespace Senapp.Engine.Utilities
{
    public class Profiler : Component
    {
        Text text1;
        Text text2;
        Text text3;
        Text text4;
        Text text5;
        Text text6;

        Sprite ProfilerScreen;

        public Profiler() : base(true) { }

        public override void Awake()
        {
            ProfilerScreen = new GameObjectUI()
              .WithParent(Game.Instance.MainScene)
              .WithName("Profiler")
              .WithColour(Color.Black)
              .WithScale(new Vector3(0.6f, 0.4f, 0.5f))
              .WithPosition(new Vector3(0, 10, 0))
              .AddComponent(new Sprite());

            ProfilerScreen.UIConstriant = UIPosition.TopRight;

            var tempText = new GameObjectUI()
            .WithParent(ProfilerScreen.gameObject)
            .WithName("Profiler_Static_Statistics")
            .WithPosition(new Vector3(44, 5f, 0))
                .AddComponent(new Text("Statistics", Game.Instance.MainFont, 4));

            var tempText2 = new GameObjectUI()
            .WithParent(ProfilerScreen.gameObject)
            .WithName("Profiler_Static_Graphics")
            .WithPosition(new Vector3(22, 0f, 0))
               .AddComponent(new Text("Graphics:", Game.Instance.MainFont, 4));

            text1 = new GameObjectUI()
            .WithParent(ProfilerScreen.gameObject)
            .WithName("Profiler_Dynamic_FPS")
            .WithPosition(new Vector3(60, 0f, 0))
                .AddComponent(new Text("FPS", Game.Instance.MainFont, 4));

            text2 = new GameObjectUI()
            .WithParent(ProfilerScreen.gameObject)
            .WithName("Profiler_Dynamic_ScreenSize")
            .WithPosition(new Vector3(25, -3f, 0))
                .AddComponent(new Text("Screen: ", Game.Instance.MainFont, 4));

            text3 = new GameObjectUI()
            .WithParent(ProfilerScreen.gameObject)
            .WithName("Profiler_Dynamic_GameObjectsAll")
            .WithPosition(new Vector3(25, -6f, 0))
              .AddComponent(new Text("GameObjects - All: ", Game.Instance.MainFont, 4));

            text4 = new GameObjectUI()
            .WithParent(ProfilerScreen.gameObject)
            .WithName("Profiler_Dynamic_GameObjectsScene")
            .WithPosition(new Vector3(25, -9f, 0))
              .AddComponent(new Text("GameObjects - Scene: ", Game.Instance.MainFont, 4));

            text5 = new GameObjectUI()
           .WithParent(ProfilerScreen.gameObject)
           .WithName("Profiler_Dynamic_ComponentsAll")
           .WithPosition(new Vector3(25, -12f, 0))
             .AddComponent(new Text("Components - All: ", Game.Instance.MainFont, 4));

            text6 = new GameObjectUI()
          .WithParent(ProfilerScreen.gameObject)
          .WithName("Profiler_Dynamic_ComponentsDynamic")
          .WithPosition(new Vector3(25, -15f, 0))
            .AddComponent(new Text("Components - Dynamic: ", Game.Instance.MainFont, 4));
        }

        private void DebugScreenTexts()
        {
            var gameObjects = Game.Instance.GetAllGameObjects();
            var sceneObjects = Game.Instance.GetSceneAllGameObjects();
            var components = gameObjects.SelectMany(x => x.ComponentManager.GetComponents().Values).ToList();
            var dynamicComponents = gameObjects.SelectMany(x => x.ComponentManager.GetDynamicComponents().Values).ToList();

            text1.UpdateText("FPS: " + FrameRate.FPS.ToString());
            text2.UpdateText($"Screen: {Game.Instance.Width}x{Game.Instance.Height}");
            text3.UpdateText($"GameObjects - All: {gameObjects.Where(x => x.enabled).Count()} / {gameObjects.Count} Enabled");
            text4.UpdateText($"GameObjects - Scene: {sceneObjects.Where(x => x.enabled).Count()} / {sceneObjects.Count} Enabled");
            text5.UpdateText($"Components - All: {components.Where(x => x.enabled).Count()} / {components.Count} Enabled");
            text6.UpdateText($"Components - Dynamic: {dynamicComponents.Where(x => x.enabled).Count()} / {dynamicComponents.Count} Enabled");
        }

        private void PrintTree()
        {
            var gameObjectsAll = Game.Instance.GetAllGameObjects();

            StringBuilder sb = new StringBuilder();
            foreach (var gameObject in gameObjectsAll)
            {
                sb.AppendLine(gameObject.name);
                foreach (var child in gameObject.Children.Values)
                {
                    searchChild(child, sb, 1);
                }
            }
            File.WriteAllText("C:\\Users\\albin\\Documents\\Test\\GameObjects-All.txt", sb.ToString());

            var gameObjectsScene = Game.Instance.GetAllGameObjects();

            StringBuilder sb2 = new StringBuilder();
            foreach (var gameObject in gameObjectsScene)
            {
                sb2.AppendLine(gameObject.name);
                foreach (var child in gameObject.Children.Values)
                {
                    searchChild(child, sb2, 1);
                }
            }
            File.WriteAllText("C:\\Users\\albin\\Documents\\Test\\GameObjects-Scene.txt", sb2.ToString());
        }

        private void searchChild(GameObject gameObject, StringBuilder sb, int step)
        {
            string temp = "";
            for (int i = 0; i < step; i++)
            {
                temp += "-";
            }
            sb.AppendLine(temp + gameObject.name);
            foreach (var child in gameObject.Children.Values)
            {
                searchChild(child, sb, step + 1);
            }
        }

        public override void Update(GameUpdatedEventArgs args)
        {
            DebugScreenTexts();

            if (Input.GetKeyDown(Key.Escape) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.Back))
            {
                Game.Instance.Exit();
            }
            if (Input.GetKeyDown(Key.Z) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadUp))
            {
                WireFrame.Enable(!WireFrame.IsEnabled());
            }
            if (Input.GetKeyDown(Key.Q) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadLeft))
            {
                ProfilerScreen.gameObject.enabled = !ProfilerScreen.gameObject.enabled;
            }
            if (Input.GetKeyDown(Key.V) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadRight))
            {
                if (Game.Instance.VSync == VSyncMode.Off) Game.Instance.VSync = VSyncMode.On;
                else Game.Instance.VSync = VSyncMode.Off;
            }
            if (Input.GetKeyDown(Key.P))
            {
                PrintTree();
            }
        }
    }
}
