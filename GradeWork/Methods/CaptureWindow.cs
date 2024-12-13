using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace GradeWork.Methods
{
    public class CaptureWindow
    {
        private bool _isCapturing;
        private Thread _captureThread;
        private string Ip;

        public CaptureWindow(string ip)
        {
            Ip = ip;
        }

        public void StartCapture()
        {
            if (_isCapturing)
                throw new InvalidOperationException("Screen capture is already running.");

            _isCapturing = true;
            _captureThread = new Thread(CaptureScreenLoop)
            {
                IsBackground = true
            };
            _captureThread.Start();
        }

        public void StopCapture()
        {
            _isCapturing = false;
            _captureThread?.Join();
        }

        private bool isFirstScreenDebug = false;
        
        private void CaptureScreenLoop()
        {
            try
            {
                // Создание временного файла для записи
                using (var ffmpegProcess = CreateFFmpegProcess())
                {
                    while (_isCapturing)
                    {
                        using (Bitmap bitmap = CaptureScreen())
                        {
                            bitmap.Save(ffmpegProcess.StandardInput.BaseStream, ImageFormat.Jpeg);
                            
                            if (!isFirstScreenDebug)
                            {
                                isFirstScreenDebug = true;
                                bitmap.Save("debug.png",ImageFormat.Png);
                            }
                        }

                        Thread.Sleep(10);
                    }

                    ffmpegProcess.StandardInput.Close();
                    ffmpegProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Capture error: {ex.Message}");
                StopCapture();
            }
        }
        private Process CreateFFmpegProcess()
        {
            var ffmpegStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg", // Убедитесь, что FFmpeg добавлен в PATH
                Arguments = $"-f image2pipe -i - -vf fps=60 -vcodec libx264 -f mpegts {Ip}",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            return Process.Start(ffmpegStartInfo);
        }
        
        private Bitmap CaptureScreen()
        {
            var bitmap = new Bitmap(1920, 1080);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(1920,1080));
            }

            return bitmap;
        }
    }
}