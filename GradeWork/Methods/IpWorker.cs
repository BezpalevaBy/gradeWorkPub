using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GradeWork.Methods;

public class IpWorker
{
    public static string GetIp()
    {
        try
        {
            using (var client = new WebClient())
            {
                string externalIp = client.DownloadString("http://checkip.dyndns.org/");
                externalIp = externalIp.Split(':')[1].Split('<')[0].Trim();
                return externalIp;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return String.Empty;
        }

        return string.Empty;
    }

    public static string GetLocalIpAddress()
    {
        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (!networkInterface.Name.Contains("Radmin")) continue;
            
            // Проверяем, является ли интерфейс активным и поддерживает IPv4
            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                // Адаптер беспроводной локальной сети Беспроводная сеть:
                var properties = networkInterface.GetIPProperties();
                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return address.Address.ToString();
                    }
                }
            }
        }

        throw new Exception("Local IP Address Not Found!");
    }

}