using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace RiceProject4
{
    internal class Drawable
    {

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle? TexPart { get; set; }
        public Vector2 Scale { get; set; }
        public float Layer { get; set; }
        public Color Color { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Flip { get; set; }

        public Drawable(Vector2 position, Vector2 origin, Texture2D texture)
        {
            this.Position = position;
            this.Texture = texture;
            this.Origin = origin;

            Rotation = 0;
            TexPart = null;
            Scale = Vector2.One;
            Layer = 0;
            Color = Color.White;
            Flip = SpriteEffects.None;

        }
        public Drawable(Vector2 position, Vector2 origin, Texture2D texture, Vector2 scale)
        {
            this.Position = position;
            this.Texture = texture;
            this.Origin = origin;
            this.Scale = scale;

            Rotation = 0;
            TexPart = null;
            Layer = 0;
            Color = Color.White;
            Flip = SpriteEffects.None;

        }
        public int getTextureHeight() { return Texture.Height; }
        public int getTextureWidth() { return Texture.Width; }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, TexPart, Color, Rotation, Origin, Scale, Flip, Layer);
        }
    }
}
