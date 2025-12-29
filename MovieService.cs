using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MovieExplorer.Models;

namespace MovieExplorer.Services
{
    // This class handles downloading and caching movie data
    public class MovieService
    {
        // Tool to download from internet
        private readonly HttpClient _httpClient;

        // Where to save the movie file on the phone
        private readonly string _cacheFilePath;

        // The URL where movies are stored online
        private const string MovieJsonUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/refs/heads/main/moviesemoji.json";

        // Constructor - runs when we create a MovieService
        public MovieService()
        {
            _httpClient = new HttpClient();

            // FileSystem.AppDataDirectory = special folder for our app's data
            // We'll save movies in a file called "movies_cache.json"
            _cacheFilePath = Path.Combine(FileSystem.AppDataDirectory, "movies_cache.json");
        }

        // Main method: Get all movies (download first time, then load from cache)
        public async Task<List<Movie>> GetMoviesAsync()
        {
            try
            {
                // Does the cache file exist on the phone?
                if (File.Exists(_cacheFilePath))
                {
                    // YES - Load from cache (fast!)
                    string cachedJson = await File.ReadAllTextAsync(_cacheFilePath);
                    var movies = JsonSerializer.Deserialize<List<Movie>>(cachedJson);
                    return movies ?? new List<Movie>();
                }
                else
                {
                    // NO - Download from internet (first time only)
                    return await DownloadAndCacheMoviesAsync();
                }
            }
            catch (Exception ex)
            {
                // If something goes wrong, show error and return empty list
                System.Diagnostics.Debug.WriteLine($"Error loading movies: {ex.Message}");
                return new List<Movie>();
            }
        }

        // Download movies from internet and save to phone
        private async Task<List<Movie>> DownloadAndCacheMoviesAsync()
        {
            try
            {
                // Step 1: Download JSON text from URL
                string json = await _httpClient.GetStringAsync(MovieJsonUrl);

                // Step 2: Save to phone (cache it)
                await File.WriteAllTextAsync(_cacheFilePath, json);

                // Step 3: Convert JSON text to List of Movie objects
                var movies = JsonSerializer.Deserialize<List<Movie>>(json);
                return movies ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                // If download fails, show error and return empty list
                System.Diagnostics.Debug.WriteLine($"Error downloading movies:");
                return new List<Movie>();
            }
        }

        // Delete the cache file (useful for testing or refresh)
        public void ClearCache()
        {
            if (File.Exists(_cacheFilePath))
            {
                File.Delete(_cacheFilePath);
            }
        }
    }
}