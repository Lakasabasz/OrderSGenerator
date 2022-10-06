using System;
using System.Linq;
using System.Windows.Input;
using Generator_rozkazów_S.Models;
using Microsoft.EntityFrameworkCore;

namespace Generator_rozkazów_S.Commands;

public class LoadBeforeOrderCommand: LoadCommandBase, ICommand
{
    public LoadBeforeOrderCommand(DatabaseContext dbCtx, int currentMajor, int currentMinor, Func<User> verifySession, Action<FrozenRozkazS, ArchivalOrderButtonSet> updateOrder, Action? newOrderTemplate, bool yearlyMode) : base(dbCtx, currentMajor, currentMinor, verifySession, updateOrder, newOrderTemplate, yearlyMode)
    {
    }
    
    public bool CanExecute(object? parameter)
    {
        return _dbCtx.IsBefore(_currentMajor, _currentMinor);
    }

    public void Execute(object? parameter)
    {
        User loggedIn = _verifySession();
        OrderS orderS = _dbCtx.OrdersS
            .Where(x => x.MajorNumber < _currentMajor || (x.MajorNumber == _currentMajor && x.MinorNumber < _currentMinor))
            .OrderByDescending(x => x.MajorNumber)
            .ThenByDescending(x => x.MinorNumber)
            .Include(x => x.Authorized)
            .First();
        FrozenRozkazS fbs = new FrozenRozkazS(orderS, _yearlyMode);
        ArchivalOrderButtonSet aobs = Aobs(fbs, loggedIn, orderS);
        _update(fbs, aobs);
    }
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}