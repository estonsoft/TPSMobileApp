namespace TPSMobileApp.Controls;

public class QtyBreakLabel : Label
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(
            nameof(ItemNo),
            typeof(int),
            typeof(QtyBreakLabel),
            0);

    public static readonly BindableProperty QtyBreakProperty =
        BindableProperty.Create(
            nameof(QtyBreak),
            typeof(int),
            typeof(QtyBreakLabel),
            0);

    public int ItemNo
    {
        get => (int)GetValue(ItemNoProperty);
        set => SetValue(ItemNoProperty, value);
    }

    public int QtyBreak
    {
        get => (int)GetValue(QtyBreakProperty);
        set => SetValue(QtyBreakProperty, value);
    }

    public QtyBreakLabel()
    {
    }
}
