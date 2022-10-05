using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Generator_rozkazów_S.Models;

public class VMOrderS: INotifyPropertyChanged
{
    private bool _lastValidation = false;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void _notifyPropertyChanged(string propName, bool commands = false)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    private int _minor;

    public int OrderNumber
    {
        get => _minor;
        set
        {
            _minor = value;
            _notifyPropertyChanged(nameof(OrderNumber), true);
        }
    }

    private string? _trainDriverNumber;
    public string? TrainDriverNumber { 
        get => _trainDriverNumber;
        set
        {
            _trainDriverNumber = value;
            _notifyPropertyChanged(nameof(TrainDriverSlash));
            _notifyPropertyChanged(nameof(TrainDriverNumber), true);
        }
    }
    public Visibility TrainDriverSlash => TrainDriverNumber is null ? Visibility.Hidden : Visibility.Visible;

    private bool? _forTrain;
    public bool? ForTrain
    {
        get => _forTrain;
        set
        {
            _forTrain = value;
            _notifyPropertyChanged(nameof(ForTrainDecoration));
            _notifyPropertyChanged(nameof(ForShuntDecoration), true);
        }
    }
    public TextDecorationCollection? ForTrainDecoration {
        get
        {
            if (ForTrain is null) return null;
            return ForTrain.Value ? null : TextDecorations.Strikethrough;
        }
    }
    public TextDecorationCollection? ForShuntDecoration {
        get
        {
            if (ForTrain is null) return null;
            return ForTrain.Value ? TextDecorations.Strikethrough : null;
        }
    }

