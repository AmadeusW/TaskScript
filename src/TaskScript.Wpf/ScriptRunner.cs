using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScript.Wpf
{
    class ScriptRunner
    {
        private MainWindow _mainWindow;

        public ScriptRunner(MainWindow mainWindow)
        {
            this._mainWindow = mainWindow;
        }

        internal void RunScript(string path, string args = null)
        {
            Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo("scriptcs")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    Arguments = $"{path} -- {args}",
                    CreateNoWindow = true,
                };

                using (var process = new Process()
                {
                    StartInfo = startInfo
                })
                { 
                    process.OutputDataReceived += (s, e) => _mainWindow.Output = e.Data;
                    process.Start();
                    process.BeginOutputReadLine();

                    //_process.StandardInput.WriteLine($);

                    process.WaitForExit();
                }
            });
        }

        internal void Stop()
        {
            // TODO: keep track of open processes and kill them
        }
    }
}
