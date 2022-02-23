using System;

using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.UI.Components;

namespace Senapp.Engine.Raycasts
{
    public class RaycastTargetUI : ComponentUI
    {
        public bool hovering = false;
        public bool focused = false;

        public Action onEnter = null;
        public Action onClick = null;
        public Action onExit = null;
        public Action onLoseFocus = null;

        public RaycastTargetUI() { }
        public RaycastTargetUI(Action onEnter = null, Action onClick = null, Action onExit = null, Action onLoseFocus = null)
        {
            this.onEnter = onEnter;
            this.onClick = onClick;
            this.onExit = onExit;
            this.onLoseFocus = onLoseFocus;
        }

        public override bool ComponentConditions(GameObject gameObject)
        {
            return gameObject.HasComponent<Sprite>() || gameObject.HasComponent<Text>();
        }
    }
}
