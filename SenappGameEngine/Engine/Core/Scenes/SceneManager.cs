using System;
using System.Collections.Generic;

namespace Senapp.Engine.Core.Scenes
{
    public class SceneManager
    {
        public Dictionary<Guid, Scene> scenes = new();

        public void AddScene(Scene scene)
        {
            if (!scenes.ContainsKey(scene.Id))
            {
                scenes.Add(scene.Id, scene);
            }
        }

        public void RemoveScene(Scene scene)
        {
            if (scenes.ContainsKey(scene.Id))
            {
                scenes.Remove(scene.Id);
            }
        }

        public void Dispose()
        {
            foreach (var scene in scenes.Values)
            {
                scene.Dispose();
            }
        }
    }
}
