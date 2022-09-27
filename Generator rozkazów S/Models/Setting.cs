using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Generator_rozkazów_S.Models;

public class Setting
{
    public int id { get; set; }
    public string Post { get; set; }
    public bool YearlyMode { get; set; }
}

public class Station
{
    public int StationId { get; set; }
    [Column("StationName")]
    public string Name { get; set; }
    public ICollection<PhysicalLocation> Locations { get; set; }
}

public class PhysicalLocation
{
    [Key]
    public int LocationId { get; set; }
    public string LocationName { get; set; }
    public Station Station { get; set; }
}