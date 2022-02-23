using System.Drawing;

using OpenTK;

using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Combinations;
using Senapp.Engine.UI.Components;

namespace Senapp.Programs.Moba
{
    public class MobaHUD : ComponentUI
    {
        public Sprite Overlay;
        public Sprite MiniMap;
        public Button SettingsButton;
        public Button CameraButton;
        public Button MiniMapButton;
        public Button StoreButton;
        public Button[] Items;
        public Sprite CharFace;
        public Text Level;

        public MobaHUD() { }
        public MobaHUD(GameFont font)
        {
            this.font = font;

            Overlay = new Sprite("overlay")
                .WithUIConstraint(UIPosition.Bottom)
                .WithSortingLayer(1)
                .WithSize(new Vector2(1.4f, overlaySizeY * 1.4f));

            MiniMap = new Sprite("mapView")
                .WithUIConstraint(UIPosition.BottomRight)
                .WithSize(new Vector2(0.4f, 0.4f));

            SettingsButton = new Button("settingsButton",
                    onEnter: () => SettingsButton.SetColour(Color.Gold),
                    onExit: () => SettingsButton.SetColour(Color.White),
                    onClick: () => MobaMainMenu.Instance.gameObject.enabled = !MobaMainMenu.Instance.gameObject.enabled
                )
                .WithSize(new Vector2(0.05f))
                .WithUIConstraint(UIPosition.BottomRight);

            CameraButton = new Button("cameraButton",
                   onEnter: () => CameraButton.SetColour(Color.Gold),
                   onExit: () => CameraButton.SetColour(Color.White),
                   onClick: () => { }
                )
                .WithSize(new Vector2(0.05f))
                .WithUIConstraint(UIPosition.BottomRight);

            MiniMapButton = new Button("mapButton",
                  onEnter: () => MiniMapButton.SetColour(Color.Gold),
                  onExit: () => MiniMapButton.SetColour(Color.White),
                  onClick: () => { }
                )
                .WithSize(new Vector2(0.05f))
                .WithUIConstraint(UIPosition.BottomRight);

            StoreButton = new Button("store",
                  onEnter: () => StoreButton.SetColour(Color.Gold),
                  onExit: () => StoreButton.SetColour(Color.White),
                  onClick: () => { }
                )
                .WithSortingLayer(1)
                .WithSize(new Vector2(0.325f, storeSizeY * 0.325f));

            Items = new Button[]
            {
                new Button("noItem").WithSize(new Vector2(0.075f)),
                new Button("noItem").WithSize(new Vector2(0.075f)),
                new Button("noItem").WithSize(new Vector2(0.075f)),
                new Button("noItem").WithSize(new Vector2(0.075f)),
                new Button("noItem").WithSize(new Vector2(0.075f)),
                new Button("noItem").WithSize(new Vector2(0.075f)),
            };

            CharFace = new Sprite("lux_face")
               .WithSize(new Vector2(0.175f));

            Level = new Text("1", font, 4, Capitalization.Regularcase, Dock.Center).WithSortingLayer(1);
        }

        public override void Awake()
        {
            Overlay = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("Overlay")
                .AddComponent(Overlay);

            MiniMap = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("Mini Map")
                .AddComponent(MiniMap);

            SettingsButton = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("Settings Button")
                .WithPosition(new Vector3(7.5f, -47.5f, 0))
                .AddComponent(SettingsButton);

            CameraButton = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("Settings Button")
                .WithPosition(new Vector3(2.8f, -47.5f, 0))
                .AddComponent(CameraButton);

            MiniMapButton = new GameObjectUI()
               .WithParent(gameObject)
               .WithName("Mini Map Button")
               .WithPosition(new Vector3(15, -15, 0))
               .AddComponent(MiniMapButton);

            StoreButton = new GameObjectUI()
               .WithParent(Overlay.gameObject)
               .WithName("Store Button")
               .WithPosition(new Vector3(50, -9.5f, 0))
               .AddComponent(StoreButton);

            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new GameObjectUI()
                    .WithParent(Overlay.gameObject)
                    .WithName($"Item {i}")
                    .AddComponent(Items[i]);
            }

            Items[0].gameObject.transform.SetPosition(new Vector3(38.25f, 6.4f, 0));
            Items[1].gameObject.transform.SetPosition(new Vector3(46.5f, 6.4f, 0));
            Items[2].gameObject.transform.SetPosition(new Vector3(55, 6.4f, 0));
            Items[3].gameObject.transform.SetPosition(new Vector3(38.25f, -2f, 0));
            Items[4].gameObject.transform.SetPosition(new Vector3(46.5f, -2f, 0));
            Items[5].gameObject.transform.SetPosition(new Vector3(55, -2f, 0));


            CharFace = new GameObjectUI()
                .WithParent(Overlay.gameObject)
                .WithName("Char Face")
                .WithPosition(new Vector3(-57.75f, 0.5f, 0))
                .AddComponent(CharFace);

            Level = new GameObjectUI()
                .WithParent(Overlay.gameObject)
                .WithName("Level")
                .WithPosition(new Vector3(-2.25f, -8.25f, 0))
                .AddComponent(Level);
        }

        private const float overlaySizeY = 300 / 1536f;
        private const float storeSizeY = 50 / 256f;

        private readonly GameFont font;
    }
}
