using MovieExplorer.Models;
using System.Collections.ObjectModel;

namespace MovieExplorer.Pages
{
    public partial class MoviesPage : ContentPage
    {
        // Store all movies from service
        private List<Movie> _allMovies = new List<Movie>();

        // Movies currently shown on screen (can be filtered)
        private ObservableCollection<Movie> _displayedMovies = new ObservableCollection<Movie>();

        public MoviesPage()
        {
            InitializeComponent();

            // Connect the list to the screen
            MoviesCollectionView.ItemsSource = _displayedMovies;
        }

        // Called when page appears on screen
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMoviesAsync();
        }

        // Load movies from service
        private async Task LoadMoviesAsync()
        {
            // Show loading spinner
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            try
            {
                // Get movies from MovieService
                _allMovies = await App.MovieService.GetMoviesAsync();

                // Display them
                UpdateDisplayedMovies(_allMovies);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load movies: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading spinner
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        // Update what's shown on screen
        private void UpdateDisplayedMovies(List<Movie> movies)
        {
            _displayedMovies.Clear();
            foreach (var movie in movies)
            {
                _displayedMovies.Add(movie);
            }
        }

        // Called when user types in search bar
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Empty search = show all
                UpdateDisplayedMovies(_allMovies);
            }
            else
            {
                // Filter movies by title or genre
                var filtered = _allMovies.Where(m =>
                    m.Title.ToLower().Contains(searchText) ||
                    m.Genres.Any(g => g.ToLower().Contains(searchText))
                ).ToList();

                UpdateDisplayedMovies(filtered);
            }
        }

        // Show all movies button
        private void OnShowAllClicked(object sender, EventArgs e)
        {
            UpdateDisplayedMovies(_allMovies);
        }

        // Sort by year button (newest first)
        private void OnSortByYearClicked(object sender, EventArgs e)
        {
            var sorted = _allMovies.OrderByDescending(m => m.Year).ToList();
            UpdateDisplayedMovies(sorted);
        }

        // Sort by rating button (highest first)
        private void OnSortByRatingClicked(object sender, EventArgs e)
        {
            var sorted = _allMovies.OrderByDescending(m => m.ImdbRating).ToList();
            UpdateDisplayedMovies(sorted);
        }

        // Called when user taps a movie - UPDATED VERSION
        private async void OnMovieSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
            {
                // Clear the selection (so it doesn't stay highlighted)
                MoviesCollectionView.SelectedItem = null;

                // Navigate to detail page with this movie
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Movie", selectedMovie }
                };

                await Shell.Current.GoToAsync(nameof(MovieDetailPage), navigationParameter);
            }
        }
    }
}