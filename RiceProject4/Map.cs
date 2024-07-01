using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct2D1.Effects;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace RiceProject4
{
    internal class Map : Drawable
    {

        public int width;
        public int height;
        public Node[,] Nodes { get; private set; }
        public enum GroundType
        {
            Road,
            Wall,
            Cross,
            Nothing,
            Rice
        }

        public static class GroundColors
        {
            public static Color Nothing = Color.White;
            public static Color Road = Color.Blue;
            public static Color Wall = Color.Yellow;
            public static Color Rice = Color.Black;
            public static Color Cross = new Color(255, 0, 0, 255);
        }

        public GroundType[,] grounds;

        public GroundType this[float x, float y]
        {
            get
            {
                x /= Scale.X;
                y /= Scale.Y;
                if (x < 0 || x >= grounds.GetLength(0) ||
                    y < 0 || y >= grounds.GetLength(1))
                    return GroundType.Wall;
                return grounds[(int)(x), (int)(y)];
            }
        }

        // Constructor that takes a texture and processes it to create a map.
        public Map(Texture2D tex) :
            this(Vector2.Zero, Vector2.Zero, tex)
        {
            this.width = tex.Width;
            this.height = tex.Height;
        }
        public Map(Vector2 position, Vector2 origin, Texture2D tex)
            : base(Vector2.Zero, Vector2.Zero, tex)
        {
            // Initialize the grounds array based on the texture size.
            grounds = new GroundType[tex.Width, tex.Height];

            // Extract color data from the texture.
            Color[] texColor = new Color[tex.Width * tex.Height];
            tex.GetData(texColor);

            // Process each pixel to determine the ground type.
            for (int x = 0; x < tex.Width; x++)
            {
                for (int y = 0; y < tex.Height; y++)
                {

                    if (texColor[x + y * tex.Width] == Color.Yellow)
                        grounds[x, y] = GroundType.Wall;
                    else if (texColor[x + y * tex.Width] == Color.Blue)
                        grounds[x, y] = GroundType.Road;
                    //else if (texColor[x + y * tex.Width] == GroundColors.Rice)
                    //    grounds[x, y] = GroundType.Rice;
                    else if (texColor[x + y * tex.Width] == new Color(255, 0, 0, 255))
                        grounds[x, y] = GroundType.Cross;
                    else if (texColor[x + y * tex.Width] == Color.White)
                        grounds[x, y] = GroundType.Nothing;
                }
            }
        }
        public bool IsValidRoad(Vector2 position)
        {
            int x = (int)(position.X / Scale.X);
            int y = (int)(position.Y / Scale.Y);
            // Check if within bounds and the tile is a road or an edge.
            return x >= 0 && x < width && y >= 0 && y < height &&
                    (grounds[x, y] == GroundType.Road || grounds[x, y] == GroundType.Cross);
        }

        public bool IsCross(Vector2 position)
        {
            int x = (int)(position.X / Scale.X);
            int y = (int)(position.Y / Scale.Y);

            if (x < 0 || y < 0 || x >= width || y >= height)
                return false; // Out of bounds

            return grounds[x, y] == GroundType.Cross;
        }


        
        public void InitializeNodes()
        {
            Nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Nodes[x, y] = new Node(Position = new Vector2(x, y), grounds[x, y] == GroundType.Road || grounds[x, y] == GroundType.Cross);                 
                }
            }
        }


        public IEnumerable<Node> GetNeighbors(Node node)
        {
            var directions = new List<Vector2>
            {
                new Vector2(0, -1), // Up
                new Vector2(1, 0),  // Right
                new Vector2(0, 1),  // Down
                new Vector2(-1, 0)  // Left
            };

            foreach (var direction in directions)
            {
                var newX = (int)(node.Position.X + direction.X);
                var newY = (int)(node.Position.Y + direction.Y);
                if (newX >= 0 && newX < width && newY >= 0 && newY < height && Nodes[newX, newY].IsWalkable)
                {
                    yield return Nodes[newX, newY];
                }
            }
        }


    }
}
