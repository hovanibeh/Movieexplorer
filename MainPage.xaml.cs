using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace MovieExplorer;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private MovieService _movieService;
    private UserProfile _userProfile;
    private ObservableCollection<Movie> _allMovies;
    private ObservableCollection<Movie> _filteredMovies;
    private string _searchText;
    private string _selectedGenre;
    private string _selectedSort;
    private bool _isRefreshing;
    private string _welcomeMessage;
    private double _fontSize;
    private double _smallFontSize;
```
public MainPage()
    {
        InitializeComponent();
        _movieService = new MovieService();
        _userProfile = new UserProfile();
        _allMovies = new ObservableCollection<Movie>();
        _filteredMovies = new ObservableCollection<Movie>();
        // Initialize font sizes from settings
        var settings = SettingsService.LoadSettings();
        FontSize = settings.FontSize;
        SmallFontSize = settings.FontSize - 2;
        SearchCommand = new Command(PerformSearch);
        RefreshCommand = new Command(async () => await LoadMoviesAsync());
        BindingContext = this;
        LoadUserProfile();
    }
    public ObservableCollection<Movie> FilteredMovies
    {
        get => _filteredMovies;
        set { _filteredMovies = value; OnPropertyChanged(); }
    }
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }
    public string SelectedGenre
    {
        get => _selectedGenre;
        set
        {
            _selectedGenre = value;
            OnPropertyChanged();
            PerformSearch();
        }
    }
    public string SelectedSort
    {
        get => _selectedSort;
        set
        {
            _selectedSort = value;
            OnPropertyChanged();
            PerformSearch();
        }
    }
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set { _isRefreshing = value; OnPropertyChanged(); }
    }
    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set { _welcomeMessage = value; OnPropertyChanged(); }
    }
    public double FontSize
    {
        get => _fontSize;
        set { _fontSize = value; OnPropertyChanged(); }
    }
    public double SmallFontSize
    {
        get => _smallFontSize;
        set { _smallFontSize = value; OnPropertyChanged(); }
    }
    public ObservableCollection<string> Genres { get; set; }
    public ObservableCollection<string> SortOptions { get; set; }
    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Reload settings when page appears
        var settings = SettingsService.LoadSettings();
        FontSize = settings.FontSize;
        SmallFontSize = settings.FontSize - 2;
        if (_allMovies.Count == 0)
        {
            await LoadMoviesAsync();
        }
    }
    private async void LoadUserProfile()
    {
        _userProfile = UserProfileService.LoadProfile();
        if (string.IsNullOrEmpty(_userProfile.Name))
        {
            string name = await DisplayPromptAsync("Welcome!", "Please enter your name:");
            if (!string.IsNullOrWhiteSpace(name))
            {
                _userProfile.Name = name;
                UserProfileService.SaveProfile(_userProfile);
            }
        }
        WelcomeMessage = $"Welcome, {_userProfile.Name}! 🎬";
    }
    private async Task LoadMoviesAsync()
    {
        IsRefreshing = true;
        try
        {
            var movies = await _movieService.GetMoviesAsync();
            _allMovies.Clear();
            foreach (var movie in movies)
            {
                // Check if movie is in favorites
                movie.IsFavorite = _userProfile.Favorites.Any(f => f.Title == movie.Title);
                _allMovies.Add(movie);
            }
            // Populate genres
            var genres = _allMovies
                .SelectMany(m => m.Genres)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
            genres.Insert(0, "All Genres");
            Genres = new ObservableCollection<string>(genres);
            // Sort options
            SortOptions = new ObservableCollection<string>
       {
           "Title A-Z",
           "Year (Newest)",
           "Year (Oldest)",
           "Rating (High)",
           "Rating (Low)"
       };
            SelectedGenre = "All Genres";
            SelectedSort = "Title A-Z";
            PerformSearch();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load movies: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        PerformSearch();
    }
    private void PerformSearch()
    {
        var filtered = _allMovies.AsEnumerable();
        // Filter by search text
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(m =>
                m.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        // Filter by genre
        if (!string.IsNullOrEmpty(SelectedGenre) && SelectedGenre != "All Genres")
        {
            filtered = filtered.Where(m => m.Genres.Contains(SelectedGenre));
        }
        // Sort
        filtered = SelectedSort switch
        {
            "Year (Newest)" => filtered.OrderByDescending(m => m.Year),
            "Year (Oldest)" => filtered.OrderBy(m => m.Year),
            "Rating (High)" => filtered.OrderByDescending(m => m.ImdbRating),
            "Rating (Low)" => filtered.OrderBy(m => m.ImdbRating),
            _ => filtered.OrderBy(m => m.Title)
        };
        FilteredMovies = new ObservableCollection<Movie>(filtered);
    }
    private async void OnMovieTapped(object sender, EventArgs e)
    {
        var gesture = (TapGestureRecognizer)sender;
        var movie = (Movie)gesture.CommandParameter;
        if (movie != null)
        {
            // Add to history
            _userProfile.AddToHistory(movie);
            UserProfileService.SaveProfile(_userProfile);
            await Navigation.PushAsync(new MovieDetailPage(movie, _userProfile));
        }
    }
    private void OnMovieSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Movie movie)
        {
            ((CollectionView)sender).SelectedItem = null;
        }
    }
    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var movie = (Movie)button.CommandParameter;
        if (movie != null)
        {
            movie.IsFavorite = !movie.IsFavorite;
            if (movie.IsFavorite)
            {
                _userProfile.AddToFavorites(movie);
                // Animate the button
                await button.ScaleTo(1.3, 100);
                await button.ScaleTo(1.0, 100);
            }
            else
            {
                _userProfile.RemoveFromFavorites(movie);
            }
            UserProfileService.SaveProfile(_userProfile);
            // Refresh the display
            var index = FilteredMovies.IndexOf(movie);
            if (index >= 0)
            {
                FilteredMovies[index] = movie;
            }
        }
    }
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistoryPage(_userProfile));
    }
    private async void OnFavoritesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FavoritesPage(_userProfile));
    }
    public new event PropertyChangedEventHandler PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
```
}