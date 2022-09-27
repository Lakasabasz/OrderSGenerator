using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Generator_rozkazów_S.Models;

[Table("Users")]
public class User
{
    public int Userid { get; set; }
    public string Username { get; set; }
    public string? LastName { get; set; }
    public Role? Role { get; set; }
    public string? Password { get; set; }
    public DateTime? LoggedInTill { get; set; }
    public string? LoggedInFrom { get; set; }
    
    [Column("LoggedInLocation"), ForeignKey("Location")]
    public int? LoggedInLocationId { get; set; }
    public PhysicalLocation? Location { get; set; }
}