using Ftp2Azure.Ftp;

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

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(214, "Log in first, use FEAT to see supported extended commands");
        }
    }
}