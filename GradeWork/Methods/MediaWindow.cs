using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GradeWork.Network;
using GradeWork.Network.Messages;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using Type = GradeWork.Network.Messages.Type;

namespace GradeWork.Methods
{
    public class MediaWindow
    {
        private LibVLC _libVlc;
        private MediaPlayer _mediaPlayer;
        private Control _videoView;
        public bool IsStillPlaying { get; set; } = true;
        public Thread MouseCaptureThread { get; set; }
        
        
        public string IpTarget { get; set; }
        
        public MediaWindow()
        {
            Core.Initialize();
        }

        public MediaWindow CreateAndPlay(string url, Control container, string ipTarget)
        {
            IpTarget = ipTarget;

            if (container.InvokeRequired)
            {
                container.Invoke(new Action(() => CreateAndPlay(url, container, ipTarget)));
                return this;
            }

            _videoView = container;

            Task.Run(CaptureMouse);

            var videoView = new VideoView
            {
                Dock = DockStyle.Fill
            };

            container.Controls.Clear();
            container.Controls.Add(videoView);

            Core.Initialize();
            _libVlc = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVlc);
            videoView.MediaPlayer = _mediaPlayer;

            var media = new Media(_libVlc, new Uri(url));
            _mediaPlayer.Play(media);

            new MouseHook().StartGlobalHook(IpTarget, _videoView);
            
            return this;
        }

        public void ChangeSpeed(float speed)
        {
            _mediaPlayer?.SetRate(speed);
        }

        public void Dispose()
        {
            _mediaPlayer?.Dispose();
            _libVlc?.Dispose();
            _videoView?.Dispose();
        }

        public void CaptureMouse()
        {
            MouseCaptureThread = new Thread(() =>
            {
                while (IsStillPlaying)
                {
                    Thread.Sleep(100);

                    if (!MouseTracker.GetCursorPos(out MouseTracker.POINT cursorPosition))
                    {
                        continue;
                    }

                    var mouseX = cursorPosition.X;
                    var mouseY = cursorPosition.Y;

                    var mediaPlayerBounds = _videoView.Bounds;
                    var mediaPlayerScreenBounds = _videoView.Parent?.RectangleToScreen(mediaPlayerBounds) ?? mediaPlayerBounds;

                    if (mouseX < mediaPlayerScreenBounds.Left || mouseX > mediaPlayerScreenBounds.Right ||
                        mouseY < mediaPlayerScreenBounds.Top || mouseY > mediaPlayerScreenBounds.Bottom) continue;
                    
                    var relativeX = mouseX - mediaPlayerScreenBounds.Left;
                    var relativeY = mouseY - mediaPlayerScreenBounds.Top;

                    var scaledX = relativeX * 1920 / mediaPlayerScreenBounds.Width;
                    var scaledY = relativeY * 1080 / mediaPlayerScreenBounds.Height;
                    
                    new Sender(IpTarget).ClientHandler(NetMessageParser.GetNetMessageWithValue(new HashSet<Type>()
                    {
                        Type.MoveMouse
                    }, $"{scaledX}|{scaledY}"));

                    Console.WriteLine($"Scaled Mouse Position: X={scaledX}, Y={scaledY}");
                }
            });

            MouseCaptureThread.Start();
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine($"DETECTED MOUSE CLICK {e.X}|{e.Y}|{e.Button}");
                
            new Sender(IpTarget).ClientHandler(NetMessageParser.GetNetMessageWithValue(new HashSet<Type>()
            {
                Type.ClickMouse
            }, $"{e.X}|{e.Y}|{e.Button}"));
        }
    }
}