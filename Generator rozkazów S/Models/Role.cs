namespace Generator_rozkazów_S.Models;

public class Role
{
    public int Roleid { get; set; }
    public string Rolename { get; set; }
    public bool GivingOrdersIndependently { get; set; }
    public bool UserManagement { get; set; }
    public bool Admin { get; set; }
}