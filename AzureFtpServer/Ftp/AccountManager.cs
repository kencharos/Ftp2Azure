using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.ServiceRuntime;
using AzureFtpServer.Provider;

namespace AzureFtpServer.Ftp
{
    /// <summary>
    /// AccountManager Class
    /// Read account information from config settings and store valid (username,password) pairs
    /// </summary>
    class AccountManager
    {
        #region Member Variables

        private const char separator = ':';
        private Dictionary<string, string> _accounts;
        private int _usernum;
        
        #endregion

        #region Construction

        public AccountManager()
        {
            _accounts = new Dictionary<string, string>();
            _usernum = 0;
        }

        #endregion

        #region Properties
        
        public int UserNum 
        {
            get { return _usernum; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Read the settings in RoleEnvironment, insert into the accounts dictionary
        /// </summary>
        /// <returns></returns>
        public int LoadConfigration()
        {
            // init member vars 
            _usernum = 0;
            _accounts.Clear();

            //int idx = 1; // use idx to concat the setting name
            
            //bool hasNext = true; // bool value, need find the next setting
            // Get the account setting
            string accountInfo = StorageProviderConfiguration.FtpAccount;

            while (true)
            {
                // Get the begin tag (
                int beginIdx = accountInfo.IndexOf('(');
                // Get the end tag )
                int endIdx = accountInfo.IndexOf(')');

                if ((beginIdx < 0) || (endIdx < 0) || (beginIdx > endIdx) || (endIdx == beginIdx + 1))
                    break;

                string oneAccount = accountInfo.Substring(beginIdx + 1, endIdx - beginIdx - 1);

                // modify accountInfo for loop
                accountInfo = accountInfo.Substring(endIdx + 1);

                int separatoridx = oneAccount.IndexOf(separator);

                if (separatoridx < 0)
                {
                    Trace.WriteLine(string.Format("Invalid <username, password> pair ({0}) in cscfg.", oneAccount), "Warnning");
                    continue;
                }

                // get the username substr
                string username = oneAccount.Substring(0, separatoridx);

                // check the username whether conform to the naming rules
                if (!CheckUsername(username))
                {
                    Trace.WriteLine(string.Format("Invalid <username, password> pair ({0}) in cscfg.", oneAccount), "Warnning");
                    continue;
                }

                // check whether the username already exists
                if (_accounts.ContainsKey(username))
                    continue;

                // get the password substr
                string password = oneAccount.Substring(separatoridx + 1);
                // simple check, password can not be empty
                if (password.Length == 0)
                    continue;

                _accounts.Add(username, password);
                _usernum++;
            }

            Trace.WriteLine(string.Format("Load {0} accounts.", _usernum), "Information");
            
            return _usernum;
        }

        /// <summary>
        /// Checks if (username, password) is a valid account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckAccount(string username, string password)
        {
            if (!_accounts.ContainsKey(username))
                return false;
            return (_accounts[username] == password);
        }

        /// <summary>
        /// checks whether username conform to the Azure container naming rules
        /// 1. start with a letter or number, and can contain only letters, numbers, and the dash (-) character
        /// 2. Every dash (-) character must be immediately preceded and followed by a letter or number; 
        ///    consecutive dashes are not permitted in container names.
        /// 3. All letters in a container name must be lowercase.
        /// 4. Container names must be from 3 through 63 characters long.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private bool CheckUsername(string username)
        {
            if (!Regex.IsMatch(username, @"^\$root$|^[a-z0-9]([a-z0-9]|(?<=[a-z0-9])-(?=[a-z0-9])){2,62}$"))
                return false;

            return true;
        }

        #endregion
    }
}
