using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// ABOR command handler
    /// abort current data connection, TODO
    /// </summary>
    internal class AbortCommandHandler : FtpCommandHandler
    {
        public AbortCommandHandler(FtpConnectionObject connectionObject)
            : base("ABOR", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            // TODO: stop current service & close data connection
            return GetMessage(226, "Current data connection aborted");
        }
    }
}