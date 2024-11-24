using System.IO.Compression;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace buttereditor
{
    public class Button
    {
        public bool isActive = true;
        
        Game1 game;
        Handler handler;
        Spritesheet ss;

        public enum Id
        {
            empty,
            newLevel,
            loadLevel,
            addWidth,
            addHeight,
            subtractWidth,
            subtractHeight,
            save,
            exit
        };
        Id id = Id.empty;

        public Vector2 position;
        public Rectangle bounds;

        public Button(Vector2 position, Id id, Handler handler, Spritesheet ss, Game1 game)
        {
            this.position = position;
            this.id = id;
            this.handler = handler;
            this.ss = ss;
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            // set bounding box
            if (id.Equals(Id.addWidth) || id.Equals(Id.addHeight) ||
                id.Equals(Id.subtractWidth) || id.Equals(Id.subtractHeight) || 
                id.Equals(Id.save) || id.Equals(Id.exit))
            {
                bounds = new Rectangle((int)position.X, (int)position.Y, 64, 64);
            }
            else bounds = new Rectangle((int)position.X, (int)position.Y, 128, 64);

            if (!isActive)
            {
                bounds.Width = 0;
                bounds.Height = 0;
            }

            if (game.cursor.state.LeftButton == ButtonState.Pressed && game.cursor.buttonTimer <= 0)
            {
                if (this.bounds.Intersects(game.cursor.screenBounds))
                {
                    OnClick();
                    game.cursor.buttonTimer = 1;
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            switch (id)
            {
                case Id.empty:
                break;

                case Id.newLevel:
                _spriteBatch.Draw(ss.frames[0, 0], position, Color.White);
                _spriteBatch.Draw(ss.frames[1, 0], new Vector2(position.X + 64, position.Y), Color.White);
                break;

                case Id.loadLevel:
                _spriteBatch.Draw(ss.frames[0, 1], position, Color.White);
                _spriteBatch.Draw(ss.frames[1, 1], new Vector2(position.X + 64, position.Y), Color.White);
                break;

                case Id.addWidth:
                _spriteBatch.Draw(ss.frames[3, 1], position, Color.White);
                break;

                case Id.addHeight:
                _spriteBatch.Draw(ss.frames[3, 0], position, Color.White);
                break;

                case Id.subtractWidth:
                _spriteBatch.Draw(ss.frames[2, 1], position, Color.White);
                break;

                case Id.subtractHeight:
                _spriteBatch.Draw(ss.frames[2, 0], position, Color.White);
                break;

                case Id.save:
                break;

                case Id.exit:
                break;
            }
        }

        public void OnClick()
        {
            KeyboardState kb = Keyboard.GetState();

            switch (id)
            {
                case Id.empty:
                break;

                case Id.newLevel:
                isActive = false;
                //game.InitSetupMenu();
                game.timer = 1;
                game._nextState = Game1.State.Setup;
                break;

                case Id.loadLevel:
                isActive = false;
                game.tilemap.LoadMap();
                game.InitMain(1);
                break;

                case Id.addWidth:
                if (kb.IsKeyDown(Keys.LeftControl)) game.tilemap.mapWidth += 10;
                else game.tilemap.mapWidth++;
                break;

                case Id.addHeight:
                if (kb.IsKeyDown(Keys.LeftControl)) game.tilemap.mapHeight += 10;
                else game.tilemap.mapHeight++;
                break;

                case Id.subtractWidth:
                if (kb.IsKeyDown(Keys.LeftControl)) game.tilemap.mapWidth -= 10;
                else game.tilemap.mapWidth--;
                break;

                case Id.subtractHeight:
                if (kb.IsKeyDown(Keys.LeftControl)) game.tilemap.mapHeight -= 10;
                else game.tilemap.mapHeight--;
                break;

                case Id.save:
                break;

                case Id.exit:
                break;
            }
        }
    }
}