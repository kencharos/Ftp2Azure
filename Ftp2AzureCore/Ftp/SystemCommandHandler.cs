using Ftp2Azure.Ftp;
using System;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// SYST command handler
    /// show the system version of this ftp server
    /// </summary>
    internal class SystemCommandHandler : FtpCommandHandler
    {
        public SystemCommandHandler(FtpConnectionObject connectionObject)
            : base("SYST", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            return GetMessage(215, Environment.OSVersion.VersionString);
        }
    }
}