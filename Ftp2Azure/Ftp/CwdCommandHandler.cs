using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// CWD command handler
    /// change working directory
    /// </summary>
    internal class CwdCommandHandler : CwdCommandHandlerBase
    {
        public CwdCommandHandler(FtpConnectionObject connectionObject)
            : base("CWD", connectionObject)
        {
        }
    }
}