namespace TPSMobileApp.Controls;

public class AddToOrderButton : Button
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(
            nameof(ItemNo),
            typeof(int),
            typeof(AddToOrderButton),
            default(int));

    public static readonly BindableProperty AllocationQtyProperty =
        BindableProperty.Create(
            nameof(AllocationQty),
            typeof(int),
            typeof(AddToOrderButton),
            default(int));

    public int ItemNo
    {
        get => (int)GetValue(ItemNoProperty);
        set => SetValue(ItemNoProperty, value);
    }

    public int AllocationQty
    {
        get => (int)GetValue(AllocationQtyProperty);
        set => SetValue(AllocationQtyProperty, value);
    }

    public AddToOrderButton()
    {
    }
}
