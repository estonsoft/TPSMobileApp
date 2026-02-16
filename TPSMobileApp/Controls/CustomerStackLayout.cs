namespace TPSMobileApp.Controls;

public class CustomerStackLayout : StackLayout
{
    public static readonly BindableProperty CustNoProperty =
        BindableProperty.Create(
            nameof(CustNo),
            typeof(string),
            typeof(CustomerStackLayout),
            default(string));

    public string CustNo
    {
        get => (string)GetValue(CustNoProperty);
        set => SetValue(CustNoProperty, value);
    }

    public CustomerStackLayout()
    {
    }
}
