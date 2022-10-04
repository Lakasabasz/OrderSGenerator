using System;
using System.Windows.Input;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S.Commands;

public class NewOrderCommand : ICommand
{
    private Func<User> _verifySession;
    private Action _newOrder;

    public NewOrderCommand(Func<User> verifySession, Action newOrder)
    {
        _verifySession = verifySession;
        _newOrder = newOrder;
    }

    public bool CanExecute(object? parameter)
    {
        User user = _verifySession();
        return user.Role is { GivingOrdersIndependently: true };
    }

    public void Execute(object? parameter)
    {
        _newOrder();
    }

    public event EventHandler? CanExecuteChanged;
}