using util_api_csharp.Models;

namespace util_api_csharp.Interfaces;

public interface IFilmeService
{
    Task<ActorResult> GetActorAsync(string query, int page = 1, int limit = 15);
    Task<List<string>> GetAllActorsAsync();
    Task<List<Movie>> GetMoviesAsync();
}