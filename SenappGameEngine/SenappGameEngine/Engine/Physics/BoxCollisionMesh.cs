using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Senapp.Engine.Base;
using Senapp.Engine.Entities;
using Senapp.Engine.Events;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Physics
{
    public class BoxCollisionMesh : Component
    {
        public static List<BoxCollisionMesh> colliders = new List<BoxCollisionMesh>();

        Vector3 maxVector = new Vector3();
        Vector3 minVector = new Vector3();

        Vector3 maxVertex = new Vector3();
        Vector3 minVertex = new Vector3();

        Vector3 weight = new Vector3();

        public BoxCollisionMesh() { }
        public override bool ComponentConditions(GameObject gameObject)
        {
            return gameObject.HasComponent<Entity>();
        }
        public override void Awake()
        {
            colliders.Add(this);
            CalculateBox();
        }
        private void CalculateBox()
        {
            var entity = gameObject.GetComponent<Entity>();
            Vector3 currentVertice = Vector3.Zero;
            for (int i = 0; i < entity.model.rawModel.modelData.positions.Length; i++)
            {
                currentVertice.X = entity.model.rawModel.modelData.positions[i];
                currentVertice.Y = entity.model.rawModel.modelData.positions[i + 1];
                currentVertice.Z = entity.model.rawModel.modelData.positions[i + 2];

                maxVertex.X = Math.Max(maxVertex.X, currentVertice.X);
                maxVertex.Y = Math.Max(maxVertex.Y, currentVertice.Y);
                maxVertex.Z = Math.Max(maxVertex.Z, currentVertice.Z);

                minVertex.X = Math.Min(minVertex.X, currentVertice.X);
                minVertex.Y = Math.Min(minVertex.Y, currentVertice.Y);
                minVertex.Z = Math.Min(minVertex.Z, currentVertice.Z);

                var worldVertice = gameObject.transform.GetVerticePosition(currentVertice);

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

                weight.X = (maxVertex.X + minVertex.X) / maxVertex.X;
                weight.Y = (maxVertex.Y + minVertex.Y) / maxVertex.Y;
                weight.Z = (maxVertex.Z + minVertex.Z) / maxVertex.Z;

                i += 2;
            }
        }
        public void UpdateBoxTransform()
        {
            var worldVerticeMin = gameObject.transform.GetVerticePosition(new Vector3(minVertex.X, minVertex.Y, minVertex.Z));
            var worldVerticeMax = gameObject.transform.GetVerticePosition(new Vector3(maxVertex.X, maxVertex.Y, maxVertex.Z));

            minVector.X = worldVerticeMin.X;
            minVector.Y = worldVerticeMin.Y;
            minVector.Z = worldVerticeMin.Z;
            maxVector.X = worldVerticeMax.X;
            maxVector.Y = worldVerticeMax.Y;
            maxVector.Z = worldVerticeMax.Z;
        }
        public bool CheckCollision(BoxCollisionMesh col, out Vector3 colliderPosition, out bool grounded)
        {
            bool x = maxVector.X > col.minVector.X && minVector.X < col.maxVector.X;
            bool y = maxVector.Y > col.minVector.Y && minVector.Y < col.maxVector.Y;
            bool z = maxVector.Z > col.minVector.Z && minVector.Z < col.maxVector.Z;

            if (x && y && z) colliderPosition = GetColliderPosition(col);
            else colliderPosition = col.gameObject.transform.position;

            if (x && y && z) grounded = minVector.Y <= col.maxVector.Y;
            else grounded = false;

            return x && y && z;
        }
        private Vector3 GetColliderPosition(BoxCollisionMesh col)
        {
            float colX = col.gameObject.transform.position.X;
            float colY = col.gameObject.transform.position.Y;
            float colZ = col.gameObject.transform.position.Z;

            float boxX = gameObject.transform.position.X;
            float boxY = gameObject.transform.position.Y;
            float boxZ = gameObject.transform.position.Z;

            float finalX = 0;
            float finalY = 0;
            float finalZ = 0;

            if (maxVector.X > col.minVector.X) { colX = col.minVector.X; boxX = maxVector.X; finalX =      boxX + (col.maxVector.X - col.minVector.X) / 2 + (col.minVector.X * col.weight.X) / 2; }
            else if (col.maxVector.X > minVector.X) { colX = col.maxVector.X; boxX = minVector.X; finalX = boxX + (col.minVector.X - col.maxVector.X) / 2 + (col.maxVector.X * col.weight.X) / 2; }
                                                                                                                                                                                                                                    
            if (maxVector.Y > col.minVector.Y) { colY = col.minVector.Y; boxY = maxVector.Y; finalY =      boxY + (col.maxVector.Y - col.minVector.Y) / 2 + (col.minVector.Y * col.weight.Y) / 2; }
            else if (col.maxVector.Y > minVector.Y) { colY = col.maxVector.Y; boxY = minVector.Y; finalY = boxY + (col.minVector.Y - col.maxVector.Y) / 2 + (col.maxVector.Y * col.weight.Y) / 2; }

            if (maxVector.Z > col.minVector.Z) { colZ = col.minVector.Z; boxZ = maxVector.Z; finalZ =      boxZ + (col.maxVector.Z - col.minVector.Z) / 2 + (col.minVector.Z * col.weight.Z) / 2; }
            else if (col.maxVector.Z >= minVector.Z) { colZ = col.maxVector.Z; boxZ = minVector.Z; finalZ = boxZ + (col.minVector.Z - col.maxVector.Z) / 2 + (col.maxVector.Z * col.weight.Z) / 2; }

            float difX = Math.Max(colX, boxX) - Math.Min(colX, boxX);
            float difY = Math.Max(colY, boxY) - Math.Min(colY, boxY);
            float difZ = Math.Max(colZ, boxZ) - Math.Min(colZ, boxZ);

            if (difX <= difY && difX <= difZ) { return new Vector3(finalX, col.gameObject.transform.position.Y, col.gameObject.transform.position.Z); }
            if (difY <= difX && difY <= difZ) { return new Vector3(col.gameObject.transform.position.X, finalY, col.gameObject.transform.position.Z); }
            if (difZ <= difX && difZ <= difY) { return new Vector3(col.gameObject.transform.position.X, col.gameObject.transform.position.Y, finalZ); }
            if (difX == difY && difX == difZ) { return new Vector3(col.gameObject.transform.position.X, col.gameObject.transform.position.Y, col.gameObject.transform.position.Z); }

            return col.gameObject.transform.position;
        }

    }
}
