using System.Runtime.InteropServices;

namespace Mirador
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private const string UniqueIdentifier = "M1R4D0R-3RGO-3LFN-I99B-1NT1M3-1S0Z";

        [STAThread]
        public static void Main()
        {
            AllocConsole();
            bool createdNew;
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueIdentifier, out createdNew);

            if (!createdNew)
            {
                Console.WriteLine("Another instance is already running. Exiting new instance.");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mirador = new Mirador();
            mirador.Initialize();
            Application.ApplicationExit += mirador.OnApplicationExit;
            Application.Run();
        }
    }
}
