using System;
using MoonWorks;
using MoonWorks.Graphics;
using Riateu.Graphics;

namespace Riateu;

/// <summary>
/// The main class entry point for your game. It handles the content, initialization, 
/// update loop, and drawing.
/// </summary>
public abstract class GameApp : Game
{
    /// <summary>
    /// A window width of the application.
    /// </summary>
    public int Width { get; private set; }
    /// <summary>
    /// A window height of the application.
    /// </summary>
    public int Height { get; private set; }

    private Scene nextScene;

    /// <summary>
    /// A current scene that is running. Note that if you change this, the scene won't 
    /// actually changed yet until next frame started.
    /// </summary>
    public Scene Scene 
    { 
        get => scene;
        set 
        {
            nextScene = value;
        }
    }
    private Scene scene;

    private Batch batch;
    /// <summary>
    /// The default batch for the game. If you want a custom batch to render, 
    /// create a canvas instead. 
    /// </summary>
    public Batch Batch => batch;

    /// <summary>
    /// A constructor use for initializng the application. 
    /// </summary>
    /// <param name="title">A title of the window</param>
    /// <param name="width">A width of the window</param>
    /// <param name="height">A height of the window</param>
    /// <param name="screenMode">The screen mode for the window</param>
    /// <param name="debugMode">Enable or disable debug mode, use for debugging graphics</param>
    protected GameApp(string title, uint width, uint height, ScreenMode screenMode = ScreenMode.Windowed, bool debugMode = false)
        : this(
            new WindowCreateInfo(title, width, height, screenMode, PresentMode.FIFO),
            new FrameLimiterSettings(FrameLimiterMode.Capped, 60)
        ) 
    {

    }

    /// <summary>
    /// A constructor use for initializng the application. 
    /// </summary>
    /// <param name="windowCreateInfo">An info for creating window</param>
    /// <param name="frameLimiterSettings">A settings to cap the frame</param>
    /// <param name="targetTimestep">The maximum fps timestep</param>
    /// <param name="debugMode">Enable or disable debug mode, use for debugging graphics</param>
    protected GameApp(WindowCreateInfo windowCreateInfo, FrameLimiterSettings frameLimiterSettings, int targetTimestep = 60, bool debugMode = false) 
        : base(windowCreateInfo, frameLimiterSettings, targetTimestep, debugMode)
    {
        Width = (int)windowCreateInfo.WindowWidth;
        Height = (int)windowCreateInfo.WindowHeight;
        GameContext.Init(GraphicsDevice, MainWindow);
        Input.Initialize(Inputs);
        LoadContent();
        Initialize();
        batch = new Batch(GraphicsDevice, Width, Height);
    }

    /// <summary>
    /// A method for loading content. You can freely acquire and submit the 
    /// <see cref="MoonWorks.Graphics.CommandBuffer" /> here.
    /// </summary>
    public virtual void LoadContent() {}

    /// <summary>
    /// A method to also initialize your other resources, and to set your scene.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// A method that handles the draw loop. Do your draw calls here.
    /// </summary>
    /// <param name="alpha">A delta time for the draw loop</param>
    protected override void Draw(double alpha)
    {
        CommandBuffer cmdBuf = GraphicsDevice.AcquireCommandBuffer();
        Texture backbuffer =  cmdBuf.AcquireSwapchainTexture(MainWindow);

        scene.InternalBeforeDraw(cmdBuf, batch);
        if (backbuffer != null) 
        {
            scene.InternalDraw(cmdBuf, backbuffer, batch);
        }
        scene.InternalAfterDraw(cmdBuf, batch);

        GraphicsDevice.Submit(cmdBuf);
    }

    /// <summary>
    /// A method that handles the update loop.
    /// </summary>
    /// <param name="delta">A delta time for the update loop</param>
    protected override void Update(TimeSpan delta)
    {
        Time.Update(delta);
        Input.Update();
        if (scene == null || (scene != nextScene)) 
        {
            scene?.End();
            scene = nextScene;
            scene.Begin();
        }
        scene.InternalUpdate(Time.Delta);
    }
}