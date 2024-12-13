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
        public Listener Server;
        
        public string userName;
        public string userIp;

        private Label labelUserInfo;
        public Panel mainPanel;
        private Button buttonCallForm;

        public Form1()
        {
            Instance = this;
            Server = new Listener(IpWorker.GetLocalIpAddress(), this);
            
            InitializeComponents();

            InitUser();

            //TestFunction2();
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

            labelUserInfo = new Label
            {
                Text = $"User: {userName}, IP: {userIp}",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = 40
            };
            Controls.Add(labelUserInfo);

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            Controls.Add(mainPanel);

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
    }
}