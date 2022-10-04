using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Generator_rozkazów_S.Commands;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _posterunek;
        private DatabaseContext _dbCtx;
        private User _loggedInUser;
        private IList<Station> _availableStations;
        public ObservableCollection<OrderS> WaitingOrders;
        private Setting _settings;
        public bool IsEnabledWaitingOrders { get; set; }
        public bool IsEnabledRedirectButton { get; set; }
        public IRozkazS? Rozkaz;

        public MainWindow()
        {
            InitializeComponent();
            WaitingOrders = new ObservableCollection<OrderS>();
            DataContext = this;
            Hide();
            if (!_login())
            {
                throw new InitException();
            }
            Show();
            _updateWaitingList();
            Rozkaz?.Update_Time();
        }

        private void _updateWaitingList()
        {
            if (_loggedInUser.Role is null)
                throw new NullReferenceException("_loggedInUser.Role");
            if (_loggedInUser.Role.GivingOrdersIndependently) return;
            var update = _dbCtx.OrdersS.Where(x => x.Status == OrderStatus.Redirected.ToString()).ToList();
            WaitingOrders.Clear();
            foreach (var orderSe in update) WaitingOrders.Add(orderSe);
        }

        private bool _login()
        {
            Init initdata = new Init();
            initdata.ShowDialog();
            if(initdata.LoggedInUser is null ||
               initdata.Posterunek is null ||
               initdata.DbCtx is null)
            {
                Close();
                return false;
            }
            _posterunek = initdata.Posterunek;
            _settings = initdata.Settings;
            _dbCtx = initdata.DbCtx;
            _loggedInUser = initdata.LoggedInUser;
            if (_loggedInUser.Role is null) return false;

            if (_loggedInUser.Role.GivingOrdersIndependently || _loggedInUser.Role.Admin)
                UserManagemant.IsEnabled = true;
            
            _availableStations = _dbCtx.Stations.ToList();
            IsEnabledWaitingOrders = !_loggedInUser.Role.GivingOrdersIndependently;
            if (_loggedInUser.Role.GivingOrdersIndependently)
            {
                _newEditableOrder();
                if (Rozkaz is null) throw new NullReferenceException();
                ButtonSet.Content = 
                    new NewOrderButtonSet(
                        new ClearOrderCommand(CleanOrder), 
                        new UpdateOrderTimeCommand((OrderSEditableView) Rozkaz),
                        new PrintCommand(Rozkaz, () =>
                        {
                            _verifySession();
                            return _loggedInUser;
                        }, _dbCtx, _newEditableOrder),
                        new RadiotelephoneCommand(Rozkaz, () =>
                        {
                            _verifySession();
                            return _loggedInUser;
                        }, _dbCtx, _newEditableOrder),
                        new RedirectCommand(Rozkaz, () =>
                        {
                            _verifySession();
                            return _loggedInUser;
                        }, _dbCtx, _newEditableOrder), 
                        new LoadBeforeOrderCommand(_dbCtx,
                            DatabaseContext.MajorNumberCalc(_settings.YearlyMode, Rozkaz.Date),
                            Rozkaz.Number,
                            () =>
                            {
                                _verifySession();
                                return _loggedInUser;
                            }, _loadFrozenOrder, _newEditableOrder, _settings.YearlyMode));
            }
            else
            {
                _loadLastFrozenOrder();
                var beforeOrderCommand = new LoadBeforeOrderCommand(
                    _dbCtx,
                    Rozkaz is not null ? DatabaseContext.MajorNumberCalc(_settings.YearlyMode, Rozkaz.Date) : 0,
                    Rozkaz?.Number ?? 0,
                    () =>
                    {
                        _verifySession();
                        return _loggedInUser;
                    }, _loadFrozenOrder, null, _settings.YearlyMode);
                var nextOrderCommand = new LoadNextOrderCommand(
                    _dbCtx,
                    Rozkaz is not null ? DatabaseContext.MajorNumberCalc(_settings.YearlyMode, Rozkaz.Date) : 0,
                    Rozkaz?.Number ?? 0,
                    () =>
                    {
                        _verifySession();
                        return _loggedInUser;
                    }, _loadFrozenOrder, null, _settings.YearlyMode);
                if (Rozkaz is null)
                    ButtonSet.Content = new ArchivalOrderButtonSet(null, null, null,
                        beforeOrderCommand, nextOrderCommand);
                else
                    ButtonSet.Content = new ArchivalOrderButtonSet(null, null,
                        new PrintCommand(
                            Rozkaz,
                            () =>
                            {
                                _verifySession();
                                return _loggedInUser;
                            }, 
                            _dbCtx,
                            null),
                        beforeOrderCommand, nextOrderCommand);
            }
            return true;
        }

        private void _loadLastFrozenOrder()
        {
            throw new NotImplementedException();
        }
        
        private void _loadFrozenOrder(FrozenRozkazS frozenRozkazS, ArchivalOrderButtonSet archivalOrderButtonSet)
        {
            RozkazUC.Content = frozenRozkazS;
            Rozkaz = frozenRozkazS;
            ButtonSet.Content = archivalOrderButtonSet;
        }

        private void _verifySession()
        {
            if (_loggedInUser.LoggedInTill is not null && !(_loggedInUser.LoggedInTill < DateTime.Now)) return;
            Extensions.MessageBox.Warning("Sesja wygasła, zaloguj się ponownie", "Sesja wygasła");
            while (_login()){}
        }
        
        private void _newEditableOrder()
        {
            if (_loggedInUser.Role is null) throw new NullReferenceException();
            User? isedr = _loggedInUser.Role.GivingOrdersIndependently ? _loggedInUser : null;
            User? fromOrder = _loggedInUser.Role.GivingOrdersIndependently ? null : _loggedInUser;
            Rozkaz = OrderSEditableView.Create(_getNextOrderNumber(), isedr, fromOrder, _availableStations,
                _posterunek, _settings.YearlyMode);
            RozkazUC.Content = Rozkaz;
            Rozkaz.Update_Time();
        }
        
        private int _getNextOrderNumber()
        {
            int majorNumber = DatabaseContext.MajorNumberCalc(_settings.YearlyMode, DateOnly.FromDateTime(DateTime.Now));
            OrderS? lastOrder = _dbCtx.OrdersS
                .Where(x => x.MajorNumber == majorNumber)
                .OrderByDescending(x => x.MinorNumber)
                .FirstOrDefault();
            if (lastOrder is null) return 1;
            return lastOrder.MinorNumber + 1;
        }

        private void CleanOrder()
        {
            _verifySession();
            if (_loggedInUser.Role is null) throw new NullReferenceException();
            _newEditableOrder();
        }
        
        
        private void UserManagement_OnClick(object sender, RoutedEventArgs e)
        {
            UserManagement um = new UserManagement(_dbCtx, _loggedInUser);
            um.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}