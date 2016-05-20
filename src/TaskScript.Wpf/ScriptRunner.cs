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
        private Process _process;

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
                    //CreateNoWindow = true,
                };
                _process = new Process()
                {
                    StartInfo = startInfo
                };
                _process.OutputDataReceived += (s, e) => _mainWindow.Output = e.Data;
                _process.Start();
                _process.BeginOutputReadLine();

                //_process.StandardInput.WriteLine($);

                _process.WaitForExit();
            });
        }

        internal void Stop()
        {
            if (!_process.HasExited)
                _process.Kill();
        }
    }
}
