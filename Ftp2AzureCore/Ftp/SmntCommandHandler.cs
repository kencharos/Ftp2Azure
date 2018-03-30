using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// SMNT command handler, superfluous at this site
    /// </summary>
    internal class SmntCommandHandler : FtpCommandHandler
    {
        public SmntCommandHandler(FtpConnectionObject connectionObject)
            : base("SMNT", connectionObject)
        {
        }


        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            return GetMessage(202, "");
        }
    }
}