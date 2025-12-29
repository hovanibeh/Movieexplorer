using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace MovieExplorer.Models
{
    // This class represents a single movie
    public class Movie
    {
        // The movie's title (e.g., "The Shawshank Redemption")
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        // The year it was released (e.g., 1994)
        [JsonPropertyName("year")]
        public int Year { get; set; }

        // List of genres (e.g., "Drama", "Crime")
        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; } = new List<string>();

        // Who directed it
        [JsonPropertyName("director")]
        public string Director { get; set; } = string.Empty;

        // IMDB rating (e.g., 9.3)
        [JsonPropertyName("imdb_rating")]
        public double ImdbRating { get; set; }

        // An emoji to represent the movie (e.g., "🎬")
        [JsonPropertyName("emoji")]
        public string Emoji { get; set; } = "🎬";

        // Helper: Shows genres as "Action, Drama, Thriller"
        public string GenresString => string.Join(", ", Genres);
    }
}