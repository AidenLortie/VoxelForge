using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Main game window that handles OpenTK rendering and window management.
/// Extends OpenTK's GameWindow to provide the rendering context and game loop.
/// </summary>
public class GameWindow : OpenTK.Windowing.Desktop.GameWindow
{
    private readonly VoxelForge.Client.Client _client;
    
    /// <summary>
    /// Initializes a new GameWindow with the specified client.
    /// </summary>
    /// <param name="client">The game client instance.</param>
    public GameWindow(VoxelForge.Client.Client client) 
        : base(CreateGameWindowSettings(), CreateNativeWindowSettings())
    {
        _client = client;
    }
    
    /// <summary>
    /// Creates the game window settings for OpenTK.
    /// </summary>
    private static GameWindowSettings CreateGameWindowSettings()
    {
        return new GameWindowSettings
        {
            UpdateFrequency = 60.0, // 60 updates per second
        };
    }
    
    /// <summary>
    /// Creates the native window settings for OpenTK.
    /// </summary>
    private static NativeWindowSettings CreateNativeWindowSettings()
    {
        return new NativeWindowSettings
        {
            ClientSize = new OpenTK.Mathematics.Vector2i(1280, 720),
            Title = "VoxelForge",
            APIVersion = new Version(3, 3), // OpenGL 3.3
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible,
            Vsync = VSyncMode.On,
        };
    }
    
    /// <summary>
    /// Called when the window is loaded. Initialize OpenGL state here.
    /// </summary>
    protected override void OnLoad()
    {
        base.OnLoad();
        
        Console.WriteLine("GameWindow loaded");
        Console.WriteLine($"OpenGL Version: {GL.GetString(StringName.Version)}");
        Console.WriteLine($"OpenGL Vendor: {GL.GetString(StringName.Vendor)}");
        Console.WriteLine($"OpenGL Renderer: {GL.GetString(StringName.Renderer)}");
        
        // Set up OpenGL state
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f); // Dark gray background
        GL.Enable(EnableCap.DepthTest); // Enable depth testing for 3D
        GL.Enable(EnableCap.CullFace); // Enable face culling for performance
        GL.CullFace(TriangleFace.Back); // Cull back faces
    }
    
    /// <summary>
    /// Called every frame to update game logic.
    /// </summary>
    /// <param name="args">Frame event arguments containing delta time.</param>
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        // Handle input
        var keyboardState = KeyboardState;
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Close(); // Close window when Escape is pressed
        }
        
        // Poll network for incoming packets
        _client.Poll();
    }
    
    /// <summary>
    /// Called every frame to render the scene.
    /// </summary>
    /// <param name="args">Frame event arguments containing delta time.</param>
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        // Clear the screen and depth buffer
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        // TODO: Render world and entities here (Phase 2+)
        
        // Swap front and back buffers to display rendered frame
        SwapBuffers();
    }
    
    /// <summary>
    /// Called when the window is resized.
    /// </summary>
    /// <param name="e">Resize event arguments.</param>
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        
        // Update the OpenGL viewport to match the new window size
        GL.Viewport(0, 0, e.Width, e.Height);
        
        Console.WriteLine($"Window resized to {e.Width}x{e.Height}");
    }
    
    /// <summary>
    /// Called when the window is closing. Clean up resources here.
    /// </summary>
    protected override void OnUnload()
    {
        base.OnUnload();
        
        Console.WriteLine("GameWindow unloading");
        
        // TODO: Clean up rendering resources here (shaders, buffers, textures)
    }
}
