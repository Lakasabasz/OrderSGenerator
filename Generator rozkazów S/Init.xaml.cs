using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using EncryptionCore;
using Generator_rozkazów_S.Models;
using Microsoft.EntityFrameworkCore;
using MessageBox = Generator_rozkazów_S.Extensions.MessageBox;

namespace Generator_rozkazów_S;

enum AuthenticationResult
{
    Success,
    DatabaseError,
    LoginPasswordError,
    LoggedInFromOtherHost,
    UnknownLocation,
    LoggedInFromOtherLocation,
    UserHasNoRole,
    TrafficDispatcherAlreadyLoggedIn,
    NotFoundOnlineTrafficDispatcher,
    ForcedLoginWithoutTrainDispatcher,
    AutomaticSuccess
}

public partial class Init
{
    private List<DbConnectionSet> _targetOptions;
    public string? Posterunek { get; private set; }
    public DatabaseContext? DbCtx {get; private set; }
    public User? LoggedInUser { get; private set; }
    public Setting Settings { get; set; }

    private readonly IPAddress? _currentIpAddress;

    private readonly Dictionary<AuthenticationResult, (string, bool)> _result = new()
    {
        { AuthenticationResult.Success, ("", true) },
        { AuthenticationResult.DatabaseError, ("Błąd bazy danych", false) },
        { AuthenticationResult.LoginPasswordError, ("Błędny login lub hasło", false) },
        { AuthenticationResult.LoggedInFromOtherHost, ("Zalogowano z innego komputera", false) },
        { AuthenticationResult.UnknownLocation, ("Lokalizacja logowania nieznana", false) },
        { AuthenticationResult.LoggedInFromOtherLocation, ("Zalogowano z innej lokalizacji", false) },
        { AuthenticationResult.UserHasNoRole, ("Użytkownik nie ma przypisanej roli", false) },
        { AuthenticationResult.TrafficDispatcherAlreadyLoggedIn, ("Dyżurny dysponujący już jest zalogowany", false) },
        { AuthenticationResult.NotFoundOnlineTrafficDispatcher, ("Nie można zalogować do lokalizacji bez dyżurnego dysponującego", false) },
        { AuthenticationResult.ForcedLoginWithoutTrainDispatcher, ("Zalogowano do lokalizacji bez zalogowanego dyżurnego", true) },
        { AuthenticationResult.AutomaticSuccess, ("Zalogowano automatycznie. Proszę sprawdzić czy lokalizacja jest prawidłowa", true) },
    };
    
