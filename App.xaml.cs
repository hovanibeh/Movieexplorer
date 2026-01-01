using MovieExplorer.Services;
using MovieExplorer.Pages;


namespace MovieExplorer
{
    public partial class App : Application
    {
        // Services available throughout the app
        public static MovieService MovieService { get; private set; } = null!;
        public static DataService DataService { get; private set; } = null!;

        public App()
        {
            InitializeComponent();

            // Create the services
            MovieService = new MovieService();
            DataService = new DataService();
        }

        // This creates the main window - NEW .NET 9 way
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Simple: just show WelcomePage in a NavigationPage
            Page startPage = new NavigationPage(new WelcomePage());

            return new Window(startPage);
        }
    }
}