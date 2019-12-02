using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NuggetNugget2
{
    public class ChatBox
    {
        Rectangle chatBoxRectangle;
        Rectangle inputBoxRectangle;
        Rectangle blinkerRectangle;
        Rectangle deletionRectangle;
        Texture2D chatBoxTexture;
        Texture2D inputBoxTexture;
        Texture2D blinkerTexture;
        List<ChatMessage> messages;
        SpriteFont font;
        float blinkerOpacity = 1.0f;
        float chatBoxActiveOpacity = 0.6f;
        float chatBoxInactiveOpacity = 0.2f;
        bool blinkerShowing = true;
        bool chatBoxActive = false;
        bool disableBoxOnNext = false;
        int blinkRateMs = 350;
        double msElapsed = 0;
        string inputString = "";
        Networker networker;
        
        public ChatBox(GraphicsDevice graphicsDevice, SpriteFont font, GameWindow window, Networker networker)
        {
            this.font = font;
            window.TextInput += Window_TextInput;
            messages = new List<ChatMessage>();
            chatBoxRectangle = new Rectangle(0,260,400,200);
            chatBoxTexture = Utils.CreateTexture(graphicsDevice, 1, 1, pixel => Color.Black);
            inputBoxTexture = Utils.CreateTexture(graphicsDevice, 1, 1, pixel => Color.Black);
            blinkerTexture = Utils.CreateTexture(graphicsDevice, 1, 1, pixel => Color.White);
            

            inputBoxRectangle = new Rectangle(0, 460, 400, 20);
            blinkerRectangle = new Rectangle(inputBoxRectangle.X, inputBoxRectangle.Y, 2, inputBoxRectangle.Height);
            deletionRectangle = new Rectangle(chatBoxRectangle.X, chatBoxRectangle.Y - 50, chatBoxRectangle.Width, 50);
            this.networker = networker;
        }

        public List<ChatMessage> GetMessages()
        {
            return this.messages;
        }

        public void HandleForeignMessage(ChatMessage chatMessage)
        {
            messages.Add(chatMessage);
        }

        private string wrappifyText(string text)
        {
            string completeString = "";
            string progressString = "";
            bool lastIterationAddedString = false;
            for(int i = 0; i < text.Length; i++)
            {
                string candidateProgressString = progressString + text[i];
                int length = (int)font.MeasureString(candidateProgressString + "-").X; // + " " so it leaves some space at the end
                if(length >= chatBoxRectangle.Width)
                {
                    // String became too long.
                    // Candidate string is invalid.
                    // --->
                    completeString += progressString + "\n";
                    progressString = text[i].ToString(); // Start of our new line.
                    lastIterationAddedString = true;
                }
                else
                {
                    // This addition to our progress string is valid!
                    progressString = candidateProgressString;
                    lastIterationAddedString = false;
                }
            }

            if (!lastIterationAddedString)
                completeString += progressString;

            return completeString;
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (chatBoxActive)
            {
                if (e.Character == (char)Keys.Back)
                {
                    if (inputString != "")
                    {
                        inputString = inputString.Remove(inputString.Length - 1);
                    }
                }
                else if (e.Character == (char)Keys.Tab)
                {
                    inputString += "    ";
                }
                else if (e.Character == (char)Keys.Escape)
                {
                    disableBoxOnNext = true;
                }
                else if(e.Character == (char)Keys.Enter)
                {
                    ChatMessage newChatMessage = new ChatMessage(inputString, "Player");
                    messages.Add(newChatMessage);

                    // Send over the networking:
                    networker.SendChatMessage(newChatMessage);

                    inputString = "";
                }
                else
                {
                    inputString += e.Character;
                }

                for(blinkerRectangle.X = (int)font.MeasureString(inputString).X; !inputBoxRectangle.Contains(blinkerRectangle);)
                {
                    inputString = inputString.Remove(inputString.Length - 1);
                    blinkerRectangle.X = (int)font.MeasureString(inputString).X;
                }
            }
        }

        public void Update(GameTime gameTime, ref bool chatBoxActive)
        {
            if (disableBoxOnNext)
            {
                chatBoxActive = false;
                disableBoxOnNext = false;
            }
            this.chatBoxActive = chatBoxActive;
            msElapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
            if(msElapsed >= blinkRateMs)
            {
                msElapsed = 0;
                blinkerShowing = !blinkerShowing;
                if (!chatBoxActive) blinkerShowing = true;
            }

            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            var keyboardState = Keyboard.GetState();
            var keys = keyboardState.GetPressedKeys();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if(chatBoxRectangle.Contains(mousePosition) || inputBoxRectangle.Contains(mousePosition))
                {
                    chatBoxActive = true;
                }
                else
                {
                    chatBoxActive = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(chatBoxTexture, chatBoxRectangle, Color.White * (chatBoxActive ? chatBoxActiveOpacity : chatBoxInactiveOpacity));
            
            float totalHeight = 0;

            for(int i = messages.Count - 1; i >= 0; i--)
            {
                ChatMessage chatMessage = messages[i];
                string printString = wrappifyText(chatMessage.timestamp.ToShortTimeString() + ":" + chatMessage.author + ": " + chatMessage.message);
                totalHeight += font.MeasureString(printString).Y;
                Vector2 newMessagePosition = new Vector2(chatBoxRectangle.X, chatBoxRectangle.Y + chatBoxRectangle.Height - totalHeight);
                if (deletionRectangle.Contains(newMessagePosition)) break;
                spriteBatch.DrawString(font, printString, newMessagePosition, Color.White);
            }
            spriteBatch.Draw(inputBoxTexture, inputBoxRectangle, Color.White * 0.8f);
            blinkerRectangle.X = (int)font.MeasureString(inputString).X;
            spriteBatch.Draw(blinkerTexture, blinkerRectangle, Color.White * (blinkerShowing ? 1.0f : 0.0f));
            spriteBatch.DrawString(font, inputString, new Vector2(inputBoxRectangle.X, inputBoxRectangle.Y), Color.White);
        }
    }
}