using System;

using OpenTK;

using Senapp.Engine.Core;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.Utilities;
using static Senapp.Engine.Physics.PhysicsExtensions;

namespace Senapp.Engine.Physics
{
    public class RigidEntity : Component
    {
        private Vector3 maxVector;
        private Vector3 minVector;
         
        private Vector3 maxVertex;
        private Vector3 minVertex;
         
        private Vector3 weight;
        private Vector3 lastPosition;

        private readonly float mass = 1;
        public float MovedDistance = 0;

        public RigidEntity() { }

        public RigidEntity(Vector3 lastPosition, float mass = 0.1f) 
        {
            this.lastPosition = lastPosition;
            this.mass = mass;
        }

        public override bool ComponentConditions(GameObject gameObject)
        {           
            return base.ComponentConditions(gameObject) && gameObject.HasComponent<Entity>();
        }

        public override void Awake()
        {
            Game.Instance.PhysicsManager.rigidEntities.Add(this);
            CalculateBoundingBox();
        }

        public void GravityUpdate(GameUpdatedEventArgs args)
        {
            gameObject.transform.Translate(0, PhysicsManager.gravity * args.DeltaTime * mass, 0);
        }

        public Vector3 GetEntityDirection()
        {
            var direction = gameObject.transform.GetWorldPosition() - lastPosition;
            return direction / (float)direction.Magnitude();
        }

        public void SetLastPosition(Vector3 newPosition)
        {
            lastPosition = newPosition;
        }

        private void CalculateBoundingBox()
        {
            var entity = gameObject.GetComponent<Entity>();
            Vector3 currentVertice = Vector3.Zero;

            for (int i = 0; i < entity.model.rawModel.ModelData.positions.Length; i++)
            {
                currentVertice.X = entity.model.rawModel.ModelData.positions[i];
                currentVertice.Y = entity.model.rawModel.ModelData.positions[i + 1];
                currentVertice.Z = entity.model.rawModel.ModelData.positions[i + 2];

                maxVertex.X = Math.Max(maxVertex.X, currentVertice.X);
                maxVertex.Y = Math.Max(maxVertex.Y, currentVertice.Y);
                maxVertex.Z = Math.Max(maxVertex.Z, currentVertice.Z);

                minVertex.X = Math.Min(minVertex.X, currentVertice.X);
                minVertex.Y = Math.Min(minVertex.Y, currentVertice.Y);
                minVertex.Z = Math.Min(minVertex.Z, currentVertice.Z);

                var worldVertice = gameObject.transform.GetWorldVerticePosition(currentVertice);

                if (i == 0)
                {
                    maxVector.X = worldVertice.X;
                    maxVector.Y = worldVertice.Y;
                    maxVector.Z = worldVertice.Z;

                    minVector.X = worldVertice.X;
                    minVector.Y = worldVertice.Y;
                    minVector.Z = worldVertice.Z;
                }

                maxVector.X = Math.Max(maxVector.X, worldVertice.X);
                maxVector.Y = Math.Max(maxVector.Y, worldVertice.Y);
                maxVector.Z = Math.Max(maxVector.Z, worldVertice.Z);

                minVector.X = Math.Min(minVector.X, worldVertice.X);
                minVector.Y = Math.Min(minVector.Y, worldVertice.Y);
                minVector.Z = Math.Min(minVector.Z, worldVertice.Z);

                var potWeight = new Vector3((maxVertex.X + minVertex.X) / maxVertex.X, (maxVertex.Y + minVertex.Y) / maxVertex.Y, (maxVertex.Z + minVertex.Z) / maxVertex.Z);

                weight.X = float.IsFinite(potWeight.X) ? potWeight.X : 0;
                weight.Y = float.IsFinite(potWeight.Y) ? potWeight.Y : 0;
                weight.Z = float.IsFinite(potWeight.Z) ? potWeight.Z : 0;

                i += 2;
            }
        }

