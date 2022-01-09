using Senapp.Engine.Events;

namespace Senapp.Engine.Base
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
