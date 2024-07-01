using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RiceProject4
{
    internal class Rice : Drawable
    {
        public bool IsTargeted { get; set; } // is a tractor already targeted this

        public bool IsCollected { get; private set; }

        public Rice(Vector2 position, Texture2D texture,Vector2 scale)
            : base(position, new Vector2(texture.Width / 2, texture.Height / 2), texture,scale)
        {
            IsCollected = false;
        }

        public void Collect()
        {
            IsCollected = true;
        }

    }
}