        public void UpdatePosition(out bool boundingBoxUpdated)
        {
            var worldVerticeMin = gameObject.transform.GetWorldVerticePosition(new Vector3(minVertex.X, minVertex.Y, minVertex.Z));
            var worldVerticeMax = gameObject.transform.GetWorldVerticePosition(new Vector3(maxVertex.X, maxVertex.Y, maxVertex.Z));

            boundingBoxUpdated = minVector != worldVerticeMin || maxVector != worldVerticeMax;    
            
            minVector.X = worldVerticeMin.X;
            minVector.Y = worldVerticeMin.Y;
            minVector.Z = worldVerticeMin.Z;
            maxVector.X = worldVerticeMax.X;
            maxVector.Y = worldVerticeMax.Y;
            maxVector.Z = worldVerticeMax.Z;

            if (lastPosition != gameObject.transform.GetWorldPosition())
            {
                MovedDistance = Vector3.Distance(lastPosition, gameObject.transform.GetWorldPosition());
            }
        }

        public bool CheckCollision(RigidEntity col, out Vector3 colliderPosition)
        {
            bool x = maxVector.X > col.minVector.X && minVector.X < col.maxVector.X;
            bool y = maxVector.Y > col.minVector.Y && minVector.Y < col.maxVector.Y;
            bool z = maxVector.Z > col.minVector.Z && minVector.Z < col.maxVector.Z;

            if (x && y && z) colliderPosition = GetColliderPosition(col);
            else colliderPosition = col.gameObject.transform.GetWorldPosition();

            return x && y && z;
        }

        private Vector3 GetColliderPosition(RigidEntity col)
        {
            float finalX = 0;
            float finalY = 0;
            float finalZ = 0;

            float colX = 0;
            float colY = 0;
            float colZ = 0;

            var directionState = GetDirectionState(col.GetEntityDirection());

            if (directionState.X != Direction.None)
            {
                if (col.minVector.X < maxVector.X && directionState.X == Direction.Negative)
                {
                    colX = Math.Abs(maxVector.X - col.minVector.X);
                    finalX = maxVector.X + (col.maxVector.X - col.minVector.X) / 2 + (col.minVector.X * col.weight.X) / 2;
                }
                else if (col.maxVector.X > minVector.X && directionState.X == Direction.Positive)
                {
                    colX = Math.Abs(col.maxVector.X - minVector.X);
                    finalX = minVector.X - (col.maxVector.X - col.minVector.X) / 2 + (col.maxVector.X * col.weight.X) / 2;
                }
            }
            if (directionState.Y != Direction.None)
            {
                if (col.minVector.Y < maxVector.Y && directionState.Y == Direction.Negative)
                {
                    colY = Math.Abs(maxVector.Y - col.minVector.Y);
                    finalY = maxVector.Y + (col.maxVector.Y - col.minVector.Y) / 2 + (col.minVector.Y * col.weight.Y) / 2;
                }
                else if (col.maxVector.Y > minVector.Y && directionState.Y == Direction.Positive)
                {
                    colY = Math.Abs(col.maxVector.Y - minVector.Y);
                    finalY = minVector.Y - (col.maxVector.Y - col.minVector.Y) / 2 + (col.maxVector.Y * col.weight.Y) / 2;
                }
            }

            if (directionState.Z != Direction.None)
            {
                if (col.minVector.Z < maxVector.Z && directionState.Z == Direction.Negative)
                {
                    colZ = Math.Abs(maxVector.Z - col.minVector.Z);
                    finalZ = maxVector.Z + (col.maxVector.Z - col.minVector.Z) / 2 + (col.minVector.Z * col.weight.Z) / 2;
                }
                else if (col.maxVector.Z > minVector.Z && directionState.Z == Direction.Positive)
                {
                    colZ = Math.Abs(col.maxVector.Z - minVector.Z);
                    finalZ = minVector.Z - (col.maxVector.Z - col.minVector.Z) / 2 + (col.maxVector.Z * col.weight.Z) / 2;
                }
            }

            var colPos = col.gameObject.transform.GetWorldPosition();

            if (colX == 0 || colY != 0 && colX > colY || colZ != 0 && colX > colZ)
            {
                finalX = colPos.X;
            }

            if (colY == 0 || colX != 0 && colY > colX || colZ != 0 && colY > colZ)
            {
                finalY = colPos.Y;
            }

            if (colZ == 0 || colX != 0 && colZ > colX || colY != 0 && colZ > colY)
            {
                finalZ = colPos.Z;
            }

            return new Vector3(finalX, finalY, finalZ);
        }
    }
}
