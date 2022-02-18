using System;
using System.Collections.Generic;
using System.Linq;

using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;

namespace Senapp.Engine.Core.Scenes
{
    public class Scene
    {
        private readonly GameObject sceneRefrence = new();

        public Guid Id => sceneRefrence.Id;

        public void AddGameObject(GameObject gameObject)
        {
            sceneRefrence.AddChild(gameObject);
        }

        public List<GameObject> GetAllGameObjects()
        {
            return sceneRefrence.GetAllChildren();
        }
        public List<T> GetAllComponents<T>(bool includeDisabled = false) where T : Component, new()
        {
            return sceneRefrence.GetComponentFromChildren<T>(includeDisabled);
        }
        public List<GameObject> GetGameObjects()
        {
            return sceneRefrence.Children.Values.ToList();
        }

        public Scene WithGameObject(GameObject gameObject)
        {
            AddGameObject(gameObject);
            return this;
        }

        public void Dispose()
        {
            sceneRefrence.Dispose();
        }
    }
}
