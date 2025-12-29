using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieExplorer.Models
{
    // This represents a movie that was favourited or viewed
    public class FavouriteMovie
    {
        // Basic movie info
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public string Emoji { get; set; } = "🎬";

        // When was it added/viewed?
        public DateTime Timestamp { get; set; }

        // Is it a favourite (true) or just viewed (false)?
        public bool IsFavourite { get; set; }

        // Helper: Shows genres as text
        public string GenresString => string.Join(", ", Genres);

        // Helper: Shows date/time nicely
        public string TimestampString => Timestamp.ToString("g");
    }
}