using ASS_Editor.Documents;
using ASS_Editor.Framework;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

namespace ASS_Editor;

internal class Program
{
    private static bool throwa;
    private static bool Quit;
    public static bool Debug;

    private static List<Document> OpenDocuments = new();

    static void Main(string[] args)
    {

        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint);
        Raylib.InitWindow(1280, 800, "ASS Editor");
        Raylib.SetTargetFPS(144);

        rlImGui.Setup(true, true);
        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

        Inspector inspector = new Inspector();
        OpenDocuments.Add(inspector);

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
    }

    private static void DoMainMenu()
    {
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
        }
    }
}