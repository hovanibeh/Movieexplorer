using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MovieExplorer.Models;

namespace MovieExplorer.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private AppSettings? _settings;
        private UserProfile? _profile;

        public SettingsPage()
        {
            InitializeComponent();
        }

        // Called when page appears - load settings
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadSettingsAsync();
            await LoadStatisticsAsync();
        }

        // Load current settings
        private async Task LoadSettingsAsync()
        {
            _settings = await App.DataService.LoadSettingsAsync();
            _profile = await App.DataService.LoadProfileAsync();

            // Update UI
            if (_profile != null)
            {
                UserNameLabel.Text = $"Welcome, {_profile.UserName}!";
            }

            if (_settings != null)
            {
                DarkModeSwitch.IsToggled = _settings.IsDarkMode;
                FontSizePicker.SelectedItem = _settings.FontSize;

                // Apply dark mode
                Application.Current!.UserAppTheme = _settings.IsDarkMode ? AppTheme.Dark : AppTheme.Light;
            }
        }

        // Load and display statistics
        private async Task LoadStatisticsAsync()
        {
            var profile = await App.DataService.LoadProfileAsync();

            FavouritesCountLabel.Text = $"Favourite Movies: {profile.Favourites.Count}";
            ViewedCountLabel.Text = $"Movies Viewed: {profile.ViewingHistory.Count}";
        }

        // Handle dark mode toggle
        private async void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            if (_settings == null) return;

            _settings.IsDarkMode = e.Value;
            await App.DataService.SaveSettingsAsync(_settings);

            // Apply theme change
            Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }

        // Handle font size change
        private async void OnFontSizeChanged(object sender, EventArgs e)
        {
            if (_settings == null || FontSizePicker.SelectedItem == null) return;

            _settings.FontSize = FontSizePicker.SelectedItem.ToString()!;
            await App.DataService.SaveSettingsAsync(_settings);

            await DisplayAlert("Font Size", $"Font size changed to {_settings.FontSize}. Restart app to see changes.", "OK");
        }
    }
}
