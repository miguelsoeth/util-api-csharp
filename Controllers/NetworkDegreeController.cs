using Microsoft.AspNetCore.Mvc;
using util_api_csharp.Interfaces;

namespace util_api_csharp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NetworkDegreeController : ControllerBase
{
    private readonly IFilmeService _filmeService;
    private readonly IGrafoService _grafoService;
    private readonly ITmdbImageService _tmdbImageService;

    public NetworkDegreeController(IFilmeService filmeService, IGrafoService grafoService, ITmdbImageService tmdbImageService)
    {
        _filmeService = filmeService;
        _grafoService = grafoService;
        _tmdbImageService = tmdbImageService;
    }
    
    [HttpGet("/actors/all")]
    public async Task<IActionResult> getActors()
    {
        try
        {
            var actors = await _filmeService.GetAllActorsAsync();
                
            if (actors.Any())
            {
                return Ok(actors);
            }
            else
            {
                return NotFound(new { message = "Actors cannot be found" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching actors: {ex.Message}");
            return StatusCode(500, new { error = "Failed to fetch actors" });
        }
    }
    
    [HttpGet("/actors")]
    public async Task<IActionResult> searchActor([FromQuery] string query)
    {
        try
        {
            var actors = await _filmeService.GetActorAsync(query);

            if (actors != null)
            {
                return Ok(actors);
            }
            else
            {
                return NotFound(new { message = "Actors cannot be found" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching actor: {ex.Message}");
            return StatusCode(500, new { error = "Failed to fetch actor" });
        }
    }
    
    [HttpGet("/network")]
    public async Task<IActionResult> getNetwork([FromQuery] string origin, [FromQuery] string destiny)
    {
        try
        {
            var relationship = await _grafoService.GetActorNetworkAsync(origin, destiny);
            var relationWithImages = new Dictionary<string, string>();

            if (relationship != null)
            {
                foreach (var relation in relationship)
                {
                    var images = await _tmdbImageService.FetchTheMovieDBImages(relation);
                    if (images.Count > 0)
                    {
                        relationWithImages[relation] = images[0];
                    }
                }

                return Ok(relationWithImages);
            }
            else
            {
                return NotFound(new { message = "Relationship cannot be found" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating actors network: {ex.Message}");
            return StatusCode(500, new { error = "Error calculating actors network" });
        }
    }
    
    [HttpGet("/image")]
    public async Task<IActionResult> searchImage(string query)
    {
        try
        {
            var images = await _tmdbImageService.FetchTheMovieDBImages(query);
                
            if (images.Any())
            {
                return Ok(images);
            }
            else
            {
                return NotFound(new { message = "No images found" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching images: {ex.Message}");
            return StatusCode(500, new { error = "Failed to fetch images" });
        }
    }
}