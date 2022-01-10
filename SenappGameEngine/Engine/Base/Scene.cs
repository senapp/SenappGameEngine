using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senapp.Engine.Base
{
    public class Scene
    {
        private GameObject sceneRefrence = new GameObject();
        public Guid id => sceneRefrence.id;

        public void AddGameObject(GameObject gameObject)
        {
            sceneRefrence.AddChild(gameObject);
        }
        public List<GameObject> GetAllGameObjects()
        {
            return sceneRefrence.GetChildren();
        }
        public List<GameObject> GetGameObjects()
        {
            return sceneRefrence.children.Values.ToList();
        }
        public void Dispose()
        {
            sceneRefrence.Dispose();
        }

        public Scene WithGameObject(GameObject gameObject)
        {
            AddGameObject(gameObject);
            return this;
        }
    }
}
