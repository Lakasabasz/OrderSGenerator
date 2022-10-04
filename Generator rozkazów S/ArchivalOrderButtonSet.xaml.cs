using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Generator_rozkazów_S;

public partial class ArchivalOrderButtonSet : UserControl
{
    public ICommand? CancelCommand { get; }
    public ICommand? NewCommand { get; }
    public ICommand? PrintCommand { get; }
    public ICommand BeforeCommand { get; }
    public ICommand NextCommand { get; }

    public ArchivalOrderButtonSet(
        ICommand? cancelOrder,
        ICommand? newOrder,
        ICommand? printOrder,
        ICommand beforeOrder,
        ICommand nextOrder)
    {
        InitializeComponent();
        CancelCommand = cancelOrder;
        NewCommand = newOrder;
        PrintCommand = printOrder;
        BeforeCommand = beforeOrder;
        NextCommand = nextOrder;
        DataContext = this;
    }
}