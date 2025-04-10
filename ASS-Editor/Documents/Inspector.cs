using ASS_Editor.Framework;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASS_Editor.Documents;

public class Inspector : Document
{
    public new string FriendlyName = "Inspector";
    internal new Vector2 DocumentSize;

    public override void Dispose()
    {
    }

    public override void Init()
    {
    }

    public override void Render()
    {
        if (ImGui.Begin("Inspector", ref IsVisible))
        {
            ImGui.Text("you suck");

            if (Program.Debug)
            {
                CatGui.SkipBody();

                Vector2 winSize = ImGui.GetWindowSize();
                ImGui.Text($"Size: {winSize.X} x {winSize.Y}");
            }
        }

        ImGui.End();
    }

    public override void Update()
    {
    }
}
