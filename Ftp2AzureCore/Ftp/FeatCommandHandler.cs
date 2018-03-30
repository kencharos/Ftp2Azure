using Ftp2Azure.Ftp;
using Ftp2Azure.General;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// FEAT command handler
    /// list the supported command extensions on this ftp server
    /// </summary>
    internal class FeatCommandHandler : FtpCommandHandler
    {
        public FeatCommandHandler(FtpConnectionObject connectionObject)
            : base("FEAT", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            if (sMessage.Length != 0)
                return GetMessage(501, "Invalid syntax for FEAT command");

            string response = "211-Extensions supported:\r\n";
            response += " XCUP\r\n";
            response += " XCWD\r\n";
            response += " XMKD\r\n";
            response += " XPWD\r\n";
            response += " XRMD\r\n";
            response += " MDTM\r\n";
            response += " MLSD\r\n";
            response += " MLST\r\n";
            response += " SIZE\r\n";
            response += "211 END\r\n";
            SocketHelpers.Send(ConnectionObject.Socket, response, ConnectionObject.Encoding);
            return "";
        }
    }
}
