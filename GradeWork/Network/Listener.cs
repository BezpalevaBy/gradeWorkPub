using System;
using System.Collections.Generic;
using System.Globalization;
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
    private readonly TcpListener _tcpListener = new(IPAddress.Parse(IpWorker.GetLocalIpAddress()), 27000);

    private Form1 Instance;

    public List<Message> HistoryMessage { get; set; } = new();

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
        
        Console.WriteLine(message);

        if (CheckAboutStop()) return;
        
        CheckAboutVideoAccess();
        CheckAboutOpeningIncomingForm();
        CheckAboutResponse();
        CheckAboutTerminalAccess();
        CheckAboutTerminalMessage();
        CheckAboutTerminalResponse();
        CheckAboutMovingMouse();
        CheckAboutClickingMouse();

        bool CheckAboutStop()
        {
            return message.Contains("stop");
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
                    media.CreateAndPlay(localIp, Instance.mainPanel, ip);
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

            var sender = new Sender(ip);
            sender.ClientHandler(NetMessageParser.GetNetMessage(new HashSet<Type>()
            {
            }));
        }

        void CheckAboutTerminalAccess()
        {
            if (!message.Contains("terminalaccess")) return;
            var subMessages = message.Split('|');
            if (subMessages.Length < 3) return;
            var ip = subMessages.ElementAt(1).ToString(CultureInfo.InvariantCulture);
            var userName = subMessages.ElementAt(2);

            Terminal.CreateTerminal(ip);
        }

        void CheckAboutTerminalMessage()
        {
            Console.WriteLine($"IS THIS MESSAGE TERMINAL MESSAGE {message}");
            
            if (!message.Contains("terminalmessage")) return;
            
            Console.WriteLine($"THIS MESSAGE IS TERMINAL MESSAGE {message}");
            
            var subMessages = message.Split('|');
            if (subMessages.Length < 3) return;
            var ip = subMessages.ElementAt(1);
            var userName = subMessages.ElementAt(2);

            var valueMessage = subMessages.ElementAt(4);
            var isReturnMessage = message.Contains("terminalnotresp");

            var messageFromConsole = Terminal.ExecuteCommandInTerminal(ip, valueMessage);
            
            Console.WriteLine($"IT HAS SEND TO {ip} TO {userName} VALUE {valueMessage} ISRETURN {isReturnMessage} OUTPUTMESSAGE TO RETURN {messageFromConsole}");

            Task.Run( (() =>
            {
                new Sender(ip).ClientHandler(NetMessageParser.GetNetMessageWithValue(new HashSet<Type>()
                {
                    Type.TerminalResponse
                }, messageFromConsole));
            }));
        }
        
        void CheckAboutTerminalResponse()
        {
            if (!message.Contains("terminalresp")) return;
            var subMessages = message.Split('|');
            if (subMessages.Length < 3) return;
            var ip = subMessages.ElementAt(1);
            var userName = subMessages.ElementAt(2);

            var valueMessage = subMessages.ElementAt(4);

            Terminal.InsertInConsoleOfTerminal(ip, valueMessage);
        }
        
        void CheckAboutMovingMouse()
        {
            if (!message.Contains("movemouse")) return;
            var subMessages = message.Split('|');
            if (subMessages.Length < 3) return;
            var ip = subMessages.ElementAt(1);
            var userName = subMessages.ElementAt(2);

            var x = subMessages.ElementAt(4);
            var y = subMessages.ElementAt(5);

            if (int.TryParse(x, out var intX) && int.TryParse(y, out var intY))
            {
                MouseTracker.SetCursorPos(intX, intY);
            }
        }
        
        void CheckAboutClickingMouse()
        {
            if (!message.Contains("clickmouse")) return;
            var subMessages = message.Split('|');
            if (subMessages.Length < 3) return;
            var ip = subMessages.ElementAt(1);
            var userName = subMessages.ElementAt(2);

            var x = subMessages.ElementAt(4);
            var y = subMessages.ElementAt(5);
            var isLeft = subMessages.ElementAt(6);

            if (int.TryParse(x, out var intX) && int.TryParse(y, out var intY))
            {
                MouseSimulator.ClickMouse(intX, intY, isLeft);
            }
        }
    }
}