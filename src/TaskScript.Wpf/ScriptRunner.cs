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
        INotifyUser _host;
        Dictionary<string, Process> _processes = new Dictionary<string, Process>();

        public ScriptRunner(INotifyUser host)
        {
            _host = host;
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
                    process.OutputDataReceived += (s, e) => _host.Output = e.Data;
                    process.Start();
                    //process.BeginOutputReadLine();
                    _processes.Add(path, process);

                    process.WaitForExit();
                    var code = process.ExitCode;
                    if (process.ExitCode == 0)
                    {
                        _host.NotifyOfSuccess(path);
                    }
                    else
                    {
                        _host.NotifyOfError(path);
                    }
                    Stop(path);
                }
            });
        }

        internal void Stop(string path)
        {
            if (_processes.ContainsKey(path))
            {
                var process = _processes[path];
                if (!process.HasExited)
                    process.Kill();

                _processes.Remove(path);
            }
        }

        internal void Stop()
        {
            foreach (var processInfo in new Dictionary<string, Process>(_processes))
            {
                var process = processInfo.Value;
                if (!process.HasExited)
                    process.Kill();

                _processes.Remove(processInfo.Key);
            }
            // TODO: keep track of open processes and kill them
        }
    }
}
