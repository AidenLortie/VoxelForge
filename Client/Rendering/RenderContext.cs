using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoxelForge.Client.Rendering;

/// <summary>
/// Manages the OpenGL rendering context and state.
/// Provides utilities for managing common OpenGL operations.
/// </summary>
public class RenderContext
{
    private Vector4 _clearColor;
    private bool _depthTestEnabled;
    private bool _cullFaceEnabled;
    
    // /// Initializes a new RenderContext with default settings.
    public RenderContext()
    {
        _clearColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
        _depthTestEnabled = true;
        _cullFaceEnabled = true;
    }
    
    // /// Gets or sets the clear color used when clearing the screen.
    public Vector4 ClearColor
    {
        get => _clearColor;
        set
        {
            _clearColor = value;
            GL.ClearColor(value.X, value.Y, value.Z, value.W);
        }
    }
    
    // /// Gets or sets whether depth testing is enabled.
    public bool DepthTestEnabled
    {
        get => _depthTestEnabled;
        set
        {
            _depthTestEnabled = value;
            if (value)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);
        }
    }
    
    // /// Gets or sets whether face culling is enabled.
    public bool CullFaceEnabled
    {
        get => _cullFaceEnabled;
        set
        {
            _cullFaceEnabled = value;
            if (value)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);
        }
    }
    
    // /// Sets the viewport dimensions.
    public void SetViewport(int x, int y, int width, int height)
    {
        GL.Viewport(x, y, width, height);
    }
    
    // /// Clears the screen with the current clear color.
    public void Clear(bool clearDepth = true)
    {
        var mask = ClearBufferMask.ColorBufferBit;
        if (clearDepth)
            mask |= ClearBufferMask.DepthBufferBit;
        
        GL.Clear(mask);
    }
    
    // /// Initializes the render context with default OpenGL state.
    public void Initialize()
    {
        GL.ClearColor(_clearColor.X, _clearColor.Y, _clearColor.Z, _clearColor.W);
        
        if (_depthTestEnabled)
            GL.Enable(EnableCap.DepthTest);
        
        if (_cullFaceEnabled)
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);
        }
        
        Console.WriteLine("RenderContext initialized");
    }
}
