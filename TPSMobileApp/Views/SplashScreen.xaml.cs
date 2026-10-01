namespace TPSMobileApp.Views;

public partial class SplashScreen : ContentPage
{
    public event EventHandler? RetryRequested;

    public SplashScreen()
    {
        InitializeComponent();
    }

    public void ShowLoading()
    {
        LoadingIndicator.IsRunning = true;
        StatusLabel.Text = "Loading app data...";
        ErrorLabel.IsVisible = false;
        RetryButton.IsVisible = false;
        RetryButton.IsEnabled = false;
    }

    public void ShowLoadError(string message)
    {
        LoadingIndicator.IsRunning = false;
        StatusLabel.Text = "App data could not be loaded.";
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
        RetryButton.IsVisible = true;
        RetryButton.IsEnabled = true;
    }

    private void RetryButton_Clicked(object sender, EventArgs e)
    {
        RetryRequested?.Invoke(this, EventArgs.Empty);
    }
}