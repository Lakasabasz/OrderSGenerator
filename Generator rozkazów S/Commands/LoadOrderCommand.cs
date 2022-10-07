using System;
using System.Linq;
using System.Windows.Input;
using Generator_rozkazów_S.Models;
using Microsoft.EntityFrameworkCore;

namespace Generator_rozkazów_S.Commands;

public class LoadOrderCommand: LoadCommandBase, ICommand
{
    private OrderS _orderModel;
    
    public LoadOrderCommand(DatabaseContext dbCtx, OrderS order, Func<User> verifySession, Action<FrozenRozkazS, ArchivalOrderButtonSet> updateOrder, Action newOrderTemplate, bool yearlyMode) 
        : base(dbCtx, order.MajorNumber, order.MinorNumber, verifySession, updateOrder, newOrderTemplate, yearlyMode)
    {
        _orderModel = order;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        User loggedIn = _verifySession();
        FrozenRozkazS fbs = new FrozenRozkazS(_orderModel, _yearlyMode);
        ArchivalOrderButtonSet aobs = Aobs(fbs, loggedIn, _orderModel);
        _update(fbs, aobs);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}