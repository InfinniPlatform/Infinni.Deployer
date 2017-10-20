using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Serilog;

namespace Infinni.Deployer.Helpers
{
    /// <summary>
    /// Wrapper for executing external services with output intercepting.
    /// </summary>
    public static class ProcessExecutor
    {
        private const int DefaultTimeout = 60 * 1000;

        public static void Execute(string executable, string command, string arguments)
        {
            Log.Information("Executing {Executable} {arguments}", executable, arguments);

            using (var process = new Process())
            {
                process.StartInfo.FileName = executable;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                using (var outputCloseEvent = new AutoResetEvent(false))
                using (var errorCloseEvent = new AutoResetEvent(false))
                {
                    var copyOutputCloseEvent = outputCloseEvent;

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(e.Data))
                        {
                            copyOutputCloseEvent.Set();
                        }
                        else
                        {
                            outputBuilder.AppendLine(e.Data);
                        }
                    };

                    var copyErrorCloseEvent = errorCloseEvent;

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(e.Data))
                        {
                            copyErrorCloseEvent.Set();
                        }
                        else
                        {
                            errorBuilder.AppendLine(e.Data);
                        }
                    };

                    bool isStarted;

                    try
                    {
                        isStarted = process.Start();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Process exit with error: {Error}", e);

                        isStarted = false;
                    }

                    if (isStarted)
                    {
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(DefaultTimeout)
                            && outputCloseEvent.WaitOne(DefaultTimeout)
                            && errorCloseEvent.WaitOne(DefaultTimeout))
                        {
                            Log.Debug("{outputBuilder}", outputBuilder.ToString());

                            if (process.ExitCode != 0)
                            {
                                Log.Error("{errorBuilder}", errorBuilder.ToString());
                            }
                        }
                        else
                        {
                            try
                            {
                                process.Kill();
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
            }

            Log.Information("{Executable} {Command} command completed.", executable, command);
        }
    }
}