using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using MovieExplorer.Models;

namespace MovieExplorer.Services
{
    public class MovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _cacheFilePath;
        private const string MovieJsonUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/refs/heads/main/moviesemoji.json";

        public MovieService()
        {
            _httpClient = new HttpClient();
            _cacheFilePath = Path.Combine(FileSystem.AppDataDirectory, "movies_cache.json");

            // DEBUG: Show where we're saving files
            System.Diagnostics.Debug.WriteLine($"Cache file path: {_cacheFilePath}");
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== GetMoviesAsync called ===");

                // Check if cache file exists
                if (File.Exists(_cacheFilePath))
                {
                    System.Diagnostics.Debug.WriteLine("Cache file exists, loading from cache...");
                    string cachedJson = await File.ReadAllTextAsync(_cacheFilePath);

                    System.Diagnostics.Debug.WriteLine($"Cache file size: {cachedJson.Length} characters");

                    var movies = JsonSerializer.Deserialize<List<Movie>>(cachedJson);

                    System.Diagnostics.Debug.WriteLine($"Loaded {movies?.Count ?? 0} movies from cache");

                    return movies ?? new List<Movie>();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No cache file, downloading from web...");
                    return await DownloadAndCacheMoviesAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in GetMoviesAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<Movie>();
            }
        }

        private async Task<List<Movie>> DownloadAndCacheMoviesAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Downloading from: {MovieJsonUrl}");

                // Download JSON from URL
                string json = await _httpClient.GetStringAsync(MovieJsonUrl);

                System.Diagnostics.Debug.WriteLine($"Downloaded {json.Length} characters");

                // Save to cache file
                await File.WriteAllTextAsync(_cacheFilePath, json);

                System.Diagnostics.Debug.WriteLine("Saved to cache file");

                // Parse and return
                var movies = JsonSerializer.Deserialize<List<Movie>>(json);

                System.Diagnostics.Debug.WriteLine($"Parsed {movies?.Count ?? 0} movies");

                return movies ?? new List<Movie>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR downloading movies: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<Movie>();
            }
        }

        public void ClearCache()
        {
            if (File.Exists(_cacheFilePath))
            {
                File.Delete(_cacheFilePath);
                System.Diagnostics.Debug.WriteLine("Cache cleared");
            }
        }
    }
}