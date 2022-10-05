using System;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Input;
using Generator_rozkazów_S.Extensions;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S.Commands;

public class PrintCommand : DeployCommandBase, ICommand
{
    public PrintCommand(IRozkazS order, Func<User> verifySession, DatabaseContext ctx, Action? newEditableOrder)
    : base(order, verifySession, ctx, newEditableOrder){ }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        OrderStatus? status = null;
        User loggedInUser = _verifySession();
        if (_order is FrozenRozkazS fRozkaz)
        {
            if (fRozkaz.Status is null) throw new IllegalStateException();
            Enum.Parse<OrderStatus>(fRozkaz.Status);
        }
        
        if (loggedInUser.Role is null) throw new NullReferenceException();
        if (!loggedInUser.Role.GivingOrdersIndependently && status is null)
        {
            MessageBox.Error("Nie masz uprawnień do samodzielnego wydania rozkazu S", "Brak uprawnień");
            return;
        }
        FrozenRozkazS frozenOrder;
        if (_order is OrderSEditableView editable)
        {
            if (editable.Isedr is null) throw new NullReferenceException();
            if (editable.Isedr.Userid != loggedInUser.Userid)
            {
                MessageBox.Error("Nie możesz wydać rozkazu wypisanego przez kogoś innego!", "Błąd podpisu");
                return;
            }
            frozenOrder = editable.Froze();
        }
        else
            frozenOrder = (FrozenRozkazS)_order;

        try
        {
            _dbCtx.SaveOrder(frozenOrder, status == OrderStatus.Rt ? OrderStatus.Rt : OrderStatus.Printed);
        }
        catch (NullReferenceException)
        {
            MessageBox.Error("Sprawdź poprawność rozkazu", "Błąd poprawności rozkazu");
            return;
        }

        if (status is null)
        {
            _newEditableOrder?.Invoke();
        }

        frozenOrder.Width = 10.5 / (2.54 / 96);
        PrintDialog printDialog = new PrintDialog();
        if (printDialog.ShowDialog() != true) return;
        printDialog.PrintTicket.PageOrientation = PageOrientation.Portrait;
        printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
        printDialog.PrintVisual(frozenOrder, "Drukuj rozkaz S");
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}