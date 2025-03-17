using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MaterialSkin;
using Npgsql;
using WarehouseManagement.Models;

namespace WarehouseManagement.Forms
{
    public partial class DashboardForm : Form
    {
        public DashboardForm()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                LoadInventorySummary();
                LoadSalesSummary();
                LoadPopularProducts();
                LoadInventoryValueChart();
                LoadMonthlySalesChart();
                LoadABCAnalysis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных для аналитической панели: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загрузка общей информации о запасах
        private void LoadInventorySummary()
        {
            string query = @"
                SELECT 
                    COUNT(*) AS TotalProducts,
                    SUM(i.Quantity) AS TotalItems,
                    SUM(i.Quantity * p.Price) AS TotalValue,
                    COUNT(CASE WHEN i.Quantity <= p.MinimumQuantity THEN 1 END) AS LowStockItems
                FROM Inventory i
                JOIN Products p ON i.ProductID = p.ProductID";

            var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

            if (dataTable.Rows.Count > 0)
            {
                int totalProducts = Convert.ToInt32(dataTable.Rows[0]["TotalProducts"]);
                int totalItems = Convert.ToInt32(dataTable.Rows[0]["TotalItems"]);
                decimal totalValue = Convert.ToDecimal(dataTable.Rows[0]["TotalValue"]);
                int lowStockItems = Convert.ToInt32(dataTable.Rows[0]["LowStockItems"]);

                lblTotalProducts.Text = totalProducts.ToString();
                lblTotalItems.Text = totalItems.ToString();
                lblInventoryValue.Text = totalValue.ToString("C");
                lblLowStockItems.Text = lowStockItems.ToString();

                // Устанавливаем красный цвет для предупреждения о низком запасе
                if (lowStockItems > 0)
                {
                    lblLowStockItems.ForeColor = Color.Red;
                    lblLowStockTitle.ForeColor = Color.Red;
                }
            }
        }

        // Загрузка общей информации о продажах
        private void LoadSalesSummary()
        {
            string query = @"
                SELECT 
                    COUNT(*) AS TotalOrders,
                    SUM(TotalAmount) AS TotalSales,
                    COUNT(CASE WHEN Status IN ('Pending', 'Processing') THEN 1 END) AS PendingOrders,
                    COUNT(CASE WHEN Status = 'Delivered' THEN 1 END) AS CompletedOrders
                FROM Orders
                WHERE OrderDate >= @StartDate";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@StartDate", DateTime.Now.AddMonths(-1))
            };

            var dataTable = Utils.DatabaseHelper.ExecuteQuery(query, parameters);

