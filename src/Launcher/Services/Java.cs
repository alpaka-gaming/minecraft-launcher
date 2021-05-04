using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{

    public interface IJava
    {
        string JavaExecutable { get; }
        string JavaInstallationPath { get; }
        string JavaBitInstallation { get; }
    }
    public class Java : IJava
    {

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public Java(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _configuration = configuration;
        }

        public string JavaExecutable
            => GetJavaInstallationPath() == null ? null : string.Format("{0}\\bin\\java.exe", GetJavaInstallationPath());

        public string JavaInstallationPath => GetJavaInstallationPath();

        private bool _isNotWow6432Installation;

        public string JavaBitInstallation
        {
            get
            {
                if (JavaExecutable == null)
                {
                    return "null";
                }
                if ((_isNotWow6432Installation && !Environment.Is64BitOperatingSystem) ||
                    (!_isNotWow6432Installation && Environment.Is64BitOperatingSystem))
                {
                    return "32";
                }
                if (_isNotWow6432Installation && Environment.Is64BitOperatingSystem)
                {
                    return "64";
                }
                return "null";
            }
        }

        private string GetJavaInstallationPath()
        {
            _isNotWow6432Installation = true;
            string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
            while (true)
            {
                using (RegistryKey baseKey =
                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey))
                {
                    if (baseKey == null)
                    {
                        if (javaKey == "SOFTWARE\\Wow6432Node\\JavaSoft\\Java Runtime Environment")
                        {
                            break;
                        }
                        _isNotWow6432Installation = false;
                        javaKey = "SOFTWARE\\Wow6432Node\\JavaSoft\\Java Runtime Environment";
                        continue;
                    }
                    string currentVersion = baseKey.GetValue("CurrentVersion").ToString();
                    using (RegistryKey homeKey = baseKey.OpenSubKey(currentVersion))
                        if (homeKey != null)
                        {
                            return homeKey.GetValue("JavaHome").ToString();
                        }
                }
                break;
            }
            return null;
        }
    }
}
