using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WarehouseManagement.Models
{
    // Статический класс для хранения информации о текущей сессии пользователя
    public static class UserSession
    {
        public static int UserID { get; set; }
        public static string Username { get; set; }
        public static string FullName { get; set; }
        public static string Role { get; set; }

        // Свойства для проверки роли
        public static bool IsAdmin => Role == "Administrator";
        public static bool IsManager => Role == "Manager" || IsAdmin;
        public static bool IsWarehouse => Role == "Warehouse" || IsManager;

        // Метод для очистки сессии при выходе
        public static void Clear()
        {
            UserID = 0;
            Username = null;
            FullName = null;
            Role = null;
        }

        // Метод для хеширования пароля (AES для выполнения требования по безопасности)
        public static string HashPassword(string password)
        {
            // В реальном приложении лучше использовать более безопасные алгоритмы
            // например, bcrypt или PBKDF2
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Метод для шифрования данных (для выполнения требования по безопасности)
        public static string EncryptData(string data, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(data);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        // Метод для расшифровки данных
        public static string DecryptData(string data, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(data);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}