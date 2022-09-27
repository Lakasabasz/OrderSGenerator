using System;
using System.Windows;
using System.Windows.Controls;

namespace Generator_rozkazów_S;

public partial class NewOrderButtonSet : UserControl
{
    public EventHandler<RoutedEventArgs> CleanOrder { get; }
    public EventHandler<RoutedEventArgs> UpdateTime { get; }
    public EventHandler<RoutedEventArgs> PrintOrder { get; }
    public EventHandler<RoutedEventArgs> Radiophone { get; }
    public EventHandler<RoutedEventArgs> Redirect { get; }
    public EventHandler<RoutedEventArgs> BeforeOrder { get; }

    public NewOrderButtonSet(
        EventHandler<RoutedEventArgs> cleanOrder,
        EventHandler<RoutedEventArgs> updateTime,
        EventHandler<RoutedEventArgs> printOrder,
        EventHandler<RoutedEventArgs> radiophone,
        EventHandler<RoutedEventArgs> redirect,
        EventHandler<RoutedEventArgs> beforeOrder)
    {
        InitializeComponent();
        CleanOrder = cleanOrder;
        UpdateTime = updateTime;
        PrintOrder = printOrder;
        Radiophone = radiophone;
        Redirect = redirect;
        BeforeOrder = beforeOrder;
        DataContext = this;
    }

    private void Before_OnClick(object sender, RoutedEventArgs e) => BeforeOrder(sender, e);
    private void Redirect_OnClick(object sender, RoutedEventArgs e) => Redirect(sender, e);
    private void Radiophone_OnClick(object sender, RoutedEventArgs e) => Radiophone(sender, e);
    private void Print_OnClick(object sender, RoutedEventArgs e) => PrintOrder(sender, e);
    private void UpdateTime_OnClick(object sender, RoutedEventArgs e) => UpdateTime(sender, e);
    private void CleanOrder_OnClick(object sender, RoutedEventArgs e) => CleanOrder(sender, e);
}