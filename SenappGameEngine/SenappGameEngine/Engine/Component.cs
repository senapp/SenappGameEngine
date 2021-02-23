using ImGuiNET;
using Senapp.Engine.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine
{
    public abstract class Component
    {
        public GameObject gameObject = null;
        public Component() { }
        public virtual void Dispose() { }
        public virtual bool ComponentConditions(GameObject gameObject) { return true; }
        public virtual void Update(GameUpdatedEventArgs args) { }
        public virtual void Awake() { }
    }
}