    public Init()
    {
        InitializeComponent();
        if (!File.Exists("product.key"))
        {
            MessageBox.Critical("Nie można zainicjować programu z powodu braku klucza produktu", "Błąd inicjacji programu");
            throw new InitException();
        }

        string code = File.ReadAllText("product.key");
        byte[] bytes = code.Split(" ").Select(x => byte.Parse(x)).ToArray();
        var interfaces = (from nic in NetworkInterface.GetAllNetworkInterfaces()
            where nic.OperationalStatus == OperationalStatus.Up &&
                  (nic.GetIPProperties().GatewayAddresses
                      .Any(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
            select nic.GetPhysicalAddress());
        
        if (interfaces is null)
        {
            MessageBox.Critical(
                "Nie można zainicjować programu z powodu braku karty sieciowej do komunikacji z bazą danych",
                "Błąd inicjacji programu");
            throw new InitException();
        }

        string? decrypted = null;
        foreach (var mac in interfaces)
        {
            if (mac is null) continue;
            var key = AesTool.CreateKey(mac.GetAddressBytes());
            var iv = AesTool.CreateIv(key);
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            try
            {
                decrypted = AesTool.Decrypt(aes, bytes);
                break;
            }
            catch (CryptographicException e)
            {
                continue;
            }
        }
        if (decrypted is null)
        {
            MessageBox.Critical("Nie można zdekodować klucza produktu", "Błąd inicjalizacji");
            throw new InitException();
        }
        
        var lines = decrypted.Split("\n");

        _targetOptions = new List<DbConnectionSet>
        {
            new()
            {
                Name = "Automatyczny wybór",
                LocationId = -1
            }
        };
        foreach(var line in lines)
        {
            var parameters = line.Split(" | ");
            if (parameters[0] != "HPDLUPN")
            {
                MessageBox.Critical("Struktura klucza produktu jest wadliwa", "Błąd inicjacji programu");
                throw new InitException();
            }
            _targetOptions.Add(new DbConnectionSet()
            {
                Db = parameters[3],
                DbPass = parameters[6],
                Host = parameters[1],
                Name = parameters[7],
                DbUser = parameters[5],
                Port = Int32.Parse(parameters[2]),
                LocationId = Int32.Parse(parameters[4])
            });
        }
        
        _currentIpAddress = (
            from nic in NetworkInterface.GetAllNetworkInterfaces()
            where nic.OperationalStatus == OperationalStatus.Up && nic.GetIPProperties().GatewayAddresses.Any(x=>x.Address.AddressFamily == AddressFamily.InterNetwork)
            select nic.GetIPProperties().UnicastAddresses.FirstOrDefault(x=>x.Address.AddressFamily == AddressFamily.InterNetwork).Address)
            .FirstOrDefault();

        DbSetup.ItemsSource = _targetOptions;
        DbSetup.SelectedIndex = 0;
    }

    private AuthenticationResult Authentication(string username, string password, DbConnectionSet dbAccess, bool force = false)
    {
        DatabaseContext dbctx =
            new DatabaseContext(dbAccess.Host, dbAccess.Port.ToString(), dbAccess.Db,
                dbAccess.DbUser, dbAccess.DbPass);
        if (!dbctx.Database.CanConnect()) return AuthenticationResult.DatabaseError;

        PhysicalLocation? physicalLocation = dbctx.PhysicalLocations
            .Include(x=>x.Station)
            .FirstOrDefault(x => x.LocationId == dbAccess.LocationId);
        if (physicalLocation is null) return AuthenticationResult.UnknownLocation;
        
        User? user = dbctx.Users
            .Where(u => u.Username == username)
            .Include(x => x.Role)
            .FirstOrDefault();
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return AuthenticationResult.LoginPasswordError;
        if (user.Role is null) return AuthenticationResult.UserHasNoRole;

        if (user.LoggedInTill > DateTime.Now)
        {
            if (user.LoggedInFrom != (_currentIpAddress != null ? _currentIpAddress.ToString() : string.Empty))
                return AuthenticationResult.LoggedInFromOtherHost;
            if (user.LoggedInLocationId != physicalLocation.LocationId)
                return AuthenticationResult.LoggedInFromOtherLocation;
        }

        var dt = DateTime.Now;
        bool trainDispatcherLoggedIn =
            dbctx.Users.Include(x => x.Role)
                .Where(x => x.Role != null && x.Role.GivingOrdersIndependently)
                .FirstOrDefault(x => x.LoggedInTill > dt) is not null;

        if (!user.Role.GivingOrdersIndependently && trainDispatcherLoggedIn)
        {
            user.LoggedInFrom = _currentIpAddress is null ? string.Empty : _currentIpAddress.ToString();
            user.Location = physicalLocation;
            user.LoggedInTill = DateTime.Now.AddMinutes(15);
            dbctx.SaveChanges();
            DbCtx = dbctx;
            LoggedInUser = user;
            return AuthenticationResult.Success;
        }

        bool loggedInBefore = !(user.LoggedInTill is null || user.LoggedInTill < DateTime.Now);
        if (!loggedInBefore && user.Role.GivingOrdersIndependently && trainDispatcherLoggedIn)
            return AuthenticationResult.TrafficDispatcherAlreadyLoggedIn;
        
        if (user.LoggedInTill is null || user.LoggedInTill < DateTime.Now)
            user.LoggedInFrom = _currentIpAddress != null ? _currentIpAddress.ToString() : string.Empty;

        if (!user.Role.GivingOrdersIndependently && !trainDispatcherLoggedIn)
        {
            if (!force) return AuthenticationResult.NotFoundOnlineTrafficDispatcher;
            
            user.LoggedInFrom = _currentIpAddress is null ? string.Empty : _currentIpAddress.ToString();
            user.Location = physicalLocation;
            user.LoggedInTill = DateTime.Now.AddMinutes(15);
            dbctx.SaveChanges();
            DbCtx = dbctx;
            LoggedInUser = user;
            return AuthenticationResult.ForcedLoginWithoutTrainDispatcher;
        }
        
        if(user.Role is null) return AuthenticationResult.UserHasNoRole;
        if (user.Role.GivingOrdersIndependently)
        {
            User? other = dbctx.Users
                .Include(x => x.Role)
                .Where(x => x.LoggedInTill > DateTime.Now)
                .Where(x => x.Role != null && x.Role.GivingOrdersIndependently == true)
                .FirstOrDefault(x => x.Username != user.Username);
            if (other is not null) return AuthenticationResult.TrafficDispatcherAlreadyLoggedIn;
        }
        else
        {
            User? other = dbctx.Users
                .Include(x => x.Role)
                .Where(x => x.LoggedInTill > DateTime.Now)
                .FirstOrDefault(x => x.Role != null && x.Role.GivingOrdersIndependently == true);
            if (other is not null && !force) return AuthenticationResult.NotFoundOnlineTrafficDispatcher;
        }
        
        user.LoggedInFrom = _currentIpAddress is null ? string.Empty : _currentIpAddress.ToString();
        user.Location = physicalLocation;
        user.LoggedInTill = DateTime.Now.AddMinutes(15);
        dbctx.SaveChanges();
        DbCtx = dbctx;
        LoggedInUser = user;
        switch (loggedInBefore)
        {
            case false when !trainDispatcherLoggedIn && user.Role.GivingOrdersIndependently && !force:
            case true when trainDispatcherLoggedIn && user.Role.GivingOrdersIndependently && !force:
                return AuthenticationResult.AutomaticSuccess;
            default:
                return AuthenticationResult.Success;
        }
    }
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var dbcs = DbSetup.SelectedItem as DbConnectionSet;
        if (dbcs is null)
        {
            MessageBox.Error("Nie wybrano prawidłowej lokacji logowania", "Błąd logowania");
            return;
        }

        AuthenticationResult result;
        (string, bool) resultDescription;
        if (dbcs.LocationId == -1)
        {
            foreach (var dbAccess in _targetOptions.Where(x => x.LocationId != -1))
            {
                result = Authentication(Login.Text, Password.Password, dbAccess);
                resultDescription = _result[result];
                if (resultDescription.Item2)
                {
                    if(resultDescription.Item1 != string.Empty) MessageBox.Warning(resultDescription.Item1, "Ostrzeżenie logowania");
                    break;
                }
                MessageBox.Error(resultDescription.Item1, "Błąd logowania");
                return;
            }

            _fillMissing();
            Close();
            return;
        }

        result = Authentication(Login.Text, Password.Password, dbcs, true);
        resultDescription = _result[result];
        if (!resultDescription.Item2)
        {
            MessageBox.Error(resultDescription.Item1, "Błąd logowania");
            return;
        }

        if (resultDescription.Item1 != string.Empty)
            MessageBox.Warning(resultDescription.Item1, "Ostrzeżenie logowania");

        Close();
    }

    private void _fillMissing()
    {
        if (DbCtx is null)
        {
            MessageBox.Critical("Wewnętrzny błąd logowania 0x00", "Błąd logowania");
            throw new InitException();
        }
        Setting basicsSetting = DbCtx.Settings.First();
        Settings = basicsSetting;
        Posterunek = basicsSetting.Post;
        if (LoggedInUser is null)
        {
            MessageBox.Critical("Wewnętrzny błąd logowania 0x01", "Błąd logowania");
            throw new InitException();
        }
    }

    private void Login_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (Login.Text.Length == 0) Submit.IsEnabled = false;
        else if (Password.Password.Length != 0) Submit.IsEnabled = true;
    }

    private void Password_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (Password.Password.Length == 0) Submit.IsEnabled = false;
        else if (Login.Text.Length != 0) Submit.IsEnabled = true;
    }

    public new void Close()
    {
        if (LoggedInUser is not null)
        {
            if (LoggedInUser.Role is null)
            {
                MessageBox.Critical("Wewnętrzny błąd logowania 0x02", "Błąd logowania");
                throw new InitException();
            }
            if (LoggedInUser.Location is null)
            {
                MessageBox.Critical("Wewnętrzny błąd logowania 0x03", "Błąd logowania");
                throw new InitException();
            }
            if (LoggedInUser.Location.Station is null)
            {
                MessageBox.Critical("Wewnętrzny błąd logowania 0x04", "Błąd logowania");
                throw new InitException();
            }
        }

        base.Close();
    }
}