using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace RiceProject4
{
    public class Node
    {
        public Vector2 Position; // The position in the grid
        public Node Parent; // Parent node for path tracing
        public bool IsWalkable; // If the node is walkable (not a wall)
        public float Cost; // Cost from start node
        public float Heuristic; // Heuristic - estimated cost from this node to the goal
        public float Total => Cost + Heuristic; // Total cost (F = G + H)

        // Constructor
        public Node(Vector2 position, bool isWalkable)
        {
            this.Position = position;
            this.IsWalkable = isWalkable;
            this.Parent = null;
            this.Cost = float.MaxValue; // We start with maximum cost
            this.Heuristic = 0;
        }
        public Node(Vector2 position, bool isWalkable, float cost, float heuristic, Node parent)
        {
            this.Position = position;
            this.IsWalkable = isWalkable;
            this.Parent = parent;
            this.Cost = cost; // We start with maximum cost
            this.Heuristic = heuristic;
        }
    }
}
