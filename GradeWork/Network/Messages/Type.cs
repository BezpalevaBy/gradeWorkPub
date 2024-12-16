namespace GradeWork.Network.Messages;

public enum Type
{
    WaitingRESPONSE = 0,
    Stop = 1,
    CallIncomingForm = 2,
    VideoAccess = 3,
    TerminalAccess = 4,
    TerminalMessage = 5,
    TerminalNotResponse = 6,
    TerminalResponse = 7,
    MoveMouse = 8,
    ClickMouse = 9
}