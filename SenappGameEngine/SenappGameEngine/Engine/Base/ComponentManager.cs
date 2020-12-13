using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Base
{
    public class ComponentManager
    {
        private Dictionary<Type, Component> components;

        public ComponentManager()
        {
            components = new Dictionary<Type, Component>();
        }
        public Dictionary<Type, Component> GetComponents()
        {
            return components;
        }
        public void AddComponent(Component component)
        {
            components[component.GetType()] = component;
        }
        public bool HasComponent(Component component)
        {
            return components.ContainsKey(component.GetType());
        }
        public bool HasComponent<T>() where T : Component
        {
            return components.ContainsKey(typeof(T));
        }
        public T GetComponent<T>() where T : Component
        {
            return components.TryGetValue(typeof(T), out var value) ? (T)value : null;
        }
        public void RemoveComponent<T>() where T : Component
        {
            components.Remove(typeof(T));
        }
        public void Dispose()
        {
            foreach (var component in components)
            {
                component.Value.Dispose();
            }
        }
    }
}
