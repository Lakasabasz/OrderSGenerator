using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Generator_rozkazów_S.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Generator_rozkazów_S;

public partial class Redirection : Window, INotifyPropertyChanged
{
    private DatabaseContext _ctx;
    private OrderSEditableView _ers;
    public Redirection(DatabaseContext ctx, Station orderStation, OrderSEditableView ers)
    {
        InitializeComponent();
        var dbStation = ctx.Stations.Include(x => x.Locations)
            .First(x => x.Name == orderStation.Name);
        Locations = new ObservableCollectionListSource<PhysicalLocation>(dbStation.Locations);
        DataContext = this;
        _ctx = ctx;
        _ers = ers;
    }

    public bool Result = false;
    private PhysicalLocation? _physicalLocation;
    public PhysicalLocation? Location { 
        get => _physicalLocation;
        set
        {
            _physicalLocation = value;
            OnPropertyChanged(nameof(PersonVisiblity));
            _loadLoggedInAccounts();
            OnPropertyChanged(nameof(Persons));
        }
    }

    private void _loadLoggedInAccounts()
    {
        var userList = _ctx.Users
            .Include(x => x.Location)
            .Include(x => x.Role)
            .Where(x => x.Location != null && x.Location.LocationId == Location.LocationId)
            .Where(x => x.Role != null && !x.Role.GivingOrdersIndependently)
            .ToList();
        Persons = new ObservableCollectionListSource<User>(userList);
    }

    public ObservableCollection<PhysicalLocation> Locations { get; set; }
    public Visibility PersonVisiblity => Location is not null ? Visibility.Visible : Visibility.Hidden;
    private User? _person;
    public User? Person
    {
        get => _person;
        set
        {
            _person = value;
            OnPropertyChanged(nameof(RedirectEnabled));
        }
    }
    public ObservableCollection<User> Persons { get; set; }
    public bool RedirectEnabled => Person is not null;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Redirect_OnClick(object sender, RoutedEventArgs e)
    {
        _ers.FromOrder = _person;
        FrozenRozkazS frs = _ers.Froze();
        _ctx.SaveOrder(frs, OrderStatus.Redirected);
        Result = true;
        Close();
    }
}