// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using System.Net.Sockets;

var mac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
    where nic.OperationalStatus == OperationalStatus.Up &&
          (nic.GetIPProperties().GatewayAddresses.Any(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
    select nic).ToList();
foreach (var bytes in mac)
{
    Console.WriteLine(String.Join(" ", bytes.GetPhysicalAddress().GetAddressBytes()) + " " + bytes.GetIPProperties().UnicastAddresses[1].Address);
}   