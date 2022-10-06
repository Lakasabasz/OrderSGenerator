using System;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S.Commands;

public class LoadCommandBase
{
    protected readonly DatabaseContext _dbCtx;
    protected readonly int _currentMajor;
    protected readonly int _currentMinor;
    protected readonly Func<User> _verifySession;
    protected readonly Action<FrozenRozkazS, ArchivalOrderButtonSet> _update;
    protected readonly bool _yearlyMode;
    protected readonly Action? _newOrderTemplate;

    public LoadCommandBase(DatabaseContext dbCtx, int currentMajor, int currentMinor, Func<User> verifySession,
        Action<FrozenRozkazS, ArchivalOrderButtonSet> updateOrder, Action newOrderTemplate, bool yearlyMode)
    {
        _dbCtx = dbCtx;
        _currentMajor = currentMajor;
        _currentMinor = currentMinor;
        _verifySession = verifySession;
        _update = updateOrder;
        _yearlyMode = yearlyMode;
        _newOrderTemplate = newOrderTemplate;
    }
    
    protected ArchivalOrderButtonSet Aobs(FrozenRozkazS fbs, User loggedIn, OrderS orderS)
    {
        return new ArchivalOrderButtonSet(
            new CancelOrderCommand(_verifySession, fbs, _dbCtx, _yearlyMode),
            new NewOrderCommand(_verifySession, _newOrderTemplate),
            new PrintCommand(fbs, _verifySession, _dbCtx, loggedIn.Role is { GivingOrdersIndependently: true } ? _newOrderTemplate : null),
            new LoadBeforeOrderCommand(_dbCtx, orderS.MajorNumber, orderS.MinorNumber, _verifySession, _update, _newOrderTemplate,
                _yearlyMode),
            new LoadNextOrderCommand(_dbCtx, orderS.MajorNumber, orderS.MinorNumber, _verifySession, _update, _newOrderTemplate,
                _yearlyMode));
    }
}