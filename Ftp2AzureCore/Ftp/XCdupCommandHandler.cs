using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// XCUP command handler
    /// change to the parent directory
    /// </summary>
    internal class XCdupCommandHandler : CdupCommandHandlerBase
    {
        public XCdupCommandHandler(FtpConnectionObject connectionObject)
            : base("XCUP", connectionObject)
        {
        }
    }
}