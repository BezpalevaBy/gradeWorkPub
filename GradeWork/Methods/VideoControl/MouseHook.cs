using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using GradeWork.Network;
using GradeWork.Network.Messages;

public class MouseHook
{
    private IKeyboardMouseEvents _globalHook;

    public void StartGlobalHook(string ipTarget, Control _videoView)
    {
        _globalHook = Hook.GlobalEvents();

        _globalHook.MouseDownExt += (sender, e) =>
        {
            Console.WriteLine($"Global Mouse Click Detected: {e.Button}, X={e.X}, Y={e.Y}");

            if (!MouseTracker.GetCursorPos(out MouseTracker.POINT cursorPosition))
            {
                return;
            }

            var mouseX = cursorPosition.X;
            var mouseY = cursorPosition.Y;

            var mediaPlayerBounds = _videoView.Bounds;
            var mediaPlayerScreenBounds = _videoView.Parent?.RectangleToScreen(mediaPlayerBounds) ?? mediaPlayerBounds;

            if (mouseX < mediaPlayerScreenBounds.Left || mouseX > mediaPlayerScreenBounds.Right ||
                mouseY < mediaPlayerScreenBounds.Top || mouseY > mediaPlayerScreenBounds.Bottom) return;
                    
            var relativeX = mouseX - mediaPlayerScreenBounds.Left;
            var relativeY = mouseY - mediaPlayerScreenBounds.Top;

            var scaledX = relativeX * 1920 / mediaPlayerScreenBounds.Width;
            var scaledY = relativeY * 1080 / mediaPlayerScreenBounds.Height;
            
            new Sender(ipTarget).ClientHandler(NetMessageParser.GetNetMessageWithValue(new HashSet<GradeWork.Network.Messages.Type>()
            {
                GradeWork.Network.Messages.Type.ClickMouse
            }, $"{scaledX}|{scaledY}|{e.Button}"));
        };
    }
}