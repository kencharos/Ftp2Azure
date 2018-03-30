using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// XRMD command handler
    /// remove directory
    /// </summary>
    internal class XRmdCommandHandler : RemoveDirectoryCommandHandlerBase
    {
        public XRmdCommandHandler(FtpConnectionObject connectionObject)
            : base("XRMD", connectionObject)
        {
        }
    }
}