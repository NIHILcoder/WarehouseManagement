using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }

        // Получить список всех клиентов
        public static List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            string query = "SELECT * FROM Customers ORDER BY Name";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    customers.Add(new Customer
                    {
                        CustomerID = Convert.ToInt32(row["CustomerID"]),
                        Name = row["Name"].ToString(),
                        ContactPerson = row["ContactPerson"] == DBNull.Value ? null : row["ContactPerson"].ToString(),
                        Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString(),
                        Phone = row["Phone"] == DBNull.Value ? null : row["Phone"].ToString(),
                        Address = row["Address"] == DBNull.Value ? null : row["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении списка клиентов: " + ex.Message);
            }

            return customers;
        }

        // Получить клиента по ID
        public static Customer GetCustomerByID(int customerID)
        {
            string query = "SELECT * FROM Customers WHERE CustomerID = @CustomerID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@CustomerID", customerID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    return new Customer
                    {
                        CustomerID = Convert.ToInt32(row["CustomerID"]),
                        Name = row["Name"].ToString(),
                        ContactPerson = row["ContactPerson"] == DBNull.Value ? null : row["ContactPerson"].ToString(),
                        Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString(),
                        Phone = row["Phone"] == DBNull.Value ? null : row["Phone"].ToString(),
                        Address = row["Address"] == DBNull.Value ? null : row["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении клиента: " + ex.Message);
            }
        }

        // Добавить нового клиента
        public bool AddCustomer()
        {
            string query = @"
                INSERT INTO Customers (Name, ContactPerson, Email, Phone, Address)
                VALUES (@Name, @ContactPerson, @Email, @Phone, @Address)
                RETURNING CustomerID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@Name", Name),
                new NpgsqlParameter("@ContactPerson", (object)ContactPerson ?? DBNull.Value),
                new NpgsqlParameter("@Email", (object)Email ?? DBNull.Value),
                new NpgsqlParameter("@Phone", (object)Phone ?? DBNull.Value),
                new NpgsqlParameter("@Address", (object)Address ?? DBNull.Value)
            };

            try
            {
                var result = DatabaseHelper.ExecuteScalar(query, parameters);
                CustomerID = Convert.ToInt32(result);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении клиента: " + ex.Message);
            }
        }

        // Обновить информацию о клиенте
        public bool UpdateCustomer()
        {
            string query = @"
                UPDATE Customers
                SET Name = @Name,
                    ContactPerson = @ContactPerson,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address
                WHERE CustomerID = @CustomerID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@CustomerID", CustomerID),
                new NpgsqlParameter("@Name", Name),
                new NpgsqlParameter("@ContactPerson", (object)ContactPerson ?? DBNull.Value),
                new NpgsqlParameter("@Email", (object)Email ?? DBNull.Value),
                new NpgsqlParameter("@Phone", (object)Phone ?? DBNull.Value),
                new NpgsqlParameter("@Address", (object)Address ?? DBNull.Value)
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при обновлении клиента: " + ex.Message);
            }
        }

        // Удалить клиента
        public static bool DeleteCustomer(int customerID)
        {
            // Сначала проверяем, есть ли заказы у этого клиента
            string checkQuery = "SELECT COUNT(*) FROM Orders WHERE CustomerID = @CustomerID";

            NpgsqlParameter[] checkParameters = {
                new NpgsqlParameter("@CustomerID", customerID)
            };

            try
            {
                int orderCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParameters));
                if (orderCount > 0)
                {
                    throw new Exception("Невозможно удалить клиента, так как у него есть заказы");
                }

                // Если заказов нет, удаляем клиента
                string deleteQuery = "DELETE FROM Customers WHERE CustomerID = @CustomerID";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(deleteQuery, checkParameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удалении клиента: " + ex.Message);
            }
        }

        // Получить заказы клиента
        public List<Order> GetCustomerOrders()
        {
            List<Order> orders = new List<Order>();

            string query = @"
                SELECT o.*, u.Username AS UserName
                FROM Orders o
                LEFT JOIN Users u ON o.UserID = u.UserID
                WHERE o.CustomerID = @CustomerID
                ORDER BY o.OrderDate DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@CustomerID", CustomerID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    orders.Add(new Order
                    {
                        OrderID = Convert.ToInt32(row["OrderID"]),
                        CustomerID = Convert.ToInt32(row["CustomerID"]),
                        CustomerName = Name,
                        OrderNumber = row["OrderNumber"].ToString(),
                        OrderDate = Convert.ToDateTime(row["OrderDate"]),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                        Status = row["Status"].ToString(),
                        UserID = Convert.ToInt32(row["UserID"]),
                        UserName = row["UserName"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении заказов клиента: " + ex.Message);
            }

            return orders;
        }

        // Поиск клиентов
        public static List<Customer> SearchCustomers(string searchText)
        {
            List<Customer> customers = new List<Customer>();

            string query = @"
                SELECT * FROM Customers
                WHERE Name ILIKE @SearchText
                   OR ContactPerson ILIKE @SearchText
                   OR Email ILIKE @SearchText
                   OR Phone ILIKE @SearchText
                   OR Address ILIKE @SearchText
                ORDER BY Name";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@SearchText", $"%{searchText}%")
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    customers.Add(new Customer
                    {
                        CustomerID = Convert.ToInt32(row["CustomerID"]),
                        Name = row["Name"].ToString(),
                        ContactPerson = row["ContactPerson"] == DBNull.Value ? null : row["ContactPerson"].ToString(),
                        Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString(),
                        Phone = row["Phone"] == DBNull.Value ? null : row["Phone"].ToString(),
                        Address = row["Address"] == DBNull.Value ? null : row["Address"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при поиске клиентов: " + ex.Message);
            }

            return customers;
        }
    }
}