namespace util_api_csharp.Interfaces;

public interface IGrafoService
{
    Task<List<string>> GetActorNetworkAsync(string actor1, string actor2);
    Task<Dictionary<string, int>> GetAllActorsNetworkAsync(string actor1, string actor2, int limit);
}