    public string TrainNumber
    {
        get => _trainNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Numer pociągu/manewru nie może być pusty");
            _trainNumber = value;
        }
    }

    public DateOnly Date { get; set; }

    private bool? _signalDriverOrder;
    public bool? SignalDriveOrder
    {
        get => _signalDriverOrder;
        set
        {
            _signalDriverOrder = value;
            _notifyPropertyChanged(nameof(ForSignalDriveOrderDecoration));
            _notifyPropertyChanged(nameof(ForOnlyOrderDecoration), true);
        }
    }
    public TextDecorationCollection? ForSignalDriveOrderDecoration
    {
        get
        {
            if (SignalDriveOrder is null) return null;
            return SignalDriveOrder.Value ? null : TextDecorations.Strikethrough;
        }
    }
    public TextDecorationCollection? ForOnlyOrderDecoration
    {
        get
        {
            if (SignalDriveOrder is null) return null;
            return SignalDriveOrder.Value ? TextDecorations.Strikethrough : null;
        }
    }

    private bool Plot1Border => UseSemaphoreS1Out || UseSemaphoreS1SignpostOut || UseWithoutSemaphoreOut;
    public Thickness Plot1BorderThickness => Plot1Border ? new Thickness(2) : new Thickness(0, 2, 0, 2);
    private bool UseSemaphoreS1Out => !string.IsNullOrEmpty(_semaphoreS1OutName);
    private string? _semaphoreS1OutName;
    public string? SemaphoreS1OutName { 
        get => _semaphoreS1OutName;
        set
        {
            _semaphoreS1OutName = value;
            _notifyPropertyChanged(nameof(Plot1BorderThickness));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostOutDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostOutEnable));
            _notifyPropertyChanged(nameof(WithoutSemaphoreOutDecoration));
            _notifyPropertyChanged(nameof(WithoutSemaphoreOutEnable), true);
        }
    }
    public bool SemaphoreS1OutEnable => !(UseSemaphoreS1SignpostOut || UseWithoutSemaphoreOut);
    public TextDecorationCollection? SemaphoreS1OutDecoration => UseSemaphoreS1Out || !(UseSemaphoreS1Out || UseWithoutSemaphoreOut || UseSemaphoreS1SignpostOut) ? null : TextDecorations.Strikethrough;
    private bool UseSemaphoreS1SignpostOut => !string.IsNullOrEmpty(_semaphoreS1SingpostOutName);
    private string? _semaphoreS1SingpostOutName;
    public string? SemaphoreS1SignpostOutName {
        get => _semaphoreS1SingpostOutName;
        set
        {
            _semaphoreS1SingpostOutName = value;
            _notifyPropertyChanged(nameof(Plot1BorderThickness));
            _notifyPropertyChanged(nameof(SemaphoreS1OutDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1OutEnable));
            _notifyPropertyChanged(nameof(WithoutSemaphoreOutDecoration));
            _notifyPropertyChanged(nameof(WithoutSemaphoreOutEnable), true);
        }
    }
    public bool SemaphoreS1SignpostOutEnable => !(UseSemaphoreS1Out || UseWithoutSemaphoreOut);
    public TextDecorationCollection? SemaphoreS1SignpostOutDecoration => UseSemaphoreS1SignpostOut || !(UseSemaphoreS1Out || UseWithoutSemaphoreOut || UseSemaphoreS1SignpostOut) ? null : TextDecorations.Strikethrough;
    private bool UseWithoutSemaphoreOut => !string.IsNullOrEmpty(_withoutSemaphoreInNumber);
    private string? _withoutSemaphoreOutNumber;
    public string? WithoutSemaphoreOutNumber
    {
        get => _withoutSemaphoreOutNumber;
        set
        {
            _withoutSemaphoreOutNumber = value;
            _notifyPropertyChanged(nameof(Plot1BorderThickness));
            _notifyPropertyChanged(nameof(SemaphoreS1OutDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1OutEnable));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostOutDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostOutEnable), true);
        }
    }
    public bool WithoutSemaphoreOutEnable => !(UseSemaphoreS1Out || UseSemaphoreS1SignpostOut);
    public TextDecorationCollection? WithoutSemaphoreOutDecoration => UseWithoutSemaphoreOut || !(UseSemaphoreS1Out || UseWithoutSemaphoreOut || UseSemaphoreS1SignpostOut) ? null : TextDecorations.Strikethrough;

    private bool Plot2Border =>
        UseSemaphoreS1In || UseSemaphoreS1SignpostIn || UseSemaphoreS1SpaceIn || UseWithoutSemaphoreIn;
    public Thickness Plot2BorderThickness => Plot2Border ? new Thickness(2, 0, 2, 2) : new Thickness(0, 0, 0, 2);
    public bool UseSemaphoreS1In => !string.IsNullOrEmpty(_semaphoreS1InName);
    private string? _semaphoreS1InName;
    public string? SemaphoreS1InName { 
        get => _semaphoreS1InName;
        set
        {
            _semaphoreS1InName = value;
            _notifyPropertyChanged(nameof(Plot2BorderThickness));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostInDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostInEnable));
            _notifyPropertyChanged(nameof(SemaphoreS1SpaceInDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SpaceInEnable));
            _notifyPropertyChanged(nameof(WithoutSemaphoreInDecoration));
            _notifyPropertyChanged(nameof(WithoutSemaphoreInEnable), true);
        }
    }
    public bool SemaphoreS1InEnable => !(UseSemaphoreS1SignpostIn || UseSemaphoreS1SpaceIn || UseWithoutSemaphoreIn);
    public TextDecorationCollection? SemaphoreS1InDecoration => UseSemaphoreS1In || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    public bool UseSemaphoreS1SignpostIn => !string.IsNullOrEmpty(_semaphoreS1SignpostInName);
    private string? _semaphoreS1SignpostInName;
    public string? SemaphoreS1SignpostInName { 
        get => _semaphoreS1SignpostInName;
        set
        {
            _semaphoreS1SignpostInName = value;
            _notifyPropertyChanged(nameof(Plot2BorderThickness));
            _notifyPropertyChanged(nameof(SemaphoreS1InDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1InEnable));
            _notifyPropertyChanged(nameof(SemaphoreS1SpaceInDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SpaceInEnable));
            _notifyPropertyChanged(nameof(WithoutSemaphoreInDecoration));
            _notifyPropertyChanged(nameof(WithoutSemaphoreInEnable), true);
        }
    }
    public bool SemaphoreS1SignpostInEnable => !(UseSemaphoreS1In || UseSemaphoreS1SpaceIn || UseWithoutSemaphoreIn);
    public TextDecorationCollection? SemaphoreS1SignpostInDecoration => UseSemaphoreS1SignpostIn || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    public bool UseSemaphoreS1SpaceIn => !string.IsNullOrEmpty(_semaphoreS1SpaceInName);
    private string? _semaphoreS1SpaceInName;
    public string? SemaphoreS1SpaceInName { 
        get => _semaphoreS1SpaceInName;
        set
        {
            _semaphoreS1SpaceInName = value;
            _notifyPropertyChanged(nameof(Plot2BorderThickness));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostInDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostInEnable));
            _notifyPropertyChanged(nameof(SemaphoreS1InDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1InEnable));
            _notifyPropertyChanged(nameof(WithoutSemaphoreInDecoration));
            _notifyPropertyChanged(nameof(WithoutSemaphoreInEnable), true);
        }
    }
    public bool SemaphoreS1SpaceInEnable => !(UseSemaphoreS1SignpostIn || UseSemaphoreS1In || UseWithoutSemaphoreIn);
    public TextDecorationCollection? SemaphoreS1SpaceInDecoration => UseSemaphoreS1SpaceIn || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    public bool UseWithoutSemaphoreIn => !string.IsNullOrEmpty(_withoutSemaphoreInNumber);
    private string? _withoutSemaphoreInNumber;
    public string? WithoutSemaphoreInNumber { 
        get => _withoutSemaphoreInNumber;
        set
        {
            _withoutSemaphoreInNumber = value;
            _notifyPropertyChanged(nameof(Plot2BorderThickness));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostInDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SignpostInEnable));
            _notifyPropertyChanged(nameof(SemaphoreS1SpaceInDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1SpaceInEnable));
            _notifyPropertyChanged(nameof(SemaphoreS1InDecoration));
            _notifyPropertyChanged(nameof(SemaphoreS1InEnable), true);
        }
    }
    public bool WithoutSemaphoreInEnable => !(UseSemaphoreS1SignpostIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1In);
    public TextDecorationCollection? WithoutSemaphoreInDecoration => UseWithoutSemaphoreIn || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    
    private bool Plot3Border => !(string.IsNullOrEmpty(From) &&
                                 string.IsNullOrEmpty(To) &&
                                 string.IsNullOrEmpty(TrackNr) &&
                                 string.IsNullOrEmpty(LastTrainNr) &&
                                 string.IsNullOrEmpty(LastTrainDestination) &&
                                 string.IsNullOrEmpty(LastTrainTime));
    public Thickness Plot3BorderThickness => Plot3Border ? new Thickness(2, 0, 2, 2) : new Thickness(0, 0, 0, 2);
    private string? _from;
    public string? From
    {
        get => _from;
        set
        {
            _from = value;
            _notifyPropertyChanged(nameof(Plot3BorderThickness), true);
        }
    }
    private string? _to;
    public string? To
    {
        get => _to;
        set
        {
            _to = value;
            _notifyPropertyChanged(nameof(Plot3BorderThickness), true);
        }
    }
    private string? _trackNr;
    public string? TrackNr
    {
        get => _trackNr;
        set
        {
            _trackNr = value;
            _notifyPropertyChanged(nameof(Plot3BorderThickness), true);
        }
    }
    private string? _lastTrainNr;
    public string? LastTrainNr
    {
        get => _lastTrainNr;
        set
        {
            _lastTrainNr = value;
            _notifyPropertyChanged(nameof(Plot3BorderThickness), true);
        }
    }
    private string? _lastTrainDestination;
    public string? LastTrainDestination
    {
        get => _lastTrainDestination;
        set
        {
            _lastTrainDestination = value;
            _notifyPropertyChanged(nameof(Plot3BorderThickness), true);
        }
    }
    private string? _lastTrainTime;
    public string? LastTrainTime
    {
        get => _lastTrainTime;
        set
        {
            _lastTrainTime = value;
            _notifyPropertyChanged(nameof(Plot3BorderThickness), true);
        }
    }

    private bool Plot4Border => !string.IsNullOrEmpty(Other);
    public Thickness Plot4BorderThickness => Plot4Border ? new Thickness(2, 0, 2, 2) : new Thickness(0, 0, 0, 2);
    private string? _other;
    public string? Other
    {
        get => _other;
        set
        {
            _other = value;
            _notifyPropertyChanged(nameof(Plot4BorderThickness), true);
        }
    }

    private Station _station = null!;
    public Station Station
    {
        get => _station;
        set
        {
            _station = value;
            _notifyPropertyChanged(nameof(Station), true);
        }
    }

    private string _post = null!;

    public string Post
    {
        get => _post;
        set
        {
            _post = value;
            _notifyPropertyChanged(nameof(Post), true);
        }
    }

    private int _hour;
    public int Hour
    {
        get => _hour;
        set
        {
            _hour = value;
            _notifyPropertyChanged(nameof(Hour), true);
        }
    }
    
    private int _minute;
    public int Minute
    {
        get => _minute;
        set
        {
            _minute = value;
            _notifyPropertyChanged(nameof(Minute), true);
        }
    }

    private User? _isedr;
    public User? IsedrSet
    {
        get => _isedr;
        set 
        {
            _isedr = value;
            _notifyPropertyChanged(nameof(Isedr), true);
        }
    }

    public string Isedr => _isedr?.LastName ?? string.Empty;
    private User? _fromOrder;
    public User? FromOrderSet
    {
        set
        {
            _fromOrder = value;
            _notifyPropertyChanged(nameof(FromOrder), true);
        }
    }
    public string FromOrder => _fromOrder?.LastName ?? string.Empty;
    public string? TrainManager { get; set; }
    private string _trainDriver = null!;
    public string TrainDriver { 
        get => _trainDriver;
        set
        {
            if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Nazwisko maszynisty nie może być puste");
            _trainDriver = value;
        }
    }
    private IList<Station> _stations;

    public IList<Station> Stations
    {
        get => _stations;
        set
        {
            _stations = value;
            Station = _stations[0];
            _notifyPropertyChanged(nameof(Stations));
            _notifyPropertyChanged(nameof(Station), true);
        }
    }

    public bool? YearMode { get; set; }
    public User? IsedrUser => _isedr;
    
    private bool _canceled;
    private string _trainNumber = null!;
    public Visibility CancelVisiblity => _canceled ? Visibility.Visible : Visibility.Hidden;
    public bool Canceled { 
        get => _canceled;
        set
        {
            _canceled = value;
            _notifyPropertyChanged(nameof(CancelVisiblity), true);
        }
    }

    public OrderS ToOrderS()
    {
        if (_forTrain is null) throw new NullReferenceException(nameof(_forTrain));
        if (YearMode is null) throw new NullReferenceException(nameof(YearMode));

        return new OrderS()
        {
            MajorNumber = DatabaseContext.MajorNumberCalc(YearMode.Value, Date),
            MinorNumber = _minor,
            TrainDriverNumber = _trainDriverNumber,
            ForTrain = _forTrain.Value,
            TrainNumber = TrainNumber,
            Date = Date,
            SignalDriveOrder = _signalDriverOrder,
            SemaphoreS1OutName = _semaphoreS1OutName,
            SemaphoreS1SingpostOutName = _semaphoreS1SingpostOutName,
            WithoutSemaphoreOutNumber = _withoutSemaphoreOutNumber,
            SemaphoreS1InName = _semaphoreS1InName,
            SemaphoreS1SignpostInName = _semaphoreS1SignpostInName,
            SemaphoreS1SpaceInName = _semaphoreS1SpaceInName,
            WithoutSemaphoreInNumber = _withoutSemaphoreInNumber,
            From = _from,
            To = _to,
            TrackNr = _trackNr,
            LastTrainNr = _lastTrainNr,
            LastTrainDestination = _lastTrainDestination,
            LastTrainTime = _lastTrainTime,
            Other = _other,
            Station = _station,
            OrderHour = _hour,
            OrderMinute = _minute,
            Authorized = _isedr,
            OnCommand = _fromOrder,
            TrainManager = TrainManager,
            TrainDriver = TrainDriver
        };
    }

    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(_trainDriver) &&
               _forTrain is not null &&
               !string.IsNullOrWhiteSpace(_trainNumber) &&
               _d1rule() && _d2rule() && _d3rule() && _anyRule();
    }

    private bool _anyRule()
    {
        return Plot1Border || Plot2Border || Plot3Border || Plot4Border;
    }

    private bool _d3rule()
    {
        return !(_to is null || _from is null || _trackNr is null || _lastTrainNr is null ||
                 _lastTrainDestination is null || _lastTrainTime is null) ||
               (_to is null && _from is null && _trackNr is null && _lastTrainNr is null &&
                _lastTrainDestination is null && _lastTrainTime is null);
    }

    private bool _d2rule()
    {
        return (UseSemaphoreS1In ? 1 : 0) + (UseWithoutSemaphoreIn ? 1 : 0) +
            (UseSemaphoreS1SignpostIn ? 1 : 0) + (UseSemaphoreS1SignpostIn ? 1 : 0) <= 1;
    }

    private bool _d1rule()
    {
        bool onlyOneOrZero = (UseSemaphoreS1Out ? 1 : 0) + (UseWithoutSemaphoreOut ? 1 : 0) +
            (UseSemaphoreS1SignpostOut ? 1 : 0) <= 1;
        return _signalDriverOrder is not null && onlyOneOrZero;
    }
}
