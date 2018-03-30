using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// MKD command handler
    /// make directory
    /// </summary>
    internal class MakeDirectoryCommandHandler : MakeDirectoryCommandHandlerBase
    {
        public MakeDirectoryCommandHandler(FtpConnectionObject connectionObject)
            : base("MKD", connectionObject)
        {
        }
    }
}