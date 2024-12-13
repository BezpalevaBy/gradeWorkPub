using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GradeWork;
using GradeWork.Network;
using GradeWork.Network.Messages;
using Type = GradeWork.Network.Messages.Type;

public class Promise
{
    public string Ip { get; set; }
    public Sender Sender { get; set; }
    public float SecondsForPromise { get; set; }
    private CancellationTokenSource _cancellationTokenSource;
    
    public Promise(string ip, float seconds, HashSet<Type> typesForSender)
    {
        Ip = ip;
        Sender = new Sender(ip);
        SecondsForPromise = seconds;

        Sender.ClientHandler(NetMessageParser.GetNetMessage(ip, typesForSender));
    }

    public async Task<bool> IsGetAnswer()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        var responseTask = ListenForResponseAsync(token);

        var completedTask = await Task.WhenAny(responseTask, Task.Delay((int)(SecondsForPromise * 1000), token));

        if (completedTask == responseTask)
        {
            return await responseTask;
        }

        return false;
    }

    private async Task<bool> ListenForResponseAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            bool receivedResponse = CheckForResponse();
            if (receivedResponse)
            {
                return true;
            }

            await Task.Delay(10);
        }

        return false;
    }

    private bool CheckForResponse()
    {
        var lastMessage = Form1.Instance.Server.HistoryMessage.LastOrDefault();
        if (lastMessage == null) return false;
        if (lastMessage.Time.AddSeconds(3) < DateTime.Now) return false;
        return true;
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }
}
