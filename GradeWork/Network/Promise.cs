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
        // Set the timeout using the provided seconds
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        // Start listening for the response in the background
        var responseTask = ListenForResponseAsync(token);

        // Wait for either the response or the timeout
        var completedTask = await Task.WhenAny(responseTask, Task.Delay((int)(SecondsForPromise * 1000), token));

        // If the response task completed first, return true
        if (completedTask == responseTask)
        {
            return await responseTask;
        }

        // If timeout occurs, return false
        return false;
    }

    private async Task<bool> ListenForResponseAsync(CancellationToken token)
    {
        // This method simulates listening for a response
        // Here you should implement the actual logic to check if a response is received
        // For example, you might use a shared variable or a callback to notify when a response is received

        // Simulate waiting for a response from the listener (e.g., checking messages from the listener)
        while (!token.IsCancellationRequested)
        {
            // Check if a response has been received. For now, simulate a response check
            bool receivedResponse = CheckForResponse();
            if (receivedResponse)
            {
                return true;
            }

            await Task.Delay(10); // Check every 100ms for the response
        }

        return false; // Return false if cancellation is requested (i.e., timeout reached)
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
        // Cancel the task if needed
        _cancellationTokenSource?.Cancel();
    }
}
