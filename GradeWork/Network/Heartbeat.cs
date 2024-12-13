using System;
using System.Net.NetworkInformation;
using System.Threading;

namespace GradeWork.Network
{
    public class Heartbeat
    {
        private readonly string targetIp;
        private readonly int intervalSeconds;
        private Thread heartbeatThread;
        private bool isRunning;

        public Heartbeat(string targetIp, int intervalSeconds)
        {
            this.targetIp = targetIp;
            this.intervalSeconds = intervalSeconds;
        }

        public void Start()
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Heartbeat is already running.");
            }

            isRunning = true;
            heartbeatThread = new Thread(RunHeartbeat) { IsBackground = true };
            heartbeatThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            heartbeatThread?.Join();
        }

        private void RunHeartbeat()
        {
            while (isRunning)
            {
                try
                {
                    var reply = PingTarget();
                    Console.WriteLine(reply ? $"Ping to {targetIp} succeeded." : $"Ping to {targetIp} failed.");
                    if(!reply) Stop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during ping: {ex.Message}");
                }

                Thread.Sleep(intervalSeconds * 1000);
            }
        }

        private bool PingTarget()
        {
            using var ping = new Ping();
            var reply = ping.Send(targetIp, 1000); // Таймаут 1 секунда
            return reply?.Status == IPStatus.Success;
        }
    }
}