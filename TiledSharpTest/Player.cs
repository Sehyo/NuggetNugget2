using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NuggetNugget2
{
    public class Player
    {
        public int PID = -1;
        Texture2D playerTexture;
        public Rectangle playerRectangle = new Rectangle(0, 0, 69 / 2, 130 / 2);

        public void setPlayerTexture(Texture2D playerTexture)
        {
            this.playerTexture = playerTexture;
        }

        public Texture2D GetPlayerTexture()
        {
            return playerTexture;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            float speed = 0.2f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (state.IsKeyDown(Keys.W)) playerRectangle.Y -= (int)speed;
            if (state.IsKeyDown(Keys.A)) playerRectangle.X -= (int)speed;
            if (state.IsKeyDown(Keys.S)) playerRectangle.Y += (int)speed;
            if (state.IsKeyDown(Keys.D)) playerRectangle.X += (int)speed;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(playerRectangle.X, playerRectangle.Y);
        }

        public void SetPosition(int x, int y)
        {
            playerRectangle.X = x;
            playerRectangle.Y = y;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 localPos = Camera.GlobalPosToLocalPos(new Vector2(playerRectangle.X, playerRectangle.Y));
            spriteBatch.Draw(playerTexture, new Rectangle((int)localPos.X, (int)localPos.Y, playerRectangle.Width, playerRectangle.Height), Color.White);
        }
    }
}
