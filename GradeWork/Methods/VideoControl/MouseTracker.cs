using System;
using System.Runtime.InteropServices;

public class MouseTracker
{
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }
    
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);

    public void SetCursorPosition(int x, int y)
    {
        if (!SetCursorPos(x, y))
        {
            Console.WriteLine("Failed to set cursor position.");
        }
    }
}