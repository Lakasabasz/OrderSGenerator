using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S;

public partial class FrozenRozkazS : UserControl
{
    private VMOrderS _vmodel;
    public int Number {
        set => _vmodel.OrderNumber = value;
    }

    public User? Isedr { set => _vmodel.IsedrSet = value; }
    public User? FromOrder { set => _vmodel.FromOrderSet = value; }
    public Station Station { set => _vmodel.Station = value; }
    public string Post { set => _vmodel.Post = value; }
    public IList<Station> Stations { set => _vmodel.Stations = value; }

    public FrozenRozkazS(OrderS orderS)
    {
        InitializeComponent();
        _vmodel = new VMOrderS
        {
            Date = DateOnly.FromDateTime(DateTime.Now)
        };
        DataContext = _vmodel;
    }

    public FrozenRozkazS()
    {
        throw new NotImplementedException();
    }

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
}