using System.Text.Json.Serialization;

namespace util_api_csharp.Models;

public class Movie
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("cast")]
    public List<string> Cast { get; set; } = new List<string>();
}