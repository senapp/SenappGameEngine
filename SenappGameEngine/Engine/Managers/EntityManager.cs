using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senapp.Engine.Managers
{
    public class EntityManager
    {
        public EntityManager() { }

        public Dictionary<TexturedModel, List<Entity>> renderList = new();

        public void OnNewEntity(Entity entity)
        {
            var found = false;
            foreach (var model in renderList.Keys)
            {
                if (model.Equals(entity.model))
                {
                    renderList[model].Add(entity);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                var newEntry = new List<Entity> { entity };
                renderList.Add(entity.model, newEntry);
            }
        }
    }
}
