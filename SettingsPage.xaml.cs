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
            System.Diagnostics.Debug.WriteLine("=== SettingsPage Constructor ===");
        }

        // Called when page appears - load settings
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            System.Diagnostics.Debug.WriteLine("=== SettingsPage OnAppearing ===");
            await LoadSettingsAsync();
            await LoadStatisticsAsync();
        }

        // Load current settings
        private async Task LoadSettingsAsync()
        {
            System.Diagnostics.Debug.WriteLine("Loading settings...");

            _settings = await App.DataService.LoadSettingsAsync();
            _profile = await App.DataService.LoadProfileAsync();

            // Update UI
            if (_profile != null)
            {
                UserNameLabel.Text = $"Welcome, {_profile.UserName}!";
                System.Diagnostics.Debug.WriteLine($"User: {_profile.UserName}");
            }

            if (_settings != null)
            {
                System.Diagnostics.Debug.WriteLine($"Settings loaded - Dark Mode: {_settings.IsDarkMode}");

                // Set the switch WITHOUT triggering the event
                DarkModeSwitch.Toggled -= OnDarkModeToggled;
                DarkModeSwitch.IsToggled = _settings.IsDarkMode;
                DarkModeSwitch.Toggled += OnDarkModeToggled;

                // Set font size
                if (!string.IsNullOrEmpty(_settings.FontSize))
                {
                    FontSizePicker.SelectedItem = _settings.FontSize;
                }
                else
                {
                    FontSizePicker.SelectedItem = "Medium";
                }

                // Apply the current theme
                ApplyTheme(_settings.IsDarkMode);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Settings is NULL!");
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
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine($"Dark Mode TOGGLED to: {e.Value}");

            if (_settings == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Settings is NULL!");
                return;
            }

            _settings.IsDarkMode = e.Value;
            await App.DataService.SaveSettingsAsync(_settings);

            System.Diagnostics.Debug.WriteLine("Settings saved");

            // Apply theme change
            ApplyTheme(e.Value);

            System.Diagnostics.Debug.WriteLine("========================================");
        }

        // Apply theme
        private void ApplyTheme(bool isDarkMode)
        {
            System.Diagnostics.Debug.WriteLine($"ApplyTheme called with isDarkMode={isDarkMode}");

            try
            {
                if (Application.Current == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Application.Current is NULL!");
                    return;
                }

                var oldTheme = Application.Current.UserAppTheme;

                if (isDarkMode)
                {
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    System.Diagnostics.Debug.WriteLine($"Changed theme from {oldTheme} to Dark");
                }
                else
                {
                    Application.Current.UserAppTheme = AppTheme.Light;
                    System.Diagnostics.Debug.WriteLine($"Changed theme from {oldTheme} to Light");
                }

                var newTheme = Application.Current.UserAppTheme;
                System.Diagnostics.Debug.WriteLine($"Current theme is now: {newTheme}");

                // Force UI update
                this.Background = null; // This can help refresh the page
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR applying theme: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        // Handle font size change
        private async void OnFontSizeChanged(object sender, EventArgs e)
        {
            if (_settings == null || FontSizePicker.SelectedItem == null) return;

            _settings.FontSize = FontSizePicker.SelectedItem.ToString()!;
            await App.DataService.SaveSettingsAsync(_settings);

            await DisplayAlert("Font Size", $"Font size changed to {_settings.FontSize}. Restart app to see changes.", "OK");
        }

        // Test button to manually force theme change
        private void OnTestThemeClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("TEST THEME BUTTON CLICKED");

            if (Application.Current == null)
            {
                System.Diagnostics.Debug.WriteLine("Application.Current is NULL");
                return;
            }

            var currentTheme = Application.Current.UserAppTheme;
            System.Diagnostics.Debug.WriteLine($"Current theme: {currentTheme}");

            // Toggle theme
            if (currentTheme == AppTheme.Dark)
            {
                Application.Current.UserAppTheme = AppTheme.Light;
                System.Diagnostics.Debug.WriteLine("Switched to LIGHT");
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
                System.Diagnostics.Debug.WriteLine("Switched to DARK");
            }

            System.Diagnostics.Debug.WriteLine($"New theme: {Application.Current.UserAppTheme}");
            System.Diagnostics.Debug.WriteLine("========================================");
        }
    }
}
