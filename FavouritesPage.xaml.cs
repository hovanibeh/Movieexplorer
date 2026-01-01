using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MovieExplorer.Models;
using System.Collections.ObjectModel;

namespace MovieExplorer.Pages
{
    public partial class FavouritesPage : ContentPage
    {
        // List of favourite movies to display
        private ObservableCollection<FavouriteMovie> _favourites = new ObservableCollection<FavouriteMovie>();

        public FavouritesPage()
        {
            InitializeComponent();

            // Connect the list to the screen
            FavouritesCollectionView.ItemsSource = _favourites;
        }

        // Called when page appears - reload favourites
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadFavouritesAsync();
        }

        // Load favourites from user profile
        private async Task LoadFavouritesAsync()
        {
            var profile = await App.DataService.LoadProfileAsync();

            // Clear and reload
            _favourites.Clear();
            foreach (var fav in profile.Favourites)
            {
                _favourites.Add(fav);
            }
        }
    }
}