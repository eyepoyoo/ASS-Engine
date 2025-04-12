using ASS_Editor.Documents;
using ASS_Editor.Framework;
using ImGuiNET;
using Newtonsoft.Json;
using Raylib_cs;
using rlImGui_cs;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace ASS_Editor;

internal class Program
{
    private static bool Quit;
    public static bool Debug;
    private static bool loaddefaultlayout;

    private static float MenuBarHeight;
    private static readonly ImGuiWindowFlags WndowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                  ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                  ImGuiWindowFlags.NoBringToFrontOnFocus |
                  ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoBackground;

    private static List<Document> OpenDocuments = new();
    private static List<Document> TotalDocuments = new();

    private static List<string> Workspaces = new();
    static void Main(string[] args)
    {
        if (!File.Exists("imgui.ini"))
        {
            Console.WriteLine("first run");
            loaddefaultlayout = true;
        }

        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1280, 800, "ASS Editor");
        Raylib.SetTargetFPS(144);

        rlImGui.Setup(true, true);
        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        var style = ImGui.GetStyle();
        if ((ImGui.GetIO().ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
        {
            style.WindowRounding = 0.0f;
            style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
        }

        if (loaddefaultlayout)
        {
            if (File.Exists("workspaces/default.uwc"))
                File.Copy("workspaces/default.uwc", "uiworkspace.uwc");

            if (File.Exists("workspaces/default.ini"))
                ImGui.LoadIniSettingsFromDisk("workspaces/default.ini");

            loaddefaultlayout = false;
        }

        RegisterDocument(new Explorer(), new Inspector(), new SceneView());

        if (File.Exists("uiworkspace.uwc"))
        {
            string json = File.ReadAllText("uiworkspace.uwc");

            List<string>? nameofthedocs = JsonConvert.DeserializeObject<List<string>>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    args.ErrorContext.Handled = true;
                },
            });


            if (nameofthedocs != null)
            {
                //NOT good code
                foreach (var item in TotalDocuments)
                {
                    for (int i = 0; i < nameofthedocs.Count; i++)
                    {
                        if (item.FriendlyName == nameofthedocs[i])
                        {
                            item.IsVisible = true;
                            OpenDocuments.Add(item);
                            nameofthedocs.RemoveAt(i);
                        }
                    }
                }
            }

            for (int i = 0; i < OpenDocuments.Count; i++)
            {
                OpenDocuments[i].IsVisible = true;
            }
        }

        Workspaces = GetWorkspaceDefaults();

        while (!Raylib.WindowShouldClose() && !Quit)
        {
            foreach (var doc in OpenDocuments)
            {
                doc.Update();
            }

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

        List<string> docNames = new();
        foreach (var doc in OpenDocuments)
        {
            docNames.Add(doc.FriendlyName);
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
            TotalDocuments.Add(docs[i]);
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
                //if (ImGui.BeginMenu("Workspaces"))
                //{
                //    if (Workspaces != null)
                //    {
                //        if (Workspaces == null)
                //        {
                //            ImGui.Text("Workspaces not found");
                //        } else
                //        {
                //            foreach (var workspace in Workspaces)
                //            {
                //                if (ImGui.MenuItem(workspace))
                //                {
                //                    ImGui.LoadIniSettingsFromDisk(workspace);
                //                }
                //            }
                //        }
                //    }
                //    ImGui.EndMenu();
                //}

                foreach (var doc in TotalDocuments)
                {
                    ImGui.MenuItem(doc.FriendlyName, string.Empty, ref doc.IsVisible);
                    if (doc.IsVisible && !OpenDocuments.Contains(doc))
                    {
                        OpenDocuments.Add(doc);
                        SaveWorkspace();
                    } else if (!doc.IsVisible && OpenDocuments.Contains(doc))
                    {
                        OpenDocuments.Remove(doc);
                        SaveWorkspace();
                    }
                }

                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();

            MenuBarHeight = ImGui.GetFrameHeight();
        }
    }

    private static List<string> GetWorkspaceDefaults()
    {
        List<string> foundWorkspaces;
        if (Directory.Exists("workspaces"))
        {
            foundWorkspaces = Directory.GetFiles("workspaces").ToList();
            return foundWorkspaces;
        }

        return null;
    }

    private static void SaveWorkspace()
    {
        List<string> docNames = new();
        foreach (var doc in OpenDocuments)
        {
            docNames.Add(doc.FriendlyName);
        }

        File.WriteAllText("uiworkspace.uwc", JsonConvert.SerializeObject(docNames, Formatting.None));
    }
}