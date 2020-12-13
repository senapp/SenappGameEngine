using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Base
{
    public abstract class Component
    {
        public GameObject parent = null;
        public Component() { }
        public virtual void Dispose() { }
        public virtual bool ComponentConditions(GameObject gameObject) { return true; }
        public virtual void Update() { }
        public virtual void Awake() { }
    }
}
