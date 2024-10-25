using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using util_api_csharp.Interfaces;
using util_api_csharp.Models;

namespace util_api_csharp.Services;

public class FilmeService : IFilmeService
{
    public async Task<ActorResult> GetActorAsync(string query, int page = 1, int limit = 15)
    {
        var movies = await GetMoviesAsync();
        var actorsResult = new HashSet<string>();

        foreach (var movie in movies)
        {
            foreach (var actor in movie.Cast)
            {
                if (actor.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    actorsResult.Add(actor);
                }
            }
        }

        var actorsArray = actorsResult.ToList();
        var total = actorsArray.Count;
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var result = new ActorResult
        {
            Results = actorsArray.Skip((page - 1) * limit).Take(limit).ToList(),
            Total = total,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return result;
    }

    public async Task<List<string>> GetAllActorsAsync()
    {
        var movies = await GetMoviesAsync();
        var uniqueActors = new HashSet<string>();

        foreach (var movie in movies)
        {
            foreach (var actor in movie.Cast)
            {
                uniqueActors.Add(actor);
            }
        }

        return uniqueActors.ToList();
    }

    public async Task<List<Movie>> GetMoviesAsync()
    {
        try
        {
            var data = await File.ReadAllTextAsync("./Assets/latest_movies.json");
            var result = JsonSerializer.Deserialize<List<Movie>>(data) ?? new List<Movie>();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return new List<Movie>();
        }
    } 
}