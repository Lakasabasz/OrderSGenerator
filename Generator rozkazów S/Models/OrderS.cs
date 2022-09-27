using System.ComponentModel.DataAnnotations.Schema;

namespace Generator_rozkazów_S.Models;

public class OrderS
{
    public int OrderSid { get; set; }
    public int MinorNumber { get; set; }
    public int MajorNumber { get; set; }
    
    [Column("Authorized"), ForeignKey("Authorized")]
    public int? AuthorizedId { get; set; }
    public User? Authorized { get; set; }
    
    [Column("OnCommand"), ForeignKey("OnCommand")]
    public int? OnCommandId { get; set; }
    public User? OnCommand { get; set; }
    public string Status { get; set; } = null!;
}