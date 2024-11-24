using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// what da hellllllll

namespace buttereditor 
{
    public class Cursor
    {
        Game1 game;
        Handler handler;
        Spritesheet ss;

        public Vector2 screenPosition = Vector2.Zero;
        public Vector2 worldPosition = Vector2.Zero;

        public Rectangle screenBounds;
        public Rectangle worldBounds;

        public MouseState state;
        public float buttonTimer = 0;
        public bool prevReleased = true;
         
        public int tileMapX = 0;
        public int tileMapY = 0;

        private Vector2 dragPos = Vector2.Zero;
        private bool DRAG = false;

        private int prevWheelPos;
        private float switchObjTimer;
        public float uniqueObjTimer;

        public int layer = 1;
        private float switchLayerTimer = 10;
        private float switchColorTimer = 10;
        
        public bool altView = false;

        public enum State
        {
            Pointer,
            Pencil,
            Eraser
        };

        public State _state = State.Pointer;
        private State _prevState = State.Pointer;

        public Tilemap.Object selected = Tilemap.Object.Tile;
        private bool inBounds = true;
        private bool isAddValid = true;
        public float startTimer = 5;

        public int orientation = 0;
        private float switchOrientationTimer = 0;


        public Cursor(Game1 game, Handler handler, Spritesheet ss)
        {
            this.game = game;
            this.handler = handler;
            this.ss = ss;

            MouseState mouse = Mouse.GetState();
            prevWheelPos = mouse.ScrollWheelValue;
        }

        public void Update(GameTime gameTime)
        {
            altView = false;

            prevReleased = true;
            _prevState = _state;

            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            state = ms;

            if (ms.LeftButton == ButtonState.Released) 
            {
                buttonTimer--;
                uniqueObjTimer--;
            }

            if (buttonTimer <= 0) buttonTimer = 0;
            if (uniqueObjTimer <= 0) uniqueObjTimer = 0;

            if (kb.IsKeyUp(Keys.Q) && kb.IsKeyUp(Keys.E)) switchObjTimer --;
            if (switchObjTimer <= 0) switchObjTimer = 0;

            if (kb.IsKeyUp(Keys.Q) && kb.IsKeyUp(Keys.E)) switchColorTimer --;
            if (switchColorTimer <= 0) switchColorTimer = 0;

            if (kb.IsKeyUp(Keys.L)) switchLayerTimer --;
            if (switchLayerTimer <= 0) switchLayerTimer = 0;
            if (kb.IsKeyDown(Keys.L) && switchLayerTimer <= 0)
            {
                switchLayerTimer = 10;
                if (layer == 0)
                {
                    layer = 1;
                }
                else if (layer == 1)
                {
                    layer = 0;
                }
            }

            if (kb.IsKeyUp(Keys.R)) switchOrientationTimer --;

            screenPosition.X = ms.X - 32;
            screenPosition.Y = ms.Y - 32;

            worldPosition.X = (screenPosition.X + game.camera.position.X);
            worldPosition.Y = (screenPosition.Y + game.camera.position.Y);

            //change to world positon later to account for camera transformations
            tileMapX = (int)Math.Round(worldPosition.X / 64, 0);
            tileMapY = (int)Math.Round(worldPosition.Y / 64, 0);

            screenBounds = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, 16, 16);

