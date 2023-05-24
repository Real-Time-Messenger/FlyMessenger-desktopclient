using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FlyMessenger.Core.Utils
{
    /// <summary>
    /// Token settings.
    /// </summary>
    public class TokenSettings
    {
        // Create token file path.
        // Path: C:\Users\{username}\AppData\Roaming\FlyMessenger\token.dat
        private readonly string _tokenFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlyMessenger", "token.dat");
        
        // Save data to config
        public void Save(string data)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_tokenFilePath) ?? string.Empty))
                Directory.CreateDirectory(Path.GetDirectoryName(_tokenFilePath) ?? string.Empty);
            
            // string to byte array
            var dataBytes = Encoding.UTF8.GetBytes(data);

            // Encrypt data
            var encryptedData = ProtectedData.Protect(
                dataBytes,
                null,
                DataProtectionScope.CurrentUser
            );

            // Save encrypted data to file
            File.WriteAllBytes(_tokenFilePath, encryptedData);
        }

        // Load data from config
        public string Load()
        {
            // if config file exists
            if (!File.Exists(_tokenFilePath)) return string.Empty;
            
            // Read encrypted data from file
            var encryptedData = File.ReadAllBytes(_tokenFilePath);

            // Decrypt data
            var dataBytes = ProtectedData.Unprotect(
                encryptedData,
                null,
                DataProtectionScope.CurrentUser
            );

            // byte array to string
            return Encoding.UTF8.GetString(dataBytes);
        }
        
        // Delete config file
        public void Delete()
        {
            File.Delete(_tokenFilePath);
        }
    }
}
