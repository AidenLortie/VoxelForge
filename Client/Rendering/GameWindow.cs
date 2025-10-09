using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Main game window that handles OpenTK rendering and window management.
/// Extends OpenTK's GameWindow to provide the rendering context and game loop.
/// </summary>
public class GameWindow : OpenTK.Windowing.Desktop.GameWindow
{
    private readonly VoxelForge.Client.Client _client;
    private Camera? _camera;
    private ShaderProgram? _chunkShader;
    private ChunkRenderer? _chunkRenderer;
    private EntityRenderer? _entityRenderer;
    private Models.ModelManager? _modelManager;
    private Texture? _blockTexture;
    private Player.PlayerController? _player;
    private Vector2 _lastMousePos;
    private bool _firstMove = true;
    private bool _showLoadingScreen = true;
    
    public ChunkRenderer? ChunkRenderer => _chunkRenderer;
    public EntityRenderer? EntityRenderer => _entityRenderer;
    
    // /// Initializes a new GameWindow with the specified client.
    public GameWindow(VoxelForge.Client.Client client) 
        : base(CreateGameWindowSettings(), CreateNativeWindowSettings())
    {
        _client = client;
    }
    
    // /// Creates the game window settings for OpenTK.
    private static GameWindowSettings CreateGameWindowSettings()
    {
        return new GameWindowSettings
        {
            UpdateFrequency = 60.0, // 60 updates per second
        };
    }
    
    // /// Creates the native window settings for OpenTK.
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
    
    // /// Called when the window is loaded. Initialize OpenGL state here.
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
        // set winding direction to counter-clockwise
        GL.FrontFace(FrontFaceDirection.Cw);
        
        // Initialize camera at center of world (8*16 = 128 blocks, elevated for view)
        _camera = new Camera(new Vector3(128, 60, 128));
        _camera.AspectRatio = (float)Size.X / Size.Y;
        
        // Initialize player controller
        _player = new Player.PlayerController(_client.World, new Vector3(128, 60, 128));
        
        // Load shaders
        string vertexSource = File.ReadAllText("Rendering/Shaders/chunk.vert");
        string fragmentSource = File.ReadAllText("Rendering/Shaders/chunk.frag");
        _chunkShader = new ShaderProgram(vertexSource, fragmentSource);
        
        // Load block texture
        _blockTexture = new Texture("Assets/Textures/Blocks/default.png");
        
        // Initialize chunk renderer
        _chunkRenderer = new ChunkRenderer(_chunkShader, _blockTexture);
        
        // Initialize model manager and entity renderer
        _modelManager = new Models.ModelManager();
        _entityRenderer = new EntityRenderer(_modelManager);
        _entityRenderer.Initialize();
        
        // Capture mouse for camera control
        CursorState = CursorState.Grabbed;
    }
    
    // /// Called every frame to update game logic.
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        if (_camera == null || _player == null)
            return;
        
        // Check if initial loading is complete
        if (_showLoadingScreen && _client.IsInitialLoadComplete)
        {
            _showLoadingScreen = false;
            Console.WriteLine("Loading complete - player controls enabled");
        }
        
        // Handle input
        var keyboardState = KeyboardState;
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Close(); // Close window when Escape is pressed
        }
        
        float deltaTime = (float)args.Time;
        
        // Only allow player movement after loading is complete
        if (!_showLoadingScreen)
        {
            // Calculate movement direction from camera
            Vector3 forward = _camera.Forward;
            forward.Y = 0; // Don't move vertically with forward/backward
            if (forward.Length > 0)
                forward = Vector3.Normalize(forward);
            
            Vector3 right = _camera.Right;
            right.Y = 0; // Don't move vertically with strafe
            if (right.Length > 0)
                right = Vector3.Normalize(right);
            
            // WASD movement with player controller
            Vector3 moveDir = Vector3.Zero;
            if (keyboardState.IsKeyDown(Keys.W))
                moveDir += forward;
            if (keyboardState.IsKeyDown(Keys.S))
                moveDir -= forward;
            if (keyboardState.IsKeyDown(Keys.A))
                moveDir -= right;
            if (keyboardState.IsKeyDown(Keys.D))
                moveDir += right;
            
            if (moveDir.Length > 0)
            {
                moveDir = Vector3.Normalize(moveDir);
                _player.Move(moveDir, 5.0f); // 5 units/sec movement speed
            }
            
            // Jump with Space
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _player.Jump();
            }
            
            // Toggle gravity with G
            if (keyboardState.IsKeyPressed(Keys.G))
            {
                _player.GravityEnabled = !_player.GravityEnabled;
                Console.WriteLine($"Gravity: {(_player.GravityEnabled ? "ON" : "OFF")}");
            }
            
            // Toggle collision with C
            if (keyboardState.IsKeyPressed(Keys.C))
            {
                _player.CollisionEnabled = !_player.CollisionEnabled;
                Console.WriteLine($"Collision: {(_player.CollisionEnabled ? "ON" : "OFF")}");
            }
            
            // Update player physics
            _player.Update(deltaTime);
        }
        
        // Sync camera position to player (at eye level)
        _camera.Position = _player.Position + new Vector3(0, 1.6f, 0);
        
        // Update entity renderer interpolation
        if (_entityRenderer != null)
        {
            _entityRenderer.Update(deltaTime);
        }
        
        // Poll network for incoming packets
        _client.Poll();
    }
    
    // /// Called when mouse moves. Updates camera rotation.
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        
        if (_camera == null)
            return;
        
        if (_firstMove)
        {
            _lastMousePos = new Vector2(e.X, e.Y);
            _firstMove = false;
            return;
        }
        
        float deltaX = e.X - _lastMousePos.X;
        float deltaY = e.Y - _lastMousePos.Y;
        _lastMousePos = new Vector2(e.X, e.Y);
        
        _camera.Rotate(deltaX, deltaY);
    }
    
    // /// Called every frame to render the scene.
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        // Clear the screen and depth buffer
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        // Show loading screen if initial load is not complete
        if (_showLoadingScreen)
        {
            // Render a darker background during loading
            GL.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // Note: In a real game, you'd render text here saying "Loading terrain..."
            // For now, we just show a darker screen
        }
        else
        {
            // Restore normal background color
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            
            // Render chunks if camera and renderer are initialized
            if (_camera != null && _chunkRenderer != null)
            {
                var view = _camera.GetViewMatrix();
                var projection = _camera.GetProjectionMatrix();
                _chunkRenderer.Render(view, projection);
                
                // Render entities
                if (_entityRenderer != null)
                {
                    _entityRenderer.Render(view, projection);
                }
            }
        }
        
        // Swap front and back buffers to display rendered frame
        SwapBuffers();
    }
    
    // /// Called when the window is resized.
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        
        // Update the OpenGL viewport to match the new window size
        GL.Viewport(0, 0, e.Width, e.Height);
        
        // Update camera aspect ratio
        if (_camera != null)
        {
            _camera.AspectRatio = (float)e.Width / e.Height;
        }
        
        Console.WriteLine($"Window resized to {e.Width}x{e.Height}");
    }
    
    // /// Called when the window is closing. Clean up resources here.
    protected override void OnUnload()
    {
        base.OnUnload();
        
        Console.WriteLine("GameWindow unloading");
        
        // Clean up rendering resources
        _chunkRenderer?.Dispose();
        _chunkShader?.Dispose();
        _blockTexture?.Dispose();
        _entityRenderer?.Dispose();
    }
}
