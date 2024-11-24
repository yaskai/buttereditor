using System.Collections.Specialized;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace buttereditor; 

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public int realWidth = 320 * 2;
    public int realHeight = 180 * 2;

    public Handler handler;
    public Tilemap tilemap;
    public Cursor cursor;
    public Camera camera;

    public float timer = 0;

    public bool altView = false;

    public enum State
    {
        Start,
        Setup,
        Main,
        Exit
    };
    public State _state = State.Start;
    public State _nextState = State.Start;

    private Vector2 tileSize = new Vector2(64, 64);

    public KeyboardState kb;
    private float screenshotTimer = 0;

    public SpriteFont font;
    public Color textColor = new Color(228, 220, 204);
    public int gridColor = 20;

    private Texture2D cursorPNG;
    public Spritesheet cursor_ss;

    private Texture2D pixPNG;
    public Spritesheet pix;

    private Texture2D buttonsPNG;
    public Spritesheet buttons_ss;

    private Texture2D tilesetPNG;
    public Spritesheet tileset_ss;

    private Texture2D miscPNG;
    public Spritesheet misc_ss;

    public Texture2D tree00;

    private Texture2D bgTilesetPNG;
    public Spritesheet bgTileset_ss;

    private Texture2D fragileBlockPNG;
    public Spritesheet fragileBlock_ss;

    public Texture2D doorImage;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        this.IsFixedTimeStep = true;
        this.IsMouseVisible = false;

        realWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        realHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        
        _graphics.PreferredBackBufferWidth = realWidth;
        _graphics.PreferredBackBufferHeight = realHeight;

        _graphics.IsFullScreen = true;
        _graphics.SynchronizeWithVerticalRetrace = true;

        _graphics.ApplyChanges();

        base.Initialize();

        handler = new Handler();
        tilemap = new Tilemap(this, handler, tileset_ss);
        cursor = new Cursor(this, handler, cursor_ss);
        camera = new Camera(this, handler, tilemap);

        InitStartMenu();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        font = Content.Load<SpriteFont>("debugFont");

        pixPNG = Content.Load<Texture2D>("blank_pixels");
        pix = new Spritesheet(GraphicsDevice, pixPNG, Vector2.One);
        pix.LoadContent(Content);

        cursorPNG = Content.Load<Texture2D>("cursor01");
        cursor_ss = new Spritesheet(GraphicsDevice, cursorPNG, new Vector2(16, 16));
        cursor_ss.LoadContent(Content);

        buttonsPNG = Content.Load<Texture2D>("buttons_spritesheet");
        buttons_ss = new Spritesheet(GraphicsDevice, buttonsPNG, tileSize);
        buttons_ss.LoadContent(Content);

        tilesetPNG = Content.Load<Texture2D>("tileset00_64x64");
        tileset_ss = new Spritesheet(GraphicsDevice, tilesetPNG, tileSize);
        tileset_ss.LoadContent(Content);

        miscPNG = Content.Load<Texture2D>("lvle_misc");
        misc_ss = new Spritesheet(GraphicsDevice, miscPNG, tileSize);
        misc_ss.LoadContent(Content);

        bgTilesetPNG = Content.Load<Texture2D>("bg_tileset00");
        bgTileset_ss = new Spritesheet(GraphicsDevice, bgTilesetPNG, tileSize);
        bgTileset_ss.LoadContent(Content);

        tree00 = Content.Load<Texture2D>("beri_tree00");

        fragileBlockPNG = Content.Load<Texture2D>("fblock");
        fragileBlock_ss = new Spritesheet(GraphicsDevice, fragileBlockPNG, tileSize);
        fragileBlock_ss.LoadContent(Content);

        doorImage = Content.Load<Texture2D>("door0064x64");
    }

    protected override void Update(GameTime gameTime)
    {
        kb = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

        altView = false;
        
        camera.Update(gameTime);
        tilemap.Update(gameTime);
        handler.Update(gameTime);
        cursor.Update(gameTime); 

        altView = cursor.altView;

        // TODO: Add your update logic here

        timer --;
        if (timer <= 0)
        {
            switch (_nextState)
            {
                case State.Start:
                break;

                case State.Setup:
                InitSetupMenu();
                break;

                case State.Main:
                break;

                case State.Exit:
                break;
            }
            timer = 0;
        }

        screenshotTimer --;
        if (screenshotTimer < 0) screenshotTimer = 0;

        if (_state == State.Setup)
        {
            if (kb.IsKeyDown(Keys.Enter)) InitMain(0);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        switch (_state)
        {
            case State.Start:
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            _spriteBatch.End();
            break;

            case State.Setup:
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            _spriteBatch.End();
            break;

            case State.Main:

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
            
            if (cursor.layer == 0)
            {
                tilemap.Draw(_spriteBatch);
                DrawGrid(_spriteBatch);
                tilemap.DrawBgMap(_spriteBatch);
            }
            else if (cursor.layer == 1)
            {
                tilemap.DrawBgMap(_spriteBatch);
                DrawGrid(_spriteBatch);
                tilemap.Draw(_spriteBatch);
            }

            cursor.TransformDraw(_spriteBatch);

            _spriteBatch.End();

            if (altView)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, null, camera.scale);
                DrawAltView(_spriteBatch);
                _spriteBatch.End();
            }


            if (kb.IsKeyDown(Keys.P) && kb.IsKeyDown(Keys.LeftControl) && screenshotTimer <= 0)
            {
                tilemap.PrintMapImage(GraphicsDevice, _spriteBatch);
                screenshotTimer = 10;
            }

            break;

            case State.Exit:
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            _spriteBatch.End();
            break;
        }

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

        handler.Draw(_spriteBatch);
        if (_state == State.Setup) DisplayMapInfo(_spriteBatch);
        cursor.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void InitStartMenu()
    {
        Button newButton = new Button(new Vector2(100, 100), Button.Id.newLevel, handler,  buttons_ss, this);
        handler.buttons.Add(newButton);

        Button loadButton = new Button(new Vector2(100, 200), Button.Id.loadLevel, handler, buttons_ss, this);
        handler.buttons.Add(loadButton);
    }

    public void InitSetupMenu()
    {
        _state = State.Setup;

        handler.ClearUI();

        Button subHeight = new Button(new Vector2(100, 132), Button.Id.subtractHeight, handler, buttons_ss, this);
        handler.buttons.Add(subHeight);

        Button addHeight = new Button(new Vector2(100, 232), Button.Id.addHeight, handler, buttons_ss, this);
        handler.buttons.Add(addHeight);

        Button subWidth = new Button(new Vector2(200, 32), Button.Id.subtractWidth, handler, buttons_ss, this);
        handler.buttons.Add(subWidth);

        Button addWidth = new Button(new Vector2(300, 32), Button.Id.addWidth, handler, buttons_ss, this);
        handler.buttons.Add(addWidth);

        _nextState = State.Main;
    }

    public void InitMain(int mode)
    {
        handler.ClearUI();
        
        if (mode == 0) tilemap.NewMap(tilemap.mapWidth, tilemap.mapHeight);
        camera.position.Y = tilemap.mapHeight * 64;

        _state = State.Main;
        cursor.startTimer = 10;
    }

    private void DisplayMapInfo(SpriteBatch _spriteBatch)
    {
        _spriteBatch.DrawString(font, "width: " + tilemap.mapWidth, new Vector2(100, 64), textColor);
        _spriteBatch.DrawString(font, "height: " + tilemap.mapHeight, new Vector2(100, 64 + 32), textColor);

        _spriteBatch.DrawString(font, "hit enter to confirm", new Vector2(400, 64), textColor);

        Vector2 ratio = new Vector2(2, 3);
        //Vector2 ratio = new Vector2(realWidth / realHeight, realHeight / realWidth);

        Rectangle mapPreview = new Rectangle(200, 132, 
                                            tilemap.mapWidth * (int)ratio.X, tilemap.mapHeight * (int)ratio.Y);
        _spriteBatch.Draw(pix.frames[21, 0], mapPreview, Color.White);

        Rectangle fill = new Rectangle(mapPreview.X + 2, mapPreview.Y + 2, mapPreview.Width -4, mapPreview.Height - 4);
        _spriteBatch.Draw(pix.frames[0, 0], fill, Color.White);

        Rectangle frame = new Rectangle(mapPreview.X + (int)ratio.X,
                                        mapPreview.Y + (int)ratio.Y,
                                        (24 * (int)ratio.X),
                                        (14 * (int)ratio.Y));

        _spriteBatch.Draw(pix.frames[12, 0], frame, Color.White * 0.75f);
    }

    private void DrawAltView(SpriteBatch _spriteBatch)
    {
        Vector2 colorTextPos = new Vector2(32, 32);

        _spriteBatch.DrawString(font, "color: " + gridColor, colorTextPos, textColor);
    }

    public void DrawGrid(SpriteBatch _spriteBatch)
    {
        float w = (tilemap.mapWidth * tileSize.X);
        float h = (tilemap.mapHeight * tileSize.Y);

        for (int c = 0; c < (tilemap.mapWidth + 1); c++)
        {
            float x = (c * tileSize.X);
            
            //_spriteBatch.Draw(pix.frames[gridColor, 0], new Rectangle(x, 0, 1, h), Color.White);
            _spriteBatch.Draw(pix.frames[gridColor, 0], new Vector2(x - 1, 0), 
            null, Color.White * 0.5f, 0, Vector2.Zero, new Vector2(2, h),
             SpriteEffects.None, 1);
        }

        for (int r = 0; r < (tilemap.mapHeight + 1); r++)
        {
            float y = (r * tileSize.Y);

            //_spriteBatch.Draw(pix.frames[gridColor, 0], new Rectangle(0, y, w, 1), Color.White);

            _spriteBatch.Draw(pix.frames[gridColor, 0], new Vector2(0, y - 1), 
            null, Color.White * 0.5f, 0, Vector2.Zero, new Vector2(w, 2),
             SpriteEffects.None, 1);
        }
    }
}
