using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// PWD command handler
    /// Present working directory
    /// </summary>
    internal class PwdCommandHandler : PwdCommandHandlerBase
    {
        public PwdCommandHandler(FtpConnectionObject connectionObject)
            : base("PWD", connectionObject)
        {
        }
    }
}