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
    public override void Dispose()
    {
    }

    public override void Init()
    {
        FriendlyName = "Inspector";
    }

    public override void Render()
    {
        if (ImGui.Begin("Inspector", ref IsVisible))
        {
            IsFocused = CatGui.IsFocused();
            ImGui.Text("you suck");

            CatGui.DrawDebug();
        }

        ImGui.End();
    }

    public override void Update()
    {
    }
}
