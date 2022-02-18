using OpenTK.Input;

using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Components;

namespace Senapp.Programs.Moba
{
    public class MobaUI : ComponentUI
    {
        public MobaMainMenu MobaMainMenu;
        public MobaHUD MobaHUD;

        public MobaUI() { }
        public MobaUI(GameFont font)
        {
            MobaMainMenu = new MobaMainMenu(font);
            MobaHUD = new MobaHUD(font);
        }
        public override void Awake()
        {
            MobaMainMenu = new GameObjectUI()
                .WithParent(gameObject)
                .WithName("Moba Main Menu")
                .WithEnable(false)
                .AddComponent(MobaMainMenu);

            MobaHUD = new GameObjectUI()
               .WithParent(gameObject)
               .WithName("Moba HUD")
               .AddComponent(MobaHUD);
        }

        public override void Update(GameUpdatedEventArgs args)
        {
            if (Input.GetKeyDown(Key.Escape) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.Start))
            {
                MobaMainMenu.gameObject.enabled = !MobaMainMenu.gameObject.enabled;
            }
        }
    }
}
