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

public class SceneView : Document
{
    private Camera2D SceneCam;
    private Image rlBob;
    private Texture2D bob;
    private bool openBobContextMenu = false;

    private Rectangle sourceRect;
    private Rectangle destRect;

    private void refBob()
    {
        bob = Raylib.LoadTextureFromImage(rlBob);
        sourceRect = new Rectangle(0, 0, bob.Width, bob.Height);
        destRect = new Rectangle(0, 0, bob.Width, bob.Height);
    }

    public override void Dispose()
    {
        Raylib.UnloadRenderTexture(ViewTex);
    }

    public override void Init()
    {
        FriendlyName = "Scene View";
        ViewTex = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        rlBob = Raylib.LoadImage("assets/bob.png");
        refBob();

        SceneCam = new Camera2D();
        SceneCam.Zoom = 0.1f;
    }

    public override void Render()
    {
        if (ImGui.Begin(FriendlyName, ref IsVisible, ImGuiWindowFlags.NoScrollbar))
        {
            IsFocused = CatGui.IsFocused();
            rlImGui.ImageRenderTextureFit(ViewTex, true);

            Vector2 overlayPos = ImGui.GetItemRectMin() + new Vector2(30, 30);
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            drawList.AddText(overlayPos, ImGui.GetColorU32(ImGuiCol.Text), $"[DEBUG] cam targ: {SceneCam.Target}");
            drawList.AddText(overlayPos + new Vector2(0,15), ImGui.GetColorU32(ImGuiCol.Text), $"[DEBUG] cam zoom: {SceneCam.Zoom}"); //i will not kms i will not kms i will not kms

            if (openBobContextMenu)
            {
                ImGui.OpenPopup("BobContextMenu");
                openBobContextMenu = false;
            }

            if (ImGui.BeginPopup("BobContextMenu"))
            {
                CatGui.AddShortcut(KeyboardKey.F5, () => {
                    Raylib.UnloadTexture(bob);
                    bob = Raylib.LoadTexture("assets/bob_happy.png");
                });

                if (ImGui.MenuItem("Pet Bob", "F5"))
                {
                    Console.WriteLine("Bob is happy :)");
                    Raylib.UnloadTexture(bob);
                    bob = Raylib.LoadTexture("assets/bob_happy.png");
                }
                if (ImGui.MenuItem("Yell at Bob"))
                {
                    Console.WriteLine("Bob is sad :(");
                    Raylib.UnloadTexture(bob);
                    bob = Raylib.LoadTexture("assets/bob_mad.png");
                }
                ImGui.EndPopup();
            }

        }
        ImGui.End();
    }

    public bool debugMode = true;

    public override void Update()
    {
        if (Raylib.IsWindowResized())
        {
            Raylib.UnloadRenderTexture(ViewTex);
            ViewTex = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }

        if (!IsFocused) return;
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
        Raylib.DrawTexturePro(bob, sourceRect, destRect, Vector2.Zero, 0, Color.White);
        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            Vector2 mousePosScreen = Raylib.GetMousePosition();

            Vector2 mousePosWorld = Raylib.GetScreenToWorld2D(mousePosScreen, SceneCam);

            // Check if world-space mouse is within the destination rectangle
            Rectangle clickBounds = new Rectangle(destRect.X, destRect.Y, destRect.Width, destRect.Height);

            if (Raylib.CheckCollisionPointRec(mousePosWorld, clickBounds))
            {
                openBobContextMenu = true;
            }
        }


        Raylib.EndMode2D();
        Raylib.EndTextureMode();
    }

}
