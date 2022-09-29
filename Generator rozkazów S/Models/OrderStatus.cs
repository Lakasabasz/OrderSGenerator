namespace Generator_rozkazów_S.Models;

public enum OrderStatus
{
    Rt,
    Printed,
    Redirected,
    Canceled
}

static class OrderStatusExtension
{
    public static bool PossibleUpdate(this OrderStatus current, OrderStatus next)
    {
        if (current == OrderStatus.Canceled) return false;
        if (next == OrderStatus.Canceled) return true;
        return current == OrderStatus.Redirected && next == OrderStatus.Printed;
    }
}