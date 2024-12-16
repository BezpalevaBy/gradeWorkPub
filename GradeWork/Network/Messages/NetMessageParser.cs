using System.Collections.Generic;
using GradeWork.Methods;

namespace GradeWork.Network.Messages;

public class NetMessageParser
{
    public static string GetNetMessageWithValue(HashSet<Type> types, string messageToSend)
    {
        var message = GetNetMessage(types);

        message += $"VALUETOSEND|{messageToSend}|VALUETOSEND";

        return message;
    }
    public static string GetNetMessage(HashSet<Type> types)
    {
        var ip = IpWorker.GetLocalIpAddress();
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
                case Type.TerminalAccess:
                    message += "TERMINALACCESS";
                    break;
                case Type.TerminalMessage:
                    message += "TERMINALMESSAGE";
                    break;
                case Type.TerminalNotResponse:
                    message += "TERMINALNOTRESP";
                    break;
                case Type.TerminalResponse:
                    message += "TERMINALRESP";
                    break;
                case Type.MoveMouse:
                    message += "MOVEMOUSE";
                    break;
                case Type.ClickMouse:
                    message += "CLICKMOUSE";
                    break;
            }
        }

        return message;
    }
}