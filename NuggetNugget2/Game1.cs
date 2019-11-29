using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;
using System.Collections.Generic;

namespace NuggetNugget2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        
        

        TmxMap map;
        Texture2D tileSet;

        

        Player player = new Player();
        List<Player> otherPlayers = new List<Player>();

        ChatBox chatBox;
        bool chatBoxActive = false;

        Networker networker;

        int tileWidth;
        int tileHeight;
        int tileSetTilesWide;
        int tileSetTilesHigh;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            networker = new Networker(player, otherPlayers);
            base.Initialize();

            chatBox = new ChatBox(graphics.GraphicsDevice, this.Content.Load<SpriteFont>("NuggetText"), this.Window, networker);
            networker.SetChatBox(chatBox);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.setPlayerTexture(this.Content.Load<Texture2D>("nugget"));
            map = new TmxMap("Content/testExported.tmx");
            tileSet = Content.Load<Texture2D>(map.Tilesets[0].Name.ToString());
            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tileSetTilesWide = tileSet.Width / tileWidth;
            tileSetTilesHigh = tileSet.Height / tileHeight;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here :)
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        int elapsedMsSinceNetworkUpdate = 100;
        int networkMsLimit = 20;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            chatBox.Update(gameTime, ref chatBoxActive);
            player.Update(gameTime, chatBoxActive);
            
            Camera.position = player.GetPosition();

            if (elapsedMsSinceNetworkUpdate >= networkMsLimit)
            {
                elapsedMsSinceNetworkUpdate = 0;
                networker.Update();
            }
            else elapsedMsSinceNetworkUpdate += gameTime.ElapsedGameTime.Milliseconds;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            for (int layer = 0; layer < map.TileLayers.Count; layer++)
            {
                for (int i = 0; i < map.TileLayers[layer].Tiles.Count; i++)
                {
                    int gid = map.TileLayers[layer].Tiles[i].Gid;
                    if (gid == 0) continue; // Empty tile.
                    int tileFrame = gid - 1;
                    int column = tileFrame % tileSetTilesWide;
                    int row = (int)System.Math.Floor((double)tileFrame / (double)tileSetTilesWide);
                    float x = (i % map.Width) * map.TileWidth;
                    float y = (float)System.Math.Floor(i / (double)map.Width) * map.TileHeight;

                    Vector2 localPos = Camera.GlobalPosToLocalPos(new Vector2(x, y));
                    
                    //System.Console.WriteLine("---\nTile Frame: {0}\nColumn: {1}\nRow: {2}\nx: {3}, y: {4}", tileFrame, column, row, x, y);

                    Rectangle tileSetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                    spriteBatch.Draw(tileSet, new Rectangle((int)localPos.X, (int)localPos.Y, tileWidth, tileHeight), tileSetRec, Color.White);
                }
            }

            foreach (var otherPlayer in otherPlayers)
            {
                Vector2 localPos = Camera.GlobalPosToLocalPos(new Vector2(otherPlayer.GetPosition().X, otherPlayer.GetPosition().Y));
                spriteBatch.Draw(otherPlayer.GetPlayerTexture(), new Rectangle((int)localPos.X, (int)localPos.Y, otherPlayer.playerRectangle.Width, otherPlayer.playerRectangle.Height), Color.Blue);
            }

            player.Draw(gameTime, spriteBatch);
            chatBox.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}