using System;

using Senapp.Engine.Core.Components;

namespace Senapp.Engine.Raycasts
{
    public class RaycastTarget : Component
    {
        public float hitRadius = 1;
        public bool hovering = false;
        public bool focused = false;

        public Action onEnter = null;
        public Action onClick = null;
        public Action onExit = null;
        public Action onLoseFocus = null;

        public RaycastTarget() { }
        public RaycastTarget(float hitRadius, Action onEnter = null, Action onClick = null, Action onExit = null, Action onLoseFocus = null)
        {
            this.hitRadius = hitRadius;
            this.onEnter = onEnter;
            this.onClick = onClick;
            this.onExit = onExit;
            this.onLoseFocus = onLoseFocus;
        }
    }
}
