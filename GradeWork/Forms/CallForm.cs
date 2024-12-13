using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using GradeWork.Network;
using Type = GradeWork.Network.Messages.Type;

namespace GradeWork.Forms
{
    public class CallForm : Form
    {
        private TextBox inputIpAddress;
        private Label labelIpAddress;
        private Button buttonSubmit;

        public string TargetIp { get; private set; }

        public CallForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Call Machine";
            Size = new System.Drawing.Size(400, 150);
            StartPosition = FormStartPosition.CenterScreen;

            labelIpAddress = new Label
            {
                Text = "Enter target IP address:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };
            Controls.Add(labelIpAddress);

            inputIpAddress = new TextBox
            {
                Location = new System.Drawing.Point(20, 50),
                Width = 340
            };
            Controls.Add(inputIpAddress);

            buttonSubmit = new Button
            {
                Text = "Connect",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true
            };
            buttonSubmit.Click += ButtonSubmit_Click;
            Controls.Add(buttonSubmit);
        }

        private void ButtonSubmit_Click(object sender, EventArgs e)
        {
            TargetIp = inputIpAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(TargetIp))
            {
                MessageBox.Show("Please enter a valid IP address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidIp(TargetIp))
            {
                MessageBox.Show("Invalid IP address format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Waiting for response.", "Click ok", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;

            Task.Run(Smth);
        }

        private async void Smth()
        {
            var promise = new Promise(TargetIp, 1,new HashSet<Type>()
            {
                Type.WaitingRESPONSE,
                Type.CallIncomingForm
            });
            
            var isGetAnswer = await promise.IsGetAnswer();
            MessageBox.Show(isGetAnswer ? "YES CONNECTION" : "NO CONNECTION");
        }

        private bool IsValidIp(string ip)
        {
            var parts = ip.Split('.');
            if (parts.Length != 4) return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out var num) || num < 0 || num > 255)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
