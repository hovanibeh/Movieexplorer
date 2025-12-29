using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieExplorer.Models
{
    // This stores user preferences/settings
    public class AppSettings
    {
        // Is dark mode on? (false = light mode)
        public bool IsDarkMode { get; set; } = false;

        // Font size: "Small", "Medium", or "Large"
        public string FontSize { get; set; } = "Medium";

        // How to sort: "Title", "Year", or "Rating"
        public string SortBy { get; set; } = "Title";
    }
}