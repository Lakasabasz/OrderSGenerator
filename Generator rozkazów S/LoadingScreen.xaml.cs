using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace Generator_rozkazów_S;

public partial class LoadingScreen : Window, INotifyPropertyChanged
{
    private int _currentState = 0;

    private Brush[] _stages = new[]
    {
        Brushes.Black, Brushes.Gray, Brushes.Transparent, Brushes.Transparent,
        Brushes.Transparent, Brushes.Transparent, Brushes.Transparent, Brushes.Transparent
    };

    public Brush F1 => _stages[(_currentState + 0) % _stages.Length];
    public Brush F2 => _stages[(_currentState + 1) % _stages.Length];
    public Brush F3 => _stages[(_currentState + 2) % _stages.Length];
    public Brush F4 => _stages[(_currentState + 3) % _stages.Length];
    public Brush F5 => _stages[(_currentState + 4) % _stages.Length];
    public Brush F6 => _stages[(_currentState + 5) % _stages.Length];
    public Brush F7 => _stages[(_currentState + 6) % _stages.Length];
    public Brush F8 => _stages[(_currentState + 7) % _stages.Length];

    private Timer _timer;
    
    public LoadingScreen()
    {
        InitializeComponent();
        _timer = new Timer(500);
        _timer.AutoReset = true;
        _timer.Elapsed += TimerOnElapsed;
        _timer.Enabled = true;
        DataContext = this;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _currentState += 1;
        OnPropertyChanged(nameof(F1));
        OnPropertyChanged(nameof(F2));
        OnPropertyChanged(nameof(F3));
        OnPropertyChanged(nameof(F4));
        OnPropertyChanged(nameof(F5));
        OnPropertyChanged(nameof(F6));
        OnPropertyChanged(nameof(F7));
        OnPropertyChanged(nameof(F8));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}