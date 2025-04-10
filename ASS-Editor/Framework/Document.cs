using System.Numerics;

namespace ASS_Editor.Framework;

public abstract class Document
{
    public string FriendlyName = "Document";
    internal Vector2 DocumentSize;

    public bool IsVisible;
    public bool IsFocused;

    public Raylib_cs.RenderTexture2D ViewTex;

    public abstract void Init();
    public abstract void Update();
    public abstract void Render();
    public abstract void Dispose();
}
