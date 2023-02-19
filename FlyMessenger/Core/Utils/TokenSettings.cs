using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FlyMessenger.Core.Utils
{
    public class TokenSettings
    {
        // Path to config file
        // tokenFilePath = folderPath + "config.dat";
        private readonly string tokenFilePath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            "token.dat"
        );

        // Save data to config
        public void Save(string data)
        {
            // string to byte array
            var dataBytes = Encoding.UTF8.GetBytes(data);

            // Encrypt data
            var encryptedData = ProtectedData.Protect(
                dataBytes,
                null,
                DataProtectionScope.CurrentUser
            );

            // Save encrypted data to file
            File.WriteAllBytes(tokenFilePath, encryptedData);
        }

        // Load data from config
        public string Load()
        {
            // if config file exists
            if (!File.Exists(tokenFilePath)) return string.Empty;
            
            // Read encrypted data from file
            var encryptedData = File.ReadAllBytes(tokenFilePath);

            // Decrypt data
            var dataBytes = ProtectedData.Unprotect(
                encryptedData,
                null,
                DataProtectionScope.CurrentUser
            );

            // byte array to string
            return Encoding.UTF8.GetString(dataBytes);
        }
    }
}
