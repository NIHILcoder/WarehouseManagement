using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }

        // Получить список всех поставщиков
        public static List<Supplier> GetAllSuppliers()
        {
            List<Supplier> suppliers = new List<Supplier>();

            string query = "SELECT * FROM Suppliers ORDER BY Name";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    suppliers.Add(new Supplier
                    {
                        SupplierID = Convert.ToInt32(row["SupplierID"]),
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
                throw new Exception("Ошибка при получении списка поставщиков: " + ex.Message);
            }

            return suppliers;
        }

        // Получить поставщика по ID
        public static Supplier GetSupplierByID(int supplierID)
        {
            string query = "SELECT * FROM Suppliers WHERE SupplierID = @SupplierID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@SupplierID", supplierID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    return new Supplier
                    {
                        SupplierID = Convert.ToInt32(row["SupplierID"]),
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
                throw new Exception("Ошибка при получении поставщика: " + ex.Message);
            }
        }

        // Добавить нового поставщика
        public bool AddSupplier()
        {
            string query = @"
                INSERT INTO Suppliers (Name, ContactPerson, Email, Phone, Address)
                VALUES (@Name, @ContactPerson, @Email, @Phone, @Address)
                RETURNING SupplierID";

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
                SupplierID = Convert.ToInt32(result);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении поставщика: " + ex.Message);
            }
        }

        // Обновить информацию о поставщике
        public bool UpdateSupplier()
        {
            string query = @"
                UPDATE Suppliers
                SET Name = @Name,
                    ContactPerson = @ContactPerson,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address
                WHERE SupplierID = @SupplierID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@SupplierID", SupplierID),
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
                throw new Exception("Ошибка при обновлении поставщика: " + ex.Message);
            }
        }

        // Удалить поставщика
        public static bool DeleteSupplier(int supplierID)
        {
            // Сначала проверяем, есть ли поставки от этого поставщика
            string checkQuery = "SELECT COUNT(*) FROM Supplies WHERE SupplierID = @SupplierID";

            NpgsqlParameter[] checkParameters = {
                new NpgsqlParameter("@SupplierID", supplierID)
            };

            try
            {
                int supplyCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParameters));
                if (supplyCount > 0)
                {
                    throw new Exception("Невозможно удалить поставщика, так как от него есть поставки");
                }

                // Если поставок нет, удаляем поставщика
                string deleteQuery = "DELETE FROM Suppliers WHERE SupplierID = @SupplierID";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(deleteQuery, checkParameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удалении поставщика: " + ex.Message);
            }
        }

        // Получить поставки от поставщика
        public List<Supply> GetSupplierSupplies()
        {
            List<Supply> supplies = new List<Supply>();

            string query = @"
                SELECT s.*, u.Username AS UserName
                FROM Supplies s
                LEFT JOIN Users u ON s.UserID = u.UserID
                WHERE s.SupplierID = @SupplierID
                ORDER BY s.SupplyDate DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@SupplierID", SupplierID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    supplies.Add(new Supply
                    {
                        SupplyID = Convert.ToInt32(row["SupplyID"]),
                        SupplierID = Convert.ToInt32(row["SupplierID"]),
                        SupplierName = Name,
                        InvoiceNumber = row["InvoiceNumber"] == DBNull.Value ? null : row["InvoiceNumber"].ToString(),
                        SupplyDate = Convert.ToDateTime(row["SupplyDate"]),
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
                throw new Exception("Ошибка при получении поставок от поставщика: " + ex.Message);
            }

            return supplies;
        }

        // Поиск поставщиков
        public static List<Supplier> SearchSuppliers(string searchText)
        {
            List<Supplier> suppliers = new List<Supplier>();

            string query = @"
                SELECT * FROM Suppliers
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
                    suppliers.Add(new Supplier
                    {
                        SupplierID = Convert.ToInt32(row["SupplierID"]),
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
                throw new Exception("Ошибка при поиске поставщиков: " + ex.Message);
            }

            return suppliers;
        }
    }
}