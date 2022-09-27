using System;
using System.Windows;
using System.Windows.Controls;

namespace Generator_rozkazów_S;

public partial class ArchivalOrderButtonSet : UserControl
{
    public EventHandler<RoutedEventArgs>? CancelOrder { get; }
    public bool Cancelable => CancelOrder is not null;
    public EventHandler<RoutedEventArgs>? NewOrder { get; }
    public bool NewOrderEnable => NewOrder is not null;
    public EventHandler<RoutedEventArgs> PrintOrder { get; }
    public EventHandler<RoutedEventArgs> BeforeOrder { get; }
    public EventHandler<RoutedEventArgs> NextOrder { get; }
    private readonly Func<bool> _isNextOrder;
    public bool IsNextOrder => _isNextOrder();
    private readonly Func<bool> _isBeforeOrder;
    public bool IsBeforeOrder => _isBeforeOrder();

    public ArchivalOrderButtonSet(
        EventHandler<RoutedEventArgs>? cancelOrder,
        EventHandler<RoutedEventArgs>? newOrder,
        EventHandler<RoutedEventArgs> printOrder,
        EventHandler<RoutedEventArgs> beforeOrder,
        EventHandler<RoutedEventArgs> nextOrder,
        Func<bool> isNext, Func<bool> isBefore)
    {
        InitializeComponent();
        CancelOrder = cancelOrder;
        NewOrder = newOrder;
        PrintOrder = printOrder;
        BeforeOrder = beforeOrder;
        NextOrder = nextOrder;
        _isNextOrder = isNext;
        _isBeforeOrder = isBefore;
        DataContext = this;
    }

    private void Before_OnClick(object sender, RoutedEventArgs e) => BeforeOrder(sender, e);
    private void Print_OnClick(object sender, RoutedEventArgs e) => PrintOrder(sender, e);
    private void Next_OnClick(object sender, RoutedEventArgs e) => NextOrder(sender, e);
    private void Cancel_OnClick(object? sender, RoutedEventArgs e) => CancelOrder?.Invoke(sender, e);
    private void NewOrder_OnClick(object sender, RoutedEventArgs e) => NewOrder?.Invoke(sender, e);
}