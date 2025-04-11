using ASS_Editor.Framework;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASS_Editor.Documents;

public class Scene : Document
{
    private Camera2D SceneCam;
    private Texture2D bob;

    public override void Dispose()
    {
        Raylib.UnloadRenderTexture(ViewTex);
    }

    public override void Init()
    {
        FriendlyName = "Scene View";
        ViewTex = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        bob = Raylib.LoadTexture("bob.png");

        SceneCam = new Camera2D();
        SceneCam.Zoom = 0.1f;
    }

    public override void Render()
    {
        if (ImGui.Begin(FriendlyName, ref IsVisible, ImGuiWindowFlags.NoScrollbar)) 
        {
            IsFocused = CatGui.IsFocused();
            rlImGui.ImageRenderTextureFit(ViewTex, true);

            Vector2 textPos = new Vector2(30, 30);
            ImGui.SetCursorPos(textPos);
            ImGui.Text($"[DEBUG] target: {SceneCam.Target}");

            textPos.Y += 15;
            ImGui.SetCursorPos(textPos);
            ImGui.Text($"[DEBUG] zoom: {SceneCam.Zoom}");
        }

        ImGui.End();
    }

    public override void Update()
    {
        if (Raylib.IsWindowResized())
        {
            Raylib.UnloadRenderTexture(ViewTex);
            ViewTex = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }


        if (Raylib.IsKeyDown(KeyboardKey.W))
            SceneCam.Target -= Vector2.UnitY;
        if (Raylib.IsKeyDown(KeyboardKey.S))
            SceneCam.Target += Vector2.UnitY;
        if (Raylib.IsKeyDown(KeyboardKey.A))
            SceneCam.Target -= Vector2.UnitX;
        if (Raylib.IsKeyDown(KeyboardKey.D))
            SceneCam.Target += Vector2.UnitX;

        if (Raylib.GetMouseWheelMove() > 0)
            SceneCam.Zoom += 0.1f;
        if (Raylib.GetMouseWheelMove() < 0)
            SceneCam.Zoom -= 0.1f;

        Raylib.BeginTextureMode(ViewTex);
        Raylib.ClearBackground(Color.SkyBlue);
        Raylib.BeginMode2D(SceneCam);
        Raylib.DrawTextureEx(bob, new Vector2(0,0),0, 1, Color.White);
        Raylib.EndMode2D();
        Raylib.EndTextureMode();
    }
}
