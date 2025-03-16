using System;
using System.Data;
using Npgsql;
using System.Collections.Generic;

namespace WarehouseManagement.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int MinimumQuantity { get; set; }
        public string ImagePath { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Информация о запасах
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity => Quantity - ReservedQuantity;
        public string Location { get; set; }

        // Загружаем все товары
        public static List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            string query = @"
                SELECT p.*, c.Name AS CategoryName, i.Quantity, i.ReservedQuantity, i.Location
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                LEFT JOIN Inventory i ON p.ProductID = i.ProductID
                ORDER BY p.Name";

            try
            {
                var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    products.Add(new Product
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        CategoryID = row["CategoryID"] == DBNull.Value ? 0 : Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"] == DBNull.Value ? null : row["CategoryName"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        MinimumQuantity = Convert.ToInt32(row["MinimumQuantity"]),
                        ImagePath = row["ImagePath"] == DBNull.Value ? null : row["ImagePath"].ToString(),
                        SerialNumber = row["SerialNumber"] == DBNull.Value ? null : row["SerialNumber"].ToString(),
                        ExpiryDate = row["ExpiryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ExpiryDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                        Quantity = row["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["Quantity"]),
                        ReservedQuantity = row["ReservedQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["ReservedQuantity"]),
                        Location = row["Location"] == DBNull.Value ? null : row["Location"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении списка товаров: " + ex.Message);
            }

            return products;
        }

        // Получаем товар по ID
        public static Product GetProductByID(int productID)
        {
            string query = @"
                SELECT p.*, c.Name AS CategoryName, i.Quantity, i.ReservedQuantity, i.Location
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                LEFT JOIN Inventory i ON p.ProductID = i.ProductID
                WHERE p.ProductID = @ProductID";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@ProductID", productID)
            };

            try
            {
                var dataTable = Utils.DatabaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    return new Product
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        CategoryID = row["CategoryID"] == DBNull.Value ? 0 : Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"] == DBNull.Value ? null : row["CategoryName"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        MinimumQuantity = Convert.ToInt32(row["MinimumQuantity"]),
                        ImagePath = row["ImagePath"] == DBNull.Value ? null : row["ImagePath"].ToString(),
                        SerialNumber = row["SerialNumber"] == DBNull.Value ? null : row["SerialNumber"].ToString(),
                        ExpiryDate = row["ExpiryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ExpiryDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                        Quantity = row["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["Quantity"]),
                        ReservedQuantity = row["ReservedQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["ReservedQuantity"]),
                        Location = row["Location"] == DBNull.Value ? null : row["Location"].ToString()
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении товара: " + ex.Message);
            }
        }

        // Добавляем новый товар
        public bool AddProduct()
        {
            using (NpgsqlConnection connection = Utils.DatabaseHelper.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string productQuery = @"
                            INSERT INTO Products (Name, Description, CategoryID, Price, MinimumQuantity, ImagePath, SerialNumber, ExpiryDate)
                            VALUES (@Name, @Description, @CategoryID, @Price, @MinimumQuantity, @ImagePath, @SerialNumber, @ExpiryDate)
                            RETURNING ProductID";

                        NpgsqlParameter[] productParameters = {
                            new NpgsqlParameter("@Name", Name),
                            new NpgsqlParameter("@Description", (object)Description ?? DBNull.Value),
                            new NpgsqlParameter("@CategoryID", (object)CategoryID == 0 ? DBNull.Value : CategoryID),
                            new NpgsqlParameter("@Price", Price),
                            new NpgsqlParameter("@MinimumQuantity", MinimumQuantity),
                            new NpgsqlParameter("@ImagePath", (object)ImagePath ?? DBNull.Value),
                            new NpgsqlParameter("@SerialNumber", (object)SerialNumber ?? DBNull.Value),
                            new NpgsqlParameter("@ExpiryDate", (object)ExpiryDate ?? DBNull.Value)
                        };

                        using (var command = new NpgsqlCommand(productQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(productParameters);
                            ProductID = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Добавляем запись в таблицу инвентаря
                        string inventoryQuery = @"
                            INSERT INTO Inventory (ProductID, Quantity, ReservedQuantity, Location)
                            VALUES (@ProductID, @Quantity, @ReservedQuantity, @Location)";

                        NpgsqlParameter[] inventoryParameters = {
                            new NpgsqlParameter("@ProductID", ProductID),
                            new NpgsqlParameter("@Quantity", Quantity),
                            new NpgsqlParameter("@ReservedQuantity", ReservedQuantity),
                            new NpgsqlParameter("@Location", (object)Location ?? DBNull.Value)
                        };

                        using (var command = new NpgsqlCommand(inventoryQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(inventoryParameters);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Обновляем существующий товар
        public bool UpdateProduct()
        {
            using (NpgsqlConnection connection = Utils.DatabaseHelper.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string productQuery = @"
                            UPDATE Products
                            SET Name = @Name,
                                Description = @Description,
                                CategoryID = @CategoryID,
                                Price = @Price,
                                MinimumQuantity = @MinimumQuantity,
                                ImagePath = @ImagePath,
                                SerialNumber = @SerialNumber,
                                ExpiryDate = @ExpiryDate,
                                UpdatedAt = CURRENT_TIMESTAMP
                            WHERE ProductID = @ProductID";

                        NpgsqlParameter[] productParameters = {
                            new NpgsqlParameter("@ProductID", ProductID),
                            new NpgsqlParameter("@Name", Name),
                            new NpgsqlParameter("@Description", (object)Description ?? DBNull.Value),
                            new NpgsqlParameter("@CategoryID", (object)CategoryID == 0 ? DBNull.Value : CategoryID),
                            new NpgsqlParameter("@Price", Price),
                            new NpgsqlParameter("@MinimumQuantity", MinimumQuantity),
                            new NpgsqlParameter("@ImagePath", (object)ImagePath ?? DBNull.Value),
                            new NpgsqlParameter("@SerialNumber", (object)SerialNumber ?? DBNull.Value),
                            new NpgsqlParameter("@ExpiryDate", (object)ExpiryDate ?? DBNull.Value)
                        };

                        using (var command = new NpgsqlCommand(productQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(productParameters);
                            command.ExecuteNonQuery();
                        }

                        // Обновляем запись в таблице инвентаря
                        string inventoryQuery = @"
                            UPDATE Inventory
                            SET Quantity = @Quantity,
                                ReservedQuantity = @ReservedQuantity,
                                Location = @Location,
                                UpdatedAt = CURRENT_TIMESTAMP
                            WHERE ProductID = @ProductID";

                        NpgsqlParameter[] inventoryParameters = {
                            new NpgsqlParameter("@ProductID", ProductID),
                            new NpgsqlParameter("@Quantity", Quantity),
                            new NpgsqlParameter("@ReservedQuantity", ReservedQuantity),
                            new NpgsqlParameter("@Location", (object)Location ?? DBNull.Value)
                        };

                        using (var command = new NpgsqlCommand(inventoryQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(inventoryParameters);
                            command.ExecuteNonQuery();
                        }

                        // Добавляем запись в лог транзакций
                        string logQuery = @"
                            INSERT INTO TransactionLog (TransactionType, ReferenceID, ProductID, QuantityBefore, QuantityAfter, UserID)
                            VALUES ('ProductUpdate', @ProductID, @ProductID, 0, @Quantity, @UserID)";

                        NpgsqlParameter[] logParameters = {
                            new NpgsqlParameter("@ProductID", ProductID),
                            new NpgsqlParameter("@Quantity", Quantity),
                            new NpgsqlParameter("@UserID", UserSession.UserID)
                        };

                        using (var command = new NpgsqlCommand(logQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(logParameters);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Удаляем товар
        public static bool DeleteProduct(int productID)
        {
            // Проверяем, используется ли товар в заказах или поставках
            string checkQuery = @"
                SELECT COUNT(*) FROM OrderDetails WHERE ProductID = @ProductID
                UNION ALL
                SELECT COUNT(*) FROM SupplyDetails WHERE ProductID = @ProductID";

            NpgsqlParameter[] checkParameters = {
                new NpgsqlParameter("@ProductID", productID)
            };

            try
            {
                var dataTable = Utils.DatabaseHelper.ExecuteQuery(checkQuery, checkParameters);

                // Если товар используется, возвращаем false
                if (Convert.ToInt32(dataTable.Rows[0][0]) > 0 || Convert.ToInt32(dataTable.Rows[1][0]) > 0)
                {
                    return false;
                }

                // Удаляем записи из таблицы инвентаря
                string inventoryQuery = "DELETE FROM Inventory WHERE ProductID = @ProductID";
                Utils.DatabaseHelper.ExecuteNonQuery(inventoryQuery, checkParameters);

                // Удаляем записи из таблицы уведомлений
                string notificationsQuery = "DELETE FROM Notifications WHERE ProductID = @ProductID";
                Utils.DatabaseHelper.ExecuteNonQuery(notificationsQuery, checkParameters);

                // Удаляем товар
                string deleteQuery = "DELETE FROM Products WHERE ProductID = @ProductID";
                Utils.DatabaseHelper.ExecuteNonQuery(deleteQuery, checkParameters);

                return true;
            }
            catch
            {
                throw;
            }
        }

        // Список всех категорий
        public static List<KeyValuePair<int, string>> GetAllCategories()
        {
            List<KeyValuePair<int, string>> categories = new List<KeyValuePair<int, string>>();

            string query = "SELECT CategoryID, Name FROM Categories ORDER BY Name";

            try
            {
                var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    categories.Add(new KeyValuePair<int, string>(
                        Convert.ToInt32(row["CategoryID"]),
                        row["Name"].ToString()
                    ));
                }
            }
            catch
            {
                throw;
            }

            return categories;
        }

        // Получить список товаров с низким запасом
        public static List<Product> GetLowStockProducts()
        {
            List<Product> products = new List<Product>();

            string query = @"
                SELECT p.*, c.Name AS CategoryName, i.Quantity, i.ReservedQuantity, i.Location
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                JOIN Inventory i ON p.ProductID = i.ProductID
                WHERE i.Quantity <= p.MinimumQuantity
                ORDER BY p.Name";

            try
            {
                var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    products.Add(new Product
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        CategoryID = row["CategoryID"] == DBNull.Value ? 0 : Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"] == DBNull.Value ? null : row["CategoryName"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        MinimumQuantity = Convert.ToInt32(row["MinimumQuantity"]),
                        ImagePath = row["ImagePath"] == DBNull.Value ? null : row["ImagePath"].ToString(),
                        SerialNumber = row["SerialNumber"] == DBNull.Value ? null : row["SerialNumber"].ToString(),
                        ExpiryDate = row["ExpiryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ExpiryDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        ReservedQuantity = Convert.ToInt32(row["ReservedQuantity"]),
                        Location = row["Location"] == DBNull.Value ? null : row["Location"].ToString()
                    });
                }
            }
            catch
            {
                throw;
            }

            return products;
        }

        // Получить популярные товары
        public static List<Product> GetPopularProducts(int limit = 10)
        {
            List<Product> products = new List<Product>();

            string query = @"
                SELECT p.*, c.Name AS CategoryName, i.Quantity, i.ReservedQuantity, i.Location, 
                       COALESCE(SUM(od.Quantity), 0) AS TotalOrdered
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                LEFT JOIN Inventory i ON p.ProductID = i.ProductID
                LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
                LEFT JOIN Orders o ON od.OrderID = o.OrderID
                WHERE o.OrderDate >= CURRENT_DATE - INTERVAL '3 months' OR o.OrderDate IS NULL
                GROUP BY p.ProductID, c.Name, i.Quantity, i.ReservedQuantity, i.Location
                ORDER BY TotalOrdered DESC, p.Name
                LIMIT @Limit";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@Limit", limit)
            };

            try
            {
                var dataTable = Utils.DatabaseHelper.ExecuteQuery(query, parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    products.Add(new Product
                    {
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                        CategoryID = row["CategoryID"] == DBNull.Value ? 0 : Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"] == DBNull.Value ? null : row["CategoryName"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        MinimumQuantity = Convert.ToInt32(row["MinimumQuantity"]),
                        ImagePath = row["ImagePath"] == DBNull.Value ? null : row["ImagePath"].ToString(),
                        SerialNumber = row["SerialNumber"] == DBNull.Value ? null : row["SerialNumber"].ToString(),
                        ExpiryDate = row["ExpiryDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ExpiryDate"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                        Quantity = row["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["Quantity"]),
                        ReservedQuantity = row["ReservedQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["ReservedQuantity"]),
                        Location = row["Location"] == DBNull.Value ? null : row["Location"].ToString()
                    });
                }
            }
            catch
            {
                throw;
            }

            return products;
        }
    }
}