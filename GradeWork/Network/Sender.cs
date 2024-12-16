using System;
using System.Net.Sockets;
using System.Text;

namespace GradeWork.Network;

public class Sender
{
    private readonly string remoteIp;
    private readonly int port = 27000;

    private bool isRunning;

    public Sender(string ip)
    {
        remoteIp = ip;
    }

    public void ClientHandler(string message)
    {
        try
        {
            using var client = new TcpClient(remoteIp, port);
            using var stream = client.GetStream();
            
            message = message.ToLower();
            Console.WriteLine(message);
            
            var data = Encoding.UTF8.GetBytes(message);

            stream.Write(data, 0, data.Length);
            
            stream.Dispose();
            client.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
    }
}