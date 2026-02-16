namespace TPSMobileApp.Controls;

public class MySearchHandler : SearchHandler
{
    public MySearchHandler()
    {
        FontSize = 12;
        ItemsSource = null;
        ShowsResults = false;
    }

    protected override void OnQueryChanged(string oldValue, string newValue)
    {
        ShowsResults = false;
        base.OnQueryChanged(oldValue, newValue);

        if (!string.IsNullOrEmpty(newValue))
        {
            App.g_SearchText = newValue;
        }
    }

    protected override void OnQueryConfirmed()
    {
        GoToSearchPage();
    }

    protected async void GoToSearchPage()
    {
        App.g_ScanBarcode = string.Empty;
        App.g_SearchFromPage = App.g_CurrentPage;
        await App.g_Shell.GoToItemSearch();
    }

    protected override async void OnItemSelected(object item)
    {
        base.OnItemSelected(item);

        await Task.Delay(1000);
        App.g_Shell.ShowNavBar();
    }

    //private async Task QueryItems(string oldValue, string newValue)
    //{
    //    if (Application.Current?.MainPage is Shell shell)
    //    {
    //        await shell.GoToAsync("app:///HomePage");
    //    }
    //}
}
