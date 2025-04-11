using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASS_Editor.Framework
{
    public static class CatGui
    {
        public static void SkipBody()
        {
            float space = ImGui.GetContentRegionAvail().Y - ImGui.GetTextLineHeightWithSpacing();
            if (space > 0)
                ImGui.Dummy(new Vector2(0, space));
        }
        public static bool IsFocused() =>
            ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows);

        public static void DrawDebug()
        {
            if (Program.Debug)
            {
                CatGui.SkipBody();

                Vector2 winSize = ImGui.GetWindowSize();
                ImGui.Text($"Size: {winSize.X} x {winSize.Y}");
            }
        }
    }
}
