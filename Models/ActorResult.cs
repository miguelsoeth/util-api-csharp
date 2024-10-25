namespace util_api_csharp.Models;

public class ActorResult
{
    public List<string> Results { get; set; } = new List<string>();
    public int Total { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}