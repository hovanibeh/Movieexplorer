using MovieExplorer.Models;
using System.Collections.ObjectModel;

namespace MovieExplorer.Pages
{
    public partial class MoviesPage : ContentPage
    {
        // Store all movies from service
        private List<Movie> _allMovies = new List<Movie>();

        
        private ObservableCollection<Movie> _displayedMovies = new ObservableCollection<Movie>();

        public MoviesPage()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("=== MoviesPage Constructor ===");

            // Connect the list to the screen
            MoviesCollectionView.ItemsSource = _displayedMovies;
        }

        // Called when page appears on screen
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            System.Diagnostics.Debug.WriteLine("=== MoviesPage OnAppearing called ===");

            await LoadMoviesAsync();
        }

        // Load movies from service
        private async Task LoadMoviesAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== LoadMoviesAsync started ===");

            // Show loading spinner
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                System.Diagnostics.Debug.WriteLine("Calling MovieService.GetMoviesAsync()...");

                // Get movies from MovieService
                _allMovies = await App.MovieService.GetMoviesAsync();

                System.Diagnostics.Debug.WriteLine($"Received {_allMovies.Count} movies from service");

                // Display them
                UpdateDisplayedMovies(_allMovies);

                System.Diagnostics.Debug.WriteLine($"Displayed {_displayedMovies.Count} movies on screen");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR in LoadMoviesAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                await DisplayAlert("Error", $"Failed to load movies: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading spinner
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;

                System.Diagnostics.Debug.WriteLine("=== LoadMoviesAsync finished ===");
            }
        }

        // Update what's shown on screen
        private void UpdateDisplayedMovies(List<Movie> movies)
        {
            System.Diagnostics.Debug.WriteLine($"=== UpdateDisplayedMovies called with {movies.Count} movies ===");

            _displayedMovies.Clear();

            foreach (var movie in movies)
            {
                _displayedMovies.Add(movie);
            }

            System.Diagnostics.Debug.WriteLine($"DisplayedMovies now has {_displayedMovies.Count} items");
        }

     
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower()?.Trim() ?? string.Empty;

            System.Diagnostics.Debug.WriteLine($"========================================");
            System.Diagnostics.Debug.WriteLine($"Search text changed to: '{searchText}'");

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Empty search = show all
                System.Diagnostics.Debug.WriteLine("Empty search - showing all movies");
                UpdateDisplayedMovies(_allMovies);
            }
            else
            {
                // Filter movies by title, genre, or director
                var filtered = _allMovies.Where(m =>
                {
                    // Search in title
                    bool titleMatch = !string.IsNullOrEmpty(m.Title) &&
                                     m.Title.ToLower().Contains(searchText);

                    // Search in all genres 
                    bool genreMatch = false;
                    if (m.Genres != null && m.Genres.Count > 0)
                    {
                        genreMatch = m.Genres.Any(g =>
                            !string.IsNullOrEmpty(g) &&
                            g.ToLower().Contains(searchText));

                        // Show genres for each movie
                        if (genreMatch)
                        {
                            System.Diagnostics.Debug.WriteLine($"  ✓ MATCH: {m.Title} - Genres: {string.Join(", ", m.Genres)}");
                        }
                    }

                    // Search in director
                    bool directorMatch = !string.IsNullOrEmpty(m.Director) &&
                                        m.Director.ToLower().Contains(searchText);

                    if (directorMatch)
                    {
                        System.Diagnostics.Debug.WriteLine($"  ✓ MATCH: {m.Title} - Director: {m.Director}");
                    }

                    if (titleMatch)
                    {
                        System.Diagnostics.Debug.WriteLine($"  ✓ MATCH: {m.Title} - Title match");
                    }

                    // Return true if ANY match
                    return titleMatch || genreMatch || directorMatch;
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"Search found {filtered.Count} matching movies");
                System.Diagnostics.Debug.WriteLine($"========================================");

                UpdateDisplayedMovies(filtered);
            }
        }

        // Show all movies button
        private void OnShowAllClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Show All Movies clicked");
            UpdateDisplayedMovies(_allMovies);
        }

        // Sort by year button (newest first)
        private void OnSortByYearClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Sort by Year clicked");
            var sorted = _allMovies.OrderByDescending(m => m.Year).ToList();
            UpdateDisplayedMovies(sorted);
        }

        // Sort by rating button (highest first)
        private void OnSortByRatingClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Sort by Rating clicked");
            var sorted = _allMovies.OrderByDescending(m => m.ImdbRating).ToList();
            UpdateDisplayedMovies(sorted);
        }

        // Called when user taps a movie
        private async void OnMovieSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
            {
                System.Diagnostics.Debug.WriteLine($"Movie selected: {selectedMovie.Title}");

                // Clear the selection
                MoviesCollectionView.SelectedItem = null;

                // Navigate to detail page with movie data
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Movie", selectedMovie }
                };

                await Shell.Current.GoToAsync(nameof(MovieDetailPage), navigationParameter);
            }
        }
    }
}