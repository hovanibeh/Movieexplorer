using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieExplorer.Models;
using System.Collections.ObjectModel;

namespace MovieExplorer.Pages
{
    public partial class HistoryPage : ContentPage
    {
        // List of viewed movies to display
        private ObservableCollection<FavouriteMovie> _history = new ObservableCollection<FavouriteMovie>();

        public HistoryPage()
        {
            InitializeComponent();

            // Connect the list to the screen
            HistoryCollectionView.ItemsSource = _history;
        }

        // Called when page appears - reload history
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadHistoryAsync();
        }

        // Load viewing history from user profile
        private async Task LoadHistoryAsync()
        {
            var profile = await App.DataService.LoadProfileAsync();

            // Clear and reload
            _history.Clear();
            foreach (var item in profile.ViewingHistory)
            {
                _history.Add(item);
            }
        }
    }
}
