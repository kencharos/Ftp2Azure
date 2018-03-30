using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// ACCT command manger
    /// No need for this ftp server
    /// </summary>
    internal class AccountCommandHandler : FtpCommandHandler
    {
        public AccountCommandHandler(FtpConnectionObject connectionObject)
            : base("ACCT", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            // TODO: stop current service & close data connection
            return GetMessage(230, "Account information not needed");
        }
    }
}