using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Детали заказа
        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();

        // Получаем все заказы
        public static List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            string query = @"
                SELECT o.*, c.Name AS CustomerName, u.Username AS UserName
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                LEFT JOIN Users u ON o.UserID = u.UserID
                ORDER BY o.OrderDate DESC";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    orders.Add(new Order
                    {
                        OrderID = Convert.ToInt32(row["OrderID"]),
                        CustomerID = Convert.ToInt32(row["CustomerID"]),
                        CustomerName = row["CustomerName"].ToString(),
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
                throw new Exception("Ошибка при получении списка заказов: " + ex.Message);
            }

            return orders;
        }

        // Получаем заказ по ID
        public static Order GetOrderByID(int orderID)
        {
            Order order = null;

            string orderQuery = @"
                SELECT o.*, c.Name AS CustomerName, u.Username AS UserName
                FROM Orders o
                LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                LEFT JOIN Users u ON o.UserID = u.UserID
                WHERE o.OrderID = @OrderID";

            NpgsqlParameter[] orderParameters = {
                new NpgsqlParameter("@OrderID", orderID)
            };

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(orderQuery, orderParameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    order = new Order
                    {
                        OrderID = Convert.ToInt32(row["OrderID"]),
                        CustomerID = Convert.ToInt32(row["CustomerID"]),
                        CustomerName = row["CustomerName"].ToString(),
                        OrderNumber = row["OrderNumber"].ToString(),
                        OrderDate = Convert.ToDateTime(row["OrderDate"]),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                        Status = row["Status"].ToString(),
                        UserID = Convert.ToInt32(row["UserID"]),
                        UserName = row["UserName"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    };

                    // Получаем детали заказа
                    string detailsQuery = @"
                        SELECT od.*, p.Name AS ProductName, p.Price AS ProductPrice
                        FROM OrderDetails od
                        JOIN Products p ON od.ProductID = p.ProductID
                        WHERE od.OrderID = @OrderID";

                    var detailsTable = DatabaseHelper.ExecuteQuery(detailsQuery, orderParameters);

                    foreach (DataRow detailRow in detailsTable.Rows)
                    {
                        order.Details.Add(new OrderDetail
                        {
                            OrderDetailID = Convert.ToInt32(detailRow["OrderDetailID"]),
                            OrderID = Convert.ToInt32(detailRow["OrderID"]),
                            ProductID = Convert.ToInt32(detailRow["ProductID"]),
                            ProductName = detailRow["ProductName"].ToString(),
                            Quantity = Convert.ToInt32(detailRow["Quantity"]),
                            UnitPrice = Convert.ToDecimal(detailRow["UnitPrice"]),
                            SerialNumber = detailRow["SerialNumber"] == DBNull.Value ? null : detailRow["SerialNumber"].ToString()
                        });
                    }
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении заказа: " + ex.Message);
            }
        }

        // Добавляем новый заказ
        public bool AddOrder()
        {
            using (NpgsqlConnection connection = DatabaseHelper.CreateConnection())
            {
                connection.Open();
                using (var transaction = DatabaseHelper.BeginTransaction(connection))
                {
                    try
                    {
                        // Проверяем наличие достаточного количества товаров
                        foreach (var detail in Details)
                        {
                            string checkQuery = @"
                                SELECT Quantity, ReservedQuantity
                                FROM Inventory
                                WHERE ProductID = @ProductID";

                            NpgsqlParameter[] checkParameters = {
                                new NpgsqlParameter("@ProductID", detail.ProductID)
                            };

                            using (var command = new NpgsqlCommand(checkQuery, connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddRange(checkParameters);
                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        int available = Convert.ToInt32(reader["Quantity"]) - Convert.ToInt32(reader["ReservedQuantity"]);
                                        if (available < detail.Quantity)
                                        {
                                            throw new Exception($"Недостаточное количество товара {detail.ProductName} на складе. Доступно: {available}, требуется: {detail.Quantity}.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"Товар {detail.ProductName} не найден в инвентаре.");
                                    }
                                }
                            }
                        }

                        // Генерируем номер заказа, если он не указан
                        if (string.IsNullOrEmpty(OrderNumber))
                        {
                            OrderNumber = "ORD-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                        }

                        string orderQuery = @"
                            INSERT INTO Orders (CustomerID, OrderNumber, OrderDate, TotalAmount, Status, UserID)
                            VALUES (@CustomerID, @OrderNumber, @OrderDate, @TotalAmount, @Status, @UserID)
                            RETURNING OrderID";

                        NpgsqlParameter[] orderParameters = {
                            new NpgsqlParameter("@CustomerID", CustomerID),
                            new NpgsqlParameter("@OrderNumber", OrderNumber),
                            new NpgsqlParameter("@OrderDate", OrderDate),
                            new NpgsqlParameter("@TotalAmount", TotalAmount),
                            new NpgsqlParameter("@Status", Status),
                            new NpgsqlParameter("@UserID", UserSession.UserID)
                        };

                        using (var command = new NpgsqlCommand(orderQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(orderParameters);
                            OrderID = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // Добавляем детали заказа
                        foreach (var detail in Details)
                        {
                            detail.OrderID = OrderID;

                            string detailQuery = @"
                                INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice, SerialNumber)
                                VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @SerialNumber)";

                            NpgsqlParameter[] detailParameters = {
                                new NpgsqlParameter("@OrderID", detail.OrderID),
                                new NpgsqlParameter("@ProductID", detail.ProductID),
                                new NpgsqlParameter("@Quantity", detail.Quantity),
                                new NpgsqlParameter("@UnitPrice", detail.UnitPrice),
                                new NpgsqlParameter("@SerialNumber", (object)detail.SerialNumber ?? DBNull.Value)
                            };

                            using (var command = new NpgsqlCommand(detailQuery, connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddRange(detailParameters);
                                command.ExecuteNonQuery();
                            }

                            // Резервируем товар для заказа
                            string reserveQuery = @"
                                UPDATE Inventory
                                SET ReservedQuantity = ReservedQuantity + @Quantity,
                                    UpdatedAt = CURRENT_TIMESTAMP
                                WHERE ProductID = @ProductID";

                            NpgsqlParameter[] reserveParameters = {
                                new NpgsqlParameter("@ProductID", detail.ProductID),
                                new NpgsqlParameter("@Quantity", detail.Quantity)
                            };

                            using (var command = new NpgsqlCommand(reserveQuery, connection))
                            {
                                command.Transaction = transaction;
                                command.Parameters.AddRange(reserveParameters);
                                command.ExecuteNonQuery();
                            }

                            // Обновляем количество товара в инвентаре, если заказ отправлен или доставлен
                            if (Status == "Shipped" || Status == "Delivered")
                            {
                                string inventoryQuery = @"
                                    UPDATE Inventory
                                    SET Quantity = Quantity - @Quantity,
                                        UpdatedAt = CURRENT_TIMESTAMP
                                    WHERE ProductID = @ProductID";

                                using (var command = new NpgsqlCommand(inventoryQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddRange(reserveParameters);
                                    command.ExecuteNonQuery();
                                }

                                // Добавляем запись в лог транзакций
                                string logQuery = @"
                                    INSERT INTO TransactionLog (TransactionType, ReferenceID, ProductID, QuantityBefore, QuantityAfter, UserID)
                                    SELECT 'Order', @OrderID, @ProductID, Quantity + @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@OrderID", OrderID),
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

        // Обновляем статус заказа
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
                            UPDATE Orders
                            SET Status = @Status
                            WHERE OrderID = @OrderID";

                        NpgsqlParameter[] statusParameters = {
                            new NpgsqlParameter("@OrderID", OrderID),
                            new NpgsqlParameter("@Status", newStatus)
                        };

                        using (var command = new NpgsqlCommand(statusQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(statusParameters);
                            command.ExecuteNonQuery();
                        }

                        // Если статус изменен на "Отправлен" или "Доставлен", обновляем инвентарь
                        if ((newStatus == "Shipped" || newStatus == "Delivered") &&
                            (Status != "Shipped" && Status != "Delivered"))
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
                                    SELECT 'OrderStatusChange', @OrderID, @ProductID, Quantity + @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@OrderID", OrderID),
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
                        // Если статус изменен с "Отправлено" или "Доставлено" на другой статус
                        else if ((newStatus != "Shipped" && newStatus != "Delivered") &&
                                (Status == "Shipped" || Status == "Delivered"))
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
                                    SELECT 'OrderStatusChange', @OrderID, @ProductID, Quantity - @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@OrderID", OrderID),
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

                        // Если статус изменен на "Отменен", обновляем резервирование
                        if (newStatus == "Cancelled")
                        {
                            foreach (var detail in Details)
                            {
                                string reserveQuery = @"
                                    UPDATE Inventory
                                    SET ReservedQuantity = ReservedQuantity - @Quantity,
                                        UpdatedAt = CURRENT_TIMESTAMP
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] reserveParameters = {
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity)
                                };

                                using (var command = new NpgsqlCommand(reserveQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddRange(reserveParameters);
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

        // Удаляем заказ
        public static bool DeleteOrder(int orderID)
        {
            Order order = GetOrderByID(orderID);

            if (order == null)
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
                        // Если заказ был отправлен или доставлен, возвращаем товары в инвентарь
                        if (order.Status == "Shipped" || order.Status == "Delivered")
                        {
                            foreach (var detail in order.Details)
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
                                    SELECT 'OrderDelete', @OrderID, @ProductID, Quantity - @Quantity, Quantity, @UserID
                                    FROM Inventory
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] logParameters = {
                                    new NpgsqlParameter("@OrderID", orderID),
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

                        // Снимаем резервирование товаров
                        if (order.Status != "Cancelled")
                        {
                            foreach (var detail in order.Details)
                            {
                                string reserveQuery = @"
                                    UPDATE Inventory
                                    SET ReservedQuantity = ReservedQuantity - @Quantity,
                                        UpdatedAt = CURRENT_TIMESTAMP
                                    WHERE ProductID = @ProductID";

                                NpgsqlParameter[] reserveParameters = {
                                    new NpgsqlParameter("@ProductID", detail.ProductID),
                                    new NpgsqlParameter("@Quantity", detail.Quantity)
                                };

                                using (var command = new NpgsqlCommand(reserveQuery, connection))
                                {
                                    command.Transaction = transaction;
                                    command.Parameters.AddRange(reserveParameters);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        // Удаляем детали заказа
                        string detailsQuery = "DELETE FROM OrderDetails WHERE OrderID = @OrderID";

                        NpgsqlParameter[] detailsParameters = {
                            new NpgsqlParameter("@OrderID", orderID)
                        };

                        using (var command = new NpgsqlCommand(detailsQuery, connection))
                        {
                            command.Transaction = transaction;
                            command.Parameters.AddRange(detailsParameters);
                            command.ExecuteNonQuery();
                        }

                        // Удаляем заказ
                        string orderQuery = "DELETE FROM Orders WHERE OrderID = @OrderID";

                        using (var command = new NpgsqlCommand(orderQuery, connection))
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

        // Получаем список клиентов
        public static List<KeyValuePair<int, string>> GetAllCustomers()
        {
            List<KeyValuePair<int, string>> customers = new List<KeyValuePair<int, string>>();

            string query = "SELECT CustomerID, Name FROM Customers ORDER BY Name";

            try
            {
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    customers.Add(new KeyValuePair<int, string>(
                        Convert.ToInt32(row["CustomerID"]),
                        row["Name"].ToString()
                    ));
                }
            }
            catch
            {
                throw;
            }

            return customers;
        }
    }

    // Класс для хранения деталей заказа
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string SerialNumber { get; set; }

        // Вычисляемое свойство для общей стоимости позиции
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}