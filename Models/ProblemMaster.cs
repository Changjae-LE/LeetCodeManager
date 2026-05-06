namespace LeetCodeManager.Models;

public class ProblemMaster
{
    public string CanonicalSlug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public List<string> Topics { get; set; } = new();
    public List<string> IncludedIn { get; set; } = new();
    public int ListCount { get; set; }
    public string Url { get; set; } = "";
}