            if (dataTable.Rows.Count > 0)
            {
                int totalOrders = Convert.ToInt32(dataTable.Rows[0]["TotalOrders"]);
                decimal totalSales = dataTable.Rows[0]["TotalSales"] == DBNull.Value ? 0 : Convert.ToDecimal(dataTable.Rows[0]["TotalSales"]);
                int pendingOrders = Convert.ToInt32(dataTable.Rows[0]["PendingOrders"]);
                int completedOrders = Convert.ToInt32(dataTable.Rows[0]["CompletedOrders"]);

                lblTotalOrders.Text = totalOrders.ToString();
                lblTotalSales.Text = totalSales.ToString("C");
                lblPendingOrders.Text = pendingOrders.ToString();
                lblCompletedOrders.Text = completedOrders.ToString();
            }
        }

        // Загрузка популярных товаров
        private void LoadPopularProducts()
        {
            List<Product> popularProducts = Product.GetPopularProducts(5);
            dgvPopularProducts.DataSource = popularProducts;

            // Настраиваем отображение столбцов
            if (dgvPopularProducts.Columns.Contains("ProductID"))
                dgvPopularProducts.Columns["ProductID"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("CategoryID"))
                dgvPopularProducts.Columns["CategoryID"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("Description"))
                dgvPopularProducts.Columns["Description"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("MinimumQuantity"))
                dgvPopularProducts.Columns["MinimumQuantity"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("ImagePath"))
                dgvPopularProducts.Columns["ImagePath"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("SerialNumber"))
                dgvPopularProducts.Columns["SerialNumber"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("ExpiryDate"))
                dgvPopularProducts.Columns["ExpiryDate"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("CreatedAt"))
                dgvPopularProducts.Columns["CreatedAt"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("UpdatedAt"))
                dgvPopularProducts.Columns["UpdatedAt"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("Location"))
                dgvPopularProducts.Columns["Location"].Visible = false;
            if (dgvPopularProducts.Columns.Contains("ReservedQuantity"))
                dgvPopularProducts.Columns["ReservedQuantity"].Visible = false;

            if (dgvPopularProducts.Columns.Contains("Name"))
                dgvPopularProducts.Columns["Name"].HeaderText = "Наименование";
            if (dgvPopularProducts.Columns.Contains("CategoryName"))
                dgvPopularProducts.Columns["CategoryName"].HeaderText = "Категория";
            if (dgvPopularProducts.Columns.Contains("Price"))
                dgvPopularProducts.Columns["Price"].HeaderText = "Цена";
            if (dgvPopularProducts.Columns.Contains("Quantity"))
                dgvPopularProducts.Columns["Quantity"].HeaderText = "Количество";
            if (dgvPopularProducts.Columns.Contains("AvailableQuantity"))
                dgvPopularProducts.Columns["AvailableQuantity"].HeaderText = "Доступно";
        }

        // Загрузка графика стоимости запасов по категориям
        private void LoadInventoryValueChart()
        {
            string query = @"
                SELECT 
                    c.Name AS Category,
                    SUM(i.Quantity * p.Price) AS TotalValue
                FROM Inventory i
                JOIN Products p ON i.ProductID = p.ProductID
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                GROUP BY c.Name
                ORDER BY TotalValue DESC";

            var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

            chartInventoryValue.Series.Clear();
            chartInventoryValue.Titles.Clear();

            // Добавляем заголовок
            Title title = new Title();
            title.Text = "Стоимость запасов по категориям";
            title.Font = new Font("Arial", 12, FontStyle.Bold);
            chartInventoryValue.Titles.Add(title);

            // Настраиваем серию
            Series series = new Series();
            series.Name = "InventoryValue";
            series.ChartType = SeriesChartType.Pie;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0:C}";

            // Добавляем данные
            foreach (DataRow row in dataTable.Rows)
            {
                string category = row["Category"] == DBNull.Value ? "Без категории" : row["Category"].ToString();
                decimal value = Convert.ToDecimal(row["TotalValue"]);
                series.Points.AddXY(category, value);
            }

            chartInventoryValue.Series.Add(series);
            chartInventoryValue.Legends[0].Enabled = true;
            chartInventoryValue.Legends[0].Docking = Docking.Bottom;
        }

        // Загрузка графика продаж по месяцам
        private void LoadMonthlySalesChart()
        {
            string query = @"
                SELECT 
                    DATE_TRUNC('month', OrderDate) AS Month,
                    SUM(TotalAmount) AS Sales
                FROM Orders
                WHERE OrderDate >= @StartDate
                GROUP BY DATE_TRUNC('month', OrderDate)
                ORDER BY Month";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@StartDate", DateTime.Now.AddMonths(-6))
            };

            var dataTable = Utils.DatabaseHelper.ExecuteQuery(query, parameters);

            chartMonthlySales.Series.Clear();
            chartMonthlySales.Titles.Clear();

            // Добавляем заголовок
            Title title = new Title();
            title.Text = "Ежемесячные продажи";
            title.Font = new Font("Arial", 12, FontStyle.Bold);
            chartMonthlySales.Titles.Add(title);

            // Настраиваем серию
            Series series = new Series();
            series.Name = "MonthlySales";
            series.ChartType = SeriesChartType.Column;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0:C}";

            // Добавляем данные
            foreach (DataRow row in dataTable.Rows)
            {
                DateTime month = Convert.ToDateTime(row["Month"]);
                decimal sales = Convert.ToDecimal(row["Sales"]);
                series.Points.AddXY(month.ToString("MMM yyyy"), sales);
            }

            chartMonthlySales.Series.Add(series);
            chartMonthlySales.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartMonthlySales.ChartAreas[0].AxisY.LabelStyle.Format = "C";
        }

        // Загрузка анализа ABC (для классификации товаров по уровню спроса)
        private void LoadABCAnalysis()
        {
            // Отключаем событие форматирования, если оно было ранее присоединено
            dgvAbcAnalysis.CellFormatting -= AbcAnalysis_CellFormatting;

            string query = @"
                WITH ProductSales AS (
                    SELECT 
                        p.ProductID,
                        p.Name,
                        COALESCE(SUM(od.Quantity * od.UnitPrice), 0) AS TotalSales
                    FROM Products p
                    LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
                    LEFT JOIN Orders o ON od.OrderID = o.OrderID
                    WHERE o.OrderDate >= @StartDate OR o.OrderDate IS NULL
                    GROUP BY p.ProductID, p.Name
                ),
                TotalSum AS (
                    SELECT SUM(TotalSales) AS GrandTotal FROM ProductSales
                ),
                SalesWithPercentage AS (
                    SELECT 
                        p.ProductID,
                        p.Name,
                        p.TotalSales,
                        CASE 
                            WHEN t.GrandTotal = 0 OR t.GrandTotal IS NULL THEN 0
                            ELSE 100 * p.TotalSales / t.GrandTotal 
                        END AS SalesPercentage
                    FROM 
                        ProductSales p CROSS JOIN TotalSum t
                ),
                SalesRanked AS (
                    SELECT 
                        ProductID,
                        Name,
                        TotalSales,
                        SalesPercentage,
                        SUM(SalesPercentage) OVER(ORDER BY TotalSales DESC) AS CumulativePercentage
                    FROM 
                        SalesWithPercentage
                )
                SELECT 
                    ProductID,
                    Name,
                    TotalSales,
                    SalesPercentage,
                    CumulativePercentage,
                    CASE 
                        WHEN CumulativePercentage <= 80 THEN 'A'::text
                        WHEN CumulativePercentage <= 95 THEN 'B'::text
                        ELSE 'C'::text
                    END AS ABCClass
                FROM SalesRanked";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@StartDate", DateTime.Now.AddMonths(-3))
            };

            var dataTable = Utils.DatabaseHelper.ExecuteQuery(query, parameters);

            chartAbcAnalysis.Series.Clear();
            chartAbcAnalysis.Titles.Clear();

            // Добавляем заголовок
            Title title = new Title();
            title.Text = "ABC-анализ товаров";
            title.Font = new Font("Arial", 12, FontStyle.Bold);
            chartAbcAnalysis.Titles.Add(title);

            // Настраиваем серию
            Series series = new Series();
            series.Name = "ABCAnalysis";
            series.ChartType = SeriesChartType.Pie;
            series.IsValueShownAsLabel = true;
            series.LabelFormat = "{0:P1}";

            // Группируем данные по классам ABC 
            var abcGroups = dataTable.AsEnumerable()
                .Where(row => row["ABCClass"] != DBNull.Value)
                .GroupBy(row => row["ABCClass"].ToString())  // Явно преобразуем к строке
                .Select(g => new
                {
                    Class = g.Key,
                    Count = g.Count(),
                    SalesPercentage = g.Sum(row =>
                        row["SalesPercentage"] != DBNull.Value ?
                        Convert.ToDouble(row["SalesPercentage"]) : 0)
                })
                .OrderBy(g => g.Class)
                .ToList();

            // Добавляем данные и настраиваем цвета
            foreach (var group in abcGroups)
            {
                if (!string.IsNullOrEmpty(group.Class))
                {
                    var point = series.Points.AddXY($"Класс {group.Class} ({group.Count} тов.)", group.SalesPercentage / 100);

                    // Используем if-else вместо switch для более надежной обработки типов
                    if (group.Class == "A")
                        series.Points[point].Color = Color.Green;
                    else if (group.Class == "B")
                        series.Points[point].Color = Color.Orange;
                    else if (group.Class == "C")
                        series.Points[point].Color = Color.Red;
                }
            }

            chartAbcAnalysis.Series.Add(series);
            chartAbcAnalysis.Legends[0].Enabled = true;
            chartAbcAnalysis.Legends[0].Docking = Docking.Bottom;

            // Заполняем таблицу ABC-анализа
            dgvAbcAnalysis.DataSource = dataTable;

            // Настраиваем отображение столбцов
            if (dgvAbcAnalysis.Columns.Contains("ProductID"))
                dgvAbcAnalysis.Columns["ProductID"].Visible = false;

            if (dgvAbcAnalysis.Columns.Contains("CumulativePercentage"))
                dgvAbcAnalysis.Columns["CumulativePercentage"].Visible = false;

            if (dgvAbcAnalysis.Columns.Contains("Name"))
                dgvAbcAnalysis.Columns["Name"].HeaderText = "Наименование";

            if (dgvAbcAnalysis.Columns.Contains("TotalSales"))
            {
                dgvAbcAnalysis.Columns["TotalSales"].HeaderText = "Общие продажи";
                dgvAbcAnalysis.Columns["TotalSales"].DefaultCellStyle.Format = "C";
            }

            if (dgvAbcAnalysis.Columns.Contains("SalesPercentage"))
            {
                dgvAbcAnalysis.Columns["SalesPercentage"].HeaderText = "% от продаж";
                dgvAbcAnalysis.Columns["SalesPercentage"].DefaultCellStyle.Format = "P2";
            }

            if (dgvAbcAnalysis.Columns.Contains("ABCClass"))
                dgvAbcAnalysis.Columns["ABCClass"].HeaderText = "Класс ABC";

            // Подписываемся на событие форматирования ячеек
            dgvAbcAnalysis.CellFormatting += AbcAnalysis_CellFormatting;
        }

        // Обработчик события форматирования ячеек для ABC-анализа
        private void AbcAnalysis_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Применяем форматирование только к колонке ABCClass
            if (dgvAbcAnalysis.Columns.Count > e.ColumnIndex &&
                dgvAbcAnalysis.Columns[e.ColumnIndex].Name == "ABCClass" &&
                e.RowIndex >= 0 &&
                e.RowIndex < dgvAbcAnalysis.Rows.Count)
            {
                // Получаем значение ячейки
                object cellValue = dgvAbcAnalysis.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // Проверяем, что значение не null
                if (cellValue != null && cellValue != DBNull.Value)
                {
                    string abcClass = cellValue.ToString().Trim();

                    // Применяем цвет в зависимости от класса ABC
                    if (abcClass == "A")
                        e.CellStyle.BackColor = Color.LightGreen;
                    else if (abcClass == "B")
                        e.CellStyle.BackColor = Color.LightYellow;
                    else if (abcClass == "C")
                        e.CellStyle.BackColor = Color.LightPink;
                }
            }
        }

        // Обработчик кнопки обновления данных
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        // Обработчик вкладок для отложенной загрузки данных
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabAbcAnalysis)
            {
                LoadABCAnalysis();
            }
        }

        // Обработчик кнопки экспорта данных ABC-анализа
        private void btnExportAbc_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Excel файлы (*.xlsx)|*.xlsx|CSV файлы (*.csv)|*.csv";
            saveDialog.Title = "Экспорт ABC-анализа";
            saveDialog.FileName = "ABC_Analysis_" + DateTime.Now.ToString("yyyyMMdd");

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = ((DataTable)dgvAbcAnalysis.DataSource).Copy();

                bool result = false;

                if (saveDialog.FilterIndex == 1)
                {
                    // Экспорт в Excel
                    result = Utils.DataExportHelper.ExportToExcel(dataTable, saveDialog.FileName);
                }
                else
                {
                    // Экспорт в CSV
                    result = Utils.DataExportHelper.ExportToCSV(dataTable, saveDialog.FileName);
                }

                if (result)
                {
                    MessageBox.Show("Данные успешно экспортированы", "Экспорт",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Обработчик кнопки экспорта отчета по популярным товарам
        private void btnExportPopular_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PDF файлы (*.pdf)|*.pdf|Excel файлы (*.xlsx)|*.xlsx";
            saveDialog.Title = "Экспорт отчета по популярным товарам";
            saveDialog.FileName = "Popular_Products_" + DateTime.Now.ToString("yyyyMMdd");

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = ((DataTable)dgvPopularProducts.DataSource).Copy();

                bool result = false;

                if (saveDialog.FilterIndex == 1)
                {
                    // Экспорт в PDF
                    result = Utils.DataExportHelper.GeneratePDF(dataTable, saveDialog.FileName, "Отчет по популярным товарам");
                }
                else
                {
                    // Экспорт в Excel
                    result = Utils.DataExportHelper.ExportToExcel(dataTable, saveDialog.FileName);
                }

                if (result)
                {
                    MessageBox.Show("Отчет успешно экспортирован", "Экспорт",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}