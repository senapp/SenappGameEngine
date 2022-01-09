using System.Drawing;
using System.Linq;
using Senapp.Engine.Events;
using System.Collections.Generic;

using OpenTK;

namespace Senapp.Engine.Physics
{
    public class PhysicsManager
    {
        public void PhysicsUpdate(GameUpdatedEventArgs args)
        {
            var colliders = RigidEntity.colliders.Where(obj => obj.gameObject.enabled).ToList();
            var nonStaticColliders = colliders.Where(obj => !obj.gameObject.isStatic).ToList();
            var movedColliders = new List<RigidEntity>();

            foreach (var collider in nonStaticColliders)
            {
                collider.UpdateBoundingBox(out bool newPosition);
                if (newPosition)
                {
                    movedColliders.Add(collider);
                    colliders.Remove(collider);
                    colliders.Insert(0, collider);
                }
            }

            foreach (var mesh in movedColliders)
            {
                foreach (var col in colliders)
                {
                    if (col != mesh)
                    {
                        if (col.CheckCollision(mesh, out Vector3 position))
                        {
                            mesh.gameObject.SetColour(Color.Blue);
                            mesh.gameObject.transform.position = position;
                            mesh.UpdateBoundingBox(out bool newPosition);
                        }
                        else
                        {
                            if (mesh.gameObject.HasComponent<RaycastTarget>())
                            {
                                var component = mesh.gameObject.GetComponent<RaycastTarget>();
                                if (component.focused)
                                {
                                    mesh.gameObject.SetColour(Color.Red);
                                }
                                else
                                {
                                    mesh.gameObject.SetColour(Color.White);
                                }
                            }
                        }
                    }
                }

                mesh.SetLastPosition(mesh.gameObject.transform.position);
            }
        }
    }
}
