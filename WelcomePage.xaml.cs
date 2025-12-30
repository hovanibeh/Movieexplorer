using MovieExplorer.Models;

namespace MovieExplorer.Pages
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            string userName = NameEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(userName))
            {
                await DisplayAlert("Name Required", "Please enter your name to continue.", "OK");
                return;
            }

            var profile = await App.DataService.LoadProfileAsync();
            profile.UserName = userName;
            await App.DataService.SaveProfileAsync(profile);

            // NEW .NET 9 way: Navigate by replacing the Window's Page
            if (Application.Current?.Windows.Count > 0)
            {
                Application.Current.Windows[0].Page = new AppShell();
            }
        }
    }
}