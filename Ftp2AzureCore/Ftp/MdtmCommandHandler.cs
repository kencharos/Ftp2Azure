﻿using Ftp2Azure.Ftp;
using Ftp2Azure.Ftp.FileSystem;
using System.Threading.Tasks;

namespace Ftp2Azure.FtpCommands
{
    /// <summary>
    /// MDTM command handler
    /// show last modified time of files
    /// </summary>
    internal class MdtmCommandHandler : FtpCommandHandler
    {
        public MdtmCommandHandler(FtpConnectionObject connectionObject)
            : base("MDTM", connectionObject)
        {
        }

        protected override async Task<string> OnProcess(string sMessage)
        {
            string sPath = GetPath(sMessage.Trim());

            if (!await ConnectionObject.FileSystemObject.FileExists(sPath))
            {
                return GetMessage(550, string.Format("File doesn't exist ({0})", sPath));
            }

            IFileInfo info = await ConnectionObject.FileSystemObject.GetFileInfo(sPath);

            if (info == null)
            {
                return GetMessage(550, "Error in getting file information");
            }

            return GetMessage(213, info.GetModifiedTime().ToString());
        }
    }
}
