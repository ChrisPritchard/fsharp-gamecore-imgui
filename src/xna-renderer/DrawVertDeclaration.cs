using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Taken directly from https://github.com/mellinoe/ImGui.NET (src/ImGui.NET.SampleProgram.XNA)
/// All credit to mellinoe@gmail.com
/// </summary>

namespace GameCore.ImGuiXnaRenderer
{
    public static class DrawVertDeclaration
    {
        public static readonly VertexDeclaration Declaration;

        public static readonly int Size;

        static DrawVertDeclaration()
        {
            unsafe { Size = sizeof(ImDrawVert); }

            Declaration = new VertexDeclaration(
                Size,

                // Position
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),

                // UV
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

                // Color
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );
        }
    }
}