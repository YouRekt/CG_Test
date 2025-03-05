using System.ComponentModel;

public class KernelCell : INotifyPropertyChanged
{
    private double _value;
    public double Value
    {
        get => _value;
        set
        {
            if (double.TryParse(value.ToString(), out double newValue))
            {
                _value = newValue;
                OnPropertyChanged(nameof(Value));
            }
            else
            {
                _value = 0; // Default value for empty input
            }
        }
    }

    public int Row { get; }
    public int Column { get; }

    public KernelCell(int row, int col, double value)
    {
        Row = row;
        Column = col;
        _value = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
