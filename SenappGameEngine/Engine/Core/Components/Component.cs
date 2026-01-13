using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Events;

namespace Senapp.Engine.Core.Components
{
    public abstract class Component
    {
        public GameObject gameObject;
        public bool enabled = true;
        public readonly bool ComponentRequiresUpdates;

        public Component(bool componentRequiresUpdates) { ComponentRequiresUpdates = componentRequiresUpdates; }

        public virtual void Dispose() { }
        public virtual bool ComponentConditions(GameObject gameObject) {
            return !gameObject.IsGameObjectUI;
        }
        public virtual void Update(GameUpdatedEventArgs args) { }
        public virtual void Awake() { }
    }
}
