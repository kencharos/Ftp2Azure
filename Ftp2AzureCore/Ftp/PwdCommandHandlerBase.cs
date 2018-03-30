using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// Base class for PWD & XPWD command handlers
    /// </summary>
    internal class PwdCommandHandlerBase : FtpCommandHandler
    {
        public PwdCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            string sDirectory = ConnectionObject.CurrentDirectory;
            return GetMessage(257, string.Format("\"{0}\" {1} Successful.", sDirectory, Command));
        }
    }
}