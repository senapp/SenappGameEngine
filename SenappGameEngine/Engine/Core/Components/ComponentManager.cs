using System;
using System.Collections.Generic;

namespace Senapp.Engine.Core.Components
{
    public class ComponentManager
    {
        public Dictionary<Type, Component> GetComponents() => components;
        public void AddComponent(Component component) => components[component.GetType()] = component;
        public bool HasComponent<T>() where T : Component => components.ContainsKey(typeof(T));
        public T GetComponent<T>() where T : Component => components.TryGetValue(typeof(T), out var value) ? (T)value : null;
        public void RemoveComponent<T>() where T : Component => components.Remove(typeof(T));
        public void Dispose()
        {
            foreach (var component in components)
            {
                component.Value.Dispose();
            }
        }

        private readonly Dictionary<Type, Component> components = new();
    }
}
