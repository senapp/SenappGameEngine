using OpenTK;
using OpenTK.Input;
using Senapp.Engine.Base;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.UI;
using Senapp.Engine.Utilities;

namespace Senapp.Programs.Moba
{
    public class MainMenu : Component
    {
        private GameFont font;

        public Sprite Background;
        public Sprite Toolbar;
        public Text Title;
        public Text GameObjectsText;
        public Text FPSText;
        public Text ScenesText;

        public MainMenu() { }
        public MainMenu(GameFont font)
        {
            this.font = font;
        }

        public override void Awake()
        {
            Background = new GameObject()
                .WithParent(gameObject)
                .WithName("Background")
                .WithScale(new Vector3(2.5f, 1.5f, 0))
                .WithColour(new Vector3(0.1f))
                .AddComponent(new Sprite());

            Toolbar = new GameObject()
                .WithParent(gameObject)
                .WithName("Toolbar")
                .WithScale(new Vector3(2.5f, 0.2f, 0))
                .WithPosition(new Vector3(0, 65, 0))
                .WithColour(new Vector3(0.4f, 0.1f, 0.1f))
                .AddComponent(new Sprite());

            Title = new GameObject()
                .WithParent(gameObject)
                .WithName("Title")
                .WithPosition(new Vector3(10, 22, 0))
                .AddComponent(new Text("League Of Legends", font, 15));

            FPSText = new GameObject()
                .WithParent(gameObject)
                .WithName("FPSText")
                .WithPosition(new Vector3(-70, 0, 0))
                .AddComponent(new Text("GameObjects: ", font, 10));

            GameObjectsText = new GameObject()
                .WithParent(gameObject)
                .WithName("GameObjectsText")
                .WithPosition(new Vector3(-70, -10, 0))
                .AddComponent(new Text("GameObjects: ", font, 10));

            ScenesText = new GameObject()
                .WithParent(gameObject)
                .WithName("ScenesText")
                .WithPosition(new Vector3(-70, -20, 0))
                .AddComponent(new Text("Scenes: ", font, 10));
        }

        public override void Update(GameUpdatedEventArgs args)
        {
            if (Input.GetKeyDown(Key.V) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.DPadDown))
            {
                Game.Instance.VSync = Game.Instance.VSync == VSyncMode.On ? VSyncMode.Off : VSyncMode.On;
            }

            GameObjectsText.UpdateText("GameObjects: " + Game.GetAllGameObjects().Count);
            FPSText.UpdateText("FPS: " + FrameRate.Get());
            ScenesText.UpdateText("Scenes: " + Game.Instance.SceneManager.scenes.Count);
        }
    }
}
