using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NuggetNugget2
{
    public static class Camera
    {
        public static Vector2 position = new Vector2(0, 0); // Offset..................
        

        public static Vector2 GlobalPosToLocalPos(Vector2 globalPos)
        {

            return globalPos - position + new Vector2(320, 240);
        }
    }
}