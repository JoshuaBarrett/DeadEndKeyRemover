using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace DeadEndKeyRemover
{
    [SupportedOSPlatform("windows")]
    public class RegistryClient
    {
        RegistryKey? MainRegistryKey;

        public RegistryClient(string mainKeyPath)
        {
            this.MainRegistryKey = Registry.LocalMachine.OpenSubKey(mainKeyPath, true);
        }

        public string[] GetDeadEndKeys(string displayNameFilter)
        {
            if (this.MainRegistryKey == null)
            {
                throw new NullReferenceException("Main Registry is Null");
            }

            string[] subKeys = this.MainRegistryKey.GetSubKeyNames();

            string[] filteredSubKeys = subKeys.Where(x => x.Contains(displayNameFilter, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            string[] deadEndSubKeys = FindDeadEndSubKeys(filteredSubKeys).ToArray();

            return deadEndSubKeys;
        }

        public void RemoveSubKeys(string[] subKeyNames)
        {
            if (this.MainRegistryKey == null)
            {
                throw new NullReferenceException("Main Registry is Null");
            }

            foreach (string subKey in subKeyNames)
            {
                this.MainRegistryKey.DeleteSubKey(subKey);
            }
        }

        private IEnumerable<string> FindDeadEndSubKeys(string[] subKeyNames)
        {
            if (this.MainRegistryKey == null)
            {
                throw new NullReferenceException("Main Registry is Null");
            }

            foreach (string keyName in subKeyNames)
            {
                RegistryKey? subRegKey = this.MainRegistryKey.OpenSubKey(keyName, true);
                if (subRegKey == null)
                {
                    throw new NullReferenceException("Sub Registry is Null");
                }

                string uninstallFilePath = (string)(subRegKey.GetValue("UninstallString") ?? string.Empty);
                if (string.IsNullOrEmpty(uninstallFilePath) || !File.Exists(uninstallFilePath))
                {
                    yield return keyName;
                }                
            }            
        }
    }
}
