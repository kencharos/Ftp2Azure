using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// SITE command handler
    /// show site info
    /// </summary>
    internal class SiteCommandHandler : FtpCommandHandler
    {
        public SiteCommandHandler(FtpConnectionObject connectionObject)
            : base("SITE", connectionObject)
        {
        }


        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            return GetMessage(200, "Ftp Server on Windows Azure, supply operations on Azure Blob Storage.");
        }
    }
}