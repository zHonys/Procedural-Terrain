using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Numerics;
using Terrain.Generators;

namespace Terrain.Debug
{
    public class GUI : IDisposable
    {
        readonly ImGuiController controller;

        public GUI(GL GL, IWindow window, IInputContext input)
        {
            controller = new ImGuiController(GL, window, input);
        }
        public void Update(double delta) => controller.Update((float)delta);
        public static void CreateDrags(Span<HeightColor> colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                ImGui.PushID(colors[i].Name);
                ImGui.DragFloat("", ref colors[i].Height, 0.01f, 0, 1);
                ImGui.NextColumn();
                var color = colors[i].Color.ToSystem();
                ImGui.ColorEdit3(colors[i].Name, ref color, ImGuiColorEditFlags.NoInputs);
                colors[i].Color = color.ToGeneric();
                ImGui.NextColumn();
            }
        }
        float bound = 100;
        public void Draw(ref float scalar)
        {
            ImGui.Begin("Perlin");
            ImGui.DragFloat("Perlin Scalar", ref scalar, 1, -bound, bound);

            controller.Render();
        }
        public void Dispose()
        {
            controller.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
