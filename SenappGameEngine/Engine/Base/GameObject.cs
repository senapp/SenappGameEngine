using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using Senapp.Engine.Events;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Base
{
    public class GameObject
    {
        public Guid id { get; private set; } = Guid.NewGuid();
        public ComponentManager componentManager { get; private set; } = new ComponentManager();
        public Vector3 colour { get; private set; } = Vector3.One;

        public bool enabled = true;
        public bool visible = true;
        public bool isStatic = false;
        public Transform transform = new Transform();
        public string name = "GameObject";
        public Dictionary<Guid, GameObject> children = new Dictionary<Guid, GameObject>();
        public GameObject parent { get; private set; }

        public GameObject() { }
        public void SetParent(GameObject parent)
        {
            parent.AddChild(this);
            this.parent = parent;
        }
        public void AddChild(GameObject child)
        {
            if (!children.ContainsKey(child.id))
            {
                child.parent = this;
                children.Add(child.id, child);
            }
        }
        public T AddComponent<T>(T component) where T : Component, new()
        {
            if (component.ComponentConditions(this))
            {
                component.gameObject = this;
                componentManager.AddComponent(component);
                component.Awake();
                return component;
            }
            else
            {
                Console.WriteLine(string.Format("{0} cannot add the component {1} because it's conditions weren't met", name, component.GetType()));
                return null;
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
            foreach (var child in children.Values)
            {
                child.Dispose();
            }
        }
        public void Update(GameUpdatedEventArgs args)
        {
            if (enabled)
            {
                foreach (var component in componentManager.GetComponents())
                {
                    component.Value.Update(args);
                }
                foreach (var child in children.Values)
                {
                    child.Update(args);
                }
            }
        }
        public void ProccessRender<T>(Action<GameObject> renderAction) where T : Component, new()
        {
            if (enabled)
            {
                if (HasComponent<T>()) renderAction(this);
                foreach (var child in children.Values)
                {
                    child.ProccessRender<T>(renderAction);
                }
            }
        }

        public List<GameObject> GetChildren()
        {
            var total = new List<GameObject>();
            total.AddRange(children.Values);
            foreach (var child in children.Values)
            {
                total.AddRange(child.GetChildren());
            }
            return total;
        }
        public void SetColour(Color colour) => SetColour(new Vector3(colour.R / 255f, colour.G / 255f, colour.B / 255f));
        public void SetColour(Vector3 colour) => this.colour = colour;

        public GameObject WithName(string name)
        {
            this.name = name;
            return this;
        }
        public GameObject WithScale(Vector3 scale)
        {
            transform.localScale = scale;
            return this;
        }
        public GameObject WithPosition(Vector3 position)
        {
            transform.position = position;
            return this;
        }
        public GameObject WithColour(Color colour) => WithColour(new Vector3(colour.R / 255f, colour.G / 255f, colour.B / 255f));
        public GameObject WithColour(Vector3 colour)
        {
            this.colour = colour;
            return this;
        }
        public GameObject WithParent(GameObject parent)
        {
            SetParent(parent);
            return this;
        }
        public GameObject WithParent(Scene scene)
        {
            scene.AddGameObject(this);
            return this;
        }
        public GameObject WithEnable(bool enabled)
        {
            this.enabled = enabled;
            return this;
        }
        public GameObject WithComponent<T>(T component) where T : Component, new()
        {
            AddComponent(component);
            return this;
        }
    }
}
