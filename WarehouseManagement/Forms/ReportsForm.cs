using Npgsql;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Forms
{
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            InitializeComponent();

            // Настройка дат по умолчанию
            dtpStartDate.Value = DateTime.Now.AddMonths(-1);
            dtpEndDate.Value = DateTime.Now;

            // Настройка прав доступа
            ConfigureUIBasedOnUserRole();
        }

        // Настройка интерфейса в зависимости от роли пользователя
        private void ConfigureUIBasedOnUserRole()
        {
            // Все пользователи могут просматривать отчеты, но только менеджеры и администраторы 
            // могут экспортировать данные
            bool canExport = UserSession.IsManager;
            btnExport.Enabled = canExport;
        }

        // Обработчик нажатия на кнопку "Просмотр"
        private void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем тип отчета
                string reportType = GetSelectedReportType();

                // Проверяем корректность ввода дат
                if (dtpStartDate.Value > dtpEndDate.Value)
                {
                    MessageBox.Show("Дата начала не может быть позже даты окончания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Загружаем данные в зависимости от типа отчета
                switch (reportType)
                {
                    case "inventory":
                        LoadInventoryReport();
                        break;
                    case "orders":
                        LoadOrdersReport();
                        break;
                    case "supplies":
                        LoadSuppliesReport();
                        break;
                    case "popular":
                        LoadPopularProductsReport();
                        break;
                    case "abc":
                        LoadABCAnalysisReport();
                        break;
                    default:
                        MessageBox.Show("Выберите тип отчета", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия на кнопку "Экспорт"
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, есть ли данные для экспорта
                if (dgvReport.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта. Сначала сформируйте отчет.",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Получаем тип отчета
                string reportType = GetSelectedReportType();
                string reportName = lblReportTitle.Text;

                // Показываем диалог сохранения файла
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "PDF файлы (*.pdf)|*.pdf|Excel файлы (*.xlsx)|*.xlsx|CSV файлы (*.csv)|*.csv";
                saveDialog.Title = "Экспорт отчета";
                saveDialog.FileName = reportType + "_" + DateTime.Now.ToString("yyyyMMdd");

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveDialog.FileName;
                    string fileExtension = Path.GetExtension(filePath).ToLower();

                    // Конвертируем DataGridView в DataTable
                    DataTable dataTable = GetDataTableFromDGV(dgvReport);

                    bool result = false;

                    switch (fileExtension)
                    {
                        case ".pdf":
                            result = DataExportHelper.GeneratePDF(dataTable, filePath, reportName);
                            break;
                        case ".xlsx":
                            result = DataExportHelper.ExportToExcel(dataTable, filePath);
                            break;
                        case ".csv":
                            result = DataExportHelper.ExportToCSV(dataTable, filePath);
                            break;
                    }

                    if (result)
                    {
                        MessageBox.Show("Отчет успешно экспортирован", "Экспорт",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте отчета: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Получение типа выбранного отчета
        private string GetSelectedReportType()
        {
            if (rbInventory.Checked) return "inventory";
            if (rbOrders.Checked) return "orders";
            if (rbSupplies.Checked) return "supplies";
            if (rbPopular.Checked) return "popular";
            if (rbABC.Checked) return "abc";

            return string.Empty;
        }

        // Загрузка отчета по остаткам товаров
        private void LoadInventoryReport()
        {
            string query = @"
                SELECT 
                    p.Name AS 'Наименование',
                    c.Name AS 'Категория',
                    i.Quantity AS 'Количество',
                    i.ReservedQuantity AS 'Резерв',
                    (i.Quantity - i.ReservedQuantity) AS 'Доступно',
                    p.Price AS 'Цена',
                    (i.Quantity * p.Price) AS 'Стоимость',
                    CASE WHEN i.Quantity <= p.MinimumQuantity THEN 'Да' ELSE 'Нет' END AS 'Низкий_запас',
                    i.Location AS 'Расположение'
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                JOIN Inventory i ON p.ProductID = i.ProductID
                ORDER BY c.Name, p.Name";

            var dataTable = DatabaseHelper.ExecuteQuery(query);

            dgvReport.DataSource = dataTable;
            lblReportTitle.Text = "Отчет по остаткам товаров на складе";
            SetupInventoryReportColumns();
        }

        // Загрузка отчета по заказам
        private void LoadOrdersReport()
        {
            string query = @"
                SELECT 
                    o.OrderNumber AS 'Номер',
                    o.OrderDate AS 'Дата',
                    c.Name AS 'Клиент',
                    o.TotalAmount AS 'Сумма',
                    o.Status AS 'Статус',
                    u.FullName AS 'Менеджер'
                FROM Orders o
                JOIN Customers c ON o.CustomerID = c.CustomerID
                JOIN Users u ON o.UserID = u.UserID
                WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
                ORDER BY o.OrderDate DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@StartDate", dtpStartDate.Value.Date),
                new NpgsqlParameter("@EndDate", dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

            dgvReport.DataSource = dataTable;
            lblReportTitle.Text = $"Отчет по заказам с {dtpStartDate.Value.ToShortDateString()} по {dtpEndDate.Value.ToShortDateString()}";
            SetupOrdersReportColumns();
        }

        // Загрузка отчета по поставкам
        private void LoadSuppliesReport()
        {
            string query = @"
                SELECT 
                    s.InvoiceNumber AS 'Номер',
                    s.SupplyDate AS 'Дата',
                    sup.Name AS 'Поставщик',
                    s.TotalAmount AS 'Сумма',
                    s.Status AS 'Статус',
                    u.FullName AS 'Принял'
                FROM Supplies s
                JOIN Suppliers sup ON s.SupplierID = sup.SupplierID
                JOIN Users u ON s.UserID = u.UserID
                WHERE s.SupplyDate BETWEEN @StartDate AND @EndDate
                ORDER BY s.SupplyDate DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@StartDate", dtpStartDate.Value.Date),
                new NpgsqlParameter("@EndDate", dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

            dgvReport.DataSource = dataTable;
            lblReportTitle.Text = $"Отчет по поставкам с {dtpStartDate.Value.ToShortDateString()} по {dtpEndDate.Value.ToShortDateString()}";
            SetupSuppliesReportColumns();
        }

        // Загрузка отчета по популярным товарам
        private void LoadPopularProductsReport()
        {
            string query = @"
                SELECT 
                    p.Name AS 'Наименование',
                    c.Name AS 'Категория',
                    COALESCE(SUM(od.Quantity), 0) AS 'Количество_продаж',
                    COALESCE(COUNT(DISTINCT o.OrderID), 0) AS 'Количество_заказов',
                    p.Price AS 'Цена',
                    COALESCE(SUM(od.Quantity * od.UnitPrice), 0) AS 'Сумма_продаж',
                    i.Quantity AS 'Остаток'
                FROM Products p
                LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
                LEFT JOIN Orders o ON od.OrderID = o.OrderID AND o.OrderDate BETWEEN @StartDate AND @EndDate
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                LEFT JOIN Inventory i ON p.ProductID = i.ProductID
                GROUP BY p.ProductID, p.Name, c.Name, p.Price, i.Quantity
                ORDER BY 'Количество_продаж' DESC NULLS LAST
                LIMIT 50";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@StartDate", dtpStartDate.Value.Date),
                new NpgsqlParameter("@EndDate", dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

            dgvReport.DataSource = dataTable;
            lblReportTitle.Text = $"Отчет по популярным товарам с {dtpStartDate.Value.ToShortDateString()} по {dtpEndDate.Value.ToShortDateString()}";
            SetupPopularProductsReportColumns();
        }

        // Загрузка отчета ABC-анализа
        private void LoadABCAnalysisReport()
        {
            string query = @"
                WITH ProductSales AS (
                    SELECT 
                        p.ProductID,
                        p.Name,
                        c.Name AS Category,
                        COALESCE(SUM(od.Quantity * od.UnitPrice), 0) AS TotalSales
                    FROM Products p
                    LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
                    LEFT JOIN Orders o ON od.OrderID = o.OrderID AND o.OrderDate BETWEEN @StartDate AND @EndDate
                    LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                    GROUP BY p.ProductID, p.Name, c.Name
                ),
                SalesRanked AS (
                    SELECT 
                        ProductID,
                        Name,
                        Category,
                        TotalSales,
                        100 * TotalSales / NULLIF(SUM(TotalSales) OVER(), 0) AS SalesPercentage,
                        SUM(100 * TotalSales / NULLIF(SUM(TotalSales) OVER(), 0)) OVER(ORDER BY TotalSales DESC) AS CumulativePercentage
                    FROM ProductSales
                    ORDER BY TotalSales DESC
                )
                SELECT 
                    Name AS 'Наименование',
                    Category AS 'Категория',
                    TotalSales AS 'Продажи',
                    SalesPercentage AS 'Процент',
                    CumulativePercentage AS 'Накопительный_процент',
                    CASE 
                        WHEN CumulativePercentage <= 80 THEN 'A'
                        WHEN CumulativePercentage <= 95 THEN 'B'
                        ELSE 'C'
                    END AS 'ABC_класс'
                FROM SalesRanked
                ORDER BY TotalSales DESC";

            NpgsqlParameter[] parameters = {
                new NpgsqlParameter("@StartDate", dtpStartDate.Value.Date),
                new NpgsqlParameter("@EndDate", dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1))
            };

            var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

            dgvReport.DataSource = dataTable;
            lblReportTitle.Text = $"ABC-анализ продаж с {dtpStartDate.Value.ToShortDateString()} по {dtpEndDate.Value.ToShortDateString()}";
            SetupABCReportColumns();
        }

        // Настройка столбцов для отчета по остаткам
        private void SetupInventoryReportColumns()
        {
            FormatColumns();

            if (dgvReport.Columns.Contains("Цена"))
                dgvReport.Columns["Цена"].DefaultCellStyle.Format = "N2";

            if (dgvReport.Columns.Contains("Стоимость"))
                dgvReport.Columns["Стоимость"].DefaultCellStyle.Format = "N2";

            // Выделение цветом товаров с низким запасом
            foreach (DataGridViewRow row in dgvReport.Rows)
            {
                if (row.Cells["Низкий_запас"].Value.ToString() == "Да")
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
                }
            }
        }

        // Настройка столбцов для отчета по заказам
        private void SetupOrdersReportColumns()
        {
            FormatColumns();

            if (dgvReport.Columns.Contains("Дата"))
                dgvReport.Columns["Дата"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            if (dgvReport.Columns.Contains("Сумма"))
                dgvReport.Columns["Сумма"].DefaultCellStyle.Format = "N2";
        }

        // Настройка столбцов для отчета по поставкам
        private void SetupSuppliesReportColumns()
        {
            FormatColumns();

            if (dgvReport.Columns.Contains("Дата"))
                dgvReport.Columns["Дата"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

            if (dgvReport.Columns.Contains("Сумма"))
                dgvReport.Columns["Сумма"].DefaultCellStyle.Format = "N2";
        }

        // Настройка столбцов для отчета по популярным товарам
        private void SetupPopularProductsReportColumns()
        {
            FormatColumns();

            if (dgvReport.Columns.Contains("Цена"))
                dgvReport.Columns["Цена"].DefaultCellStyle.Format = "N2";

            if (dgvReport.Columns.Contains("Сумма_продаж"))
                dgvReport.Columns["Сумма_продаж"].DefaultCellStyle.Format = "N2";
        }

        // Настройка столбцов для отчета ABC-анализа
        private void SetupABCReportColumns()
        {
            FormatColumns();

            if (dgvReport.Columns.Contains("Продажи"))
                dgvReport.Columns["Продажи"].DefaultCellStyle.Format = "N2";

            if (dgvReport.Columns.Contains("Процент"))
                dgvReport.Columns["Процент"].DefaultCellStyle.Format = "P2";

            if (dgvReport.Columns.Contains("Накопительный_процент"))
                dgvReport.Columns["Накопительный_процент"].DefaultCellStyle.Format = "P2";

            // Выделение цветом классов ABC
            foreach (DataGridViewRow row in dgvReport.Rows)
            {
                string abcClass = row.Cells["ABC_класс"].Value.ToString();

                switch (abcClass)
                {
                    case "A":
                        row.Cells["ABC_класс"].Style.BackColor = System.Drawing.Color.LightGreen;
                        break;
                    case "B":
                        row.Cells["ABC_класс"].Style.BackColor = System.Drawing.Color.LightYellow;
                        break;
                    case "C":
                        row.Cells["ABC_класс"].Style.BackColor = System.Drawing.Color.LightPink;
                        break;
                }
            }
        }

        // Общая настройка столбцов
        private void FormatColumns()
        {
            // Настраиваем автоматическую ширину столбцов
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Заменяем подчеркивания на пробелы в заголовках
            foreach (DataGridViewColumn column in dgvReport.Columns)
            {
                column.HeaderText = column.HeaderText.Replace('_', ' ');
            }
        }

        // Получение DataTable из DataGridView для экспорта
        private DataTable GetDataTableFromDGV(DataGridView dgv)
        {
            // Если DataGridView уже привязан к DataTable, просто возвращаем его
            if (dgv.DataSource is DataTable)
            {
                return (DataTable)dgv.DataSource;
            }

            // Иначе создаем новую DataTable на основе данных DataGridView
            DataTable dt = new DataTable();

            // Добавляем столбцы
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                dt.Columns.Add(column.HeaderText);
            }

            // Добавляем строки
            foreach (DataGridViewRow row in dgv.Rows)
            {
                DataRow dataRow = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value ?? DBNull.Value;
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        // Обработчик изменения типа отчета
        private void ReportTypeChanged(object sender, EventArgs e)
        {
            // Настраиваем видимость элементов управления в зависимости от типа отчета
            bool showDateRange = rbOrders.Checked || rbSupplies.Checked || rbPopular.Checked || rbABC.Checked;

            lblStartDate.Visible = showDateRange;
            dtpStartDate.Visible = showDateRange;
            lblEndDate.Visible = showDateRange;
            dtpEndDate.Visible = showDateRange;
        }

        // Обработчик нажатия на кнопку "Печать"
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, есть ли данные для печати
                if (dgvReport.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для печати. Сначала сформируйте отчет.",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Создаем PrintDialog
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    // Здесь можно добавить код для печати, 
                    // но это требует дополнительных библиотек и настроек
                    MessageBox.Show("Функционал печати находится в разработке",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати отчета: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}