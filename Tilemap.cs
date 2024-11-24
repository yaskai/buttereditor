using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace buttereditor 
{
    public class Tilemap
    {
        Game1 game;
        Handler handler;
        Spritesheet ss;
        Spritesheet bg_ss;

        public static int minMapHeight = 16;
        public static int minMapWidth = 24;

        public static int maxMapWidth = 572;
        public static int maxMapHeight = 156;

        public int mapWidth = 64;
        public int mapHeight = 16;

        public char[,] mapData = new char[maxMapWidth, maxMapHeight];
        public Vector2[,] tileGraphicsMap = new Vector2[maxMapWidth, maxMapHeight];

        public int[,] bgMapData = new int[maxMapWidth, maxMapHeight];
        public Vector2[,] bgGraphicsMap = new Vector2[maxMapWidth, maxMapHeight];

        public bool isPlayerPlaced = false;
        public Vector2 playerPos = Vector2.Zero;
        private enum adjacency
        {
            TopLeft,
            TopCenter,
            TopRight,
            CenterLeft,
            Center,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
            FloorLeft,
            FloorCenter,
            FloorRight,
            WallTop,
            WallCenter,
            WallBottom,
            Island
        };
        private adjacency[,] adjacencies = new adjacency[maxMapWidth, maxMapHeight];
        //private int updateTimer = 30;

        public enum Object
        {
            None,
            Tile,
            Slope,
            Player,
            Spike,
            Enemy,
            Tree,
            FragileBlock,
            Enemy1,
            Door,

            BackGroundTile
        };
        public static int objCount = 9;

        //private StreamWriter writer = new StreamWriter("lvlNew.txt");

        private int saveMapTimer = 0;

        public Tilemap(Game1 game, Handler handler, Spritesheet ss)
        {
            this.game = game;
            this.handler = handler;
            this.ss = ss;
            bg_ss = game.bgTileset_ss;

            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                mapData[c, r] = '0';
                tileGraphicsMap[c, r] = new Vector2(3, 3);
                adjacencies[c, r] = adjacency.Island;

                bgMapData[c, r] = 0;
                bgGraphicsMap[c, r] = new Vector2(3, 3);
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.LeftControl) && kb.IsKeyDown(Keys.S) && game._state == Game1.State.Main && saveMapTimer <= 0)
            {
                //SaveMap("lvlNew.txt");
                SaveMap("lvlNew");
                System.Diagnostics.Debug.WriteLine("*********************");
                saveMapTimer = 2;
            }

            if (mapWidth < minMapWidth) mapWidth = minMapWidth;
            else if (mapWidth > maxMapWidth) mapWidth = maxMapWidth;

            if (mapHeight < minMapHeight) mapHeight = minMapHeight;
            else if (mapHeight > maxMapHeight) mapHeight = maxMapHeight;

            if (saveMapTimer > 0 && kb.IsKeyUp(Keys.S)) saveMapTimer--;
            if (saveMapTimer < 0) saveMapTimer = 0;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            //if (game.cursor.layer == 1) DrawBgMap(_spriteBatch);

            Color clr = Color.White;
            if (game.cursor.layer == 0) clr = Color.White * 0.75f;

            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                Vector2 drawPos = new Vector2(c * 64, r * 64);

                switch (mapData[c, r])
                {
                    case 't':
                    _spriteBatch.Draw(game.tree00, drawPos, clr);
                    break;
                }

                switch (mapData[c, r])
                {
                    case '1':
                    int spriteX = (int)tileGraphicsMap[c, r].X;
                    int spriteY = (int)tileGraphicsMap[c, r].Y;

                    _spriteBatch.Draw(ss.frames[spriteX, spriteY], drawPos, clr);
                    break;

                    case '2':
                    _spriteBatch.Draw(ss.frames[0, 6], drawPos, clr);
                    break;

                    case '3':
                    _spriteBatch.Draw(ss.frames[1, 6], drawPos, clr);
                    break;

                    case '4':
                    _spriteBatch.Draw(ss.frames[2, 6], drawPos, clr);
                    break;

                    case '5':
                    _spriteBatch.Draw(ss.frames[3, 6], drawPos, clr);
                    break;

                    case '6':
                    _spriteBatch.Draw(ss.frames[2, 7], drawPos, clr);
                    break;

                    case '7':
                    _spriteBatch.Draw(ss.frames[3, 7], drawPos, clr);
                    break;

                    case '@':
                    _spriteBatch.Draw(game.misc_ss.frames[0, 0], drawPos, clr);
                    break;

                    case 'e':
                    _spriteBatch.Draw(game.misc_ss.frames[1, 0], drawPos, clr);
                    break;

                    case '^':
                    _spriteBatch.Draw(game.misc_ss.frames[2, 0], drawPos, clr);
                    break;

                    case 'v':
                    _spriteBatch.Draw(game.misc_ss.frames[3, 0], drawPos, clr);
                    break;

                    case 'f':
                    _spriteBatch.Draw(game.fragileBlock_ss.frames[0, 0], drawPos, clr);
                    break;

                    case 'd':
                    _spriteBatch.Draw(game.doorImage, drawPos, clr);
                    break;
                }
            }
            //if (game.cursor.layer == 0) DrawBgMap(_spriteBatch);
        }

        public void DrawBgMap(SpriteBatch _spriteBatch)
        {
            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                Vector2 drawPos = new Vector2(c * 64, r * 64);

                switch (bgMapData[c, r])
                {
                    case 1:
                    int ssx = (int)bgGraphicsMap[c, r].X;
                    int ssy = (int)bgGraphicsMap[c, r].Y;
                    _spriteBatch.Draw(bg_ss.frames[ssx, ssy], drawPos, Color.White);
                    break;

                    case 2:
                    break;

                    case 3:
                    break;

                    case 4:
                    break;

                    case 5:
                    break;

                    case 6:
                    break;

                    case 7:
                    break;

                    case 8:
                    break;

                    case 9:
                    break;
                }
            }
        }

        public void LoadMap()
        {
            StreamReader reader = new StreamReader("lvlLoad.txt");

            string lvlWidth = reader.ReadLine();
            string lvlHeight = reader.ReadLine();

            mapWidth = Int32.Parse(lvlWidth);
            mapHeight = Int32.Parse(lvlHeight);

            string mapBuffer = "";
            for (int r = 0; r < mapHeight; r++)
            {
                string line = reader.ReadLine();
                mapBuffer += line;

                System.Diagnostics.Debug.WriteLine(line);
            }

            string seperator = reader.ReadLine();
            System.Diagnostics.Debug.WriteLine(seperator);

            string bgBuffer = "";
            for (int r = 0; r < mapHeight; r++)
            {
                string line = reader.ReadLine();
                bgBuffer += line;

                System.Diagnostics.Debug.WriteLine(line);
            }

            reader.Close();

            GenerateMap(mapBuffer);
            GenerateBg(bgBuffer);
        }

        private void GenerateMap(string map)
        {
            char[] mapBuffer = map.ToCharArray();

            for (int i = 0; i < map.Length; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                int index = GetIndex(c, r);
                mapData[c, r] = mapBuffer[index];

                if (mapData[c, r] == '@')
                {
                    isPlayerPlaced = true;
                    playerPos = new Vector2(c, r);
                }
            }

            UpdateAllTiles();
        }

        private void GenerateBg(string bg)
        {
            int[] bgBuffer = new int[mapWidth * mapHeight];

            for (int i = 0; i < bg.Length; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;
                int index = GetIndex(c, r);
                
                string s = bg[index] + "";
                int n = Int32.Parse(s);

                bgBuffer[index] = n;
                bgMapData[c, r] = bgBuffer[index];
            }

            UpdateAllBg();
        }

        public void NewMap(int width, int height)
        {
            if (width < 0) width = 0;
            else if (width > maxMapWidth) width = maxMapWidth;

            if (height < 0) height = 0;
            else if (height > maxMapHeight) height = maxMapHeight;

            mapWidth = width;
            mapHeight = height;

            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                mapData[c, r] = '0';
                bgMapData[c, r] = 0;
            }
        }

        public void SaveMap(string path)
        {
            string extension = "";
            //extension += System.DateTime.Now.ToShortDateString() + "";
            extension += System.DateTime.Now.Day.ToString() + "";
            extension += System.DateTime.Now.Hour.ToString() + "";
            extension += System.DateTime.Now.Second.ToString() + "";
            //extension += System.DateTime.Now.Millisecond.ToString() + "";

            //writeToFile(path + extension);
            writeToFile(path + extension + ".txt");
        }

        private void writeToFile(string path)
        {
            StreamWriter writer = new StreamWriter(path);
            //writer = new StreamWriter(path);

            writer.WriteLine(mapWidth);
            writer.WriteLine(mapHeight);

            System.Diagnostics.Debug.WriteLine(mapWidth);
            System.Diagnostics.Debug.WriteLine(mapHeight);

            string[] lines = new string[mapHeight];

            for (int r = 0; r < mapHeight; r++)
            {
                string line = "";

                for (int c = 0; c < mapWidth; c++)
                {
                    String s = mapData[c, r] + "";
                    line += s;
                }

                writer.WriteLine(line);
                System.Diagnostics.Debug.WriteLine(line); 
            }

            string seperator = "*";
            for (int i = 0; i < mapWidth - 2; i++)
            {
                seperator += "-";
            }
            seperator += "*";
            writer.WriteLine(seperator);
            System.Diagnostics.Debug.WriteLine(seperator);

            for (int r = 0; r < mapHeight; r++)
            {
                string line = "";

                for (int c = 0; c < mapWidth; c++)
                {
                    String s = bgMapData[c, r] + "";
                    line += s;
                }

                writer.WriteLine(line);
                System.Diagnostics.Debug.WriteLine(line);
            }

            writer.Close();
        }

        public void AddObject(int c, int r, Object obj)
        {
             if (game.cursor.selected != Object.BackGroundTile) Delete(c, r);

             switch (obj)
             {
                case Object.Tile:
                NewTile(c, r);
                break;

                case Object.Slope:
                string orientation = ".";

                if (game.cursor.orientation == 0) orientation = "+-";
                else if (game.cursor.orientation == 1) orientation = "++";

                NewSlope(c, r, orientation);

                UpdateAllTiles();
                break;

                case Object.Player:
                Delete((int)playerPos.X, (int)playerPos.Y);
                mapData[c, r] = '@';
                playerPos = new Vector2(c, r);
                break;

                case Object.Spike:
                NewSpike(c, r);
                break;

                case Object.Enemy:
                mapData[c, r] = 'e';
                break;

                case Object.Tree:
                mapData[c, r] = 't';
                break;

                case Object.BackGroundTile:
                NewBgTile(c, r);
                break;

                case Object.FragileBlock:
                mapData[c, r] = 'f';
                break;

                case Object.Door:
                mapData[c, r] = 'd';
                break;
             }
        }

        public void Delete(int c, int r)
        {
            if (mapData[c, r] != '0')
            {
                if (mapData[c, r] == '2' || mapData[c, r] == '4') mapData[c + 1, r] = '0';
                else if (mapData[c, r] == '3' || mapData[c, r] == '5') mapData[c - 1, r] = '0';

                mapData[c, r] = '0';
                UpdateAllTiles();
            }
        }

        public void DeleteBG(int c, int r)
        {
            if (bgMapData[c, r] != 0)
            {
                bgMapData[c, r] = 0;
                UpdateAllBg();
            }
        }

        private void NewTile(int c, int r)
        {
            if (!ContainsTile(c, r)) 
            {
                mapData[c, r] = '1';
                UpdateAllTiles();
            }
        }

        private void NewBgTile(int c, int r)
        {
            if (bgMapData[c, r] == 0)
            {
                bgMapData[c, r] = 1;
                UpdateAllBg();
            }
        }

        private bool ContainsTile(int c, int r)
        {
            bool contains = false;

            if (c >= 0 && c <= mapWidth &&
                r >= 0 && r <= mapWidth)
            {
                if (mapData[c, r].Equals('1')) contains = true;
            }

            return contains;
        }

        private void UpdateTile(int c, int r)
        {
            char lft = '.';
            char rgt = '.';
            char top = '.';
            char bot = '.';

            char lt = '.';
            char rt = '.';
            char lb = '.';
            char rb = '.';

            if (c > 0) lft = mapData[c - 1, r];
            if (c < mapWidth - 1) rgt = mapData[c + 1, r];
            if (r > 0) top = mapData[c, r - 1];
            if (r < mapHeight - 1) bot = mapData[c, r + 1];

            if (c > 0 && r > 0) lt = mapData[c - 1, r - 1];
            if (c < mapWidth - 1 && r > 0) rt = mapData[c + 1, r - 1];
            if (c > 0 && r < mapHeight - 1) lb = mapData[c - 1, r + 1];
            if (c < mapWidth - 1 && r < mapHeight - 1) rb = mapData[c + 1, r + 1];

            // check for slope tiles
            if (lft == '3') lft = '1';
            if (rgt == '4') rgt = '1';

            switch (top)
            {
                case '2':
                top = '1';
                break;

                case '3':
                top = '1';
                break;

                case '4':
                top = '1';
                break;

                case '5':
                top = '1';
                break;
            }

            switch (lt)
            {
                case '2':
                lt = '1';
                break;

                case '3':
                lt = '1';
                break;
            }

            switch (rt)
            {
                case '2':
                rt = '1';
                break;

                case '3':
                rt = '1';
                break;
            }

            if (lft != '1' && rgt == '1' && top != '1' && bot == '1')
            {
                // top left corner
                tileGraphicsMap[c, r] = new Vector2(0, 0);
                adjacencies[c, r] = adjacency.TopLeft;
            }
            else if (lft == '1' && rgt == '1' && top != '1' && bot == '1')
            {
                // middle floor
                tileGraphicsMap[c, r] = new Vector2(1, 0);
                adjacencies[c, r] = adjacency.TopCenter;
            }
            else if (lft == '1' && rgt != '1' && top != '1' && bot == '1')
            {
                // top right corner
                tileGraphicsMap[c, r] = new Vector2(2, 0);
                adjacencies[c, r] = adjacency.TopRight;
            }
            else if (lft != '1' && rgt == '1' && top == '1' && bot == '1')
            {
                // left wall
                tileGraphicsMap[c, r] = new Vector2(0, 1);
                adjacencies[c, r] = adjacency.CenterLeft;
            }
            else if (lft == '1' && rgt == '1' && top == '1' && bot == '1')
            {
                // center
                tileGraphicsMap[c, r] = new Vector2(1, 1);
                adjacencies[c, r] = adjacency.Center;
            }
            else if (lft == '1' && rgt != '1' && top == '1' && bot == '1')
            {
                // right wall
                tileGraphicsMap[c, r] = new Vector2(2, 1);
                adjacencies[c, r] = adjacency.CenterRight;
            }
            else if (lft != '1' && rgt == '1' && top ==  '1' && bot != '1')
            {
                // bottom left corner
                tileGraphicsMap[c, r] = new Vector2(0, 2);
                adjacencies[c, r] = adjacency.BottomLeft;
            } 
            else if (lft == '1' && rgt == '1' && top == '1' && bot != '1')
            {
                // middle ceiling
                tileGraphicsMap[c, r] = new Vector2(1, 2);
                adjacencies[c, r] = adjacency.BottomCenter;
            }
            else if (lft == '1' && rgt != '1' && top == '1' && bot != '1')
            {
                // bottom right corner
                tileGraphicsMap[c, r] = new Vector2(2, 2);
                adjacencies[c, r] = adjacency.BottomLeft;
            }
            else if (lft != '1' && rgt != '1' && top != '1' && bot == '1')
            {
                // '
                tileGraphicsMap[c, r] = new Vector2(3, 0);
                adjacencies[c, r] = adjacency.WallTop;
            }
            else if (lft != '1' && rgt != '1' && top == '1' && bot == '1')
            {
                // -
                tileGraphicsMap[c, r] = new Vector2(3, 1);
                adjacencies[c, r] = adjacency.WallCenter;
            }
            else if (lft != '1' && rgt != '1' && top == '1' && bot != '1')
            {
                // .
                tileGraphicsMap[c, r] = new Vector2(3, 2);
                adjacencies[c, r] = adjacency.WallBottom;
            }
            else if (lft != '1' && rgt == '1' && top != '1' && bot != '1')
            {
                // *--
                tileGraphicsMap[c, r] = new Vector2(0, 3);
                adjacencies[c, r] = adjacency.FloorLeft;
            }
            else if (lft == '1' && rgt == '1' && top != '1' && bot != '1')
            {
                // -*-
                tileGraphicsMap[c, r] = new Vector2(1, 3);
                adjacencies[c, r] = adjacency.FloorCenter;
            }
            else if (lft == '1' && rgt != '1' && top != '1' && bot != '1')
            {
                // --*
                tileGraphicsMap[c, r] = new Vector2(2, 3);
                adjacencies[c, r] = adjacency.FloorRight;
            }
            else 
            {
                tileGraphicsMap[c, r] = new Vector2(3, 3);
                adjacencies[c, r] = adjacency.Island;
            }
        }

        private void SecondaryTileUpdate(int c, int r)
        {
            char lft = '.';
            char rgt = '.';
            char top = '.';
            char bot = '.';

            char lt = '.';
            char rt = '.';
            char lb = '.';
            char rb = '.';

            if (c > 0) lft = mapData[c - 1, r];
            if (c < mapWidth - 1) rgt = mapData[c + 1, r];
            if (r > 0) top = mapData[c, r - 1];
            if (r < mapHeight - 1) bot = mapData[c, r + 1];

            if (c > 0 && r > 0) lt = mapData[c - 1, r - 1];
            if (c < mapWidth - 1 && r > 0) rt = mapData[c + 1, r - 1];
            if (c > 0 && r < mapHeight - 1) lb = mapData[c - 1, r + 1];
            if (c < mapWidth - 1 && r < mapHeight - 1) rb = mapData[c + 1, r + 1];

            if (r == 1) top = '1';
            if (r == mapHeight - 1) bot = '1';

                        // check for slope tiles
            if (lft == '3') lft = '1';
            if (rgt == '4') rgt = '5';

            switch (top)
            {
                case '2':
                top = '1';
                break;

                case '3':
                top = '1';
                break;

                case '4':
                top = '1';
                break;

                case '5':
                top = '1';
                break;
            }

            switch (lt)
            {
                case '2':
                lt = '1';
                break;

                case '3':
                lt = '1';
                break;

                case '4':
                lt = '1';
                break;

                case '5':
                lt = '1';
                break;
            }

            switch (rt)
            {
                case '2':
                rt = '1';
                break;

                case '3':
                rt = '1';
                break;

                case '4':
                rt = '1';
                break;

                case '5':
                rt = '1';
                break;
            }

            Vector2 coords = tileGraphicsMap[c, r];
            
            if (adjacencies[c, r] == adjacency.Center)
            {
                if (lt == '1' && rt == '1' && lb == '1' && rb == '1')
                {
                    coords = new Vector2(1, 1);
                }
                else if (lt != '1' && rt == '1' && lb == '1' && rb == '1')
                {
                    coords = new Vector2(4, 0);
                }
                else if (lt == '1' && rt != '1' && lb == '1' && rb == '1')
                {
                    coords = new Vector2(4, 1);
                }
                else if (lt == '1' && rt == '1' && lb != '1' && rb == '1')
                {
                    coords = new Vector2(4, 2);
                }
                else if (lt == '1' && rt == '1' && lb == '1' && rb != '1')
                {
                    coords = new Vector2(4, 3);
                }
                else if (lt != '1' && rt != '1') coords = new Vector2(3, 4);
            }
            else if (adjacencies[c, r] == adjacency.CenterRight)
            {
                if (lft == '1')
                {
                    if (lt != '1' && lb != '1')
                    {
                        coords = new Vector2(0, 4);
                    }
                    else if (lt != '1' && lb == '1')
                    {
                        coords = new Vector2(1, 4);
                    }
                    else if (lt == '1' && lb != '1')
                    {
                        coords = new Vector2(2, 4);
                    }
                }
            }
            else if (adjacencies[c, r] == adjacency.CenterLeft)
            {
                if (rgt == '1')
                {
                    if (rt != '1' && rb != '1')
                    {
                        coords = new Vector2(0, 5);
                    }
                    else if (rt != '1' && rb == '1')
                    {
                        coords = new Vector2(1, 5);
                    }
                    else if (rt == '1' && rb != '1')
                    {
                        coords = new Vector2(2, 5);
                    }
                }
            }
            else if (adjacencies[c, r] == adjacency.BottomCenter)
            {
                if (lt != '1' && rt == '1')
                {
                    coords = new Vector2(3, 5);
                }
                else if (lt == '1' && rt != '1')
                {
                    coords = new Vector2(4, 5);
                }
            }

            tileGraphicsMap[c, r] = coords;
        }

        private void ExtraAdjustments(int c, int r)
        {
            char lft = '.';
            char rgt = '.';
            char top = '.';
            char bot = '.';

            char lt = '.';
            char rt = '.';
            char lb = '.';
            char rb = '.';

            if (c > 0) lft = mapData[c - 1, r];
            if (c < mapWidth - 1) rgt = mapData[c + 1, r];
            if (r > 0) top = mapData[c, r - 1];
            if (r < mapHeight - 1) bot = mapData[c, r + 1];

            if (c > 0 && r > 0) lt = mapData[c - 1, r - 1];
            if (c < mapWidth - 1 && r > 0) rt = mapData[c + 1, r - 1];
            if (c > 0 && r < mapHeight - 1) lb = mapData[c - 1, r + 1];
            if (c < mapWidth - 1 && r < mapHeight - 1) rb = mapData[c + 1, r + 1];

            if (r == 1) top = '1';
            if (r == mapHeight - 1) bot = '1';

            if (mapData[c, r] == '1')
            {
                switch (adjacencies[c, r])
                {
                    case adjacency.Center:
                    if (top == '2' && lft == '3')
                    {
                        tileGraphicsMap[c, r] = new Vector2(0, 7);
                    }

                    if (top == '5' && rgt == '4')
                    {
                        tileGraphicsMap[c, r] = new Vector2(1, 7);
                    }

                    if (top == '2' && lft == '1')
                    {
                        tileGraphicsMap[c, r] = new Vector2(0, 7);
                    }

                    if (top == '5' && rgt == '1')
                    {
                        tileGraphicsMap[c, r] = new Vector2(1, 7);
                    }

                    break;

                    case adjacency.CenterLeft:
                    if (top == '2') tileGraphicsMap[c, r] = new Vector2(4, 6);
                    break;

                    case adjacency.CenterRight:
                    if (top == '5') tileGraphicsMap[c, r] = new Vector2(4, 7);
                    break;
                }
                
            }
        }

        private void UpdateAllTiles()
        {
            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                UpdateTile(c, r);
                SecondaryTileUpdate(c, r);
                ExtraAdjustments(c, r);
            }
        }

        private void UpdateBgTile(int c, int r)
        {
            int lft = 0;
            int rgt = 0;
            int top = 0;
            int bot = 0;

            if (c > 0) lft = bgMapData[c - 1, r];
            if (c < mapWidth - 1) rgt = bgMapData[c + 1, r];
            if (r > 0) top = bgMapData[c, r - 1];
            if (r < mapHeight - 1) bot = bgMapData[c, r + 1];

            if (r == mapHeight - 1) bot = 1;
            if (r == 0) top = 1;

            if (lft != 1 && rgt == 1 && top != 1 && bot == 1)
            {
                // top left corner
                bgGraphicsMap[c, r] = new Vector2(0, 0);
            }
            else if (lft == 1 && rgt == 1 && top != 1 && bot == 1)
            {
                // middle floor
                bgGraphicsMap[c, r] = new Vector2(1, 0);
            }
            else if (lft == 1 && rgt != 1 && top != 1 && bot == 1)
            {
                // top right corner
                bgGraphicsMap[c, r] = new Vector2(2, 0);
            }
            else if (lft != 1 && rgt == 1 && top == 1 && bot == 1)
            {
                // left wall
                bgGraphicsMap[c, r] = new Vector2(0, 1);
            }
            else if (lft == 1 && rgt == 1 && top == 1 && bot == 1)
            {
                // center
                bgGraphicsMap[c, r] = new Vector2(1, 1);
            }
            else if (lft == 1 && rgt != 1 && top == 1 && bot == 1)
            {
                // right wall
                bgGraphicsMap[c, r] = new Vector2(2, 1);
            }
            else if (lft != 1 && rgt == 1 && top == 1 && bot != 1)
            {
                // bottom left corner
                bgGraphicsMap[c, r] = new Vector2(0, 2);
            } 
            else if (lft == 1 && rgt == 1 && top == 1 && bot != 1)
            {
                // middle ceiling
                bgGraphicsMap[c, r] = new Vector2(1, 2);
            }
            else if (lft == 1 && rgt != 1 && top == 1 && bot != 1)
            {
                // bottom right corner
                bgGraphicsMap[c, r] = new Vector2(2, 2);
            }
            else if (lft != 1 && rgt != 1 && top != 1 && bot == 1)
            {
                // '
                bgGraphicsMap[c, r] = new Vector2(3, 0);
            }
            else if (lft != 1 && rgt != 1 && top == 1 && bot == 1)
            {
                // -
                bgGraphicsMap[c, r] = new Vector2(3, 1);
            }
            else if (lft != 1 && rgt != 1 && top == 1 && bot != 1)
            {
                // .
                bgGraphicsMap[c, r] = new Vector2(3, 2);
            }
            else if (lft != 1 && rgt == 1 && top != 1 && bot != 1)
            {
                // *--
                bgGraphicsMap[c, r] = new Vector2(0, 3);
            }
            else if (lft == 1 && rgt == 1 && top != 1 && bot != 1)
            {
                // -*-
                bgGraphicsMap[c, r] = new Vector2(1, 3);
            }
            else if (lft == 1 && rgt != 1 && top != 1 && bot != 1)
            {
                // --*
                bgGraphicsMap[c, r] = new Vector2(2, 3);
            }
            else 
            {
                bgGraphicsMap[c, r] = new Vector2(3, 3);
            }            
        }

        private void UpdateAllBg()
        {
            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                UpdateBgTile(c, r);
            }
        }

        private void NewSpike(int c, int r)
        {
            char lft = '.';
            char rgt = '.';
            char top = '.';
            char bot = '.';

            char value = '^';

            if (c > 0) lft = mapData[c - 1, r];
            else lft = mapData[0, r];

            if (c < mapWidth - 1) rgt = mapData[c + 1, r];
            else rgt = mapData[mapWidth - 1, r];

            if (r > 0) top = mapData[c, r - 1];
            else top = mapData[c, 0];

            if (r < mapHeight - 1) bot = mapData[c, r + 1];
            else bot = mapData[c, mapHeight - 1];

            if (top != '1' && bot == '1')
            {
                value = '^';
            }
            else if (top == '1' && bot != '1')
            {
                value = 'v';
            }

            mapData[c, r] = value;
        }

        private void NewSlope(int c, int r, string dir)
        {
            char val0 = '0'; 
            char val1 = '0';

            switch (dir)
            {
                case "+-":
                val0 = '2';
                val1 = '3';
                break;

                case "++":
                val0 = '4';
                val1 = '5';
                break;
            }

            mapData[c, r] = val0;
            mapData[c + 1, r] = val1;
        }

        private int GetIndex(int x, int y)
        {
            return x + mapWidth * y;
        }

        public void PrintMapImage(GraphicsDevice graphicsDevice, SpriteBatch _spriteBatch)
        {
            RenderTarget2D captureTarget;
            captureTarget = new RenderTarget2D(graphicsDevice, mapWidth * 64, mapHeight * 64);

            graphicsDevice.SetRenderTarget(captureTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            game.cursor.layer = 1;
            game.DrawGrid(_spriteBatch);
            Draw(_spriteBatch);
            _spriteBatch.End();

            Stream stream = File.OpenWrite("/home/kai/capture.png");
            captureTarget.SaveAsPng(stream, mapWidth * 64, mapHeight * 64);

            graphicsDevice.SetRenderTarget(null);

            stream.Dispose();
            captureTarget.Dispose();
        }
    }
}