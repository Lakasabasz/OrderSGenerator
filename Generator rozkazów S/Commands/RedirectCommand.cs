using System;
using System.Windows.Input;
using Generator_rozkazów_S.Extensions;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S.Commands;

public class RedirectCommand: DeployCommandBase, ICommand
{
    public RedirectCommand(IRozkazS order, Func<User> verifySession, DatabaseContext ctx, Action newEditableOrder) : base(order, verifySession, ctx, newEditableOrder)
    {
    }

    public override bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        User loggedInUser = _verifySession();
        if (_order is not OrderSEditableView view) return;
        if (view.Isedr is null) throw new NullReferenceException();
        if (view.Isedr.Userid != loggedInUser.Userid)
        {
            MessageBox.Error("Nie możesz wydać rozkazu wypisanego przez kogoś innego!", "Błąd podpisu");
            return;
        }
        
        Redirection redirection = new Redirection(_dbCtx, _order.Station, view);
        try
        {
            redirection.ShowDialog();
        }
        catch (NullReferenceException)
        {
            MessageBox.Error("Sprawdź poprawność rozkazu", "Błędnie wypełniony rozkaz");
            return;
        }
        if(!redirection.Result) MessageBox.Warning("Przerwano wysyłanie rozkazu", "Rozkaz nie przekazany");
        else _newEditableOrder();
    }

    public event EventHandler? CanExecuteChanged;
}