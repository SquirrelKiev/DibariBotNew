using System.Diagnostics.CodeAnalysis;

namespace DibariBot;

public struct Bookmark
{
    public string chapter = "n/a";
    public int page = 1;

    public Bookmark()
    { }

    public Bookmark(string chapter, int page = 1)
    {
        this.chapter = chapter;
        this.page = page;
    }

    public override readonly string ToString()
    {
        return $"Chapter \"{chapter}\", page {page}.";
    }
}
