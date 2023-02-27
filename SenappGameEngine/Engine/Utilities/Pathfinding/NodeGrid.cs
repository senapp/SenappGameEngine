using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Senapp.Engine.Utilities.Pathfinding
{
    public class NodeGrid
    {
        public Node[,] grid;
        public readonly int coloums;
        public readonly int rows;
        
        public NodeGrid(int coloums, int rows)
        {
            grid = new Node[coloums, rows];
            this.coloums = coloums;
            this.rows = rows;
        }

        public List<Point> Search(Point start, Point end)
        {
            List<Node> path = new();
            List<Node> openSet = new();
            List<Node> closedSet = new();

            openSet.Add(grid[start.X, start.Y]);
            var endNode = grid[end.X, end.Y];
            Node current = null;
            while (openSet.Count > 0)
            {
                current = openSet[0];
                foreach (var open in openSet)
                    if (open.Value < current.Value || current.Value == 0)
                        current = open;

                if (current == endNode)
                {
                    break;
                }

                openSet.Remove(current);
                closedSet.Add(current);

                var neighbours = GetNeighbours(current);
                foreach (var neighbour in neighbours)
                {
                    if (neighbour != null && neighbour.Walkable && !closedSet.Contains(neighbour))
                    {
                        if (openSet.Contains(neighbour))
                        {
                            if (GetGValue(current, neighbour) < neighbour.G)
                            {
                                EvaluteNeighbour(current, neighbour, endNode);
                                neighbour.Last = current;
                            }
                        }
                        else
                        {
                            EvaluteNeighbour(current, neighbour, endNode);
                            openSet.Add(neighbour);
                            neighbour.Last = current;
                        }
                    }
                }
            }

            var temp = current;
            path.Add(temp);
            while (temp.Last != null)
            {
                path.Add(temp.Last);
                temp = temp.Last;
            }

            for (int x = 0; x < coloums; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    grid[x, y].G = 0;
                    grid[x, y].H = 0;
                    grid[x, y].Last = null;
                }
            }
            return path.Select(node => new Point(node.X, node.Y)).ToList();
        }

        private void EvaluteNeighbour(Node current, Node neighbour, Node end)
        {
            if (Math.Abs(neighbour.X - current.X) + Math.Abs(neighbour.Y - current.Y) == 2) neighbour.G = current.G + 14;
            else neighbour.G = current.G + 10;

            var xDis = Math.Abs(end.X - neighbour.X);
            var yDis = Math.Abs(end.Y - neighbour.Y);

            if (xDis == yDis) neighbour.H = xDis * 14;
            else
            {
                var max = Math.Max(xDis, yDis);
                var min = Math.Min(xDis, yDis);
                neighbour.H = Math.Abs(max - (max - min) * 14) + Math.Abs((max - min) * 10);
            }
        }

        private int GetGValue(Node current, Node neighbour)
        {
            if (Math.Abs(neighbour.X - current.X) + Math.Abs(neighbour.Y - current.Y) == 2) return current.G + 14;
            else return current.G + 10;
        }
        private Node[] GetNeighbours(Node node)
        {
            // TOPLEFT LEFT BOTTOMLEFT BOTTOM BOTTOMRIGHT RIGHT TOPRIGHT TOP
            Node[] neighbours = new Node[8];

            if (node.X - 1 >= 0)
            {
                if (node.Y - 1 >= 0) neighbours[0] = grid[node.X - 1, node.Y - 1];

                neighbours[1] = grid[node.X - 1, node.Y];

                if (node.Y + 1 < rows) neighbours[2] = grid[node.X - 1, node.Y + 1];
            }

            if (node.Y + 1 < rows) neighbours[3] = grid[node.X, node.Y + 1];

            if (node.X + 1 < coloums)
            {
                if (node.Y + 1 < rows) neighbours[4] = grid[node.X + 1, node.Y + 1];
                else neighbours[4] = UnwalkableNode;

                neighbours[5] = grid[node.X + 1, node.Y];

                if (node.Y - 1 >= 0) neighbours[6] = grid[node.X + 1, node.Y - 1];
                else neighbours[6] = UnwalkableNode;
            }

            if (node.Y - 1 >= 0) neighbours[7] = grid[node.X, node.Y - 1];

            return neighbours;
        }

        private Node UnwalkableNode = new Node(0, 0, false);
    }
}
