using System.Collections.Generic;
using GradeWork.Methods;

namespace GradeWork.Network.Messages;

public class NetMessageParser
{
    public static string GetNetMessage(string ip, HashSet<Type> types)
    {
        ip = IpWorker.GetLocalIpAddress();
        string message = $"MESSAGE IP|{ip}|{Form1.Instance.userName}|IP";
        foreach (var type in types)
        {
            switch (type)
            {
                case Type.WaitingRESPONSE:
                    message += " RESPONSE";
                    break;
                case Type.Stop:
                    message += " STOP";
                    break;
                case Type.CallIncomingForm:
                    message += " CALLINCOMINGFORM";
                    break;
                case Type.VideoAccess:
                    message += " VIDEOACCESS";
                    break;
            }
        }

        return message;
    }
}