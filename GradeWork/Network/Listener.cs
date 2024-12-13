using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GradeWork.Forms;
using GradeWork.Methods;
using GradeWork.Network.Messages;
using Type = GradeWork.Network.Messages.Type;

namespace GradeWork.Network;

public class Listener
{
    private readonly string GlobalIp;
    private readonly int port = 27000;
    private TcpListener server;
    private Thread serverThread;
    private bool isRunning;
    private TcpListener _tcpListener = new(IPAddress.Parse(IpWorker.GetLocalIpAddress()), 27000);

    private Form1 Instance;

    public List<Message> HistoryMessage { get; set; } = new List<Message>();

    public Listener(string globalIp, Form1 form)
    {
        GlobalIp = globalIp;
        Instance = form;

        Task.Run(() => { StartAsync(); });
    }

    public async Task StartAsync()
    {
        _tcpListener.Start();
        while (true)
        {
            TcpClient client = await _tcpListener.AcceptTcpClientAsync();

            _ = HandleClient(client);
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        try
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                ProcessMessage(message);

                HistoryMessage.Add(new Message()
                {
                    NetMessage = message,
                    Time = DateTime.Now
                });

                //Console.WriteLine($"Received message: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    public void ProcessMessage(string message)
    {
        message = message.ToLower();

        if (CheckAboutStop()) return;
        CheckAboutVideoAccess();
        CheckAboutOpeningIncomingForm();
        CheckAboutResponse();

        bool CheckAboutStop()
        {
            if (!message.Contains("stop")) return false;
            return true;
        }

        void CheckAboutVideoAccess()
        {
            if (!message.Contains("videoaccess")) return;

            Task.Run(() =>
            {
                try
                {
                    var subMessages = message.Split('|');
                    if (subMessages.Length < 3) return;
                    var ip = subMessages.ElementAt(1);
                    var userName = subMessages.ElementAt(2);

                    var localIp = $"udp://@{ip}:1234?pkt_size=1316";
                    localIp = "udp://@239.255.0.1:1234?pkt_size=1316";

                    Console.WriteLine($"Playing {localIp}");

                    var media = new MediaWindow();
                    media.CreateAndPlay(localIp, Instance.mainPanel);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Listener Playing Video \n {e}");
                }
            });
        }

        void CheckAboutOpeningIncomingForm()
        {
            if (!message.Contains("callincomingform")) return;

            Task.Run(() =>
            {
                var subMessages = message.Split('|');
                if (subMessages.Length < 3) return;
                var ip = subMessages.ElementAt(1);
                var userName = subMessages.ElementAt(2);
                var form = new IncomingForm(userName, ip);
                form.InitForm();
            });
        }

        void CheckAboutResponse()
        {
            if (!message.Contains("response")) return;
            var subMessages = message.Split('|');
            if (subMessages.Length < 2) return;

            var ip = subMessages.ElementAt(1);
            
            Console.WriteLine($"GOT RESPONSE MESSAGE IP TO SEND IS {ip}");
            
            var sender = new Sender(ip);
            sender.ClientHandler(NetMessageParser.GetNetMessage(ip, new HashSet<Type>()
            {
                Type.WaitingRESPONSE
            }));
        }
    }
}