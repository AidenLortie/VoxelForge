using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace VoxelForge.Client.UI;

// Simple menu system for game start screen (single player vs multiplayer)
public class MenuWindow : OpenTK.Windowing.Desktop.GameWindow
{
    private bool _startSinglePlayer = false;
    private bool _startMultiplayer = false;
    private int _selectedOption = 0; // 0 = Single Player, 1 = Multiplayer, 2 = Quit
    
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
            ClientSize = new Vector2i(800, 600),
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
        
        PrintMenu();
    }
    
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        // Handle input
        if (KeyboardState.IsKeyPressed(Keys.Down))
        {
            _selectedOption = (_selectedOption + 1) % 3;
            PrintMenu();
        }
        else if (KeyboardState.IsKeyPressed(Keys.Up))
        {
            _selectedOption = (_selectedOption - 1 + 3) % 3;
            PrintMenu();
        }
        else if (KeyboardState.IsKeyPressed(Keys.Enter))
        {
            if (_selectedOption == 0)
            {
                Console.WriteLine("\nStarting Single Player...");
                _startSinglePlayer = true;
                Close();
            }
            else if (_selectedOption == 1)
            {
                Console.WriteLine("\nStarting Multiplayer...");
                _startMultiplayer = true;
                Close();
            }
            else if (_selectedOption == 2)
            {
                Console.WriteLine("\nQuitting...");
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
        
        // Menu rendering would go here (for now, just clear screen)
        // In future, can add text rendering, buttons, etc.
        
        SwapBuffers();
    }
    
    private void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("=== VoxelForge Main Menu ===");
        Console.WriteLine("Use UP/DOWN arrows to select, ENTER to confirm, ESC to quit");
        Console.WriteLine("");
        Console.WriteLine(_selectedOption == 0 ? "  [Single Player]  - Start local world with embedded server" : "   Single Player   - Start local world with embedded server");
        Console.WriteLine(_selectedOption == 1 ? "  [Multiplayer]    - Connect to network server (localhost:25565)" : "   Multiplayer     - Connect to network server (localhost:25565)");
        Console.WriteLine(_selectedOption == 2 ? "  [Quit]" : "   Quit");
    }
}
