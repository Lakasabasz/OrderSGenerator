using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Generator_rozkazów_S;

public partial class NewOrderButtonSet : UserControl, IButtonSet, INotifyPropertyChanged
{
    public ICommand ClearCommand { get; }
    public ICommand UpdateTimeCommand { get; }
    public ICommand PrintCommand { get; }
    public ICommand RadiophoneSendCommand { get; }
    public ICommand RedirectCommand { get; }
    public ICommand BeforeOrderCommand { get; }

    public NewOrderButtonSet(
        ICommand clearCommand,
        ICommand updateTime,
        ICommand printOrder,
        ICommand radiophone,
        ICommand redirect,
        ICommand beforeOrder)
    {
        InitializeComponent();
        ClearCommand = clearCommand;
        UpdateTimeCommand = updateTime;
        PrintCommand = printOrder;
        RadiophoneSendCommand = radiophone;
        RedirectCommand = redirect;
        BeforeOrderCommand = beforeOrder;
        
        DataContext = this;
    }
    public void DatabaseUpdated(){ }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}