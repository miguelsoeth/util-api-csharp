namespace util_api_csharp.Interfaces;

public interface ITmdbImageService
{
    Task<List<string>> FetchTheMovieDBImages(string searchQuery);
}