using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Generator_rozkazów_S.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Generator_rozkazów_S;

public partial class OrderSEditableView : UserControl, IRozkazS
{
    public static OrderSEditableView Create(int number, User? isedr, User? fromOrder, IList<Station> stations, string post,
        bool yearlyMode)
    {
        return new OrderSEditableView(yearlyMode)
        {
            Isedr = isedr,
            FromOrder = fromOrder,
            Stations = stations,
            Number = number,
            Post = post
        };
    }
    
    private VMOrderS _vmodel;
    public int Number
    {
        get => _vmodel.OrderNumber;
        set => _vmodel.OrderNumber = value;
    }

    public User? Isedr { set => _vmodel.IsedrSet = value; }
    public User? FromOrder { set => _vmodel.FromOrderSet = value; }
    public Station Station { set => _vmodel.Station = value; }
    public string Post { set => _vmodel.Post = value; }
    public DateOnly Date => _vmodel.Date;
    public IList<Station> Stations { set => _vmodel.Stations = value; }

    public OrderSEditableView(bool yearlyMode)
    {
        InitializeComponent();
        _vmodel = new VMOrderS
        {
            Date = DateOnly.FromDateTime(DateTime.Now)
        };
        _yearlyMode = yearlyMode;
        DataContext = _vmodel;
    }

    private bool _yearlyMode;
    
    private void _updateTime()
    {
        _vmodel.Hour = DateTime.Now.Hour;
        _vmodel.Minute = DateTime.Now.Minute;
    }

    public void Update_Time()
    {
        _updateTime();
    }

    private void Pociag_click(object sender, MouseButtonEventArgs e)
    {
        _vmodel.ForTrain = true;
    }
    
    private void Manewr_click(object sender, MouseButtonEventArgs e)
    {
        _vmodel.ForTrain = false;
    }

    private void SyganlNakazJazdy_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _vmodel.SignalDriveOrder = true;
    }

    private void TylkoTegoRozkazu_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _vmodel.SignalDriveOrder = false;
    }

    public FrozenRozkazS Froze()
    {
        return new FrozenRozkazS(_vmodel, _yearlyMode);
    }
}