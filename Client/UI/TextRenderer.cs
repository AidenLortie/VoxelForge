using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;
using VoxelForge.Client.Rendering;

namespace VoxelForge.Client.UI
{
    // Text renderer using SharpFont (FreeType) + OpenGL for menu/UI text.
    // Replaces the placeholder rectangle renderer with a real TTF glyph renderer.
    public class TextRenderer : IDisposable
    {
        private ShaderProgram _shader;
        private int _vao;
        private int _vbo;
        private int _screenWidth;
        private int _screenHeight;

        private Library _ftLib;
        private Face _ftFace;

        // Glyph data for loaded characters
        private struct Glyph
        {
            public int TextureId;    // GL texture containing the glyph bitmap (R8)
            public Vector2 Size;     // width, height in pixels
            public Vector2 Bearing;  // left, top bearing in pixels
            public int Advance;      // advance.x in pixels
        }

        private readonly Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>();

        // Pixel height to load from the font (you can expose this or support multiple sizes)
        private readonly uint _fontPixelHeight;

        public TextRenderer(int screenWidth, int screenHeight, string ttfPath, uint fontPixelHeight = 48)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _fontPixelHeight = fontPixelHeight;

            // Simple shader: samples glyph alpha from a single-channel texture
            string vertShader = @"#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoord;
out vec3 TextColor;

uniform mat4 projection;

void main()
{
    gl_Position = projection * vec4(aPos.xy, 0.0, 1.0);
    TexCoord = aTexCoord;
}";

            string fragShader = @"#version 330 core
in vec2 TexCoord;
in vec3 TextColor;
out vec4 FragColor;

uniform sampler2D text;
uniform vec3 textColor;

void main()
{
    float alpha = texture(text, TexCoord).r;
    FragColor = vec4(textColor, alpha);
}";

            _shader = new ShaderProgram(vertShader, fragShader);

            // Prepare VAO/VBO for rendering quads (6 vertices * 4 floats each = 24 floats)
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, IntPtr.Zero, BufferUsage.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Initialize FreeType and load glyphs
            _ftLib = new Library();
            _ftFace = new Face(_ftLib, ttfPath);
            _ftFace.SetPixelSizes(0, _fontPixelHeight);

            InitializeGlyphs();
        }

        public void UpdateScreenSize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }

        private void InitializeGlyphs()
        {
            // Load printable ASCII range (space .. tilde)
            for (char c = ' '; c <= '~'; c++)
            {
                LoadChar(c);
            }

            // Optionally load replacement character if missing
            if (!_glyphs.ContainsKey('?'))
                LoadChar('?');
        }

        private void LoadChar(char c)
        {
            // Load and render glyph
            _ftFace.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
            var glyphSlot = _ftFace.Glyph;
            var bitmap = glyphSlot.Bitmap;

            int width = bitmap.Width;
            int height = bitmap.Rows;

            // Prepare pixel buffer. SharpFont uses a single-channel grayscale buffer.
            byte[] pixels = new byte[Math.Max(1, width) * Math.Max(1, height)];
            if (width > 0 && height > 0)
            {
                // Copy from unmanaged buffer to managed array
                Marshal.Copy(bitmap.Buffer, pixels, 0, pixels.Length);
            }

            // Generate GL texture
            int texId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, texId);

            GL.PixelStorei(PixelStoreParameter.UnpackAlignment, 1); // important for single-channel

            // Upload as R8 / Red channel
            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Alpha8Ext, Math.Max(1, width), Math.Max(1, height), 0, PixelFormat.Red, PixelType.UnsignedByte, pixels);

            // Texture parameters - no wrapping, linear filtering
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2d, 0);

            // Extract metrics
            // BitmapLeft = bearingX (pixels to the left of origin)
            // BitmapTop = bearingY (pixels from baseline to top)
            int bearingX = glyphSlot.BitmapLeft;
            int bearingY = glyphSlot.BitmapTop;

            // Advance is in 26.6 fixed point format -> shift by 6 to get pixels
            int advance = 0;
            try
            {
                advance = (int)(glyphSlot.Advance.X.ToInt32() >> 6);
            }
            catch
            {
                // Fallback: try metrics horizontal advance
                advance = (int)(glyphSlot.Metrics.HorizontalAdvance.ToInt32() >> 6);
            }

            Glyph g = new Glyph
            {
                TextureId = texId,
                Size = new Vector2(width, height),
                Bearing = new Vector2(bearingX, bearingY),
                Advance = advance
            };

            _glyphs[c] = g;
        }

        public void RenderText(string text, float x, float y, float scale, Vector3 color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            _shader.Use();

            // Create an orthographic projection matrix
            var projection = Matrix4.CreateOrthographicOffCenter(0, _screenWidth, _screenHeight, 0, -1, 1);
            _shader.SetUniform("projection", projection);
            _shader.SetUniform("textColor", color);

            // Ensure the shader's sampler uses texture unit 0
            _shader.SetUniform("text", 0);

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindVertexArray(_vao);

            // Enable blending for text
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            float xPos = x;

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (!_glyphs.TryGetValue(ch, out var glyph))
                {
                    glyph = _glyphs.ContainsKey('?') ? _glyphs['?'] : default;
                }

                float gw = glyph.Size.X * scale;
                float gh = glyph.Size.Y * scale;

                // Calculate position with bearing: y is top-left based in your current projection (you used off-center with top=0)
                // SharpFont's bearingY is distance from baseline to top of bitmap. We'll treat input y as baseline for convenience.

                // If you want 'y' to be top coordinate instead of baseline, adjust accordingly. Here we'll assume y is top.
                float xpos = xPos + glyph.Bearing.X * scale;
                float ypos = y + (glyph.Bearing.Y - glyph.Size.Y) * scale; // convert baseline-based to top-left

                // Vertex data for the quad (6 verts, each: pos.x, pos.y, tex.u, tex.v)
                float[] vertices = new float[]
                {
                    // x      y           u    v
                    xpos,         ypos + gh, 0.0f, 1.0f,
                    xpos,         ypos,      0.0f, 0.0f,
                    xpos + gw,    ypos,      1.0f, 0.0f,

                    xpos,         ypos + gh, 0.0f, 1.0f,
                    xpos + gw,    ypos,      1.0f, 0.0f,
                    xpos + gw,    ypos + gh, 1.0f, 1.0f
                };

                // Update VBO
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                // Bind glyph texture and draw
                GL.BindTexture(TextureTarget.Texture2d, glyph.TextureId);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                // Advance cursor
                xPos += (glyph.Advance * scale);

                // extra letter spacing if you want: xPos += 1.0f * scale;
            }

            // Unbind / cleanup
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2d, 0);
            GL.Disable(EnableCap.Blend);
        }

        public void Dispose()
        {
            // Delete glyph textures
            foreach (var kv in _glyphs)
            {
                if (kv.Value.TextureId != 0)
                    GL.DeleteTexture(kv.Value.TextureId);
            }
            _glyphs.Clear();

            // GL buffers
            if (_vbo != 0) GL.DeleteBuffer(_vbo);
            if (_vao != 0) GL.DeleteVertexArray(_vao);

            // Dispose shader
            _shader?.Dispose();

            // Free FreeType objects
            _ftFace?.Dispose();
            _ftLib?.Dispose();
        }
    }
}
