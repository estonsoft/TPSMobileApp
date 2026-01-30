namespace TPSMobileApp.Controls;

public class LabelButton : ImageButton
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(
            nameof(ItemNo),
            typeof(int),
            typeof(LabelButton),
            0);

    public int ItemNo
    {
        get => (int)GetValue(ItemNoProperty);
        set => SetValue(ItemNoProperty, value);
    }

    public LabelButton()
    {
    }
}
