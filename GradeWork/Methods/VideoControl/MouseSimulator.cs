using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class MouseSimulator
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    private const uint MOUSEEVENTF_MOVE = 0x0001;    
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002; 
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;  
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008; 
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;   
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;   

    public static void ClickMouse(int x, int y, string button)
    {
        uint absX = (uint)(x * 65535 / Screen.PrimaryScreen.Bounds.Width);
        uint absY = (uint)(y * 65535 / Screen.PrimaryScreen.Bounds.Height);

        mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, absX, absY, 0, UIntPtr.Zero);

        if (button.Equals("left", StringComparison.OrdinalIgnoreCase))
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE, absX, absY, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE, absX, absY, 0, UIntPtr.Zero);
        }
        else if (button.Equals("right", StringComparison.OrdinalIgnoreCase))
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_ABSOLUTE, absX, absY, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_RIGHTUP | MOUSEEVENTF_ABSOLUTE, absX, absY, 0, UIntPtr.Zero);
        }
    }
}
