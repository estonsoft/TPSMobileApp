namespace TPSMobileApp.Controls;

public class QtyEntry : Entry
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(
            nameof(ItemNo),
            typeof(int),
            typeof(QtyEntry),
            0);

    public static readonly BindableProperty AllocationQtyProperty =
        BindableProperty.Create(
            nameof(AllocationQty),
            typeof(int),
            typeof(QtyEntry),
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

    public QtyEntry()
    {
        // Optional MAUI defaults
        Keyboard = Keyboard.Numeric;
    }
}
