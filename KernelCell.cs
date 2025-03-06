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
    private bool _isAnchor;
    public bool IsAnchor
    {
        get => _isAnchor;
        set
        {
            _isAnchor = value;
            OnPropertyChanged(nameof(IsAnchor));
        }
    }
    public int Row { get; }
    public int Column { get; }

    public KernelCell(int row, int col, double value, bool isAnchor)
    {
        Row = row;
        Column = col;
        _value = value;
        IsAnchor = isAnchor;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
