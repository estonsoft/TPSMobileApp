namespace TPSMobileApp.Controls;

public class OrderImage : Image
{
    public static readonly BindableProperty OrderNoProperty =
        BindableProperty.Create(
            nameof(OrderNo),
            typeof(string),
            typeof(OrderLabel),
            default(string));

    public string OrderNo
    {
        get => (string)GetValue(OrderNoProperty);
        set => SetValue(OrderNoProperty, value);
    }

    public OrderImage()
    {
    }
}
