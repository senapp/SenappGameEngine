using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using OpenTK;

using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.Scenes;
using Senapp.Engine.Core.Transforms;
using Senapp.Engine.Events;

namespace Senapp.Engine.Core.GameObjects
{
    public class GameObject
    {
        public static int InstanceCounter = 0;

        public bool IsGameObjectUI { get; protected set; } = false;
        public bool IsGameObjectUpdated = false;

        public Guid Id { get; private set; } = Guid.NewGuid();
        public int InstanceId { get; private set; }
        public GameObject Parent { get; private set; }
        public Dictionary<Guid, GameObject> Children { get; } = new();
        public ComponentManager ComponentManager { get; private set; } = new();

        public string name = "GameObject";
        public bool enabled = true;
        public bool visible = true;
        public bool isStatic = false;
        public Color colour = Color.White;
        public Transform transform;

        public GameObject() 
        {
            InstanceCounter++;
            InstanceId = InstanceCounter;

            transform = new Transform(this);
            IsGameObjectUpdated = true;
        }

        public void Update(GameUpdatedEventArgs args)
        {
            if (enabled)
            {
                foreach (var component in ComponentManager.GetComponents())
                {
                    if (component.Value.enabled)
                    {
                        component.Value.Update(args);
                    }
                }
                foreach (var child in Children.Values)
                {
                    child.Update(args);
                }
            }
        }
        public void ProccessRenderHierarchy<T>(Action<T> processAction) where T : Component, new()
        {
            if (enabled)
            {
                if (visible && HasComponent<T>()) processAction(GetComponent<T>());
                foreach (var child in Children.Values)
                {
                    child.ProccessRenderHierarchy(processAction);
                }
            }
        }

        public void SetParent(GameObject parent)
        {
            parent.AddChild(this);
        }
        public void AddChild(GameObject child)
        {
            if (Parent != null && IsGameObjectUI != child.IsGameObjectUI)
            {
                Console.WriteLine($"[ENGINE] {name}: IsUI = {IsGameObjectUI}. Child {child.name} has opposing value. " +
                    $"This is likely by caused by initialzing one of the GameObjects being initialized with the wrong class. " +
                    $"Use GameObjectUI for UI Elements. And GameObject for elements in 3D space");
                return;
            }
            if (!Children.ContainsKey(child.Id))
            {
                child.Parent = this;
                child.transform.SetNewParent(this);
                Children.Add(child.Id, child);
            }
        }

        public List<GameObject> GetAllChildren()
        {
            var total = new List<GameObject>();
            total.AddRange(Children.Values);
            foreach (var child in Children.Values)
            {
                total.AddRange(child.GetAllChildren());
            }
            return total;
        }
        public List<T> GetComponentFromChildren<T>(bool includeDisabled = false) where T : Component, new()
        {
            var total = new List<T>();
            if (includeDisabled || enabled)
            {
                total.AddRange(Children.Values.Where(obj => obj.HasComponent<T>()).Select(obj => obj.GetComponent<T>()));
                foreach (var child in Children.Values)
                {
                    total.AddRange(child.GetComponentFromChildren<T>(includeDisabled));
                }
            }
            return total;
        }

        public T AddComponent<T>(T component) where T : Component, new()
        {
            if (component.ComponentConditions(this))
            {
                component.gameObject = this;
                ComponentManager.AddComponent(component);
                component.Awake();
                return component;
            }
            else
            {
                Console.WriteLine($"[ENGINE] {name} cannot add the component {component.GetType()} because it's conditions weren't met");
                return null;
            }
        }
        public bool HasComponent<T>() where T : Component, new()
        {
            return ComponentManager.HasComponent<T>();
        }
        public T GetComponent<T>() where T : Component, new()
        {
            return ComponentManager.GetComponent<T>();
        }
        public void RemoveComponent<T>() where T : Component, new()
        {
            ComponentManager.RemoveComponent<T>();
        }

        public GameObject WithName(string name)
        {
            this.name = name;
            return this;
        }
        public GameObject WithScale(Vector3 scale)
        {
            transform.SetScale(scale);
            return this;
        }
        public GameObject WithPosition(Vector3 position)
        {
            transform.SetPosition(position);
            return this;
        }
        public GameObject WithRotation(Vector3 rotation)
        {
            transform.SetRotation(rotation);
            return this;
        }
        public GameObject WithColour(Color colour)
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

        public void Dispose()
        {
            ComponentManager.Dispose();
            foreach (var child in Children.Values)
            {
                child.Dispose();
            }
        }
    }
}
