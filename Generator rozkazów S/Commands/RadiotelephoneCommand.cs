using System;
using System.Windows.Input;
using Generator_rozkazów_S.Extensions;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S.Commands;

public class RadiotelephoneCommand : DeployCommandBase, ICommand
{
    protected new Action _newEditableOrder;
    public RadiotelephoneCommand(IRozkazS order, Func<User> verifySession, DatabaseContext ctx, Action newEditableOrder):
        base(order, verifySession, ctx, null)
    {
        _newEditableOrder = newEditableOrder;
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        User loggedInUser = _verifySession();
        if (loggedInUser.Role is null) throw new NullReferenceException();
        if (!loggedInUser.Role.GivingOrdersIndependently)
        {
            MessageBox.Error("Nie masz uprawnień do samodzielnego wydania rozkazu S", "Błąd uprawnień");
            return;
        }

        if (_order is not OrderSEditableView view) return;
        if (view.Isedr is null) throw new NullReferenceException();
        if (view.Isedr.Userid != loggedInUser.Userid)
        {
            MessageBox.Error("Nie możesz wydać rozkazu wypisanego przez kogoś innego!", "Błąd podpisu");
            return;
        }
        FrozenRozkazS frozen = view.Froze();

        try
        {
            _dbCtx.SaveOrder(frozen, OrderStatus.Rt);
        }
        catch (NullReferenceException)
        {
            MessageBox.Error("Sprawdź poprawność rozkazu", "Błędnie wypełniony rozkaz");
            return;
        }
            
        MessageBox.Info("Nadano rozkaz", "Nadano rozkaz");
        _newEditableOrder();
    }
    
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}