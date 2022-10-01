using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Generator_rozkazów_S.Models;

public class VMOrderS: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void _notifyPropertyChanged(string propName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    private int _minor = 0;

    public int OrderNumber
    {
        get => _minor;
        set
        {
            _minor = value;
            _notifyPropertyChanged("OrderNumber");
        }
    }

    private string? _trainDriverNumber;
    public string? TrainDriverNumber { 
        get => _trainDriverNumber;
        set
        {
            _trainDriverNumber = value;
            _notifyPropertyChanged("TrainDriverSlash");
            _notifyPropertyChanged("TrainDriverNumber");
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
            _notifyPropertyChanged("ForTrainDecoration");
            _notifyPropertyChanged("ForShuntDecoration");
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
    public string TrainNumber { get; set; }
    public DateOnly Date { get; set; }

    private bool? _signalDriverOrder;
    public bool? SignalDriveOrder
    {
        get => _signalDriverOrder;
        set
        {
            _signalDriverOrder = value;
            _notifyPropertyChanged("ForSignalDriveOrderDecoration");
            _notifyPropertyChanged("ForOnlyOrderDecoration");
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
    private bool UseSemaphoreS1Out => _semaphoreS1OutName.Length != 0;
    private string? _semaphoreS1OutName = "";
    public string? SemaphoreS1OutName { 
        get => _semaphoreS1OutName;
        set
        {
            _semaphoreS1OutName = value;
            _notifyPropertyChanged("Plot1BorderThickness");
            _notifyPropertyChanged("SemaphoreS1SignpostOutDecoration");
            _notifyPropertyChanged("SemaphoreS1SignpostOutEnable");
            _notifyPropertyChanged("WithoutSemaphoreOutDecoration");
            _notifyPropertyChanged("WithoutSemaphoreOutEnable");
        }
    }
    public bool SemaphoreS1OutEnable => !(UseSemaphoreS1SignpostOut || UseWithoutSemaphoreOut);
    public TextDecorationCollection? SemaphoreS1OutDecoration => UseSemaphoreS1Out || !(UseSemaphoreS1Out || UseWithoutSemaphoreOut || UseSemaphoreS1SignpostOut) ? null : TextDecorations.Strikethrough;
    private bool UseSemaphoreS1SignpostOut => _semaphoreS1SingpostOutName.Length != 0;
    private string? _semaphoreS1SingpostOutName = "";
    public string? SemaphoreS1SignpostOutName {
        get => _semaphoreS1SingpostOutName;
        set
        {
            _semaphoreS1SingpostOutName = value;
            _notifyPropertyChanged("Plot1BorderThickness");
            _notifyPropertyChanged("SemaphoreS1OutDecoration");
            _notifyPropertyChanged("SemaphoreS1OutEnable");
            _notifyPropertyChanged("WithoutSemaphoreOutDecoration");
            _notifyPropertyChanged("WithoutSemaphoreOutEnable");
        }
    }
    public bool SemaphoreS1SignpostOutEnable => !(UseSemaphoreS1Out || UseWithoutSemaphoreOut);
    public TextDecorationCollection? SemaphoreS1SignpostOutDecoration => UseSemaphoreS1SignpostOut || !(UseSemaphoreS1Out || UseWithoutSemaphoreOut || UseSemaphoreS1SignpostOut) ? null : TextDecorations.Strikethrough;
    private bool UseWithoutSemaphoreOut => _withoutSemaphoreInNumber.Length != 0;
    private string? _withoutSemaphoreOutNumber = "";
    public string? WithoutSemaphoreOutNumber
    {
        get => _withoutSemaphoreOutNumber;
        set
        {
            _withoutSemaphoreOutNumber = value;
            _notifyPropertyChanged("Plot1BorderThickness");
            _notifyPropertyChanged("SemaphoreS1OutDecoration");
            _notifyPropertyChanged("SemaphoreS1OutEnable");
            _notifyPropertyChanged("SemaphoreS1SignpostOutDecoration");
            _notifyPropertyChanged("SemaphoreS1SignpostOutEnable");
        }
    }
    public bool WithoutSemaphoreOutEnable => !(UseSemaphoreS1Out || UseSemaphoreS1SignpostOut);
    public TextDecorationCollection? WithoutSemaphoreOutDecoration => UseWithoutSemaphoreOut || !(UseSemaphoreS1Out || UseWithoutSemaphoreOut || UseSemaphoreS1SignpostOut) ? null : TextDecorations.Strikethrough;

    private bool Plot2Border =>
        UseSemaphoreS1In || UseSemaphoreS1SignpostIn || UseSemaphoreS1SpaceIn || UseWithoutSemaphoreIn;
    public Thickness Plot2BorderThickness => Plot2Border ? new Thickness(2, 0, 2, 2) : new Thickness(0, 0, 0, 2);
    public bool UseSemaphoreS1In => _semaphoreS1InName.Length != 0;
    private string? _semaphoreS1InName = "";
    public string? SemaphoreS1InName { 
        get => _semaphoreS1InName;
        set
        {
            _semaphoreS1InName = value;
            _notifyPropertyChanged("Plot2BorderThickness");
            _notifyPropertyChanged("SemaphoreS1SignpostInDecoration");
            _notifyPropertyChanged("SemaphoreS1SignpostInEnable");
            _notifyPropertyChanged("SemaphoreS1SpaceInDecoration");
            _notifyPropertyChanged("SemaphoreS1SpaceInEnable");
            _notifyPropertyChanged("WithoutSemaphoreInDecoration");
            _notifyPropertyChanged("WithoutSemaphoreInEnable");
        }
    }
    public bool SemaphoreS1InEnable => !(UseSemaphoreS1SignpostIn || UseSemaphoreS1SpaceIn || UseWithoutSemaphoreIn);
    public TextDecorationCollection? SemaphoreS1InDecoration => UseSemaphoreS1In || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    public bool UseSemaphoreS1SignpostIn => _semaphoreS1SignpostInName.Length != 0;
    private string? _semaphoreS1SignpostInName = "";
    public string? SemaphoreS1SignpostInName { 
        get => _semaphoreS1SignpostInName;
        set
        {
            _semaphoreS1SignpostInName = value;
            _notifyPropertyChanged("Plot2BorderThickness");
            _notifyPropertyChanged("SemaphoreS1InDecoration");
            _notifyPropertyChanged("SemaphoreS1InEnable");
            _notifyPropertyChanged("SemaphoreS1SpaceInDecoration");
            _notifyPropertyChanged("SemaphoreS1SpaceInEnable");
            _notifyPropertyChanged("WithoutSemaphoreInDecoration");
            _notifyPropertyChanged("WithoutSemaphoreInEnable");
        }
    }
    public bool SemaphoreS1SignpostInEnable => !(UseSemaphoreS1In || UseSemaphoreS1SpaceIn || UseWithoutSemaphoreIn);
    public TextDecorationCollection? SemaphoreS1SignpostInDecoration => UseSemaphoreS1SignpostIn || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    public bool UseSemaphoreS1SpaceIn => _semaphoreS1SpaceInName.Length != 0;
    private string? _semaphoreS1SpaceInName = "";
    public string? SemaphoreS1SpaceInName { 
        get => _semaphoreS1SpaceInName;
        set
        {
            _semaphoreS1SpaceInName = value;
            _notifyPropertyChanged("Plot2BorderThickness");
            _notifyPropertyChanged("SemaphoreS1SignpostInDecoration");
            _notifyPropertyChanged("SemaphoreS1SignpostInEnable");
            _notifyPropertyChanged("SemaphoreS1InDecoration");
            _notifyPropertyChanged("SemaphoreS1InEnable");
            _notifyPropertyChanged("WithoutSemaphoreInDecoration");
            _notifyPropertyChanged("WithoutSemaphoreInEnable");
        }
    }
    public bool SemaphoreS1SpaceInEnable => !(UseSemaphoreS1SignpostIn || UseSemaphoreS1In || UseWithoutSemaphoreIn);
    public TextDecorationCollection? SemaphoreS1SpaceInDecoration => UseSemaphoreS1SpaceIn || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    public bool UseWithoutSemaphoreIn => _withoutSemaphoreInNumber.Length != 0;
    private string? _withoutSemaphoreInNumber = "";
    public string? WithoutSemaphoreInNumber { 
        get => _withoutSemaphoreInNumber;
        set
        {
            _withoutSemaphoreInNumber = value;
            _notifyPropertyChanged("Plot2BorderThickness");
            _notifyPropertyChanged("SemaphoreS1SignpostInDecoration");
            _notifyPropertyChanged("SemaphoreS1SignpostInEnable");
            _notifyPropertyChanged("SemaphoreS1SpaceInDecoration");
            _notifyPropertyChanged("SemaphoreS1SpaceInEnable");
            _notifyPropertyChanged("SemaphoreS1InDecoration");
            _notifyPropertyChanged("SemaphoreS1InEnable");
        }
    }
    public bool WithoutSemaphoreInEnable => !(UseSemaphoreS1SignpostIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1In);
    public TextDecorationCollection? WithoutSemaphoreInDecoration => UseWithoutSemaphoreIn || !(UseSemaphoreS1In || UseWithoutSemaphoreIn || UseSemaphoreS1SpaceIn || UseSemaphoreS1SignpostIn) ? null : TextDecorations.Strikethrough;
    
    private bool Plot3Border => From.Length > 0 ||
                                To.Length > 0 ||
                                TrackNr.Length > 0 ||
                                LastTrainNr.Length > 0 ||
                                LastTrainDestination.Length > 0 ||
                                LastTrainTime.Length > 0;
    public Thickness Plot3BorderThickness => Plot3Border ? new Thickness(2, 0, 2, 2) : new Thickness(0, 0, 0, 2);
    private string? _from = "";
    public string? From
    {
        get => _from;
        set
        {
            _from = value;
            _notifyPropertyChanged("Plot3BorderThickness");
        }
    }
    private string? _to = "";
    public string? To
    {
        get => _to;
        set
        {
            _to = value;
            _notifyPropertyChanged("Plot3BorderThickness");
        }
    }
    private string? _trackNr = "";
    public string? TrackNr
    {
        get => _trackNr;
        set
        {
            _trackNr = value;
            _notifyPropertyChanged("Plot3BorderThickness");
        }
    }
    private string? _lastTrainNr = "";
    public string? LastTrainNr
    {
        get => _lastTrainNr;
        set
        {
            _lastTrainNr = value;
            _notifyPropertyChanged("Plot3BorderThickness");
        }
    }
    private string? _lastTrainDestination = "";
    public string? LastTrainDestination
    {
        get => _lastTrainDestination;
        set
        {
            _lastTrainDestination = value;
            _notifyPropertyChanged("Plot3BorderThickness");
        }
    }
    private string? _lastTrainTime = "";
    public string? LastTrainTime
    {
        get => _lastTrainTime;
        set
        {
            _lastTrainTime = value;
            _notifyPropertyChanged("Plot3BorderThickness");
        }
    }

    private bool Plot4Border => Other.Length > 0;
    public Thickness Plot4BorderThickness => Plot4Border ? new Thickness(2, 0, 2, 2) : new Thickness(0, 0, 0, 2);
    private string? _other = "";
    public string? Other
    {
        get => _other;
        set
        {
            _other = value;
            _notifyPropertyChanged("Plot4BorderThickness");
        }
    }

    private Station _station = null!;
    public Station Station
    {
        get => _station;
        set
        {
            _station = value;
            _notifyPropertyChanged("Station");
        }
    }

    private string _post = null!;

    public string Post
    {
        get => _post;
        set
        {
            _post = value;
            _notifyPropertyChanged("Post");
        }
    }

    private int _hour;
    public int Hour
    {
        get => _hour;
        set
        {
            _hour = value;
            _notifyPropertyChanged("Hour");
        }
    }
    
    private int _minute;
    public int Minute
    {
        get => _minute;
        set
        {
            _minute = value;
            _notifyPropertyChanged("Minute");
        }
    }

    private User? _isedr;
    public User? IsedrSet
    {
        set 
        {
            _isedr = value;
            _notifyPropertyChanged("Isedr");
        }
    }
    public string Isedr => _isedr?.LastName ?? string.Empty;
    private User? _fromOrder;
    public User? FromOrderSet
    {
        set
        {
            _fromOrder = value;
            _notifyPropertyChanged("FromOrder");
        }
    }
    public string FromOrder => _fromOrder?.LastName ?? string.Empty;
    public string? TrainManager { get; set; }
    public string TrainDriver { get; set; }
    private IList<Station> _stations;
    public IList<Station> Stations
    {
        get => _stations;
        set
        {
            _stations = value;
            Station = _stations[0];
            _notifyPropertyChanged("Stations");
            _notifyPropertyChanged("Station");
        }
    }

    public bool? YearMode { get; set; }

    public OrderS ToOrderS()
    {
        if (_forTrain is null) throw new NullReferenceException("_forTrain");
        if (YearMode is null) throw new NullReferenceException("YearMode");

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
}
