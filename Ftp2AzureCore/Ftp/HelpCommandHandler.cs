using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// HELP command handler
    /// show help information
    /// </summary>
    internal class HelpCommandHandler : FtpCommandHandler
    {
        public HelpCommandHandler(FtpConnectionObject connectionObject)
            : base("HELP", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            return GetMessage(214, "Log in first, use FEAT to see supported extended commands");
        }
    }
}