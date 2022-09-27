using System.Windows;
using Generator_rozkazów_S.Models;

namespace Generator_rozkazów_S;

public partial class Authorization : Window
{
    public Authorization()
    {
        InitializeComponent();
    }

    public Authorization(OrderS rozkazS)
    {
        throw new System.NotImplementedException();
    }

    public OrderS? Signed { get; set; }
    public bool Canceled { get; set; }
}