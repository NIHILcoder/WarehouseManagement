using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Npgsql;
using WarehouseManagement.Models;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Forms
{
    public partial class WarehouseMapForm : Form
    {
        // Список зон склада
        private List<WarehouseZone> zones = new List<WarehouseZone>();

        // Список ячеек склада
        private List<WarehouseCell> cells = new List<WarehouseCell>();

        // Выбранная ячейка
        private WarehouseCell selectedCell = null;

        // Словарь для хранения панелей ячеек
        private Dictionary<int, Panel> cellPanels = new Dictionary<int, Panel>();

        public WarehouseMapForm()
        {
            InitializeComponent();

            // Загружаем карту склада
            LoadWarehouseMap();
        }

        // Загрузка карты склада
        private void LoadWarehouseMap()
        {
            try
            {
                // Загружаем зоны склада
                string zonesQuery = "SELECT * FROM WarehouseZones ORDER BY Name";
                var zonesTable = DatabaseHelper.ExecuteQuery(zonesQuery);

                zones.Clear();
                foreach (DataRow row in zonesTable.Rows)
                {
                    zones.Add(new WarehouseZone
                    {
                        ZoneID = Convert.ToInt32(row["ZoneID"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString()
                    });
                }

                // Заполняем выпадающий список зон
                cmbZones.DataSource = null;
                cmbZones.DisplayMember = "Name";
                cmbZones.ValueMember = "ZoneID";
                cmbZones.DataSource = new List<WarehouseZone>(zones);

                // Загружаем ячейки для первой зоны (если есть)
                if (zones.Count > 0)
                {
                    LoadCellsForZone(zones[0].ZoneID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке карты склада: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Загрузка ячеек для выбранной зоны
        private void LoadCellsForZone(int zoneID)
        {
            try
            {
                string cellsQuery = @"
                    SELECT c.*, p.Name AS ProductName, p.SerialNumber
                    FROM WarehouseCells c
                    LEFT JOIN Products p ON c.ProductID = p.ProductID
                    WHERE c.ZoneID = @ZoneID
                    ORDER BY c.Location";

                NpgsqlParameter[] parameters = {
                    new NpgsqlParameter("@ZoneID", zoneID)
                };

                var cellsTable = DatabaseHelper.ExecuteQuery(cellsQuery, parameters);

                cells.Clear();
                foreach (DataRow row in cellsTable.Rows)
                {
                    cells.Add(new WarehouseCell
                    {
                        CellID = Convert.ToInt32(row["CellID"]),
                        ZoneID = Convert.ToInt32(row["ZoneID"]),
                        Location = row["Location"].ToString(),
                        Capacity = Convert.ToInt32(row["Capacity"]),
                        UsedCapacity = Convert.ToInt32(row["UsedCapacity"]),
                        ProductID = row["ProductID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ProductID"]),
                        ProductName = row["ProductName"] == DBNull.Value ? null : row["ProductName"].ToString(),
                        SerialNumber = row["SerialNumber"] == DBNull.Value ? null : row["SerialNumber"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    });
                }

                // Отображаем ячейки на карте
                DisplayCellsOnMap();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ячеек: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Отображение ячеек на карте склада
        private void DisplayCellsOnMap()
        {
            // Очищаем предыдущие ячейки
            panelMap.Controls.Clear();
            cellPanels.Clear();

            // Проверяем, есть ли ячейки для отображения
            if (cells.Count == 0)
            {
                Label lblNoItems = new Label
                {
                    Text = "Нет ячеек для отображения в выбранной зоне",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                panelMap.Controls.Add(lblNoItems);
                return;
            }

            // Определяем максимальное количество ячеек в строке
            int maxCellsInRow = (int)Math.Ceiling(Math.Sqrt(cells.Count));
            if (maxCellsInRow < 5) maxCellsInRow = 5;

            // Вычисляем размеры ячейки
            int cellWidth = (panelMap.Width - 20) / maxCellsInRow;
            int cellHeight = cellWidth;

            // Создаем и размещаем панели ячеек
            int row = 0;
            int col = 0;

            foreach (var cell in cells)
            {
                // Создаем панель для ячейки
                Panel cellPanel = new Panel
                {
                    Width = cellWidth - 10,
                    Height = cellHeight - 10,
                    Location = new Point(col * cellWidth + 5, row * cellHeight + 5),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = cell,
                    Cursor = Cursors.Hand
                };

                // Определяем цвет ячейки в зависимости от заполненности
                if (!cell.IsActive)
                {
                    cellPanel.BackColor = Color.LightGray; // Неактивная ячейка
                }
                else if (cell.ProductID.HasValue)
                {
                    // Ячейка с товаром, определяем цвет в зависимости от заполненности
                    double fillPercentage = (double)cell.UsedCapacity / cell.Capacity * 100;

                    if (fillPercentage >= 90)
                    {
                        cellPanel.BackColor = Color.LightCoral; // Почти полная ячейка
                    }
                    else if (fillPercentage >= 50)
                    {
                        cellPanel.BackColor = Color.Khaki; // Средне заполненная ячейка
                    }
                    else
                    {
                        cellPanel.BackColor = Color.LightGreen; // Мало заполненная ячейка
                    }
                }
                else
                {
                    cellPanel.BackColor = Color.White; // Пустая ячейка
                }

                // Добавляем метку с номером ячейки
                Label lblLocation = new Label
                {
                    Text = cell.Location,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Height = 20,
                    Font = new Font(Font.FontFamily, 8, FontStyle.Bold)
                };
                cellPanel.Controls.Add(lblLocation);

                // Добавляем метку с информацией о товаре (если есть)
                if (cell.ProductID.HasValue)
                {
                    Label lblProductInfo = new Label
                    {
                        Text = $"{cell.ProductName}\n{cell.UsedCapacity} / {cell.Capacity}",
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        Font = new Font(Font.FontFamily, 8)
                    };
                    cellPanel.Controls.Add(lblProductInfo);
                }

                // Добавляем обработчик события клика
                cellPanel.Click += CellPanel_Click;

                // Добавляем панель на карту
                panelMap.Controls.Add(cellPanel);
                cellPanels.Add(cell.CellID, cellPanel);

                // Переходим к следующей позиции
                col++;
                if (col >= maxCellsInRow)
                {
                    col = 0;
                    row++;
                }
            }
        }

        // Обработчик клика на ячейке
        private void CellPanel_Click(object sender, EventArgs e)
        {
            Panel clickedPanel = (Panel)sender;
            selectedCell = (WarehouseCell)clickedPanel.Tag;

            // Помечаем выбранную ячейку
            foreach (var panel in cellPanels.Values)
            {
                panel.BorderStyle = BorderStyle.FixedSingle;
            }
            clickedPanel.BorderStyle = BorderStyle.Fixed3D;

            // Заполняем информацию о выбранной ячейке
            txtCellLocation.Text = selectedCell.Location;
            txtCellCapacity.Text = selectedCell.Capacity.ToString();
            txtCellUsed.Text = selectedCell.UsedCapacity.ToString();
            txtProductName.Text = selectedCell.ProductName ?? "-";

            // Включаем панель с информацией о ячейке
            panelCellInfo.Visible = true;
        }

        // Обработчик изменения выбранной зоны
        private void cmbZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbZones.SelectedItem != null)
            {
                WarehouseZone selectedZone = (WarehouseZone)cmbZones.SelectedItem;
                LoadCellsForZone(selectedZone.ZoneID);

                // Очищаем информацию о выбранной ячейке
                selectedCell = null;
                panelCellInfo.Visible = false;
            }
        }

        // Обработчик нажатия на кнопку поиска товара
        private void btnFindProduct_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("Введите название или серийный номер товара для поиска", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string searchQuery = @"
                    SELECT c.*, p.Name AS ProductName, p.SerialNumber
                    FROM WarehouseCells c
                    JOIN Products p ON c.ProductID = p.ProductID
                    WHERE p.Name ILIKE @SearchText OR p.SerialNumber ILIKE @SearchText
                    ORDER BY c.ZoneID, c.Location";

                NpgsqlParameter[] parameters = {
                    new NpgsqlParameter("@SearchText", $"%{txtSearch.Text}%")
                };

                var searchResults = DatabaseHelper.ExecuteQuery(searchQuery, parameters);

                if (searchResults.Rows.Count == 0)
                {
                    MessageBox.Show("Товары, соответствующие запросу, не найдены", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Если найден только один результат, переходим к этой ячейке
                if (searchResults.Rows.Count == 1)
                {
                    int cellID = Convert.ToInt32(searchResults.Rows[0]["CellID"]);
                    int zoneID = Convert.ToInt32(searchResults.Rows[0]["ZoneID"]);

                    // Выбираем зону
                    for (int i = 0; i < cmbZones.Items.Count; i++)
                    {
                        if (((WarehouseZone)cmbZones.Items[i]).ZoneID == zoneID)
                        {
                            cmbZones.SelectedIndex = i;
                            break;
                        }
                    }

                    // Находим ячейку и выбираем её
                    if (cellPanels.ContainsKey(cellID))
                    {
                        CellPanel_Click(cellPanels[cellID], EventArgs.Empty);
                        cellPanels[cellID].BackColor = Color.LightBlue; // Выделяем цветом для поиска
                    }
                }
                else
                {
                    // Если найдено несколько результатов, показываем форму выбора
                    MessageBox.Show($"Найдено {searchResults.Rows.Count} ячеек с товаром. " +
                        $"Пожалуйста, уточните поисковый запрос.", "Результаты поиска",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске товара: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия клавиши Enter в поле поиска
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFindProduct_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Обработчик кнопки обновления карты
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (cmbZones.SelectedItem != null)
            {
                WarehouseZone selectedZone = (WarehouseZone)cmbZones.SelectedItem;
                LoadCellsForZone(selectedZone.ZoneID);
            }
            else
            {
                LoadWarehouseMap();
            }
        }

        // Обработчик кнопки перемещения товара
        private void btnMoveProduct_Click(object sender, EventArgs e)
        {
            if (selectedCell == null || !selectedCell.ProductID.HasValue)
            {
                MessageBox.Show("Выберите ячейку с товаром для перемещения", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Здесь можно добавить форму для выбора новой ячейки и перемещения товара
            MessageBox.Show("Функционал перемещения товаров между ячейками находится в разработке",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Обработчик кнопки освобождения ячейки
        private void btnClearCell_Click(object sender, EventArgs e)
        {
            if (selectedCell == null || !selectedCell.ProductID.HasValue)
            {
                MessageBox.Show("Выберите ячейку с товаром для освобождения", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show($"Вы действительно хотите освободить ячейку {selectedCell.Location}?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string updateQuery = @"
                        UPDATE WarehouseCells
                        SET ProductID = NULL, UsedCapacity = 0
                        WHERE CellID = @CellID";

                    NpgsqlParameter[] parameters = {
                        new NpgsqlParameter("@CellID", selectedCell.CellID)
                    };

                    DatabaseHelper.ExecuteNonQuery(updateQuery, parameters);

                    MessageBox.Show("Ячейка успешно освобождена", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Обновляем отображение ячеек
                    if (cmbZones.SelectedItem != null)
                    {
                        WarehouseZone selectedZone = (WarehouseZone)cmbZones.SelectedItem;
                        LoadCellsForZone(selectedZone.ZoneID);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при освобождении ячейки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик кнопки отображения всех ячеек с низким запасом
        private void btnShowLowStock_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
                    SELECT c.*, p.Name AS ProductName, p.SerialNumber,
                           p.MinimumQuantity, i.Quantity
                    FROM WarehouseCells c
                    JOIN Products p ON c.ProductID = p.ProductID
                    JOIN Inventory i ON p.ProductID = i.ProductID
                    WHERE i.Quantity <= p.MinimumQuantity
                    ORDER BY c.ZoneID, c.Location";

                var lowStockResults = DatabaseHelper.ExecuteQuery(query);

                if (lowStockResults.Rows.Count == 0)
                {
                    MessageBox.Show("Ячеек с товарами с низким запасом не найдено", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Создаем список ячеек для выделения
                List<int> lowStockCellIds = new List<int>();
                foreach (DataRow row in lowStockResults.Rows)
                {
                    lowStockCellIds.Add(Convert.ToInt32(row["CellID"]));
                }

                // Информация о найденных ячейках
                MessageBox.Show($"Найдено {lowStockResults.Rows.Count} ячеек с товарами с низким запасом",
                    "Результаты поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Выбираем зону первой найденной ячейки и выделяем все ячейки с низким запасом в этой зоне
                if (lowStockResults.Rows.Count > 0)
                {
                    int zoneID = Convert.ToInt32(lowStockResults.Rows[0]["ZoneID"]);

                    // Выбираем зону
                    for (int i = 0; i < cmbZones.Items.Count; i++)
                    {
                        if (((WarehouseZone)cmbZones.Items[i]).ZoneID == zoneID)
                        {
                            cmbZones.SelectedIndex = i;
                            break;
                        }
                    }

                    // Выделяем ячейки с низким запасом
                    foreach (int cellId in lowStockCellIds)
                    {
                        if (cellPanels.ContainsKey(cellId) && cells.FirstOrDefault(c => c.CellID == cellId)?.ZoneID == zoneID)
                        {
                            cellPanels[cellId].BackColor = Color.Red;
                            cellPanels[cellId].ForeColor = Color.White;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске ячеек с низким запасом: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки распределения товаров
        private void btnAutoDistribution_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Запустить алгоритм автоматического распределения товаров по зонам?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // В полной реализации здесь бы был алгоритм, который анализирует товары
                    // и распределяет их по зонам на основе оборачиваемости и других параметров

                    // Для демонстрации показываем сообщение
                    MessageBox.Show("Функционал автоматического распределения товаров находится в разработке\n\n" +
                        "Алгоритм будет учитывать:\n" +
                        "- Оборачиваемость товаров (ABC-анализ)\n" +
                        "- Совместимость товаров\n" +
                        "- Оптимизацию маршрутов комплектации\n" +
                        "- Специальные условия хранения",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выполнении автоматического распределения: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    // Класс для представления зоны склада
    public class WarehouseZone
    {
        public int ZoneID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // Класс для представления ячейки склада
    public class WarehouseCell
    {
        public int CellID { get; set; }
        public int ZoneID { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int UsedCapacity { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string SerialNumber { get; set; }
        public bool IsActive { get; set; }
    }
}