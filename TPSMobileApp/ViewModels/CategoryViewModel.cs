using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TPSMobileApp.ViewModels;

public class CategoryViewModel : BaseViewModel
{
    Category _selectedItem;

    public ObservableCollection<Category> Categories { get; }
    public Command LoadCategoriesCommand { get; }
    public Command<Category> CategoryTapped { get; }

    public CategoryViewModel()
    {
        Title = "Categories";
        Categories = new ObservableCollection<Category>();
        LoadCategoriesCommand = new Command(async () => await ExecuteLoadCategoriesCommand());
        CategoryTapped = new Command<Category>(OnCategorySelected);
    }

    async Task ExecuteLoadCategoriesCommand()
    {
        IsBusy = true;

        try
        {
            Categories.Clear();
            var items = App.g_db.GetCategories();
            foreach (var item in items)
            {
                Categories.Add(item);
            }
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

    public Category SelectedItem
    {
        get => _selectedItem;
        set
        {
            SetProperty(ref _selectedItem, value);
            OnCategorySelected(value);
        }
    }

    async void OnCategorySelected(Category category)
    {
        if (category == null)
            return;

        await Shell.Current.DisplayAlertAsync(
            "Profit Order",
            "Category Selected (tapped)",
            "Ok");
    }
}
