using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TPSMobileApp.ViewModels;

public class CheckoutViewModel : BaseViewModel
{
    private Item _selectedItem;

    public ObservableCollection<Item> Items { get; }
    public Command LoadItemsCommand { get; }
    public Command<Item> ItemTapped { get; }

    public CheckoutViewModel()
    {
        Title = "Checkout";
        Items = new ObservableCollection<Item>();
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        ItemTapped = new Command<Item>(OnItemSelected);
    }

    async Task ExecuteLoadItemsCommand()
    {
        IsBusy = true;

        try
        {
            // load items if needed
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void OnAppearing()
    {
        IsBusy = true;
        SelectedItem = null;
    }

    public Item SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetProperty(ref _selectedItem, value);
            OnItemSelected(value);
        }
    }

    void OnItemSelected(Item item)
    {
        if (item == null)
            return;
    }
}
