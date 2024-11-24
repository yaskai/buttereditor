using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace buttereditor 
{
    public class Camera
    {
        Game1 game;
        Handler handler;
        Tilemap tilemap;

        public Matrix transform { get; private set; }
        public Matrix inverseTransform { get; private set; }
        public Matrix scale { get; private set; }

        public Vector2 position;
        public Vector2 target;
        public float zoom = 1;
        public float scaleX;
        public float scaleY;

        public Camera(Game1 game, Handler handler, Tilemap tilemap)
        {
            this.game = game;
            this.handler = handler;
            this.tilemap = tilemap;
        }

        public void Update(GameTime gameTime)
        {
            if (position.X < -128) position.X = -128;
            if (position.X > (tilemap.mapWidth + 2) * 64 - game.realWidth) position.X = (tilemap.mapWidth + 2) * 64 - game.realWidth;
            if (position.Y < -128) position.Y = -128;
            if (position.Y > (tilemap.mapHeight + 2) * 64 - game.realHeight) position.Y = (tilemap.mapHeight + 2) * 64 - game.realHeight;

            scaleX = Math.Abs((float)1 / zoom);
            scaleY = Math.Abs((float)1 / zoom);
            var scale = Matrix.CreateScale(scaleX, scaleY, 0);
            var inverseScale = Matrix.Invert(scale);

            
            var offset = Matrix.CreateTranslation(0, 0, 0);
            var pos = transform = Matrix.CreateTranslation(
            -position.X,
            -position.Y,
            0);
            
            transform = pos * offset * scale;
            inverseTransform = Matrix.Invert(transform);
            this.scale = scale;
        }
    }
}