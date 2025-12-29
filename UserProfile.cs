using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieExplorer.Models
{
    // This represents the user's profile
    public class UserProfile
    {
        // The user's name (e.g., "John Doe")
        public string UserName { get; set; } = string.Empty;

        // List of movies they favourited
        public List<FavouriteMovie> Favourites { get; set; } = new List<FavouriteMovie>();

        // List of movies they viewed
        public List<FavouriteMovie> ViewingHistory { get; set; } = new List<FavouriteMovie>();
    }
}