            if (game._state == Game1.State.Main)
            {
                inBounds = false;

                startTimer--;
                if (startTimer <= 0) startTimer = 0;

                if (startTimer > 0) isAddValid = false;

                if (tileMapX <= game.tilemap.mapWidth - 1 && tileMapX >= 0 &&
                    tileMapY <= game.tilemap.mapHeight - 1 && tileMapY >= 0)
                {
                    inBounds = true;
                    if (ms.LeftButton == ButtonState.Pressed && isAddValid) game.tilemap.AddObject(tileMapX, tileMapY, selected);
                    if (kb.IsKeyDown(Keys.Back) ||
                        (ms.LeftButton == ButtonState.Pressed
                         && _state == State.Eraser))
                         {
                            if (layer == 0) game.tilemap.DeleteBG(tileMapX, tileMapY);
                            else if (layer == 1) game.tilemap.Delete(tileMapX, tileMapY);
                         }
                }                

                if (ms.RightButton == ButtonState.Pressed)
                {
                    //Vector2 cameraCenter = Vector2.Zero;
                    //cameraCenter.X = game.camera.position.X - (game.realWidth / 2);
                    //cameraCenter.Y = game.camera.position.Y - (game.realWidth / 2);

                    if (!DRAG) 
                    {
                        DRAG = true;
                        dragPos = worldPosition;
                    }
                    else if (DRAG)
                    {
                        float vx = dragPos.X - worldPosition.X;
                        float vy = dragPos.Y - worldPosition.Y;

                        game.camera.position.X += vx;
                        game.camera.position.Y += vy;
                    }

                    _state = State.Pointer;
                }
                else if (ms.RightButton == ButtonState.Released)
                {
                    if (DRAG)
                    {
                        float vx = game.camera.position.X + (dragPos.X - worldPosition.X);
                        float vy = game.camera.position.Y + (dragPos.Y - worldPosition.Y);

                        game.camera.position = new Vector2(vx, vy);
                    }

                    DRAG = false;
                }

                if (kb.IsKeyDown(Keys.Left))
                {
                    game.camera.position.X -= 16;
                }
                else if (kb.IsKeyDown(Keys.Right))
                {
                    game.camera.position.X += 16;
                }

                if (kb.IsKeyDown(Keys.Up))
                {
                    game.camera.position.Y -= 16;
                }
                else if (kb.IsKeyDown(Keys.Down))
                {
                    game.camera.position.Y += 16;
                }
                
                if (ms.ScrollWheelValue > prevWheelPos)
                {
                    //game.camera.zoom -= 0.1f
                    if (ms.MiddleButton == ButtonState.Released)
                    {
                        game.camera.position.X -= 64;
                    }
                    else
                    {
                        game.camera.position.Y -= 64;
                        DRAG = false;
                    }
                    
                }
                else if (ms.ScrollWheelValue < prevWheelPos)
                {
                    //game.camera.zoom += 0.1f;
                    if (ms.MiddleButton == ButtonState.Released)
                    {
                        game.camera.position.X += 64;
                    }
                    else 
                    {
                        game.camera.position.Y += 64;
                        DRAG = false;
                    }
                }
                prevWheelPos = ms.ScrollWheelValue;

                if (switchObjTimer <= 0 && kb.IsKeyUp(Keys.LeftAlt))
                {
                    if (kb.IsKeyDown(Keys.Q))
                    {
                        //selected--;
                        if (selected == 0) selected = Tilemap.objCount + Tilemap.Object.None;
                        else selected --;
                        switchObjTimer = 1;
                    }

                    if (kb.IsKeyDown(Keys.E))
                    {
                        //selected++;
                        if (selected == Tilemap.objCount + Tilemap.Object.None) selected = 0;
                        else selected ++;
                        switchObjTimer = 1;
                    }
                }

                if (kb.IsKeyDown(Keys.D0))
                {
                    selected = 0;
                }
                else if (kb.IsKeyDown(Keys.D1))
                {
                    selected = Tilemap.Object.None + 1;
                }
                else if (kb.IsKeyDown(Keys.D2))
                {
                    selected = Tilemap.Object.None + 2;
                }
                else if (kb.IsKeyDown(Keys.D3))
                {
                    selected = Tilemap.Object.None + 3;
                }
                else if (kb.IsKeyDown(Keys.D4))
                {
                    selected = Tilemap.Object.None + 4;
                }
                else if (kb.IsKeyDown(Keys.D5))
                {
                    selected = Tilemap.Object.None + 5;
                }
                else if (kb.IsKeyDown(Keys.D6))
                {
                    selected = Tilemap.Object.None + 6;
                }
                else if (kb.IsKeyDown(Keys.D7))
                {
                    selected = Tilemap.Object.None + 7;
                }
                else if (kb.IsKeyDown(Keys.D8))
                {
                    selected = Tilemap.Object.None + 8;
                }
                else if (kb.IsKeyDown(Keys.D9))
                {
                    selected = Tilemap.Object.None + 9;
                }
                
                if (kb.IsKeyDown(Keys.LeftAlt))
                {
                    altView = true;

                    if (switchColorTimer <= 0)
                    {
                        if (kb.IsKeyDown(Keys.Q))
                        {
                            game.gridColor --;
                            if (game.gridColor < 0) game.gridColor = 31;
                        
                            switchColorTimer = 5;
                        }
                        else if (kb.IsKeyDown(Keys.E))
                        {
                            game.gridColor ++;
                            if (game.gridColor > 31) game.gridColor = 0;

                            switchColorTimer = 5;
                        }
                    }
                }

                if (kb.IsKeyDown(Keys.R) && switchOrientationTimer <= 0)
                {
                    orientation ++;

                    if (orientation > 1) orientation = 0;

                    switchOrientationTimer = 2;
                }

                ManageState();
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (DRAG) _spriteBatch.Draw(ss.frames[1, 0], screenPosition, Color.White);
            else if (!DRAG)
            {
                switch (_state)
                {
                    case State.Pointer:
                    _spriteBatch.Draw(ss.frames[0, 0], screenPosition, Color.White);
                    break;

                    case State.Pencil:
                    _spriteBatch.Draw(ss.frames[0, 0], screenPosition, Color.White);
                    break;

                    case State.Eraser:
                    _spriteBatch.Draw(ss.frames[2, 0], screenPosition, Color.White);
                    break;
                }
            }
            
            //_spriteBatch.DrawString(game.font, "X: " + tileMapX, new Vector2(screenPosition.X, screenPosition.Y + 16), Color.LimeGreen);
            //_spriteBatch.DrawString(game.font, "Y: " + tileMapY, new Vector2(screenPosition.X, screenPosition.Y + 32), Color.LimeGreen);
        }

