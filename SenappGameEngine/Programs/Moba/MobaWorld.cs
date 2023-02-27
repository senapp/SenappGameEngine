using OpenTK;

using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Entities;
using Senapp.Engine.Models;
using Senapp.Engine.Utilities;
using Senapp.Engine.Utilities.Pathfinding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Senapp.Programs.Moba
{
    public class MobaWorld : Component
    {
        public static MobaWorld Instance;
        public Entity Ground;

        public List<Tower> towers = new();

        public MobaWorld()
        {
            Instance = this;
            Ground = new Entity(RawModel.GenerateTerrain(TerrainSize), "map");
            walkableMap = Resources.GetImage("mapWalkable");

            factor = 10;

            walkableGrid = new NodeGrid(walkableMap.Width / factor, walkableMap.Height / factor);
            for (int x = 0; x < walkableGrid.coloums; x++)
            {
                for (int y = 0; y < walkableGrid.rows; y++)
                {
                    walkableGrid.grid[x, y] = new Node(x, y, 
                        walkableMap.GetPixel(Math.Min(x * factor + factor, walkableMap.Width - 1), Math.Min(y * factor + factor, walkableMap.Height - 1)).G > 250);
                }
            }

            towers.Add(new Tower(new Vector3(-23.4f, 0, -20), Vector3.Zero.WithY(60), true));
            towers.Add(new Tower(new Vector3(23, 0, -20), Vector3.Zero.WithY(315), false));
            towers.Add(new Tower(new Vector3(24.6f, 0, 14.3f), Vector3.Zero.WithY(265), true));
            towers.Add(new Tower(new Vector3(-23.6f, 0, 14.3f), Vector3.Zero.WithY(105), true));
            towers.Add(new Tower(new Vector3(-43.5f, 0, 10f), Vector3.Zero.WithY(65), true));
            towers.Add(new Tower(new Vector3(-40.7f, 0, -17.4f), Vector3.Zero.WithY(130), true));
            towers.Add(new Tower(new Vector3(-57.1f, 0, -5.2f), Vector3.Zero.WithY(90), true));
            towers.Add(new Tower(new Vector3(57.1f, 0, -5.2f), Vector3.Zero.WithY(270), true));
            towers.Add(new Tower(new Vector3(41f, 0, -17.4f), Vector3.Zero.WithY(225), true));
            towers.Add(new Tower(new Vector3(44.1f, 0, 9.9f), Vector3.Zero.WithY(310), true));
            towers.Add(new Tower(new Vector3(-39.8f, 0, -4.3f), Vector3.Zero.WithY(265), true));
            towers.Add(new Tower(new Vector3(40.3f, 0, -4.4f), Vector3.Zero.WithY(85), true));
        }

        public bool IsPositionValid(Vector3 pos)
        {
            var res = Vec3ToGrid(pos);
            if (res.X < 0 || res.Y < 0)
            {
                return false;
            }

            return walkableGrid.grid[res.X, res.Y].Walkable;
        }

        public List<Vector3> CalculateMovement(Vector3 start, Vector3 end)
        {
            var res = walkableGrid.Search(Vec3ToGrid(start), Vec3ToGrid(end));
            res.Reverse();
            return res.Select(item => GridToVec3(item)).ToList();
        }

        public override void Awake()
        {
            Ground = new GameObject()
               .WithParent(gameObject)
               .WithName("Moba World Terrain")
               .WithScale(new Vector3(1, 1, MapRatio))
               .WithPosition(new Vector3(TerrainSize / 2f, 0, TerrainSize / (2f / MapRatio)))
               .AddComponent(Ground);

            for (int i = 0; i < towers.Count; i++)
            {
                towers[i] = new GameObject()
                    .WithParent(gameObject)
                    .WithName("Tower " + i)
                    .AddComponent(towers[i]);
            }
        }

        private Point Vec3ToGrid(Vector3 pos)
        {
            var progressX = (pos.X - (-MapX)) / (MapX * 2);
            var progressZ = (pos.Z - (-MapZ)) / (MapZ * 2);

            var pixelX = (int)((walkableMap.Width / factor) * progressX);
            var pixelY = (int)((walkableMap.Height / factor) * progressZ);

            return new Point(pixelX, pixelY);
        }

        private Vector3 GridToVec3(Point point)
        {
            var progressX = point.X / (float)walkableGrid.coloums;
            var progressY = point.Y / (float)walkableGrid.rows;

            var posX = MapX * 2 * progressX - MapX;
            var posZ = MapZ * 2 * progressY - MapZ;

            return new Vector3(posX, 0, posZ);
        }

        private float MapRatio => 1068f / 2048f;
        private float MapX => TerrainSize / 2f;
        private float MapZ => TerrainSize / (2f / MapRatio);

        private const int TerrainSize = 125;
        private Bitmap walkableMap;
        private NodeGrid walkableGrid;
        private int factor;
    }
}
