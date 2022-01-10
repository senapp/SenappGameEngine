using System.Drawing;
using System.Linq;
using Senapp.Engine.Events;
using System.Collections.Generic;

using OpenTK;

namespace Senapp.Engine.Physics
{
    public class PhysicsManager
    {
        public static readonly float gravity = -9.81f;

        public List<RigidEntity> rigidEntities = new List<RigidEntity>();

        private int SortByMovedDistance(RigidEntity a, RigidEntity b)
        {
            var val = a.MovedDistance.CompareTo(b.MovedDistance);

            if (val == 0) return SortByY(a, b);
            else return val;
        }

        private int SortByY(RigidEntity a, RigidEntity b)
        {
            return a.gameObject.transform.position.Y.CompareTo(b.gameObject.transform.position.Y);
        }

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
                bool collision = false;
                foreach (var col in colliders)
                {
                    if (col != mesh && col.CheckCollision(mesh, out Vector3 position))
                    {
                        mesh.gameObject.SetColour(Color.Blue);
                        mesh.gameObject.transform.position = position;
                        mesh.UpdatePosition(out bool newPosition);
                        collision = true;
                    }
                }

                if (!collision)
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

                mesh.SetLastPosition(mesh.gameObject.transform.position);
            }
        }
    }
}
