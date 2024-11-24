using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace buttereditor
{
    public class ScrollBar
    {
        public bool isHidden = false;

        Game1 game;
        Handler handler;

        public enum Type
        {
            Horizontal,
            Vertical
        };

        Type type;

        public ScrollBar(Type type, Game1 game, Handler handler)
        {
            this.type = type;
            this.game = game;
            this.handler = handler;

            Init();
        }

        public void Init()
        {
            if (type == Type.Horizontal)
            {

            }
            else if (type == Type.Vertical)
            {
                
            }
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch _spriteBatch)
        {

        }
    }
}