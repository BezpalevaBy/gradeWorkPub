using System;
using System.Threading;
using GradeWork.Forms;

namespace GradeWork.Methods;

public class Terminal
{
    public static void CreateTerminal(string name)
    {
        Console.WriteLine($"IP OF TERMINAL '{name}'");
        
        Thread thread = new Thread(() =>
        {
            var terminalForm = new TerminalForm(name);
            terminalForm.ShowDialog();
        });
        thread.Start();
    }

    public static string ExecuteCommandInTerminal(string terminalName, string command)
    {
        Console.WriteLine($"TRYING TO FIND CONSOLE WITH IP {terminalName}");
        if (!Form1.Instance.Terminals.TryGetValue(terminalName, out var terminal)) return String.Empty;

        Console.WriteLine($"IT WAS FOUNDED");
        return terminal.ExecuteCommandAndGetOutput(command);
    }

    public static void InsertInConsoleOfTerminal(string terminalName, string response)
    {
        if (!Form1.Instance.Terminals.TryGetValue(terminalName, out var terminal)) return;
        
        terminal.InsertAnswerDirectInConsole(response);
    }
}