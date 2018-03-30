using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// CDUP command handler
    /// change to the parent directory
    /// </summary>
    internal class CdupCommandHandler : CdupCommandHandlerBase
    {
        public CdupCommandHandler(FtpConnectionObject connectionObject)
            : base("CDUP", connectionObject)
        {
        }
    }
}