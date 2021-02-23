using Senapp.Engine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.UI
{
    public class RaycastTargetUI : Component
    {
        public bool hovering = false;

        public Action onEnter = null;
        public Action onClick = null;
        public Action onExit = null;

        public RaycastTargetUI() { }
        public RaycastTargetUI(Action onEnter, Action onClick, Action onExit)
        {
            this.onEnter = onEnter;
            this.onClick = onClick;
            this.onExit = onExit;
        }
    }
}
