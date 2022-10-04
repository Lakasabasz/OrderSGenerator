using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Generator_rozkazów_S.Models;
using Microsoft.EntityFrameworkCore;

namespace Generator_rozkazów_S;

public class DatabaseContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<OrderS> OrdersS { get; set; }
    public DbSet<PhysicalLocation> PhysicalLocations { get; set; }
    public DbSet<Station> Stations { get; set; }

    private string _host;
    private string _port;
    private string _db;
    private string _user;
    private string _pass;
    
    public DatabaseContext(string host, string port, string database, string user, string password)
    {
        _host = host;
        _port = port;
        _db = database;
        _user = user;
        _pass = password;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString =
            $"Server={_host};Port={_port};Database={_db};Uid={_user};Pwd={_pass}";
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }
    
    public bool IsBefore(int currentMajor, int currentMinor)
    {
        return OrdersS.Any(x => x.MajorNumber < currentMajor || 
                                (x.MajorNumber == currentMajor && x.MinorNumber < currentMinor));
    }

    public bool IsNext(int currentMajor, int currentMinor)
    {
        return OrdersS.Any(x => x.MajorNumber > currentMajor || 
                                (x.MajorNumber == currentMajor && x.MinorNumber > currentMinor));
    }

    public static int MajorNumberCalc(bool yearlyMode, DateOnly date)
    {
        if (yearlyMode) return date.Year;
        return date.Year * 12 + (date.Month - 1);
    }

    public void SaveOrder(FrozenRozkazS frozen, OrderStatus status)
    {
        OrderS orderRecord = frozen.ToOrderS();
        OrderS? recorded = OrdersS.FirstOrDefault(x =>
            x.MinorNumber == orderRecord.MinorNumber && x.MajorNumber == orderRecord.MajorNumber);
        if(recorded is null)
        {
            orderRecord.Status = status.ToString();
            OrdersS.Add(orderRecord);
        }
        else
        {
            if (!recorded.CompareContent(orderRecord)) throw new IllegalStateException();
            OrderStatus inDb;
            Enum.TryParse(recorded.Status, out inDb);
            if (!inDb.PossibleUpdate(status)) throw new IllegalStateException();
            recorded.Status = status.ToString();
        }
        SaveChanges();
    }
}