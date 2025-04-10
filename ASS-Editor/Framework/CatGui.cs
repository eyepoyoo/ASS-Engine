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
    }
}
