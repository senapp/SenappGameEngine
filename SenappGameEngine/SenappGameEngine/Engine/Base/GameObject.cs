using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Base
{
    public class GameObject
    {
        public static List<GameObject> GameObjects = new List<GameObject>();
        public bool excludeFromEditor = false;
        public int id { get; private set; }
        public bool enabled = true;
        public bool isStatic = false;
        public Transform transform = new Transform();
        public string name = "GameObject";
        public ComponentManager componentManager;

        public GameObject()
        {
            id = Randomize.RangeInt(1, 2147483647);
            componentManager = new ComponentManager();
            GameObjects.Add(this);
        }
        public void AddComponent(Component component)
        {
            if (component.ComponentConditions(this))
            {
                component.parent = this;
                componentManager.AddComponent(component);
                component.Awake();
            }
            else
            {
                Console.WriteLine(string.Format("{0} cannot add the component {1} because it's conditions weren't met", name, component.GetType()));
            }
        }
        public bool HasComponent(Component component)
        {
            return componentManager.HasComponent(component);
        }
        public bool HasComponent<T>() where T : Component, new()
        {
            return componentManager.HasComponent<T>();
        }
        public T GetComponent<T>() where T : Component, new()
        {
            return componentManager.GetComponent<T>();
        }
        public void RemoveComponent<T>() where T : Component, new()
        {
            componentManager.RemoveComponent<T>();
        }
        public void Dispose()
        {
            componentManager.Dispose();
            GameObjects.Remove(this);
        }
    }
}
