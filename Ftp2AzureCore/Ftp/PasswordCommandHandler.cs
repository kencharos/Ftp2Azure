using Ftp2Azure.Ftp;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// PASS command handler
    /// get password and do login
    /// </summary>
    internal class PasswordCommandHandler : FtpCommandHandler
    {
        public PasswordCommandHandler(FtpConnectionObject connectionObject)
            : base("PASS", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            await Task.CompletedTask;
            sMessage = sMessage.Trim();
            if (sMessage == "")
                return GetMessage(501, string.Format("{0} needs a parameter", Command));

            if (ConnectionObject.Login(sMessage))
            {
                return GetMessage(220, "Password ok, FTP server ready");
            }
            else
            {
                return GetMessage(530, "Username or password incorrect");
            }
        }
    }
}