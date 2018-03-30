using Ftp2Azure.Ftp;
using System;

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

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(215, Environment.OSVersion.VersionString);
        }
    }
}