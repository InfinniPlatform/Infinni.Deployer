using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Infinni.Deployer.ApplicationHelpers
{
    public static class WindowsHelper
    {
        private const int AccessDeniedErrorCode = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("Kernel32", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        public static void SendSigIntToApplication(Process process)
        {
            var result = TrySend((uint) process.Id);

            if (result == AccessDeniedErrorCode)
            {
                process.WaitForExit(2000);

                FreeConsole();

                TrySend((uint) process.Id);

                //Re-enable Ctrl-C handling or any subsequently started
                //programs will inherit the disabled state.
                SetConsoleCtrlHandler(null, false);
            }

            process.WaitForExit(2000);

            FreeConsole();

            //Re-enable Ctrl-C handling or any subsequently started
            //programs will inherit the disabled state.
            SetConsoleCtrlHandler(null, false);
        }

        private static int TrySend(uint processId)
        {
            if (AttachConsole(processId))
            {
                SetConsoleCtrlHandler(null, true);
                GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);
            }

            return Marshal.GetLastWin32Error();
        }

        // Enumerated type for the control messages sent to the handler routine
        private enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        private delegate bool HandlerRoutine(CtrlTypes ctrlType);
    }
}