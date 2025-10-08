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
    
    /// <summary>
    /// Initializes a new RenderContext with default settings.
    /// </summary>
    public RenderContext()
    {
        _clearColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
        _depthTestEnabled = true;
        _cullFaceEnabled = true;
    }
    
    /// <summary>
    /// Gets or sets the clear color used when clearing the screen.
    /// </summary>
    public Vector4 ClearColor
    {
        get => _clearColor;
        set
        {
            _clearColor = value;
            GL.ClearColor(value.X, value.Y, value.Z, value.W);
        }
    }
    
    /// <summary>
    /// Gets or sets whether depth testing is enabled.
    /// </summary>
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
    
    /// <summary>
    /// Gets or sets whether face culling is enabled.
    /// </summary>
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
    
    /// <summary>
    /// Sets the viewport dimensions.
    /// </summary>
    /// <param name="x">The lower-left x coordinate.</param>
    /// <param name="y">The lower-left y coordinate.</param>
    /// <param name="width">The viewport width.</param>
    /// <param name="height">The viewport height.</param>
    public void SetViewport(int x, int y, int width, int height)
    {
        GL.Viewport(x, y, width, height);
    }
    
    /// <summary>
    /// Clears the screen with the current clear color.
    /// </summary>
    /// <param name="clearDepth">Whether to also clear the depth buffer.</param>
    public void Clear(bool clearDepth = true)
    {
        var mask = ClearBufferMask.ColorBufferBit;
        if (clearDepth)
            mask |= ClearBufferMask.DepthBufferBit;
        
        GL.Clear(mask);
    }
    
    /// <summary>
    /// Initializes the render context with default OpenGL state.
    /// </summary>
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
