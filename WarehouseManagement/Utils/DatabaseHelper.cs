using System;
using System.Data;
using System.Configuration;
using Npgsql;
using WarehouseManagement.Models;

namespace WarehouseManagement.Utils
{
    public static class DatabaseHelper
    {
        // Получаем строку подключения из конфигурации (для безопасности)
        private static string GetConnectionString()
        {
            string encryptedConnectionString = ConfigurationManager.ConnectionStrings["WarehouseDbConnection"].ConnectionString;
            string secretKey = "WarehouseTech2023SecretKey"; // В реальном приложении хранить в защищенном месте

            // Если строка подключения зашифрована, расшифровываем
            if (ConfigurationManager.AppSettings["EncryptedConnection"] == "true")
            {
                try
                {
                    return UserSession.DecryptData(encryptedConnectionString, secretKey);
                }
                catch
                {
                    // Если расшифровка не удалась, возвращаем строку как есть
                    return encryptedConnectionString;
                }
            }

            return encryptedConnectionString;
        }

        // Создаем новое подключение к базе данных
        public static NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(GetConnectionString());
        }

        // Выполняем запрос и возвращаем DataTable
        public static DataTable ExecuteQuery(string query, params NpgsqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();

            using (NpgsqlConnection connection = CreateConnection())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        connection.Open();
                        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Логирование ошибок
                        LogError("Database query error: " + ex.Message + " Query: " + query);
                        throw;
                    }
                }
            }

            return dataTable;
        }

        // Выполняем запрос без возвращения данных (INSERT, UPDATE, DELETE)
        public static int ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            int rowsAffected = 0;

            using (NpgsqlConnection connection = CreateConnection())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Логирование ошибок
                        LogError("Database non-query error: " + ex.Message + " Query: " + query);
                        throw;
                    }
                }
            }

            return rowsAffected;
        }

        // Выполняем запрос и возвращаем скалярное значение
        public static object ExecuteScalar(string query, params NpgsqlParameter[] parameters)
        {
            object result = null;

            using (NpgsqlConnection connection = CreateConnection())
            {
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        connection.Open();
                        result = command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        // Логирование ошибок
                        LogError("Database scalar query error: " + ex.Message + " Query: " + query);
                        throw;
                    }
                }
            }

            return result;
        }

        // Начинаем транзакцию
        public static NpgsqlTransaction BeginTransaction(NpgsqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection.BeginTransaction();
        }

        // Логирование ошибок
        private static void LogError(string message)
        {
            // Здесь может быть реализовано логирование в файл или БД
            Console.WriteLine("ERROR: " + message);
            // В реальном приложении добавить запись в лог-файл
        }
    }
}