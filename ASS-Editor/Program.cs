using ASS_Editor.Documents;
using ASS_Editor.Framework;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.IO;
using System.Numerics;

namespace ASS_Editor;

internal class Program
{
    private static bool Quit;
    public static bool Debug;


    private static float MenuBarHeight;
    private static readonly ImGuiWindowFlags WndowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                  ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                  ImGuiWindowFlags.NoBringToFrontOnFocus |
                  ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoBackground;

    private static List<Document> OpenDocuments = new();

    static void Main(string[] args)
    {

        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1280, 800, "ASS Editor");
        Raylib.SetTargetFPS(144);

        rlImGui.Setup(true, true);
        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

        var style = ImGui.GetStyle();
        if ((ImGui.GetIO().ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
        {
            style.WindowRounding = 0.0f;
            style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
        }

        RegisterDocument(new Explorer(), new Inspector());//god damn these half japanese girls, do it to me everytime of the Oh, the redhead said you shred the cello
//        And I'm jello, baby
//But you won't talk, won't look, won't think of me
//I'm the epitome of public enemy
//Why you wanna go and do me like that ?
//Come down on the street and dance with me

        while (!Raylib.WindowShouldClose() && !Quit)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            rlImGui.Begin();
            DoMainMenu();


            foreach (var doc in OpenDocuments)
            {
                if (!doc.IsVisible) continue;

                doc.Render();
            }

            rlImGui.End();
            Raylib.EndDrawing();
        }

        rlImGui.Shutdown();
        foreach (var doc in OpenDocuments)
        {
            doc.Dispose();
        }

        Raylib.CloseWindow();
    }

    private static void RegisterDocument(params Document[] docs)
    {
        for (int i = 0; i < docs.Length; i++)
        {
            docs[i].Init();
            OpenDocuments.Add(docs[i]);
        }
    }

    private static void DoMainMenu()
    {
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        ImGui.SetNextWindowSize(ImGui.GetIO().DisplaySize);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.SetNextWindowPos(new Vector2(0, MenuBarHeight));
        ImGui.SetNextWindowSize(new Vector2(
            ImGui.GetIO().DisplaySize.X,
            ImGui.GetIO().DisplaySize.Y - MenuBarHeight
        ));


        ImGui.Begin("EditorDockspace", WndowFlags);
        ImGui.PopStyleVar(2);

        ImGui.DockSpace(ImGui.GetID("EditorDockspace"), Vector2.Zero, ImGuiDockNodeFlags.None);

        ImGui.End();


        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Debug"))
                    Debug = true;

                if (ImGui.MenuItem("Exit"))
                    Quit = true;

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Window"))
            {
                foreach (var doc in OpenDocuments)
                {
                    ImGui.MenuItem(doc.FriendlyName, string.Empty, ref doc.IsVisible);
                }

                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();

            MenuBarHeight = ImGui.GetFrameHeight();
        }
    }
}