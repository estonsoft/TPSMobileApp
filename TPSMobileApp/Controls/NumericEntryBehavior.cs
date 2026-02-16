namespace TPSMobileApp.Controls;

public class NumericEntryBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        entry.TextChanged += OnEntryTextChanged;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnEntryTextChanged;
        base.OnDetachingFrom(entry);
    }

    private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
    {
        if (sender is not Entry entry)
            return;

        var current = args.NewTextValue;

        if (string.IsNullOrWhiteSpace(current))
        {
            entry.Text = "0";
            return;
        }

        current = current.TrimStart('0');
        if (current.Length == 0)
            current = "0";

        if (int.TryParse(current, out var value))
        {
            entry.Text = value.ToString();
        }
        else
        {
            entry.Text = args.OldTextValue;
        }
    }
}
