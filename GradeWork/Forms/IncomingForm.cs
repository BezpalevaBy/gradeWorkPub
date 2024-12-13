using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GradeWork.Methods;
using GradeWork.Network;
using GradeWork.Network.Messages;
using Type = GradeWork.Network.Messages.Type;

namespace GradeWork.Forms
{
    public class IncomingForm : Form
    {
        private Label labelUserInfo;
        private Button buttonConsoleAccess;
        private Button buttonVideoAccess;
        private Button buttonDeny;

        public string UserName { get; }
        public string UserIp { get; }
        public string SelectedOption { get; private set; }

        public IncomingForm(string userName, string userIp)
        {
            UserName = userName;
            UserIp = userIp;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Incoming Connection";
            Size = new System.Drawing.Size(400, 200);
            StartPosition = FormStartPosition.CenterScreen;

            labelUserInfo = new Label
            {
                Text = $"User {UserName} with IP {UserIp} wants to connect.",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            Controls.Add(labelUserInfo);

            buttonConsoleAccess = new Button
            {
                Text = "Grant Console Access",
                Location = new System.Drawing.Point(20, 60),
                AutoSize = true
            };
            buttonConsoleAccess.Click += (sender, args) =>
            {
                SelectedOption = "ConsoleAccess";
                DialogResult = DialogResult.OK;
                Close();
            };
            Controls.Add(buttonConsoleAccess);

            buttonVideoAccess = new Button
            {
                Text = "Grant Video Access",
                Location = new System.Drawing.Point(20, 100),
                AutoSize = true
            };
            buttonVideoAccess.Click += (sender, args) =>
            {
                SelectedOption = "VideoAccess";
                DialogResult = DialogResult.OK;
                Close();
            };
            Controls.Add(buttonVideoAccess);

            buttonDeny = new Button
            {
                Text = "Deny",
                Location = new System.Drawing.Point(20, 140),
                AutoSize = true
            };
            buttonDeny.Click += (sender, args) =>
            {
                SelectedOption = "Deny";
                DialogResult = DialogResult.Cancel;
                Close();
            };
            Controls.Add(buttonDeny);
        }

        public void InitForm()
        {
            void processRecord()
            {
                var localIp = $"udp://@{IpWorker.GetLocalIpAddress()}:1234?pkt_size=1316";
                localIp = "udp://@239.255.0.1:1234?pkt_size=1316";
                
                var wind = new CaptureWindow(localIp);
                wind.StartCapture();
            }

            if (ShowDialog() == DialogResult.OK)
            {
                switch (SelectedOption)
                {
                    case "ConsoleAccess":
                        MessageBox.Show("Console Access granted.");
                        // consoleaccess отправить обратно сигнал слушать две консоли
                        // Tyoe consoleaccess
                        break;
                    case "VideoAccess":
                        MessageBox.Show("Video Access granted.");
                        // videoAccess отправить обратно сигнал слушать MediaWindows
                        // Tyoe videoaccess
                        
                        processRecord();
                        
                        new Sender(UserIp).ClientHandler(NetMessageParser.GetNetMessage(UserIp, new HashSet<Type>()
                        {
                            Type.WaitingRESPONSE,
                            Type.VideoAccess
                        }));
                        break;
                    case "Deny":
                    default:
                        MessageBox.Show("Connection denied.");
                        break;
                }
            }
            else
            {
                MessageBox.Show("Connection closed without any action.");
            }
        }
    }
}