using System;
using System.Windows.Input;

namespace Generator_rozkazów_S.Commands;

public class UpdateOrderTimeCommand : ICommand
{
    private OrderSEditableView _osev;
    
    public UpdateOrderTimeCommand(OrderSEditableView order)
    {
        _osev = order;
    }
    
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _osev.Update_Time();
    }

    public event EventHandler? CanExecuteChanged;
}