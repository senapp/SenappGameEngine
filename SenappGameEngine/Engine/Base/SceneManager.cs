using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senapp.Engine.Base
{
    public class SceneManager
    {
        public Dictionary<Guid, Scene> scenes = new Dictionary<Guid, Scene>();

        public void AddScene(Scene scene)
        {
            if (!scenes.ContainsKey(scene.id))
            {
                scenes.Add(scene.id, scene);
            }
        }

        public void RemoveScene(Scene scene)
        {
            if (scenes.ContainsKey(scene.id))
            {
                scenes.Remove(scene.id);
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
