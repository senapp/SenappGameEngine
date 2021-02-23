using Senapp.Engine.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Physics
{
    public class RaycastTarget : Component
    {
        public float hitRadius = 1;
        public bool hovering = false;

        public Action onEnter = null;
        public Action onClick = null;
        public Action onExit = null;

        public RaycastTarget() { }
        public RaycastTarget(float hitRadius, Action onEnter, Action onClick, Action onExit)
        {
            this.hitRadius = hitRadius;
            this.onEnter = onEnter;
            this.onClick = onClick;
            this.onExit = onExit;
        }
    }
}
