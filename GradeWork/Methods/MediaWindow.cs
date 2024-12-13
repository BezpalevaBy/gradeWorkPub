using System;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace GradeWork.Methods
{
    public class MediaWindow
    {
        private LibVLC _libVlc;
        private MediaPlayer _mediaPlayer;
        private VideoView _videoView;
        
        public MediaWindow()
        {
            Core.Initialize();
        }

        public MediaWindow CreateAndPlay(string url, Control container)
        {
            if (container.InvokeRequired)
            {
                container.Invoke(new Action(() => CreateAndPlay(url, container)));
                return this;
            }

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
    }
}