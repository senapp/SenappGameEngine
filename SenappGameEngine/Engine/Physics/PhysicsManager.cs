using System.Linq;
using System.Collections.Generic;

using OpenTK;

using Senapp.Engine.Events;

namespace Senapp.Engine.Physics
{
    public class PhysicsManager
    {
        public static readonly float gravity = -9.81f;

        public List<RigidEntity> rigidEntities = new();

        public void PhysicsUpdate(GameUpdatedEventArgs args)
        {
            var colliders = rigidEntities.Where(obj => obj.gameObject.enabled).ToList();
            var nonStaticColliders = colliders.Where(obj => !obj.gameObject.isStatic).ToList();
            var movedColliders = new List<RigidEntity>();

            foreach (var collider in nonStaticColliders)
            {
                collider.UpdatePosition(out bool boundingBoxUpdated);
                if (boundingBoxUpdated)
                {
                    movedColliders.Add(collider);
                    colliders.Remove(collider);
                    colliders.Insert(colliders.Count, collider);
                }
            }

            foreach (var mesh in movedColliders)
            {
                foreach (var col in colliders)
                {
                    if (col != mesh && col.CheckCollision(mesh, out Vector3 position))
                    {
                        mesh.gameObject.transform.SetPosition(position);
                        mesh.UpdatePosition(out bool newPosition);
                    }
                }

                mesh.SetLastPosition(mesh.gameObject.transform.GetWorldPosition());
            }
        }
    }
}
