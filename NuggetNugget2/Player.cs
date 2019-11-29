using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NuggetNugget2
{
    public class Player : GameObject
    {
        public int PID = -1;
        
        public Player()
        {
            base.objectRectangle = new Rectangle(100, 100, 35, 65);
        }

        public void Update(GameTime gameTime, bool chatBoxActive)
        {
            KeyboardState state = Keyboard.GetState();
            float speed = 0.2f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!chatBoxActive)
            {
                if (state.IsKeyDown(Keys.W)) base.objectRectangle.Y -= (int)speed;
                if (state.IsKeyDown(Keys.A)) base.objectRectangle.X -= (int)speed;
                if (state.IsKeyDown(Keys.S)) base.objectRectangle.Y += (int)speed;
                if (state.IsKeyDown(Keys.D)) base.objectRectangle.X += (int)speed;
            }
        }

        public void SetPosition(int x, int y)
        {
            objectRectangle.X = x;
            objectRectangle.Y = y;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 localPos = Camera.GlobalPosToLocalPos(new Vector2(objectRectangle.X, objectRectangle.Y));
            spriteBatch.Draw(objectTexture, new Rectangle((int)localPos.X, (int)localPos.Y, objectRectangle.Width, objectRectangle.Height), Color.White);
        }
    }
}