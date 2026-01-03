using System.Text.Json.Serialization;

namespace MovieExplorer.Models
{
    public class Movie
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("year")]
        public int Year { get; set; }

        // FIXED: It's "genre" not "genres"!
        [JsonPropertyName("genre")]
        public List<string> Genres { get; set; } = new List<string>();

        [JsonPropertyName("director")]
        public string Director { get; set; } = string.Empty;

        // FIXED: It's "rating" not "imdb_rating"!
        [JsonPropertyName("rating")]
        public double ImdbRating { get; set; }

        [JsonPropertyName("emoji")]
        public string Emoji { get; set; } = "🎬";

        // Shows genres as "Action, Drama"
        public string GenresString => Genres != null && Genres.Any()
            ? string.Join(", ", Genres)
            : "Unknown";
    }
}