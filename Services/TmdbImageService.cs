using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using util_api_csharp.Interfaces;

namespace util_api_csharp.Services;

public class TmdbImageService : ITmdbImageService
{
    private static readonly HttpClient httpClient = new HttpClient();

    public async Task<List<string>> FetchTheMovieDBImages(string searchQuery)
    {
        var linksList = new List<string>();

        try
        {
            // Make a request to TheMovieDB
            var url = $"https://www.themoviedb.org/search?query={Uri.EscapeDataString(searchQuery)}";
            var response = await httpClient.GetStringAsync(url);

            // Load the HTML into HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Find all image elements
            var imgNodes = htmlDoc.DocumentNode.SelectNodes("//img");

            if (imgNodes != null)
            {
                foreach (var imgNode in imgNodes)
                {
                    var imgSrc = imgNode.GetAttributeValue("data-src", null) ?? imgNode.GetAttributeValue("src", null);

                    if (imgSrc != null && imgSrc.Contains("media.themoviedb.org/t/p/"))
                    {
                        var updatedLink = imgSrc
                            .Replace("w90_and_h90_face", "w300_and_h450_bestv2")
                            .Replace("w94_and_h141_bestv2", "w300_and_h450_bestv2");

                        linksList.Add(updatedLink);

                        if (linksList.Count >= 3) // Limit to 3 images
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching the page: " + ex.Message);
        }

        return linksList;
    }
}