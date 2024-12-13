using System;
using System.Drawing;
using System.Windows.Forms;
using GradeWork.Forms;
using GradeWork.Methods;
using GradeWork.Network;

namespace GradeWork
{
    public partial class Form1 : Form
    {
        public static Form1 Instance;
        public Connection Connection;
        public Listener Server;
        
        public string userName;
        public string userIp;

        private Label labelUserInfo;
        public Panel mainPanel;
        private Button buttonCallForm;

        public Form1()
        {
            Instance = this;
            Connection = new Connection();
            Server = new Listener(IpWorker.GetLocalIpAddress(), this);
            
            InitializeComponents();

            InitUser();

            //TestFunction2();
        }

        private void TestFunction2()
        {
            // URL или локальный путь к видео
            string movieUrl = "https://example.com/bunny.mp4"; // Замените ссылкой на фильм про зайца

            try
            {
                // Создаем экземпляр MediaWindow и запускаем воспроизведение
                var mediaWindow = new MediaWindow();
                mediaWindow.CreateAndPlay(movieUrl, mainPanel);

                MessageBox.Show("Now playing: Bunny Movie!", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitUser()
        {
            using var startForm = new StartForm(this);
            if (startForm.ShowDialog() == DialogResult.OK)
            {
                userName = startForm.UserName;
                userIp = startForm.UserIp;
            }

            startForm.Close();
            labelUserInfo.Text = $"User: {userName}, IP: {userIp}";
        }

        private void InitializeComponents()
        {
            Text = "Remote Control Utility";
            Size = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // Верхний Label с информацией о пользователе
            labelUserInfo = new Label
            {
                Text = $"User: {userName}, IP: {userIp}",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = 40
            };
            Controls.Add(labelUserInfo);

            // Центральная панель для консоли или видео
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            Controls.Add(mainPanel);

            // Нижняя кнопка для вызова CallForm
            buttonCallForm = new Button
            {
                Text = "Call Another Machine",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            buttonCallForm.Click += ButtonCallForm_Click;
            Controls.Add(buttonCallForm);
        }

        private void ButtonCallForm_Click(object sender, EventArgs e)
        {
            using (var callForm = new CallForm())
            {
                if (callForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Waiting...", "Waiting...", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void TestFunction()
        {
            InitializeComponent();

            string ip = "udp://127.0.0.1:1234?pkt_size=1316";

            CaptureWindow captureWindow = new CaptureWindow(ip);
            captureWindow.StartCapture();

            ip = "udp://@127.0.0.1:1234?pkt_size=1316";

            //MediaWindow mediaWindow = new MediaWindow(this);
            //var media = mediaWindow.CreateAndPlay(ip);
        }
    }
}