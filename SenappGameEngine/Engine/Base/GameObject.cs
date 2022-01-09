using System;
using System.Drawing;

using OpenTK;

using Senapp.Engine.Utilities;

namespace Senapp.Engine.Base
{
    public class GameObject
    {
        public int id { get; private set; }
        public Vector3 colour { get; private set; } = Vector3.One;
        public ComponentManager componentManager { get; private set; }

        public bool enabled = true;
        public bool isStatic = false;
        public Transform transform = new Transform();
        public string name = "GameObject";

        public GameObject()
        {
            id = Randomize.RangeInt(1, 2147483647);
            componentManager = new ComponentManager();
            Game.GameObjects.Add(this);
        }
        public void AddComponent(Component component)
        {
            if (component.ComponentConditions(this))
            {
                component.gameObject = this;
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
            Game.GameObjects.Remove(this);
        }

        public void SetColour(Color colour) => SetColour(new Vector3(colour.R / 255, colour.G / 255, colour.B / 255));
        public void SetColour(Vector3 colour) => this.colour = colour;
    }
}
