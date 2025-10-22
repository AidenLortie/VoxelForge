

using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using OpenTK.Windowing.Common;
using VoxelForge.Client.Rendering;
using VoxelForge.Client.UI;
using VoxelForge.Shared.Log;
using VoxelForge.Shared.Networking;
using VoxelForge.Shared.Networking.Packets;
using VoxelForge.Shared.State;

namespace VoxelForge.Client;

public struct ClientConsoleArgs
{
    public bool Debug;
}

public class Client
{
    public static readonly ConsoleLogger _logger = new ConsoleLogger();
    private ClientConsoleArgs _args;
    private UiStateMachine _uiContext = new UiStateMachine();
    private INetworkBridge? _networkBridge;
    public static void Main(string[] args)
    {
        var consoleArgs = new ClientConsoleArgs();
        foreach (var arg in args)
        {
            if (arg == "--debug")
            {
                consoleArgs.Debug = true;
            }
        }
        
        var client = new Client(consoleArgs);
        client.Run();
    }

    public Client(ClientConsoleArgs args)
    {
        _args = args;
        _logger.Info("Starting Client . . .");
    } 
    
    private void Run()
    {
        if (_args.Debug)
        {
            _logger.Level = LogLevel.Debug;
            _logger.Debug("Debug mode enabled.");
        }
        
        _logger.Info("Client is running.");

        ToolkitOptions options = new ToolkitOptions();
        Toolkit.Init(options);
        
        OpenGLGraphicsApiHints hints = new OpenGLGraphicsApiHints();
        WindowHandle window = Toolkit.Window.Create(hints);
        OpenGLContextHandle  context = Toolkit.OpenGL.CreateFromWindow(window);
        
        Toolkit.OpenGL.SetCurrentContext(context);
        OpenTK.Graphics.GLLoader.LoadBindings(Toolkit.OpenGL.GetBindingsContext(context));
        
        TextureRepository.Init();
        _logger.Info("Texture repository initialized with: " + TextureRepository.MaxGlTextureCount + " textures available.");
        
        void HandleEvents(PalHandle? handle, PlatformEventType type, EventArgs args)
        {
            switch (args)
            {
                case CloseEventArgs closeEvent:
                    Toolkit.Window.Destroy(window);
                    break;
                case WindowResizeEventArgs resizeEvent:
                    GL.Viewport(0, 0, resizeEvent.NewSize.X, resizeEvent.NewSize.Y);
                    break;
                default:
                    if (_uiContext.State != null)
                    {
                        _uiContext.State.HandleEvents(handle, type, args);
                    }
                    break;
                    
            }
        }

        EventQueue.EventRaised += HandleEvents;
        
        Toolkit.Window.SetMode(window, WindowMode.Normal);
        Toolkit.Window.SetTitle(window, "VoxelForge Client");
        Toolkit.Window.SetSize(window, new Vector2i(1280, 720));
        
        // Disable V-Sync
        Toolkit.OpenGL.SetSwapInterval(0);
        
        GL.Viewport(0, 0, 1280, 720);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.2f, 0.3f, 0.4f, 1.0f);


        _uiContext.SetState(new BootUiContext(_uiContext));

        _networkBridge = new NetworkBridgeLocal();
        Server.Server server = new Server.Server(_networkBridge);
        Thread serverThread = new Thread(() =>
        {
            server.RunAsync();
        });
        
        
        _networkBridge.RegisterHandler((CheckPacket packet) =>
        {
            _logger.Info("Received check packet from server with timestamp: " + packet.Timestamp);
        });
        
        _networkBridge.RegisterHandler((ChunkPacket packet) =>
        {
            _logger.Info("Received chunk packet for (" + packet.Chunk.GetChunkPosition().X + " , " + packet.Chunk.GetChunkPosition().Y + ")");
        });
        
        
        //serverThread.Start();

        double lastTime = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        _logger.Info(GL.GetString(StringName.Renderer) + "");
        
        // Main loop
        while (true)
        {
            Toolkit.Window.ProcessEvents(false);
            if (Toolkit.Window.IsWindowDestroyed(window)) break;
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            
            _networkBridge.Poll();
            _networkBridge.Send(new CheckPacket(0));

            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;
            
            float fps = 1 / (float) deltaTime;
            // display fps rounded to 2 decimal, with 4 pre-decimal places
            Toolkit.Window.SetTitle(window, $"VoxelForge Client - FPS { fps,4:0.00}");
            
            _uiContext.State.Update(deltaTime);
            
            _uiContext.State.Render();

            Toolkit.OpenGL.SwapBuffers(context);
            
        }

        _logger.Info("Client is shutting down.");
        _logger.Dispose();
    }
}