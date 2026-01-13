using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Senapp.Engine.Core.Components
{
    public class ComponentManager
    {
        public Dictionary<Type, Component> GetDynamicComponents() => dynamicComponents;
        public Dictionary<Type, Component> GetComponents() => components;
        public void AddComponent(Component component)
        {
            components[component.GetType()] = component;
            if (component.ComponentRequiresUpdates)
            {
                dynamicComponents[component.GetType()] = component;
            }
        }
        public bool HasComponent<T>() where T : Component => components.ContainsKey(typeof(T));
        public T GetComponent<T>() where T : Component => components.TryGetValue(typeof(T), out var value) ? (T)value : null;
        public void RemoveComponent<T>() where T : Component
        {
            components.Remove(typeof(T));
            if (dynamicComponents.ContainsKey(typeof(T)))
            {
                dynamicComponents.Remove(typeof(T));
            }
        }
        public void Dispose()
        {
            foreach (var component in components)
            {
                component.Value.Dispose();
            }

            components.Clear();
            dynamicComponents.Clear();
        }

        private readonly Dictionary<Type, Component> components = new();
        private readonly Dictionary<Type, Component> dynamicComponents = new();
    }
}
