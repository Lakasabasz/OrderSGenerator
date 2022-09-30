using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S;

public partial class FrozenRozkazS : UserControl, IRozkazS
{
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
    public string? Status { get; private set; }

    public FrozenRozkazS(OrderS orderS, bool yearMode)
    {
        InitializeComponent();
        _vmodel = orderS.ToVMOrderS();
        Status = orderS.Status;
        DataContext = _vmodel;
    }

    public FrozenRozkazS()
    {
        throw new NotImplementedException();
    }

    public FrozenRozkazS(VMOrderS vmOrderS, bool yearMode)
    {
        InitializeComponent();
        _vmodel = vmOrderS;
        _vmodel.YearMode = yearMode;
        DataContext = _vmodel;
    }

    public void Update_Time(){}

    public OrderS ToOrderS() => _vmodel.ToOrderS();
}