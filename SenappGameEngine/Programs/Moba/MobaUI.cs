using OpenTK;
using OpenTK.Input;
using Senapp.Engine.Base;
using Senapp.Engine.Events;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senapp.Programs.Moba
{
    public class MobaUI : Component
    {
        public MainMenu MainMenu;

        public MobaUI() { }
        public MobaUI(GameFont font)
        {
            MainMenu = new MainMenu(font);
        }
        public override void Awake()
        {
            MainMenu = new GameObject()
                .WithName("Main Menu")
                .WithParent(gameObject)
                .WithEnable(false)
                .AddComponent(MainMenu);
        }

        public override void Update(GameUpdatedEventArgs args)
        {
            if (Input.GetKeyDown(Key.Escape) || ControllerManager.ControllerExists(0) && ControllerManager.GetController(0).GetButtonDown(Buttons.Start))
            {
                MainMenu.gameObject.enabled = !MainMenu.gameObject.enabled;
            }
        }
    }
}
