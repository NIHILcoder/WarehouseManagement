using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace WarehouseManagement.Utils
{
    /// <summary>
    /// Утилита для шифрования и дешифрования данных
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// Шифрует строку с использованием AES алгоритма
        /// </summary>
        /// <param name="plainText">Исходный текст для шифрования</param>
        /// <param name="key">Секретный ключ (будет хешироваться в 256-битный)</param>
        /// <returns>Зашифрованная строка в Base64</returns>
        public static string EncryptString(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                // Создаем массив байтов из исходной строки
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                // Генерируем 256-битный ключ из пароля
                byte[] keyBytes = CreateKey(key);

                // Создаем случайный IV (вектор инициализации)
                byte[] iv = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(iv);
                }

                // Создаем AES шифровщик
                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;  // Используем режим сцепления блоков (Cipher Block Chaining)
                    aes.Padding = PaddingMode.PKCS7;  // Стандартное заполнение
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = keyBytes;
                    aes.IV = iv;

                    // Шифруем данные
                    using (var encryptor = aes.CreateEncryptor())
                    using (var memoryStream = new MemoryStream())
                    {
                        // Записываем IV в начало потока (для последующего дешифрования)
                        memoryStream.Write(iv, 0, iv.Length);

                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                            cryptoStream.FlushFinalBlock();
                        }

                        // Преобразуем зашифрованные данные в строку Base64
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                // В реальном приложении здесь должно быть логирование
                Console.WriteLine($"Ошибка при шифровании: {ex.Message}");
                throw new CryptographicException("Не удалось зашифровать данные", ex);
            }
        }

        /// <summary>
        /// Дешифрует строку, зашифрованную методом EncryptString
        /// </summary>
        /// <param name="cipherText">Зашифрованный текст в Base64</param>
        /// <param name="key">Секретный ключ (тот же, что использовался для шифрования)</param>
        /// <returns>Расшифрованная строка</returns>
        public static string DecryptString(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                // Преобразуем зашифрованную строку Base64 в массив байтов
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Генерируем 256-битный ключ из пароля
                byte[] keyBytes = CreateKey(key);

                // Извлекаем IV из первых 16 байт
                byte[] iv = new byte[16];
                Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);

                // Извлекаем зашифрованные данные (без IV)
                byte[] encryptedData = new byte[cipherBytes.Length - iv.Length];
                Buffer.BlockCopy(cipherBytes, iv.Length, encryptedData, 0, encryptedData.Length);

                // Создаем AES дешифровщик
                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = keyBytes;
                    aes.IV = iv;

                    // Дешифруем данные
                    using (var decryptor = aes.CreateDecryptor())
                    using (var memoryStream = new MemoryStream(encryptedData))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                // В реальном приложении здесь должно быть логирование
                Console.WriteLine($"Ошибка при дешифровании: {ex.Message}");
                throw new CryptographicException("Не удалось дешифровать данные", ex);
            }
        }

        /// <summary>
        /// Хеширует строку с использованием SHA256
        /// </summary>
        /// <param name="input">Исходная строка</param>
        /// <returns>Хеш строки в виде hex-строки</returns>
        public static string HashString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            try
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                    // Преобразуем хеш в строку шестнадцатеричных символов
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            catch (Exception ex)
            {
                // В реальном приложении здесь должно быть логирование
                Console.WriteLine($"Ошибка при хешировании: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Создает 256-битный ключ из строки-пароля
        /// </summary>
        /// <param name="password">Пароль или секретный ключ</param>
        /// <returns>256-битный ключ (32 байта)</returns>
        private static byte[] CreateKey(string password)
        {
            // Используем хеширование SHA256 для получения ключа нужной длины
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Генерирует случайный пароль заданной длины
        /// </summary>
        /// <param name="length">Длина пароля (по умолчанию 16 символов)</param>
        /// <returns>Случайный пароль</returns>
        public static string GenerateRandomPassword(int length = 16)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            // Объединяем все возможные символы
            string allChars = lowerChars + upperChars + numberChars + specialChars;

            // Создаем генератор случайных чисел
            using (var rng = new RNGCryptoServiceProvider())
            {
                // Создаем буфер для хранения случайных байтов
                byte[] randomBytes = new byte[length * 4]; // Умножаем на 4 для большей энтропии
                rng.GetBytes(randomBytes);

                // Создаем пароль с использованием случайных байтов
                char[] password = new char[length];

                // Гарантируем наличие хотя бы одного символа каждой категории
                password[0] = lowerChars[randomBytes[0] % lowerChars.Length];
                password[1] = upperChars[randomBytes[1] % upperChars.Length];
                password[2] = numberChars[randomBytes[2] % numberChars.Length];
                password[3] = specialChars[randomBytes[3] % specialChars.Length];

                // Заполняем оставшиеся позиции случайными символами
                for (int i = 4; i < length; i++)
                {
                    password[i] = allChars[randomBytes[i] % allChars.Length];
                }

                // Перемешиваем символы (для непредсказуемости позиций)
                Random random = new Random(BitConverter.ToInt32(randomBytes, 0));
                password = password.OrderBy(x => random.Next()).ToArray();

                return new string(password);
            }
        }
    }
}