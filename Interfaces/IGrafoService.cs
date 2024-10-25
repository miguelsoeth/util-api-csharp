namespace util_api_csharp.Interfaces;

public interface IGrafoService
{
    Task<List<string>> GetActorNetworkAsync(string actor1, string actor2);
}