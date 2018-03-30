using Ftp2Azure.Ftp;
using Ftp2Azure.Ftp.FileSystem;
using Microsoft.Extensions.Configuration;

namespace Ftp2Azure.Azure
{
    public class AzureFileSystemFactory : IFileSystemClassFactory
    {
        #region Member variables
        private AccountManager m_accountManager;
        private IConfiguration config;
        #endregion

        #region Construction
        public AzureFileSystemFactory(IConfiguration config)
        {
            m_accountManager = new AccountManager();
            m_accountManager.LoadConfigration();
            this.config = config;
        }
        #endregion

        #region Implementation of IFileSystemClassFactory

        public IFileSystem Create(string sUser, string sPassword)
        {
            if ((sUser == null) || (sPassword == null))
                return null;

            if (!m_accountManager.CheckAccount(sUser, sPassword))
                return null;

            string containerName = sUser;
            var system = new AzureFileSystem(containerName, config);

            return system;
        }

        #endregion
    }
}