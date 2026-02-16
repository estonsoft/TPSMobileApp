namespace TPSMobileApp.Controls;

public class CreditButton : ImageButton
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(
            nameof(ItemNo),
            typeof(int),
            typeof(CreditButton),
            default(int));

    public int ItemNo
    {
        get => (int)GetValue(ItemNoProperty);
        set => SetValue(ItemNoProperty, value);
    }

    public CreditButton()
    {
    }
}
