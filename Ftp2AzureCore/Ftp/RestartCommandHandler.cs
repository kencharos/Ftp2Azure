using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// REST command handler
    /// </summary>
    internal class RestartCommandHandler : FtpCommandHandler
    {
        public RestartCommandHandler(FtpConnectionObject connectionObject)
            : base("REST", connectionObject)
        {
        }


        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            return GetMessage(500, "Restart not supported!");
        }
    }
}