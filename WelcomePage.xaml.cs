using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MovieExplorer.Models;

namespace MovieExplorer.Pages
{
    public partial class WelcomePage : ContentPage
    {
        // Constructor - runs when page is created
        public WelcomePage()
        {
            InitializeComponent();
        }

        // This runs when user clicks "Continue" button
        private async void OnContinueClicked(object sender, EventArgs e)
        {
            // Get the text from the name entry box
            string userName = NameEntry.Text?.Trim();

            // Check if they actually entered something
            if (string.IsNullOrWhiteSpace(userName))
            {
                // Show error message
                await DisplayAlert("Name Required", "Please enter your name to continue.", "OK");
                return; // Stop here
            }

            // Load user profile (or create new one if first time)
            var profile = await App.DataService.LoadProfileAsync();

            // Save their name
            profile.UserName = userName;
            await App.DataService.SaveProfileAsync(profile);

            // Switch to the main app
            Application.Current.MainPage = new AppShell();
        }
    }
}