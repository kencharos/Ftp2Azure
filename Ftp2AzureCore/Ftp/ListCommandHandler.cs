using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// LIST command handler
    /// list detailed information about files/directories
    /// </summary>
    internal class ListCommandHandler : ListCommandHandlerBase
    {
        public ListCommandHandler(FtpConnectionObject connectionObject)
            : base("LIST", connectionObject)
        {
        }

        protected override async Task<string> BuildReply(string[] asFiles, string[] asDirectories)
        {
            return await BuildLongReply(asFiles, asDirectories);
        }
    }
}