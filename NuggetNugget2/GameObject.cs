using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NuggetNugget2
{
    public class GameObject
    {
        public Texture2D objectTexture;
        public Rectangle objectRectangle;
        public GameObject()
        {

        }

        public Vector2 GetPosition()
        {
            return new Vector2(objectRectangle.X, objectRectangle.Y);
        }
    }
}
