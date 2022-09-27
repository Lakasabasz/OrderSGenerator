using System;
using System.ComponentModel.DataAnnotations.Schema;
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
        throw new NotImplementedException();
    }

    public bool IsNext(int currentMajor, int currentMinor)
    {
        throw new NotImplementedException();
    }

    public static int MajorNumberCalc(bool yearlyMode, DateTime date)
    {
        if (yearlyMode) return date.Year;
        return date.Year * 12 + (date.Month - 1);
    }
}