using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace Ticket.Models.Automation
{
    public class Email
    {
        public void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:/Python/python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process.Start(start);
        }
    }
}