// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using EncryptionCore;
using SetupCardTool;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


class Program
{
    private static int _errorHandler(string error)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {error}");
        Console.ForegroundColor = old;
        try
        {
            Console.ReadKey();
        }
        catch (InvalidOperationException){ }
        return -1;
    }
    
    private static void _warningHandler(string error)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARNING: {error}");
        Console.ForegroundColor = old;
    }
    
    public static int Main()
    {
        Console.WriteLine("Uruchamianie generatora");
        var mac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
            where nic.OperationalStatus == OperationalStatus.Up &&
                  (nic.GetIPProperties().GatewayAddresses.Any(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
            select nic.GetPhysicalAddress().GetAddressBytes()).FirstOrDefault();
        if (mac is null)
        {
            return _errorHandler("Nie można załadować adresu MAC");
        }
        
        Console.WriteLine($"Generowanie klucza dla {String.Join(" ", mac)}");
        var key = AesTool.CreateKey(mac);
        var iv = AesTool.CreateIv(key);
        Console.WriteLine($"Klucz {String.Join(" ", key)}");
        
        Console.WriteLine("Podaj lokalizację pliku konfiguarcyjnego (domyślnie: config.yml):");
        var path = Console.ReadLine();
        if (path is null or "")
        {
            if (!File.Exists("config.yml"))
            {
                _warningHandler("Ścieżka do pliku konfiguracyjnego jest pusta");
                _warningHandler("Wygenerowany zostanie przykładowy plik konfiguracyjny config.yml");
                Root root = new Root
                {
                    Connections = new List<Setup>
                    {
                        new()
                        {
                            Database = "db", DbPassword = "dbPassword", DbUser = "dbUser", Host = "dbHost", Port = 3306,
                            LocationId = 1, SetupName = "Local"
                        },
                        new()
                        {
                            Database = "dbLCS", DbPassword = "dbPassword", DbUser = "dbUser", Host = "dbHost", Port = 3306,
                            LocationId = 15, SetupName = "LCS"
                        }
                    }
                };
                var ymlSerializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                var yaml = ymlSerializer.Serialize(root);
                try
                {
                    File.WriteAllText("config.tml", yaml);
                }
                catch (IOException)
                {
                    return _errorHandler("Nie można zapisać do pliku przykładowej konfiguracji");
                }
            }
            path = "config.yml";
        }

        string raw;
        try
        {
            raw = File.ReadAllText(path);
        }
        catch (IOException)
        {
            return _errorHandler("Nie można uzyskać dostępu do pliku konfiguracyjnego");
        }
        
        var yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        Root config = yamlDeserializer.Deserialize<Root>(raw);
        
        Console.WriteLine("Generowanie kodu klucza");
        List<string> productKeyLines = config.Connections.Select(setup => $"HPDLUPN | {setup.Host} | {setup.Port} | {setup.Database} | {setup.LocationId} | {setup.DbUser} | {setup.DbPassword} | {setup.SetupName}").ToList();
        string productKey = string.Join("\n", productKeyLines);
        Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        byte[] encrypted = AesTool.Encrypt(aes, productKey);
        string textToSave = string.Join(" ", encrypted);
        
        Console.WriteLine("Weryfikacja");
        Aes decryptAes = Aes.Create();
        decryptAes.Key = key;
        decryptAes.IV = AesTool.CreateIv(key);
        string verification = AesTool.Decrypt(decryptAes, encrypted);
        if (productKey != verification)
        {
            return _errorHandler("Klucz nie przeszedł weryfikacji po wygenerowaniu");
        }
        
        Console.WriteLine("Zapisywanie");
        File.WriteAllText("product.key", textToSave);
        return 0;
    }
}