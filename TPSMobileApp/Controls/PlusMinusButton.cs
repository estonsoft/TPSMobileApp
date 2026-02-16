namespace TPSMobileApp.Controls;

public class PlusMinusButton : ImageButton
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(
            nameof(ItemNo),
            typeof(int),
            typeof(PlusMinusButton),
            0);

    public static readonly BindableProperty AllocationQtyProperty =
        BindableProperty.Create(
            nameof(AllocationQty),
            typeof(int),
            typeof(PlusMinusButton),
            0);

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

    public PlusMinusButton()
    {
    }
}
