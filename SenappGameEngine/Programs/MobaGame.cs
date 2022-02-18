using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

using Senapp.Engine.Core;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Core.Scenes;
using Senapp.Engine.Events;
using Senapp.Engine.UI;
using Senapp.Engine.Utilities;
using Senapp.Programs.Moba;

namespace Senapp.Programs
{
    public class MobaGame : Game
    {
        // Assets and gameplay features in this program are owned by Riot Games and their game League of Legends. 
        // This is just a recreation with no commercial gain of the previous game mode Twisted Treelines from League of Legends.
        // MobaGame was created under Riot Games' "Legal Jibber Jabber" policy using assets owned by Riot Games. Riot Games does not endorse or sponsor this project.
        public MobaGame(GraphicsMode GRAPHICS_MODE) : base(WIDTH, HEIGHT, GRAPHICS_MODE, TITLE)
        {
            GameInitializedEvent += Initialize;
            GameUpdatedEvent += Update;
            Run();
        }

        public static readonly int WIDTH = 800;
        public static readonly int HEIGHT = 600;
        public static readonly string TITLE = "League Of Legends";

        public MobaUI MobaUI;
        public MobaView MobaView;
        public Scene UIScene = new();
        public Scene GameScene = new();
        public GameFont font = new();

        private void Initialize(object sender)
        {
            Icon = Resources.GetIcon("new_icon");
            VSync = VSyncMode.On;

            font.LoadFont("opensans");

            MobaUI = new GameObjectUI()
                .WithName("Moba UI")
                .WithParent(UIScene)
                .AddComponent(new MobaUI(font));

            MobaView = new GameObject()
                .WithName("Moba View")
                .WithParent(GameScene)
                .AddComponent(new MobaView());

            SceneManager.AddScene(UIScene);
            SceneManager.AddScene(GameScene);

            MainCamera.gameObject.transform.SetPosition(new Vector3(0, 20, 0));
        }

        private void Update(object sender, GameUpdatedEventArgs args)
        {
            if (!Focused)
                return;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            MobaPlayerController.Instance?.OnScroll(-e.DeltaPrecise);
        }
    }
}
