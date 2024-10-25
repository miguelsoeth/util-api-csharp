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
    
    public async Task<List<List<string>>> FindAllShortestPaths(string actor1, string actor2, int limit)
    {
        limit += 1;
        var queue = new Queue<List<string>>();
        queue.Enqueue(new List<string> { actor1 });
        var visited = new HashSet<string> { actor1 };
        var paths = new List<List<string>>();

        while (queue.Count > 0)
        {
            var currentPath = queue.Dequeue();
            var currentVertex = currentPath[^1];

            // Limit path length to 8 edges (9 vertices)
            if (currentPath.Count > limit)
                continue;

            if (currentVertex == actor2)
            {
                paths.Add(new List<string>(currentPath));
                Console.WriteLine(paths.Count);
                continue;
            }

            foreach (var neighbor in _vertices[currentVertex])
            {
                // Only add to the new path if neighbor is not already in the current path to prevent cycles
                if (!currentPath.Contains(neighbor))
                {
                    var newPath = new List<string>(currentPath) { neighbor };
                    queue.Enqueue(newPath);

                    // Mark neighbor as visited only within this path's context to allow for path exploration
                    if (!visited.Contains(neighbor) && newPath.Count < limit)
                    {
                        visited.Add(neighbor);
                    }
                }
            }
        }

        if (paths.Count > 0)
        {
            Console.WriteLine($"Shortest paths between {actor1} and {actor2}:");
            Console.WriteLine(paths.Count);
            return paths;
        }
        else
        {
            Console.WriteLine($"No relationship between {actor1} and {actor2}");
            return paths;
        }
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
    
    public async Task<Dictionary<string, int>> GetAllActorsNetworkAsync(string actor1, string actor2, int limit)
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
        
        var result = await _graph.FindAllShortestPaths(actor1, actor2, limit);

        var response = new Dictionary<string, int>();

        foreach (var list in result)
        {
            string line = string.Join("->", list);
            int count = list.Count-1;
            response.Add(line, count);
        }

        return response;
    }
}