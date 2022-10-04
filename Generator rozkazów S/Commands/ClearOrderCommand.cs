using System;
using System.Windows.Input;
using Generator_rozkazów_S.Extensions;

namespace Generator_rozkazów_S.Commands;

public class ClearOrderCommand: ICommand
{
    private Action _cleanOrderAction;

    public ClearOrderCommand(Action cleanOrderAction)
    {
        _cleanOrderAction = cleanOrderAction;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        MessageBox.Info("Wyczyszczono", "Wyczyszczono");
        _cleanOrderAction();
    }

    public event EventHandler? CanExecuteChanged;
}