using System.Collections.Generic;

namespace Senapp.Engine.Utilities.Pathfinding
{
    public class Node
    {
        public int X;
        public int Y;
        public bool Walkable;

        public List<Node> Neighbours = new();
        public int G;
        public int H;
        public Node Last;

        public int Value => G + H;

        public Node(int x, int y, bool walkable)
        {
            X = x;
            Y = y;
            Walkable = walkable;
        }
    }
}
