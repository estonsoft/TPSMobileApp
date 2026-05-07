using TPSMobileApp.ViewModels;

namespace TPSMobileApp.Views;

public partial class InvoiceViewer : ContentPage
{
    public InvoiceViewer()
    {
        InitializeComponent();
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PdfViewModel viewModel)
        {
            await viewModel.LoadPdfFromBase64();
        }
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}