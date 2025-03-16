using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Npgsql;
using WarehouseManagement.Models;

namespace WarehouseManagement.Utils
{
    public class PdfGenerator
    {
        // Генерация накладной для поставки
        public static bool GenerateSupplyInvoice(Supply supply, string filePath)
        {
            try
            {
                // Преобразуем детали поставки в DataTable для вывода
                DataTable detailsTable = new DataTable();
                detailsTable.Columns.Add("ProductName", typeof(string));
                detailsTable.Columns.Add("Quantity", typeof(int));
                detailsTable.Columns.Add("UnitPrice", typeof(decimal));

                foreach (var detail in supply.Details)
                {
                    detailsTable.Rows.Add(detail.ProductName, detail.Quantity, detail.UnitPrice);
                }

                return DataExportHelper.GenerateInvoicePDF(
                    detailsTable,
                    filePath,
                    "Supply",
                    supply.InvoiceNumber,
                    supply.SupplierName,
                    supply.SupplyDate,
                    supply.TotalAmount
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании накладной: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Генерация накладной для заказа
        public static bool GenerateOrderInvoice(Order order, string filePath)
        {
            try
            {
                // Преобразуем детали заказа в DataTable для вывода
                DataTable detailsTable = new DataTable();
                detailsTable.Columns.Add("ProductName", typeof(string));
                detailsTable.Columns.Add("Quantity", typeof(int));
                detailsTable.Columns.Add("UnitPrice", typeof(decimal));

                foreach (var detail in order.Details)
                {
                    detailsTable.Rows.Add(detail.ProductName, detail.Quantity, detail.UnitPrice);
                }

                return DataExportHelper.GenerateInvoicePDF(
                    detailsTable,
                    filePath,
                    "Order",
                    order.OrderNumber,
                    order.CustomerName,
                    order.OrderDate,
                    order.TotalAmount
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании накладной: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Генерация отчета по остаткам товаров
        public static bool GenerateInventoryReport(string filePath)
        {
            try
            {
                string query = @"
                    SELECT 
                        p.Name AS 'Наименование товара',
                        c.Name AS 'Категория',
                        i.Quantity AS 'Количество',
                        i.ReservedQuantity AS 'Зарезервировано',
                        (i.Quantity - i.ReservedQuantity) AS 'Доступно',
                        p.Price AS 'Цена',
                        (i.Quantity * p.Price) AS 'Общая стоимость',
                        i.Location AS 'Расположение',
                        CASE WHEN i.Quantity <= p.MinimumQuantity THEN 'Да' ELSE 'Нет' END AS 'Низкий запас'
                    FROM
                        Products p
                    JOIN
                        Inventory i ON p.ProductID = i.ProductID
                    LEFT JOIN
                        Categories c ON p.CategoryID = c.CategoryID
                    ORDER BY
                        c.Name, p.Name";

                var dataTable = DatabaseHelper.ExecuteQuery(query);

                return DataExportHelper.GeneratePDF(dataTable, filePath, "Отчет по остаткам товаров");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчета: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Генерация отчета по популярным товарам
        public static bool GeneratePopularProductsReport(string filePath, int topCount = 20)
        {
            try
            {
                string query = @"
                    SELECT 
                        p.Name AS 'Наименование товара',
                        c.Name AS 'Категория',
                        SUM(od.Quantity) AS 'Количество продаж',
                        COUNT(DISTINCT o.OrderID) AS 'Количество заказов',
                        p.Price AS 'Цена',
                        SUM(od.Quantity * od.UnitPrice) AS 'Общая сумма продаж',
                        i.Quantity AS 'Текущий остаток'
                    FROM
                        Products p
                    LEFT JOIN
                        OrderDetails od ON p.ProductID = od.ProductID
                    LEFT JOIN
                        Orders o ON od.OrderID = o.OrderID
                    LEFT JOIN
                        Categories c ON p.CategoryID = c.CategoryID
                    LEFT JOIN
                        Inventory i ON p.ProductID = i.ProductID
                    WHERE
                        o.OrderDate >= @StartDate OR o.OrderDate IS NULL
                    GROUP BY
                        p.ProductID, p.Name, c.Name, p.Price, i.Quantity
                    ORDER BY
                        SUM(od.Quantity) DESC NULLS LAST
                    LIMIT @Limit";

                NpgsqlParameter[] parameters = {
                    new NpgsqlParameter("@StartDate", DateTime.Now.AddMonths(-3)),
                    new NpgsqlParameter("@Limit", topCount)
                };

                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                return DataExportHelper.GeneratePDF(dataTable, filePath, "Отчет по популярным товарам");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчета: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Генерация отчета по заказам
        public static bool GenerateOrdersReport(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                string query = @"
                    SELECT 
                        o.OrderNumber AS 'Номер заказа',
                        o.OrderDate AS 'Дата заказа',
                        c.Name AS 'Клиент',
                        o.TotalAmount AS 'Сумма заказа',
                        o.Status AS 'Статус',
                        u.FullName AS 'Менеджер'
                    FROM
                        Orders o
                    JOIN
                        Customers c ON o.CustomerID = c.CustomerID
                    JOIN
                        Users u ON o.UserID = u.UserID
                    WHERE
                        o.OrderDate BETWEEN @StartDate AND @EndDate
                    ORDER BY
                        o.OrderDate DESC";

                NpgsqlParameter[] parameters = {
                    new NpgsqlParameter("@StartDate", startDate),
                    new NpgsqlParameter("@EndDate", endDate)
                };

                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                return DataExportHelper.GeneratePDF(dataTable, filePath,
                    $"Отчет по заказам с {startDate.ToShortDateString()} по {endDate.ToShortDateString()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчета: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Генерация отчета по поставкам
        public static bool GenerateSuppliesReport(string filePath, DateTime startDate, DateTime endDate)
        {
            try
            {
                string query = @"
                    SELECT 
                        s.InvoiceNumber AS 'Номер накладной',
                        s.SupplyDate AS 'Дата поставки',
                        sup.Name AS 'Поставщик',
                        s.TotalAmount AS 'Сумма поставки',
                        s.Status AS 'Статус',
                        u.FullName AS 'Ответственный'
                    FROM
                        Supplies s
                    JOIN
                        Suppliers sup ON s.SupplierID = sup.SupplierID
                    JOIN
                        Users u ON s.UserID = u.UserID
                    WHERE
                        s.SupplyDate BETWEEN @StartDate AND @EndDate
                    ORDER BY
                        s.SupplyDate DESC";

                NpgsqlParameter[] parameters = {
                    new NpgsqlParameter("@StartDate", startDate),
                    new NpgsqlParameter("@EndDate", endDate)
                };

                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                return DataExportHelper.GeneratePDF(dataTable, filePath,
                    $"Отчет по поставкам с {startDate.ToShortDateString()} по {endDate.ToShortDateString()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчета: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}