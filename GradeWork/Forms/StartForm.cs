using System;
using System.Net;
using System.Windows.Forms;
using GradeWork.Methods;

namespace GradeWork.Forms
{
    public class StartForm : Form
    {
        private TextBox inputUserName;
        private TextBox outputIpAddress;
        private Label labelUserName;
        private Label labelIpAddress;
        private Button submitButton;
        
        public Form1 Form { get; set; }

        public string UserName { get; private set; }
        public string UserIp { get; private set; }

        public StartForm(Form1 form)
        {
            InitializeComponents();
            Form = form;
        }

        private void InitializeComponents()
        {
            Text = "Start Form";
            Size = new System.Drawing.Size(400, 200);
            StartPosition = FormStartPosition.CenterScreen;

            labelUserName = new Label
            {
                Text = "Enter your name:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            Controls.Add(labelUserName);

            inputUserName = new TextBox
            {
                Location = new System.Drawing.Point(150, 20),
                Width = 200
            };
            Controls.Add(inputUserName);

            labelIpAddress = new Label
            {
                Text = "Your external IP:",
                Location = new System.Drawing.Point(20, 60),
                AutoSize = true
            };
            Controls.Add(labelIpAddress);

            outputIpAddress = new TextBox
            {
                Location = new System.Drawing.Point(150, 60),
                Width = 200,
                ReadOnly = true
            };
            Controls.Add(outputIpAddress);

            submitButton = new Button
            {
                Text = "Submit",
                Location = new System.Drawing.Point(150, 100),
                AutoSize = true
            };
            submitButton.Click += SubmitButton_Click;
            Controls.Add(submitButton);

            outputIpAddress.Text = IpWorker.GetLocalIpAddress();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            UserName = inputUserName.Text;
            UserIp = outputIpAddress.Text;

            Form.userName = UserName;
            Form.userIp = UserIp;

            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("Please enter your name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