        public void TransformDraw(SpriteBatch _spriteBatch)
        {
            Color c = Color.White;
            if (!isAddValid) c = Color.Red;

            if (layer == 0) 
            {
                if (_state == State.Pencil) selected = Tilemap.Object.BackGroundTile;
            }

            if (_state == State.Eraser)
            {
                if (!DRAG)
                {
                    _spriteBatch.Draw(game.pix.frames[12, 0], new Rectangle(tileMapX * 64, tileMapY * 64, 64, 64), Color.White * 0.4f);
                }
            }

            if (_state == State.Pencil)
            {
                Vector2 objDrawPos = new Vector2(tileMapX * 64, tileMapY * 64);

                switch (selected)
                {
                    case Tilemap.Object.None:
                    break;

                    case Tilemap.Object.Tile:
                    _spriteBatch.Draw(game.tileset_ss.frames[3, 3], objDrawPos, c * 0.5f);
                    break;

                    case Tilemap.Object.Slope:
                    if (orientation == 0)
                    {
                        _spriteBatch.Draw(game.tileset_ss.frames[0, 6], objDrawPos, c * 0.5f);
                        _spriteBatch.Draw(game.tileset_ss.frames[1, 6], new Vector2(objDrawPos.X + 64, objDrawPos.Y), c * 0.5f);
                    }
                    else if (orientation == 1)
                    {
                        _spriteBatch.Draw(game.tileset_ss.frames[2, 6], objDrawPos, c * 0.5f);
                        _spriteBatch.Draw(game.tileset_ss.frames[3, 6], new Vector2(objDrawPos.X + 64, objDrawPos.Y), c * 0.5f);
                    }
                    else if (orientation == 2)
                    {

                    }
                    else if (orientation == 3)
                    {

                    }
                    break;

                    case Tilemap.Object.Spike:
                    if (inBounds && tileMapY > 1 && tileMapY < game.tilemap.mapHeight)
                    {
                        if (game.tilemap.mapData[tileMapX, tileMapY - 1] == '1')
                        {
                            _spriteBatch.Draw(game.misc_ss.frames[3, 0], objDrawPos, c * 0.5f);
                        }
                        else _spriteBatch.Draw(game.misc_ss.frames[2, 0], objDrawPos, c * 0.5f);
                    }
                    else _spriteBatch.Draw(game.misc_ss.frames[2, 0], objDrawPos, c * 0.5f);
                    break;

                    case Tilemap.Object.Player:
                    _spriteBatch.Draw(game.misc_ss.frames[0, 0], objDrawPos, c * 0.5f);
                    break;

                    case Tilemap.Object.Tree:
                    _spriteBatch.Draw(game.tree00, objDrawPos, c * 0.5f);
                    break;

                    case Tilemap.Object.Enemy:
                    _spriteBatch.Draw(game.misc_ss.frames[1, 0], objDrawPos, c * 0.5f);
                    break;

                    case Tilemap.Object.BackGroundTile:
                    _spriteBatch.Draw(game.bgTileset_ss.frames[3, 3], objDrawPos, c * 0.5f);
                    break;

                    case Tilemap.Object.FragileBlock:
                    _spriteBatch.Draw(game.fragileBlock_ss.frames[0, 0], objDrawPos, c * 0.5f);
                    break;

                    case Tilemap.Object.Door:
                    _spriteBatch.Draw(game.doorImage, objDrawPos, c * 0.5f);
                    break;
                }
            }
        }

        public void ManageState()
        {
            isAddValid = false;

            if (game._state != Game1.State.Main)
            {
                _state = State.Pointer;
            }
            else
            {
                if (selected == Tilemap.Object.None) _state = State.Eraser;
                else if (!DRAG) _state = State.Pencil;
                else _state = State.Pointer;
            }

            if (_state == State.Pencil)
            {
                int x = tileMapX;
                int y = tileMapY;

                if (!inBounds)
                    {
                        isAddValid = false;
                    }
                    else
                    {
                        if (game.tilemap.mapData[x, y].Equals('1'))
                        {
                            switch (selected)
                            {
                                case Tilemap.Object.Tile:
                                isAddValid = true;
                                break;

                                case Tilemap.Object.Slope:
                                isAddValid = false;
                                break;

                                case Tilemap.Object.Player:
                                isAddValid = false;
                                break;

                                case Tilemap.Object.Spike:
                                isAddValid = false;
                                break;

                                case Tilemap.Object.Enemy:
                                isAddValid = false;
                                break;

                                case Tilemap.Object.Door:
                                isAddValid = false;
                                break;

                                case Tilemap.Object.BackGroundTile:
                                isAddValid = true;
                                break;
                            }
                        }
                        else isAddValid = true;

                        if (selected == Tilemap.Object.Tree)
                        {
                            if (game.tilemap.mapData[x, y] != '0') isAddValid = false;
                        }
                    }
            }
        }
    }
}
