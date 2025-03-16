using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Models
{
    public class Supply
    {
        public int SupplyID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime SupplyDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Детали поставки
        public List<SupplyDetail> Details { get; set; } = new List<SupplyDetail>();

        // Получаем все поставки
        public static List<Supply> GetAllSupplies()
        {
            List<Supply> supplies = new List<Supply>();

            string query = @"
                SELECT s.*, sup.Name AS SupplierName, u.Username AS UserName
                FROM Supplies s
                LEFT JOIN Suppliers sup ON s.SupplierID = sup.SupplierID
                LEFT JOIN Users u ON s.UserID = u.UserID
                ORDER BY s.SupplyDate DESC";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    supplies.Add(new Supply
                    {
                        SupplyID = Convert.ToInt32(row["SupplyID"]),
                        SupplierID = Convert.ToInt32(row["SupplierID"]),
                        SupplierName = row["SupplierName"].ToString(),
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
                throw new Exception("Ошибка при получении списка поставок: " + ex.Message);
            }

            return supplies;
        }

        // Получаем поставку по ID
        public static Supply GetSupplyByID(int supplyID)
        {
            Supply supply = null;

            string supplyQuery = @"
                SELECT s.*, sup.Name AS SupplierName, u.Username AS UserName
                FROM Supplies s
                LEFT JOIN Suppliers sup ON s.SupplierID = sup.SupplierID
                LEFT JOIN Users u ON s.UserID = u.UserID
                WHERE s.SupplyID = @SupplyID";

            NpgsqlParameter[] supplyParameters = {
                new NpgsqlParameter("@SupplyID", supplyID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(supplyQuery, supplyParameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    supply = new Supply
                    {
                        SupplyID = Convert.ToInt32(row["SupplyID"]),
                        SupplierID = Convert.ToInt32(row["SupplierID"]),
                        SupplierName = row["SupplierName"].ToString(),
                        InvoiceNumber = row["InvoiceNumber"] == DBNull.Value ? null : row["InvoiceNumber"].ToString(),
                        SupplyDate = Convert.ToDateTime(row["SupplyDate"]),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                        Status = row["Status"].ToString(),
                        UserID = Convert.ToInt32(row["UserID"]),
                        UserName = row["UserName"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    };

                    // Получаем детали поставки
                    string detailsQuery = @"
                        SELECT sd.*, p.Name AS ProductName, p.Price AS ProductPrice
                        FROM SupplyDetails sd
                        JOIN Products p ON sd.ProductID = p.ProductID
                        WHERE sd.SupplyID = @SupplyID";

                    var detailsTable = DatabaseHelper.ExecuteQuery(detailsQuery, supplyParameters);

                    foreach (DataRow detailRow in detailsTable.Rows)
                    {
                        supply.Details.Add(new SupplyDetail
                        {
                            SupplyDetailID = Convert.ToInt32(detailRow["SupplyDetailID"]),
                            SupplyID = Convert.ToInt32(detailRow["SupplyID"]),
                            ProductID = Convert.ToInt32(detailRow["ProductID"]),
                            ProductName = detailRow["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(detailRow["Quantity"]),
                            UnitPrice = Convert.ToDecimal(detailRow["UnitPrice"]),
                            ExpirationDate = detailRow["ExpirationDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(detailRow["ExpirationDate"]),
                            SerialNumber = detailRow["SerialNumber"] == DBNull.Value ? null : detailRow["SerialNumber"].ToString(),
                            BatchNumber = detailRow["BatchNumber"] == DBNull.Value ? null : detailRow["BatchNumber"].ToString()
                        });
                    }
                }

                return supply;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении поставки: " + ex.Message);
            }
        }

        // Добавляем новую поставку
        public bool AddSupply()
        {
            using (NpgsqlConnection connection = DatabaseHelper.CreateConnection())
            {
                connection.Open();
                using (var transaction = DatabaseHelper.BeginTransaction(connection))
                {
                    try
                    {
                        string supplyQuery = @"
                            INSERT INTO Supplies (SupplierID, InvoiceNumber, SupplyDate, TotalAmount, Status, UserID)
                            VALUES (@SupplierID, @InvoiceNumber, @SupplyDate, @TotalAmount, @Status, @UserID)
                            RETURNING SupplyID";

                        NpgsqlParameter[] supplyParameters = {
                            new NpgsqlParameter("@SupplierID", SupplierID),
                            new NpgsqlParameter("@InvoiceNumber", (object)InvoiceNumber ?? DBNull.Value),
                            new NpgsqlParameter("@SupplyDate", SupplyDate),
                            new NpgsqlParameter("@TotalAmount", TotalAmount),
                            new NpgsqlParameter("@Status", Status),
                            new NpgsqlParameter("@UserID", UserSession.UserID)
                        };

                        using (var command = new NpgsqlCommand(supplyQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(supplyParameters);
                            SupplyID = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Добавляем детали поставки
                        foreach (var detail in Details)
                        {
                            detail.SupplyID = SupplyID;

                            string detailQuery = @"
                                INSERT INTO SupplyDetails (SupplyID, ProductID, Quantity, UnitPrice, ExpirationDate, SerialNumber, BatchNumber)
                                VALUES (@SupplyID, @ProductID, @Quantity, @UnitPrice, @ExpirationDate, @SerialNumber, @BatchNumber)";

                            NpgsqlParameter[] detailParameters = {
                                new NpgsqlParameter("@SupplyID", detail.SupplyID),
                                new NpgsqlParameter("@ProductID", detail.ProductID),
                                new NpgsqlParameter("@Quantity", detail.Quantity),
                                new NpgsqlParameter("@UnitPrice", detail.UnitPrice),
                                new NpgsqlParameter("@ExpirationDate", (object)detail.ExpirationDate ?? DBNull.Value),
                                new NpgsqlParameter("@SerialNumber", (object)detail.SerialNumber ?? DBNull.Value),
                                new NpgsqlParameter("@BatchNumber", (object)detail.BatchNumber ?? DBNull.Value)
                            };

                            using (var command = new NpgsqlCommand(detailQuery, connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddRange(detailParameters);
                                command.ExecuteNonQuery();
                            }

                            // Обновляем количество товара на складе
                            if (Status == "Received")
                            {
                                string inventoryQuery = @"
                                    UPDATE Inventory
                                    SET Quantity = Quantity + @Quantity,
                                        UpdatedAt = CURRENT_TIMESTAMP
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] inventoryParameters = {
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity)
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
                                    SELECT 'Supply', @SupplyID, @ProductID, Quantity - @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@SupplyID", SupplyID),
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity),
                                    new NpgsqlParameter("@UserID", UserSession.UserID)
                                };

                                using (var command = new NpgsqlCommand(logQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddRange(logParameters);
                                    command.ExecuteNonQuery();
                                }
                            }
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

        // Обновляем статус поставки
        public bool UpdateStatus(string newStatus)
        {
            using (NpgsqlConnection connection = DatabaseHelper.CreateConnection())
            {
                connection.Open();
                using (var transaction = DatabaseHelper.BeginTransaction(connection))
                {
                    try
                    {
                        string statusQuery = @"
                            UPDATE Supplies
                            SET Status = @Status
                            WHERE SupplyID = @SupplyID";

                        NpgsqlParameter[] statusParameters = {
                            new NpgsqlParameter("@SupplyID", SupplyID),
                            new NpgsqlParameter("@Status", newStatus)
                        };

                        using (var command = new NpgsqlCommand(statusQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(statusParameters);
                            command.ExecuteNonQuery();
                        }

                        // Если статус изменен на "Получено", обновляем инвентарь
                        if (newStatus == "Received" && Status != "Received")
                        {
                            foreach (var detail in Details)
                            {
                                string inventoryQuery = @"
                                    UPDATE Inventory
                                    SET Quantity = Quantity + @Quantity,
                                        UpdatedAt = CURRENT_TIMESTAMP
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] inventoryParameters = {
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity)
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
                                    SELECT 'SupplyStatusChange', @SupplyID, @ProductID, Quantity - @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@SupplyID", SupplyID),
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity),
                                    new NpgsqlParameter("@UserID", UserSession.UserID)
                                };

                                using (var command = new NpgsqlCommand(logQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddRange(logParameters);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        // Если статус изменен с "Получено" на другой, уменьшаем инвентарь
                        else if (newStatus != "Received" && Status == "Received")
                        {
                            foreach (var detail in Details)
                            {
                                string inventoryQuery = @"
                                    UPDATE Inventory
                                    SET Quantity = Quantity - @Quantity,
                                        UpdatedAt = CURRENT_TIMESTAMP
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] inventoryParameters = {
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity)
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
                                    SELECT 'SupplyStatusChange', @SupplyID, @ProductID, Quantity + @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@SupplyID", SupplyID),
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity),
                                    new NpgsqlParameter("@UserID", UserSession.UserID)
                                };

                                using (var command = new NpgsqlCommand(logQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddRange(logParameters);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        Status = newStatus;
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

        // Удаляем поставку
        public static bool DeleteSupply(int supplyID)
        {
            Supply supply = GetSupplyByID(supplyID);

            if (supply == null)
            {
                return false;
            }

            using (NpgsqlConnection connection = DatabaseHelper.CreateConnection())
            {
                connection.Open();
                using (var transaction = DatabaseHelper.BeginTransaction(connection))
                {
                    try
                    {
                        // Если поставка была получена, уменьшаем количество товаров
                        if (supply.Status == "Received")
                        {
                            foreach (var detail in supply.Details)
                            {
                                string inventoryQuery = @"
                                    UPDATE Inventory
                                    SET Quantity = Quantity - @Quantity,
                                        UpdatedAt = CURRENT_TIMESTAMP
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] inventoryParameters = {
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity)
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
                                    SELECT 'SupplyDelete', @SupplyID, @ProductID, Quantity + @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@SupplyID", supplyID),
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity),
                                    new NpgsqlParameter("@UserID", UserSession.UserID)
                                };

                                using (var command = new NpgsqlCommand(logQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddRange(logParameters);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        // Удаляем детали поставки
                        string detailsQuery = "DELETE FROM SupplyDetails WHERE SupplyID = @SupplyID";

                        NpgsqlParameter[] detailsParameters = {
                            new NpgsqlParameter("@SupplyID", supplyID)
                        };

                        using (var command = new NpgsqlCommand(detailsQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(detailsParameters);
                            command.ExecuteNonQuery();
                        }

                        // Удаляем поставку
                        string supplyQuery = "DELETE FROM Supplies WHERE SupplyID = @SupplyID";

                        using (var command = new NpgsqlCommand(supplyQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(detailsParameters);
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

        // Получаем список поставщиков
        public static List<KeyValuePair<int, string>> GetAllSuppliers()
        {
            List<KeyValuePair<int, string>> suppliers = new List<KeyValuePair<int, string>>();

            string query = "SELECT SupplierID, Name FROM Suppliers ORDER BY Name";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    suppliers.Add(new KeyValuePair<int, string>(
                        Convert.ToInt32(row["SupplierID"]),
                        row["Name"].ToString()
                    ));
                }
            }
            catch
            {
                throw;
            }

            return suppliers;
        }
    }

    // Класс для хранения деталей поставки
    public class SupplyDetail
    {
        public int SupplyDetailID { get; set; }
        public int SupplyID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string SerialNumber { get; set; }
        public string BatchNumber { get; set; }

        // Вычисляемое свойство для общей стоимости позиции
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}