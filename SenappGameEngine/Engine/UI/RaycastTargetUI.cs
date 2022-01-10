using System;

using Senapp.Engine.Base;

namespace Senapp.Engine.UI
{
    public class RaycastTargetUI : Component
    {
        public bool hovering = false;
        public bool focused = false;

        public Action onEnter = null;
        public Action onClick = null;
        public Action onExit = null;
        public Action onLoseFocus = null;

        public RaycastTargetUI() { }
        public RaycastTargetUI(Action onEnter, Action onClick, Action onExit, Action onLoseFocus)
        {
            this.onEnter = onEnter;
            this.onClick = onClick;
            this.onExit = onExit;
            this.onLoseFocus = onLoseFocus;
        }
    }
}
