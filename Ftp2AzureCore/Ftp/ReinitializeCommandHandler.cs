using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// REIN command handler
    /// waiting for new user, reset all connection parameters
    /// </summary>
    internal class ReinitializeCommandHandler : FtpCommandHandler
    {
        public ReinitializeCommandHandler(FtpConnectionObject connectionObject)
            : base("REIN", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            if (sMessage.Trim() != "")
                return GetMessage(501, "REIN needs no parameters");

            // log out current user
            ConnectionObject.LogOut();

            return GetMessage(220, "Service ready for new user!");
        }
    }
}