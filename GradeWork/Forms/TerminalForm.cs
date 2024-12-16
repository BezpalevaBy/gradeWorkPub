using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using GradeWork.Network;
using GradeWork.Network.Messages;
using Type = GradeWork.Network.Messages.Type;

namespace GradeWork.Forms;

public class TerminalForm : Form
{
    private TextBox inputTextBox;
    private TextBox outputTextBox;
    private Button executeButton;

    public TerminalForm(string name)
    {
        Text = name;
        Size = new System.Drawing.Size(800, 600);

        outputTextBox = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Top,
            Height = 400,
            ReadOnly = true
        };

        inputTextBox = new TextBox
        {
            Dock = DockStyle.Top,
            Height = 30
        };

        executeButton = new Button
        {
            Text = "Execute",
            Dock = DockStyle.Top
        };

        executeButton.Click += ExecuteCommand;

        Controls.Add(outputTextBox);
        Controls.Add(inputTextBox);
        Controls.Add(executeButton);

        Form1.Instance.Terminals[Text] = this;
    }

    public void ExecuteCommand(object sender, EventArgs e)
    {
        string command = inputTextBox.Text;
        if (string.IsNullOrWhiteSpace(command)) return;

        try
        {
            new Sender(Text).ClientHandler(NetMessageParser.GetNetMessageWithValue(new HashSet<Type>()
            {
                Type.TerminalMessage
            }, command));
            
            //outputTextBox.AppendText($"> {command}\r\n{output}\r\n");
        }
        catch (Exception ex)
        {
            //outputTextBox.AppendText($"Exception: {ex.Message}\r\n");
        }

        inputTextBox.Clear();
    }

    public string ExecuteCommandAndGetOutput(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C chcp 437 && " + command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            }
        };

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(error))
        {
            output += $"Error: {error}\r\n";
        }

        return output;
    }

    public void InsertAnswerDirectInConsole(string message)
    {
        outputTextBox.AppendText($"\n{message}\r");
    }
}