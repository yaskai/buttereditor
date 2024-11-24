using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace buttereditor
{
    public class Handler
    {
        public List<Button> buttons;
        public Handler()
        {
            buttons = new List<Button>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Button btn in buttons) btn.Update(gameTime);
            RemoveObjects();
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            foreach (Button btn in buttons) btn.Draw(_spriteBatch);
        }

        private void RemoveObjects()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (!buttons[i].isActive)
                {
                    buttons.RemoveAt(i);
                    i--;
                }
            }
        }

        public void ClearUI()
        {
            foreach (Button btn in buttons) btn.isActive = false;
        }
    }
}