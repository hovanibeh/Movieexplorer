using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MovieExplorer.Models;

namespace MovieExplorer.Pages
{
    [QueryProperty(nameof(Movie), "Movie")]
    public partial class MovieDetailPage : ContentPage
    {
        private Movie? _currentMovie;
        private bool _isFavourite = false;

 
        public Movie? Movie
        {
            get => _currentMovie;
            set
            {
                _currentMovie = value;
                OnPropertyChanged();
                UpdateUI();
            }
        }

        public MovieDetailPage()
        {
            InitializeComponent();
        }

        private async void UpdateUI()
        {
            if (_currentMovie == null) return;

        
            TitleLabel.Text = _currentMovie.Title;
            YearLabel.Text = _currentMovie.Year.ToString();
            RatingLabel.Text = _currentMovie.ImdbRating.ToString();
            DirectorLabel.Text = _currentMovie.Director;
            GenresLabel.Text = _currentMovie.GenresString;
            EmojiLabel.Text = _currentMovie.Emoji;

      
            await AnimateEmoji();

          
            await AddToHistoryAsync();

           
            await CheckIfFavouriteAsync();
        }

       
        private async Task AnimateEmoji()
        {
            EmojiLabel.Scale = 0; 
            await EmojiLabel.ScaleTo(1, 500, Easing.BounceOut); 
        }

      
        private async Task AddToHistoryAsync()
        {
            if (_currentMovie == null) return;

            var profile = await App.DataService.LoadProfileAsync();

         
            var existing = profile.ViewingHistory.FirstOrDefault(h => h.Title == _currentMovie.Title);
            if (existing == null)
            {
                
                var historyItem = new FavouriteMovie
                {
                    Title = _currentMovie.Title,
                    Year = _currentMovie.Year,
                    Genres = _currentMovie.Genres,
                    Emoji = _currentMovie.Emoji,
                    Timestamp = DateTime.Now,
                    IsFavourite = false
                };

               
                profile.ViewingHistory.Insert(0, historyItem);
                await App.DataService.SaveProfileAsync(profile);
            }
        }

     
        private async Task CheckIfFavouriteAsync()
        {
            if (_currentMovie == null) return;

            var profile = await App.DataService.LoadProfileAsync();
            _isFavourite = profile.Favourites.Any(f => f.Title == _currentMovie.Title);

        
            if (_isFavourite)
            {
                FavouriteButton.Text = "Remove from Favourites 💔";
                FavouriteButton.BackgroundColor = Colors.LightCoral;
            }
            else
            {
                FavouriteButton.Text = "Add to Favourites ";
                FavouriteButton.BackgroundColor = null; 
            }
        }

        
        private async void OnAddToFavouritesClicked(object sender, EventArgs e)
        {
            if (_currentMovie == null) return;

            var profile = await App.DataService.LoadProfileAsync();

            if (_isFavourite)
            {
                
                var toRemove = profile.Favourites.FirstOrDefault(f => f.Title == _currentMovie.Title);
                if (toRemove != null)
                {
                    profile.Favourites.Remove(toRemove);
                    await App.DataService.SaveProfileAsync(profile);
                    await DisplayAlert("Removed", "Movie removed from favourites", "OK");
                    _isFavourite = false;
                }
            }
            else
            {
               
                var favourite = new FavouriteMovie
                {
                    Title = _currentMovie.Title,
                    Year = _currentMovie.Year,
                    Genres = _currentMovie.Genres,
                    Emoji = _currentMovie.Emoji,
                    Timestamp = DateTime.Now,
                    IsFavourite = true
                };

                profile.Favourites.Insert(0, favourite);
                await App.DataService.SaveProfileAsync(profile);

                
                await FavouriteButton.ScaleTo(1.2, 100);
                await FavouriteButton.ScaleTo(1, 100);

                await DisplayAlert("Added!", "Movie added to favourites ❤️", "OK");
                _isFavourite = true;
            }

            
            await CheckIfFavouriteAsync();
        }

        // Go back to movies list
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}