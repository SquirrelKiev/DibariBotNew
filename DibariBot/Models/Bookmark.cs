namespace DibariBot;

public struct Bookmark
{
    public string chapter = "";
    public int page = 1;

    public Bookmark()
    { }

    public Bookmark(string chapter, int page = 1)
    {
        this.chapter = chapter;
        this.page = page;
    }

    public readonly override string ToString()
    {
        return $"Chapter \"{chapter}\", page {page}.";
    }
}
