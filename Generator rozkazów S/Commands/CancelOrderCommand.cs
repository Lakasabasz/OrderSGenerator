using System;
using System.Linq;
using System.Windows.Input;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S.Commands;

public class CancelOrderCommand: ICommand
{
    private readonly Func<User> _verifySession;
    private readonly FrozenRozkazS _currentOrder;
    private readonly DatabaseContext _dbCtx;
    private readonly bool _yearlyMode;

    public CancelOrderCommand(Func<User> verifySession, FrozenRozkazS currentOrder, DatabaseContext dbCtx, bool yearlyMode)
    {
        _verifySession = verifySession;
        _currentOrder = currentOrder;
        _dbCtx = dbCtx;
        _yearlyMode = yearlyMode;
    }

    public bool CanExecute(object? parameter)
    {
        User user = _verifySession();
        if (_currentOrder.Isedr is null) throw new NullReferenceException();
        return _currentOrder.Date == DateOnly.FromDateTime(DateTime.Now) &&
               _currentOrder.Isedr.Userid == user.Userid;
    }

    public void Execute(object? parameter)
    {
        OrderS current = _dbCtx.OrdersS.First(x =>
            x.MinorNumber == _currentOrder.Number &&
            x.MajorNumber == DatabaseContext.MajorNumberCalc(_yearlyMode, _currentOrder.Date));
        current.Canceled = true;
        _dbCtx.SaveChanges();
        _currentOrder.Cancel();
    }

    public event EventHandler? CanExecuteChanged;
}