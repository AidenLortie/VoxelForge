# Text Rendering Bug Fix

## Issue
Text rendering in the menu was non-functional. All text appeared in grayscale/white regardless of the color parameter passed to `RenderText()`.

## Root Cause
The fragment shader in `Client/UI/TextRenderer.cs` was not using the `textColor` uniform that was being set by the application.

### Before (Broken)
```glsl
void main()
{
    float alpha = texture(text, TexCoord).r;
    FragColor = vec4(alpha, alpha, alpha, alpha);  // ❌ Outputs grayscale
}
```

This code was outputting the glyph's alpha value for all four RGBA channels, resulting in grayscale text.

### After (Fixed)
```glsl
void main()
{
    float alpha = texture(text, TexCoord).r;
    FragColor = vec4(textColor, alpha);  // ✅ Uses textColor uniform
}
```

This code correctly uses the `textColor` uniform (RGB) with the glyph's alpha channel.

## Impact
The menu now displays text with proper colors:
- **Title**: White text (`rgb(1.0, 1.0, 1.0)`)
- **Instructions**: Light gray (`rgb(0.7, 0.7, 0.7)`)
- **Selected menu item**: Yellow highlight (`rgb(1.0, 0.9, 0.3)`)
- **Unselected menu items**: Gray (`rgb(0.8, 0.8, 0.8)`)
- **Menu descriptions**: Dark gray (`rgb(0.6, 0.6, 0.6)`)

## Technical Details
- **File Modified**: `Client/UI/TextRenderer.cs`, line 71
- **Change Type**: Fragment shader bug fix
- **Lines Changed**: 1
- **Tests**: All 50 existing tests pass
- **Build**: Successful with no new warnings

## How the Text Renderer Works
1. `TextRenderer.RenderText()` is called with position, scale, and **color** parameters
2. The color is set as a uniform in the shader (line 193)
3. For each character, a glyph texture (single-channel, grayscale) is rendered
4. The fragment shader samples the texture's red channel as the alpha value
5. The shader outputs the final color: `vec4(textColor.rgb, alpha)`
6. OpenGL blends the result using the alpha channel for transparency

## Verification
The fix was verified by:
- ✅ Successful compilation with no errors
- ✅ All 50 existing unit tests pass
- ✅ Code review confirms proper GLSL syntax and logic
- ✅ Minimal change (1 line) reduces risk of side effects
