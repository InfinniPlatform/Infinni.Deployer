using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Serilog;

namespace Infinni.Deployer.Helpers
{
    /// <summary>
    /// Manager for Windows services.
    /// </summary>
    public class ServiceControlWrapper : ISystemServiceManager
    {
        private const string ScExecutable = "sc.exe";
        private const string DotnetExecutable = "dotnet.exe";
        private const string WindowsServiceKey = "--windowsService";
        private const int DefaultTimeout = 60 * 1000;

        public void Create(string packageId, string version, string executablePath)
        {
            var arguments = $"create {packageId}.{version} DisplayName= \"{packageId}.{version}\" binpath= \"{DotnetExecutable} {executablePath} {WindowsServiceKey}\"";

            Execute(nameof(Create), arguments);
        }

        public void Delete(string packageId, string version)
        {
            var arguments = $"delete {packageId}.{version}";

            Execute(nameof(Delete), arguments);
        }

        public void Start(string packageId, string version)
        {
            var arguments = $"start {packageId}.{version}";

            Execute(nameof(Start), arguments);
        }

        public void Stop(string packageId, string version)
        {
            var arguments = $"stop {packageId}.{version}";

            Execute(nameof(Stop), arguments);
        }

        private static void Execute(string commandName, string arguments)
        {
            Log.Information("Executing {File} {arguments}", ScExecutable, arguments);

            //var processStartInfo = new ProcessStartInfo
            //{
            //    FileName = ScExecutable,
            //    Arguments = arguments,
            //    UseShellExecute = false,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //    RedirectStandardInput = true,
            //    CreateNoWindow = true
            //};

            //var process = Process.Start(processStartInfo);
            //process.EnableRaisingEvents = true;
            //process.OutputDataReceived += (sender, args) => Log.Information(args.Data);
            //process.ErrorDataReceived += (sender, args) => Log.Error(args.Data);
            //process.WaitForExit();

            using (var process = new Process())
            {
                process.StartInfo.FileName = ScExecutable;
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
                    // Подписка на события записи в выходные потоки процесса

                    var copyOutputCloseEvent = outputCloseEvent;

                    process.OutputDataReceived += (s, e) =>
                    {
                        // Поток output закрылся (процесс завершил работу)
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
                        // Поток error закрылся (процесс завершил работу)
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
                    catch (Exception error)
                    {
                        // Не удалось запустить процесс, скорей всего, файл не существует или не является исполняемым

                        //result.Completed = true;
                        //result.ExitCode = -1;
                        //result.Output = string.Format(Properties.Resources.CannotExecuteCommand, command, arguments, error.Message);

                        isStarted = false;
                    }

                    if (isStarted)
                    {
                        // Начало чтения выходных потоков процесса в асинхронном режиме, чтобы не создать блокировку
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        // Ожидание завершения процесса и закрытия выходных потоков
                        if (process.WaitForExit(DefaultTimeout)
                            && outputCloseEvent.WaitOne(DefaultTimeout)
                            && errorCloseEvent.WaitOne(DefaultTimeout))
                        {

                            Log.Debug("{outputBuilder}", outputBuilder.ToString());

                            // Вывод актуален только при наличии ошибки
                            if (process.ExitCode != 0)
                            {
                                Log.Error("{errorBuilder}", errorBuilder.ToString());
                            }
                        }
                        else
                        {
                            try
                            {
                                // Зависшие процессы завершаются принудительно
                                process.Kill();
                            }
                            catch
                            {
                                // Любые ошибки в данном случае игнорируются
                            }
                        }
                    }
                }
            }

            Log.Information("{CommandName} command completed.", commandName);
        }
    }
}