using System;
using System.ComponentModel.DataAnnotations;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S.Commands;

public abstract class DeployCommandBase
{
    protected IRozkazS _order;
    protected Func<User> _verifySession;
    protected DatabaseContext _dbCtx;
    protected Action? _newEditableOrder;
    
    public DeployCommandBase(IRozkazS order, Func<User> verifySession, DatabaseContext ctx, Action? newEditableOrder)
    {
        _order = order;
        _verifySession = verifySession;
        _dbCtx = ctx;
        _newEditableOrder = newEditableOrder;
    }

    public virtual bool CanExecute(object? parameter)
    {
        return _order.Validate();
    }
}