using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using VoxelForge.Client.Rendering;

namespace VoxelForge.Client.UI;

public class BootUiContext : UiContext
{
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _shader;
    private double _fps = 0.0;
    private string _vertexShaderSource = @"
        #version 330 core
        layout(location = 0) in vec3 aPosition;
        layout(location = 1) in vec2 aVColor;
        out vec2 vColor;
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
            vColor = aVColor;       
        }
    ";
    
    private float[] vertices;
    private uint[] indices;

    private string _fragmentShaderSource = @"
        #version 330 core

        in vec2 vColor;
        out vec4 FragColor;

        void main()
        {

            float alpha = 
            if (abs(vColor.x) + abs(vColor.y) > 0.0)
            {
                // convert x y (-1, 1) to 0, 1
                float r = vColor.x * 0.5 + 0.5;
                float g = vColor.y * 0.5 + 0.5;
                FragColor = vec4(r, g, 0.0, 1.0); // Red-Green gradient
            } else {
                FragColor = vec4(0.0, 0.0, 0.0, 0.0); // Blue
            }
        }
    ";

    public BootUiContext(UiStateMachine parentStateMachine) : base(parentStateMachine)
    {
        // Initialize OpenGL
        GL.GenVertexArray(out _vao);
        GL.GenBuffer(out _vbo);
        GL.GenBuffer(out _ebo);
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        // Compile shaders
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, _vertexShaderSource);
        GL.CompileShader(vertexShader);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, _fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        _shader = GL.CreateProgram();
        GL.AttachShader(_shader, vertexShader);
        GL.AttachShader(_shader, fragmentShader);
        GL.LinkProgram(_shader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        
        // Generate square in center of screen
        vertices =
        [   // positions          // Colors (R, G only)    
             0.5f,  0.5f, 0.0f,    1.0f,  1.0f, // Top Right
             0.5f, -0.5f, 0.0f,    1.0f, -1.0f, // Bottom Right
            -0.5f, -0.5f, 0.0f,   -1.0f, -1.0f, // Bottom Left
            -0.5f,  0.5f, 0.0f,   -1.0f,  1.0f  // Top Left 
        ];

        indices =
        [
            0, 1, 3,
            1, 2, 3
        ];
        
        // send data to GPU
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsage.StaticDraw);


    }

            

    public override void Update(double deltaTime)
    {
        // Calculate FPS from deltaTime
        if (deltaTime > 0)
        {
            _fps = 1.0 / deltaTime;
        }
    }

    public override void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_shader);
        GL.BindVertexArray(_vao);
        
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        
        // Enable blending for text rendering
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        // Render FPS text in top-left corner with padding
        string fpsText = $"FPS: {_fps:0.00}";
        TextRenderer.Render(fpsText, 16, new Vector3(1.0f, 1.0f, 1.0f), 10, 10);
        
        GL.Disable(EnableCap.Blend);
    }
}