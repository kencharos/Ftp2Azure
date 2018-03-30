using Ftp2Azure.Ftp;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// XPWD command handler
    /// Present working directory
    /// </summary>
    internal class XPwdCommandHandler : PwdCommandHandlerBase
    {
        public XPwdCommandHandler(FtpConnectionObject connectionObject)
            : base("XPWD", connectionObject)
        {
        }
    }
}