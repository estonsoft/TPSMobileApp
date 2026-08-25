using System.ComponentModel;

namespace TPSMobileApp.Controls;

public partial class CustomAlertDialog : ContentView
{
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(
            nameof(IsLoading),
            typeof(bool),
            typeof(CustomAlertDialog),
            false);

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }


    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(CustomAlertDialog),
            string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }


    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(
            nameof(Message),
            typeof(string),
            typeof(CustomAlertDialog),
            string.Empty);

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }


    public static readonly BindableProperty ProgressPercentageProperty =
        BindableProperty.Create(
            nameof(ProgressPercentage),
            typeof(int),
            typeof(CustomAlertDialog),
            0);

    public int ProgressPercentage
    {
        get => (int)GetValue(ProgressPercentageProperty);
        set => SetValue(ProgressPercentageProperty, value);
    }


    public static readonly BindableProperty ProgressValueProperty =
        BindableProperty.Create(
            nameof(ProgressValue),
            typeof(double),
            typeof(CustomAlertDialog),
            0.0);

    public double ProgressValue
    {
        get => (double)GetValue(ProgressValueProperty);
        set => SetValue(ProgressValueProperty, value);
    }


    public static readonly BindableProperty SyncStatusProperty =
        BindableProperty.Create(
            nameof(SyncStatus),
            typeof(string),
            typeof(CustomAlertDialog),
            string.Empty);

    public string SyncStatus
    {
        get => (string)GetValue(SyncStatusProperty);
        set => SetValue(SyncStatusProperty, value);
    }


    public CustomAlertDialog()
    {
        InitializeComponent();
    }
}