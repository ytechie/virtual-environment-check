using System;
using System.Diagnostics;
using System.Linq;
using System.Management;


namespace virtual_environment_check
{
    class Program
    {
        //adminasdf / afs#R32sdf
        //vmenvcheck
        static void Main()
        {
            Console.WriteLine("Running in Azure: {0}", IsRunningInAzure());
            Console.WriteLine("Running in Hyper-V: {0}", IsRunningInHyperV());

            Console.ReadLine();
        }

        /// <summary>
        ///     Determines if the machine executing this code is running in Azure.
        /// </summary>
        /// <remarks>
        ///     Is this code guaranteed to work forever? Absolutely not. It is completely
        ///     up to the host as to how it exposes itself. It's using the best checks
        ///     available for old and new VM's alike.
        /// </remarks>
        /// <returns>
        ///     True if this code is running in Azure, otherwise false.
        /// </returns>
        private static bool IsRunningInAzure()
        {
            var adapters = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            var hasCloudDnsSuffix = adapters.Any(adapter => adapter.GetIPProperties().DnsSuffix.EndsWith(".cloudapp.net"));

            var agentInstalled = Process.GetProcessesByName("WindowsAzureGuestAgent").Any();

            return hasCloudDnsSuffix || agentInstalled;
        }

        /// <summary>
        ///     Determines if the machine executing this code is running in a Microsoft
        ///     virtualized environment (Hyper-V).
        /// </summary>
        /// <remarks>
        ///     If running in Azure, this will return TRUE since Azure uses Hyper-V. If you want to know if
        ///     the code is running in Hyper-V outside of Azure, combine this with an Azure check.
        /// </remarks>
        /// <returns>
        ///     True if running in a Microsoft/Hyper-V environment, otherwise false.
        /// </returns>
        private static bool IsRunningInHyperV()
        {
            var managementCollection =
                new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get();
            var entries = managementCollection.OfType<ManagementBaseObject>();

            return entries.Any(x => x["Manufacturer"].ToString().ToLower() == "microsoft corporation");
        }
    }
}
