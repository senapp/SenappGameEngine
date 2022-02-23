using System.Drawing;
using OpenTK;
using OpenTK.Input;

using Senapp.Engine.Core;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.Raycasts;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Combinations;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;
using Senapp.Engine.Utilities.Testing;

namespace Senapp.Programs.Moba
{
    public class MobaMainMenu : ComponentUI
    {
        public static MobaMainMenu Instance;

        public Sprite Background;
        public Sprite Toolbar;
        public Text Title;
        public Text GameObjectsText;
        public Text FPSText;
        public Text ScenesText;
        public TextButton MinimizeButton;
        public TextButton CloseButton;
        public InputField IpAddressField;

        public MobaMainMenu() { }
        public MobaMainMenu(GameFont font)
        {
            this.font = font;
            Instance = this;
        }

        public override void Awake()
        {
            Background = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("Background")
                .WithScale(new Vector3(2.5f, 1.5f, 0))
                .WithColour(new Vector3(0.1f).ToColour())
                .AddComponent(new Sprite()
                    .WithSortingLayer(99));

            Background.gameObject.AddComponent(new RaycastTargetUI());

            Toolbar = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("Toolbar")
                .WithPosition(new Vector3(0, 65, 0))
                .WithColour(new Vector3(0.4f, 0.1f, 0.1f).ToColour())
                .AddComponent(new Sprite()
                    .WithSize(new Vector2(2.5f, 0.2f))
                    .WithSortingLayer(99));

            Title = new GameObjectUI()
                .WithParent(Toolbar.gameObject)
                .WithName("Title")
                .AddComponent(new Text("League Of Legends", font, 15)
                    .WithSortingLayer(99));

            FPSText = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("FPSText")
                .WithPosition(new Vector3(-70, 45, 0))
                .WithComponent(new Text("GameObjects: ", font, 10)
                    .WithSortingLayer(99))
                .WithComponent(new RaycastTargetUI(onEnter: () => FPSText.gameObject.colour = Color.Red, onExit: () => FPSText.gameObject.colour = Color.White))
                .GetComponent<Text>();

            GameObjectsText = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("GameObjectsText")
                .WithPosition(new Vector3(-70, 37.5f, 0))
                .WithComponent(new Text("GameObjects: ", font, 10)
                    .WithSortingLayer(99))
                .WithComponent(new RaycastTargetUI(onEnter: () => GameObjectsText.gameObject.colour = Color.Red, onExit: () => GameObjectsText.gameObject.colour = Color.White))
                .GetComponent<Text>();

            ScenesText = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("ScenesText")
                .WithPosition(new Vector3(-70, 30, 0))
                .WithComponent(new Text("Scenes: ", font, 10)
                    .WithSortingLayer(99))
                .WithComponent(new RaycastTargetUI(onEnter: () => ScenesText.gameObject.colour = Color.Red, onExit: () => ScenesText.gameObject.colour = Color.White))
                .GetComponent<Text>();

            MinimizeButton = new GameObjectUI()
                .WithParent(Toolbar.gameObject)
                .WithName("Minimize Button")
                .WithPosition(new Vector3(100, 0, 0))
                .AddComponent(new TextButton("_", font,
                    onEnter: () => MinimizeButton.SetColour(Color.White, Color.DarkOliveGreen),
                    onExit: () => MinimizeButton.SetColour(Color.White, Color.Green),
                    onClick: () => gameObject.enabled = false)
                    .WithSortingLayer(99));


            MinimizeButton.SetColour(Color.White, Color.Green);
            MinimizeButton.SetSize(0.4f, new Vector2(0.25f, 0.25f));

            CloseButton = new GameObjectUI()
               .WithParent(Toolbar.gameObject)
               .WithName("Close Button")
               .WithPosition(new Vector3(115, 0, 0))
               .AddComponent(new TextButton("X", font,
                    onEnter: () => CloseButton.SetColour(Color.White, Color.Crimson),
                    onExit: () => CloseButton.SetColour(Color.White, Color.Red),
                    onClick: () => Game.Instance.Exit())
                    .WithSortingLayer(99));

            CloseButton.SetColour(Color.White, Color.Red);
            CloseButton.SetSize(0.4f, new Vector2(0.25f, 0.25f));

            IpAddressField = new GameObjectUI()
             .WithParent(gameObject)
             .WithName("Ip Address Field")
             .WithPosition(new Vector3(-82.5f, 0f, 0))
             .AddComponent(new InputField("Text...", font, Dock.Left)
             .WithSortingLayer(99));

            IpAddressField.SetSize(0.25f, new Vector2(1.5f, 0.25f));
            IpAddressField.SetColour(Color.Black, Color.White);
        }

        public override void Update(GameUpdatedEventArgs args)
        {
            if (Input.GetKeyDown(Key.V) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadDown))
            {
                Game.Instance.VSync = Game.Instance.VSync == VSyncMode.On ? VSyncMode.Off : VSyncMode.On;
            }

            GameObjectsText.UpdateText("GameObjects: " + Game.Instance.GetAllGameObjects().Count);
            FPSText.UpdateText("FPS: " + FrameRate.FPS);
            ScenesText.UpdateText("Scenes: " + Game.Instance.SceneManager.scenes.Count);
        }

        private readonly GameFont font;
    }
}
