using System.Diagnostics.CodeAnalysis;

namespace DibariBot;

public struct Bookmark
{
    public string? chapter;
    public int page = 1;

    public Bookmark()
    { }

    public Bookmark(string chapter, int page = 1)
    {
        this.chapter = chapter;
        this.page = page;
    }

    // stupid
    [MemberNotNull(nameof(chapter))]
    public readonly void NullCheck()
    {
        if (chapter == null)
        {
            throw new NullReferenceException(nameof(chapter));
        }
    }

    public override readonly string ToString()
    {
        return $"Chapter \"{chapter}\", page {page}.";
    }
}
