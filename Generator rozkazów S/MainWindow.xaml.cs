﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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
        public IRozkazS Rozkaz;

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
            Rozkaz.Update_Time();
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
                ButtonSet.Content = new NewOrderButtonSet(CleanOrder, UpdateTime, PrintOrder, Radiophone, Redirect, BeforeOrder);
                _newEditableOrder();
            }
            else
            {
                ButtonSet.Content =
                    new ArchivalOrderButtonSet(null, null, PrintOrder, BeforeOrder, NextOrder, 
                        ()=>_dbCtx.IsNext(DatabaseContext.MajorNumberCalc(_settings.YearlyMode, Rozkaz.Date), Rozkaz.Number),
                        ()=>_dbCtx.IsBefore(DatabaseContext.MajorNumberCalc(_settings.YearlyMode, Rozkaz.Date), Rozkaz.Number));
                _loadLastFrozenOrder();
            }
            return true;
        }

        private void _loadLastFrozenOrder()
        {
            throw new NotImplementedException();
        }

        private void NextOrder(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _verifySession()
        {
            if (_loggedInUser.LoggedInTill is not null && !(_loggedInUser.LoggedInTill < DateTime.Now)) return;
            Extensions.MessageBox.Warning("Sesja wygasła, zaloguj się ponownie", "Sesja wygasła");
            while (_login()){}
        }
        
        private OrderS _saveToDatabase(IRozkazS rozkaz)
        {
            throw new NotImplementedException();
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

        private void CleanOrder(object sender, RoutedEventArgs e)
        {
            _verifySession();
            if (_loggedInUser.Role is null) throw new NullReferenceException();
            _newEditableOrder();
        }

        private void UpdateTime(object sender, RoutedEventArgs e)
        {
            Rozkaz.Update_Time();
        }

        private void PrintOrder(object sender, RoutedEventArgs e)
        {
            OrderStatus? status = null;
            if (Rozkaz is FrozenRozkazS fRozkaz)
            {
                if (fRozkaz.Status is null) throw new IllegalStateException();
                Enum.Parse<OrderStatus>(fRozkaz.Status);
            }
            
            _verifySession();
            if (_loggedInUser.Role is null) throw new NullReferenceException();
            if (!_loggedInUser.Role.GivingOrdersIndependently && status is null)
            {
                 Extensions.MessageBox.Error("Nie masz uprawnień do samodzielnego wydania rozkazu S", "Brak uprawnień");
            }

            FrozenRozkazS frozenOrder;
            if (Rozkaz is OrderSEditableView editable)
                frozenOrder = editable.Froze();
            else
                frozenOrder = (FrozenRozkazS)Rozkaz;

            try
            {
                _dbCtx.SaveOrder(frozenOrder, status == OrderStatus.Rt ? OrderStatus.Rt : OrderStatus.Printed);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Sprawdź poprawność rozkazu");
                return;
            }

            if (status is null)
            {
                _newEditableOrder();
            }

            frozenOrder.Width = 10.5 / (2.54 / 96);
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;
            printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
            printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
            printDialog.PrintVisual(frozenOrder, "Drukuj rozkaz S");
        }

        private void Radiophone(object sender, RoutedEventArgs e)
        {
            _verifySession();
            if (_loggedInUser.Role is null) throw new NullReferenceException();
            if (!_loggedInUser.Role.GivingOrdersIndependently)
            {
                MessageBox.Show("Nie masz uprawnień do samodzielnego wydania rozkazu S");
                return;
            }

            if (Rozkaz is not OrderSEditableView view) return;
            FrozenRozkazS frozen = view.Froze();

            try
            {
                _dbCtx.SaveOrder(frozen, OrderStatus.Rt);
            }
            catch (NullReferenceException)
            {
                Extensions.MessageBox.Error("Sprawdź poprawność rozkazu", "Błędnie wypełniony rozkaz");
                return;
            }
            
            MessageBox.Show("Nadano rozkaz");
            _newEditableOrder();
        }

        private void Redirect(object sender, RoutedEventArgs e)
        {
            _verifySession();
            if (Rozkaz is not OrderSEditableView) return;
            Redirection redirection = new Redirection(_dbCtx, Rozkaz.Station, (OrderSEditableView) Rozkaz);
            try
            {
                redirection.ShowDialog();
            }
            catch (NullReferenceException)
            {
                Extensions.MessageBox.Error("Sprawdź poprawność rozkazu", "Błędnie wypełniony rozkaz");
                return;
            }
            if(!redirection.Result) Extensions.MessageBox.Warning("Przerwano wysyłanie rozkazu", "Rozkaz nie przekazany");
            else _newEditableOrder();
        }

        private void BeforeOrder(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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