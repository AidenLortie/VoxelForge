using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace VoxelForge.Client.UI;

// Graphical menu system for game start screen (single player vs multiplayer)
public class MenuWindow : OpenTK.Windowing.Desktop.GameWindow
{
    private bool _startSinglePlayer = false;
    private bool _startMultiplayer = false;
    private int _selectedOption = 0; // 0 = Single Player, 1 = Multiplayer, 2 = Quit
    private TextRenderer? _textRenderer;
    
    public bool ShouldStartSinglePlayer => _startSinglePlayer;
    public bool ShouldStartMultiplayer => _startMultiplayer;
    
    public MenuWindow() : base(CreateGameWindowSettings(), CreateNativeWindowSettings())
    {
    }
    
    private static GameWindowSettings CreateGameWindowSettings()
    {
        return new GameWindowSettings
        {
            UpdateFrequency = 60.0,
        };
    }
    
    private static NativeWindowSettings CreateNativeWindowSettings()
    {
        return new NativeWindowSettings
        {
            ClientSize = new Vector2i(1280, 720),
            Title = "VoxelForge - Main Menu",
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible,
            Vsync = VSyncMode.On,
        };
    }
    
    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.15f, 0.15f, 0.2f, 1.0f);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        _textRenderer = new TextRenderer(ClientSize.X, ClientSize.Y);
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        _textRenderer?.UpdateScreenSize(e.Width, e.Height);
    }
    
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        // Handle input
        if (KeyboardState.IsKeyPressed(Keys.Down))
        {
            _selectedOption = (_selectedOption + 1) % 3;
        }
        else if (KeyboardState.IsKeyPressed(Keys.Up))
        {
            _selectedOption = (_selectedOption - 1 + 3) % 3;
        }
        else if (KeyboardState.IsKeyPressed(Keys.Enter))
        {
            if (_selectedOption == 0)
            {
                _startSinglePlayer = true;
                Close();
            }
            else if (_selectedOption == 1)
            {
                _startMultiplayer = true;
                Close();
            }
            else if (_selectedOption == 2)
            {
                Close();
            }
        }
        else if (KeyboardState.IsKeyPressed(Keys.Escape))
        {
            Close();
        }
    }
    
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        if (_textRenderer != null)
        {
            // Draw title
            _textRenderer.RenderText("VoxelForge", 460, 150, 2.0f, new Vector3(1.0f, 1.0f, 1.0f));
            
            // Draw instructions
            _textRenderer.RenderText("Use UP/DOWN to select, ENTER to confirm, ESC to quit", 300, 250, 0.8f, new Vector3(0.7f, 0.7f, 0.7f));
            
            // Draw menu options
            var option1Color = _selectedOption == 0 ? new Vector3(1.0f, 0.9f, 0.3f) : new Vector3(0.8f, 0.8f, 0.8f);
            var option2Color = _selectedOption == 1 ? new Vector3(1.0f, 0.9f, 0.3f) : new Vector3(0.8f, 0.8f, 0.8f);
            var option3Color = _selectedOption == 2 ? new Vector3(1.0f, 0.9f, 0.3f) : new Vector3(0.8f, 0.8f, 0.8f);
            
            _textRenderer.RenderText("Single Player", 500, 350, 1.2f, option1Color);
            _textRenderer.RenderText("Start local world", 500, 390, 0.7f, new Vector3(0.6f, 0.6f, 0.6f));
            
            _textRenderer.RenderText("Multiplayer", 500, 450, 1.2f, option2Color);
            _textRenderer.RenderText("Connect to server", 500, 490, 0.7f, new Vector3(0.6f, 0.6f, 0.6f));
            
            _textRenderer.RenderText("Quit", 500, 550, 1.2f, option3Color);
        }
        
        SwapBuffers();
    }
    
    protected override void OnUnload()
    {
        _textRenderer?.Dispose();
        base.OnUnload();
    }
}
