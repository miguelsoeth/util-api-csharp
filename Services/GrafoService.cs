using util_api_csharp.Interfaces;
using util_api_csharp.Models;

namespace util_api_csharp.Services;

public class Graph
{
    private readonly Dictionary<string, HashSet<string>> _vertices;

    public Graph()
    {
        _vertices = new Dictionary<string, HashSet<string>>();
    }

    public void AddVertex(string vertex)
    {
        if (!_vertices.ContainsKey(vertex))
        {
            _vertices[vertex] = new HashSet<string>();
        }
    }

    public void AddEdge(string origin, string destination)
    {
        if (!_vertices.ContainsKey(origin) || !_vertices.ContainsKey(destination))
        {
            Console.WriteLine("Vertices do not exist.");
            return;
        }

        _vertices[origin].Add(destination);
        _vertices[destination].Add(origin); // Bidirectional edge
    }

    public List<string> FindShortestRelationship(string actor1, string actor2)
    {
        var visited = new HashSet<string>();
        var queue = new Queue<List<string>>();
        
        queue.Enqueue(new List<string> { actor1 });
        visited.Add(actor1);

        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var lastVertex = path.Last();

            if (lastVertex == actor2)
            {
                return path;
            }

            foreach (var neighbor in _vertices[lastVertex])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    var newPath = new List<string>(path) { neighbor };
                    queue.Enqueue(newPath);
                }
            }
        }

        return null; // No path found
    }
}

public class GrafoService : IGrafoService
{
    private readonly Graph _graph;
    private readonly IFilmeService _filmeService;

    public GrafoService(IFilmeService filmeService)
    {
        _filmeService = filmeService;
        _graph = new Graph();
    }

    public async Task<List<string>> GetActorNetworkAsync(string actor1, string actor2)
    {
        var movies = await _filmeService.GetMoviesAsync();

        foreach (var movie in movies)
        {
            _graph.AddVertex(movie.Title);

            foreach (var actor in movie.Cast)
            {
                _graph.AddVertex(actor);
                _graph.AddEdge(movie.Title, actor);
            }
        }

        return _graph.FindShortestRelationship(actor1, actor2);
    }
}