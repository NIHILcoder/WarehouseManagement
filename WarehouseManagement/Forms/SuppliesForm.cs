using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using WarehouseManagement.Models;

namespace WarehouseManagement.Forms
{
    public partial class SuppliesForm : Form
    {
        private List<Supply> supplies = new List<Supply>();
        private int selectedSupplyId = 0;
        private List<SupplyDetail> currentSupplyDetails = new List<SupplyDetail>();
        private decimal totalAmount = 0;

        public SuppliesForm()
        {
            InitializeComponent();
            LoadSupplies();
            LoadSuppliers();
            ConfigureUIBasedOnUserRole();
            SetupDetailsGrid();
        }

        // Настройка интерфейса в зависимости от роли пользователя
        private void ConfigureUIBasedOnUserRole()
        {
            // Разрешаем редактирование только администраторам и менеджерам
            bool canEdit = UserSession.IsManager;
            btnAdd.Enabled = canEdit;
            btnEdit.Enabled = canEdit;
            btnDelete.Enabled = canEdit;

            // Устанавливаем поля формы редактирования недоступными для изменения
            gbSupplyDetails.Enabled = canEdit;
            panelSupplyItems.Enabled = canEdit;
        }

        // Загрузка списка поставок
        private void LoadSupplies()
        {
            try
            {
                supplies = Supply.GetAllSupplies();
                BindSuppliesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке поставок: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Привязка списка поставок к DataGridView
        private void BindSuppliesToGrid()
        {
            dgvSupplies.DataSource = null;
            dgvSupplies.DataSource = supplies;

            // Настраиваем отображение столбцов
            if (dgvSupplies.Columns.Contains("SupplyID"))
                dgvSupplies.Columns["SupplyID"].HeaderText = "ID";
            if (dgvSupplies.Columns.Contains("SupplierName"))
                dgvSupplies.Columns["SupplierName"].HeaderText = "Поставщик";
            if (dgvSupplies.Columns.Contains("InvoiceNumber"))
                dgvSupplies.Columns["InvoiceNumber"].HeaderText = "Номер накладной";
            if (dgvSupplies.Columns.Contains("SupplyDate"))
            {
                dgvSupplies.Columns["SupplyDate"].HeaderText = "Дата поставки";
                dgvSupplies.Columns["SupplyDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            }
            if (dgvSupplies.Columns.Contains("TotalAmount"))
            {
                dgvSupplies.Columns["TotalAmount"].HeaderText = "Общая сумма";
                dgvSupplies.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
            }
            if (dgvSupplies.Columns.Contains("Status"))
                dgvSupplies.Columns["Status"].HeaderText = "Статус";
            if (dgvSupplies.Columns.Contains("UserName"))
                dgvSupplies.Columns["UserName"].HeaderText = "Пользователь";

            // Скрываем ненужные столбцы
            if (dgvSupplies.Columns.Contains("SupplierID"))
                dgvSupplies.Columns["SupplierID"].Visible = false;
            if (dgvSupplies.Columns.Contains("UserID"))
                dgvSupplies.Columns["UserID"].Visible = false;
            if (dgvSupplies.Columns.Contains("CreatedAt"))
                dgvSupplies.Columns["CreatedAt"].Visible = false;
            if (dgvSupplies.Columns.Contains("Details"))
                dgvSupplies.Columns["Details"].Visible = false;
        }

        // Загрузка списка поставщиков
        private void LoadSuppliers()
        {
            try
            {
                var suppliers = Supply.GetAllSuppliers();
                cmbSupplier.DisplayMember = "Value";
                cmbSupplier.ValueMember = "Key";
                cmbSupplier.DataSource = suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке поставщиков: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Настройка таблицы деталей поставки
        private void SetupDetailsGrid()
        {
            // Создаем столбцы для деталей поставки
            dgvSupplyDetails.Columns.Clear();

            DataGridViewComboBoxColumn productColumn = new DataGridViewComboBoxColumn();
            productColumn.HeaderText = "Товар";
            productColumn.Name = "ProductID";
            productColumn.DisplayMember = "Value";
            productColumn.ValueMember = "Key";

            // Заполняем список товаров
            try
            {
                var products = GetProducts();
                productColumn.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.HeaderText = "Количество";
            quantityColumn.Name = "Quantity";
            quantityColumn.DefaultCellStyle.Format = "N0";

            DataGridViewTextBoxColumn priceColumn = new DataGridViewTextBoxColumn();
            priceColumn.HeaderText = "Цена";
            priceColumn.Name = "UnitPrice";
            priceColumn.DefaultCellStyle.Format = "C2";

            DataGridViewTextBoxColumn totalColumn = new DataGridViewTextBoxColumn();
            totalColumn.HeaderText = "Всего";
            totalColumn.Name = "TotalPrice";
            totalColumn.ReadOnly = true;
            totalColumn.DefaultCellStyle.Format = "C2";

            DataGridViewTextBoxColumn batchColumn = new DataGridViewTextBoxColumn();
            batchColumn.HeaderText = "Номер партии";
            batchColumn.Name = "BatchNumber";

            DataGridViewTextBoxColumn serialColumn = new DataGridViewTextBoxColumn();
            serialColumn.HeaderText = "Серийный номер";
            serialColumn.Name = "SerialNumber";

            DataGridViewTextBoxColumn expiryColumn = new DataGridViewTextBoxColumn();
            expiryColumn.HeaderText = "Срок годности";
            expiryColumn.Name = "ExpirationDate";
            expiryColumn.DefaultCellStyle.Format = "dd.MM.yyyy";

            dgvSupplyDetails.Columns.AddRange(new DataGridViewColumn[] {
                productColumn, quantityColumn, priceColumn, totalColumn, batchColumn, serialColumn, expiryColumn
            });

            // Добавляем возможность удаления строк
            dgvSupplyDetails.AllowUserToDeleteRows = true;
        }

        // Получение списка товаров для выбора
        private List<KeyValuePair<int, string>> GetProducts()
        {
            List<KeyValuePair<int, string>> products = new List<KeyValuePair<int, string>>();

            // Загружаем список товаров из базы данных
            string query = "SELECT ProductID, Name FROM Products ORDER BY Name";
            var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                products.Add(new KeyValuePair<int, string>(
                    Convert.ToInt32(row["ProductID"]),
                    row["Name"].ToString()
                ));
            }

            return products;
        }

        // Обработчик выбора поставки в таблице
        private void dgvSupplies_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSupplies.SelectedRows.Count > 0)
            {
                selectedSupplyId = Convert.ToInt32(dgvSupplies.SelectedRows[0].Cells["SupplyID"].Value);
                // Включаем кнопки редактирования и удаления если у пользователя есть права
                btnEdit.Enabled = UserSession.IsManager;
                btnDelete.Enabled = UserSession.IsManager;
            }
            else
            {
                selectedSupplyId = 0;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        // Обработчик нажатия кнопки добавления поставки
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Очищаем форму и готовим для создания новой поставки
            ClearForm();
            selectedSupplyId = 0;
            gbSupplyDetails.Enabled = true;
            panelSupplyItems.Enabled = true;
            btnSave.Text = "Добавить";
            tabControl1.SelectedTab = tabDetails;

            // Устанавливаем значения по умолчанию
            dtpSupplyDate.Value = DateTime.Now;
            cmbStatus.SelectedIndex = 0; // Pending
        }

        // Обработчик нажатия кнопки редактирования поставки
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedSupplyId > 0)
            {
                try
                {
                    // Загружаем детали поставки
                    Supply supply = Supply.GetSupplyByID(selectedSupplyId);
                    if (supply != null)
                    {
                        // Заполняем форму данными
                        for (int i = 0; i < cmbSupplier.Items.Count; i++)
                        {
                            KeyValuePair<int, string> supplier = (KeyValuePair<int, string>)cmbSupplier.Items[i];
                            if (supplier.Key == supply.SupplierID)
                            {
                                cmbSupplier.SelectedIndex = i;
                                break;
                            }
                        }

                        txtInvoiceNumber.Text = supply.InvoiceNumber ?? string.Empty;
                        txtTotalAmount.Text = supply.TotalAmount.ToString("C2");
                        totalAmount = supply.TotalAmount;

                        // Устанавливаем статус
                        for (int i = 0; i < cmbStatus.Items.Count; i++)
                        {
                            if (cmbStatus.Items[i].ToString() == supply.Status)
                            {
                                cmbStatus.SelectedIndex = i;
                                break;
                            }
                        }

                        dtpSupplyDate.Value = supply.SupplyDate;

                        // Загружаем детали поставки
                        currentSupplyDetails = supply.Details;
                        UpdateSupplyDetailsGrid();

                        // Активируем редактирование
                        gbSupplyDetails.Enabled = true;
                        panelSupplyItems.Enabled = true;
                        btnSave.Text = "Сохранить";
                        tabControl1.SelectedTab = tabDetails;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных поставки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите поставку для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обновление таблицы деталей поставки
        private void UpdateSupplyDetailsGrid()
        {
            dgvSupplyDetails.Rows.Clear();

            foreach (var detail in currentSupplyDetails)
            {
                int rowIndex = dgvSupplyDetails.Rows.Add();
                DataGridViewRow row = dgvSupplyDetails.Rows[rowIndex];

                row.Cells["ProductID"].Value = detail.ProductID;
                row.Cells["Quantity"].Value = detail.Quantity;
                row.Cells["UnitPrice"].Value = detail.UnitPrice;
                row.Cells["TotalPrice"].Value = detail.TotalPrice;
                row.Cells["BatchNumber"].Value = detail.BatchNumber ?? string.Empty;
                row.Cells["SerialNumber"].Value = detail.SerialNumber ?? string.Empty;
                row.Cells["ExpirationDate"].Value = detail.ExpirationDate;
            }

            // Обновляем общую сумму
            CalculateTotalAmount();
        }

        // Обработчик нажатия кнопки удаления поставки
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedSupplyId > 0)
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранную поставку?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = Supply.DeleteSupply(selectedSupplyId);

                        if (success)
                        {
                            MessageBox.Show("Поставка успешно удалена", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadSupplies();
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении поставки: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите поставку для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки сохранения поставки
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbSupplier.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите поставщика", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dgvSupplyDetails.Rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в поставку", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Собираем данные о поставке
                Supply supply = new Supply
                {
                    SupplyID = selectedSupplyId,
                    SupplierID = ((KeyValuePair<int, string>)cmbSupplier.SelectedItem).Key,
                    InvoiceNumber = string.IsNullOrWhiteSpace(txtInvoiceNumber.Text) ? null : txtInvoiceNumber.Text,
                    SupplyDate = dtpSupplyDate.Value,
                    TotalAmount = totalAmount,
                    Status = cmbStatus.SelectedItem.ToString(),
                    UserID = UserSession.UserID,
                    Details = new List<SupplyDetail>()
                };

                // Собираем детали поставки
                foreach (DataGridViewRow row in dgvSupplyDetails.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (row.Cells["ProductID"].Value == null)
                    {
                        MessageBox.Show($"Выберите товар в строке {row.Index + 1}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int productId = (int)row.Cells["ProductID"].Value;
                    int quantity;
                    decimal unitPrice;

                    if (!int.TryParse(row.Cells["Quantity"].Value?.ToString(), out quantity) || quantity <= 0)
                    {
                        MessageBox.Show($"Введите корректное количество в строке {row.Index + 1}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(row.Cells["UnitPrice"].Value?.ToString(), out unitPrice) || unitPrice <= 0)
                    {
                        MessageBox.Show($"Введите корректную цену в строке {row.Index + 1}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DateTime? expiryDate = null;
                    if (row.Cells["ExpirationDate"].Value != null && row.Cells["ExpirationDate"].Value != DBNull.Value)
                    {
                        if (DateTime.TryParse(row.Cells["ExpirationDate"].Value.ToString(), out DateTime date))
                        {
                            expiryDate = date;
                        }
                    }

                    supply.Details.Add(new SupplyDetail
                    {
                        ProductID = productId,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        BatchNumber = row.Cells["BatchNumber"].Value?.ToString(),
                        SerialNumber = row.Cells["SerialNumber"].Value?.ToString(),
                        ExpirationDate = expiryDate
                    });
                }

                bool success;
                if (selectedSupplyId == 0)
                {
                    // Добавляем новую поставку
                    success = supply.AddSupply();
                    if (success)
                    {
                        MessageBox.Show("Поставка успешно добавлена", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Обновляем статус существующей поставки
                    success = supply.UpdateStatus(supply.Status);
                    if (success)
                    {
                        MessageBox.Show("Статус поставки успешно обновлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadSupplies();
                    ClearForm();
                    gbSupplyDetails.Enabled = false;
                    panelSupplyItems.Enabled = false;
                    tabControl1.SelectedTab = tabList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении поставки: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия кнопки отмены
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            gbSupplyDetails.Enabled = false;
            panelSupplyItems.Enabled = false;
            tabControl1.SelectedTab = tabList;
        }

        // Очистка формы
        private void ClearForm()
        {
            if (cmbSupplier.Items.Count > 0)
                cmbSupplier.SelectedIndex = 0;
            txtInvoiceNumber.Text = string.Empty;
            txtTotalAmount.Text = "0";
            totalAmount = 0;
            if (cmbStatus.Items.Count > 0)
                cmbStatus.SelectedIndex = 0;
            dtpSupplyDate.Value = DateTime.Now;

            // Очищаем таблицу деталей
            dgvSupplyDetails.Rows.Clear();
            currentSupplyDetails.Clear();
        }

        // Обработчик нажатия кнопки обновления списка поставок
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadSupplies();
        }

        // Обработчик кнопки поиска
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadSupplies();
                return;
            }

            try
            {
                // Выполняем поиск по номеру накладной или наименованию поставщика
                List<Supply> filteredSupplies = supplies.FindAll(s =>
                    (s.InvoiceNumber != null && s.InvoiceNumber.Contains(searchText)) ||
                    (s.SupplierName != null && s.SupplierName.Contains(searchText)));

                dgvSupplies.DataSource = null;
                dgvSupplies.DataSource = filteredSupplies;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске поставок: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия Enter в поле поиска
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Обработчик просмотра деталей поставки
        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            if (selectedSupplyId > 0)
            {
                try
                {
                    Supply supply = Supply.GetSupplyByID(selectedSupplyId);
                    if (supply != null)
                    {
                        string details = $"Поставка №{supply.SupplyID}\n" +
                            $"Поставщик: {supply.SupplierName}\n" +
                            $"Накладная: {supply.InvoiceNumber}\n" +
                            $"Дата: {supply.SupplyDate.ToString("dd.MM.yyyy HH:mm")}\n" +
                            $"Статус: {supply.Status}\n" +
                            $"Сумма: {supply.TotalAmount.ToString("C2")}\n\n" +
                            "Позиции в поставке:\n";

                        foreach (var detail in supply.Details)
                        {
                            details += $"- {detail.ProductName}, {detail.Quantity} шт. по {detail.UnitPrice.ToString("C2")}, " +
                                $"всего: {detail.TotalPrice.ToString("C2")}\n";
                        }

                        MessageBox.Show(details, "Детали поставки", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке деталей поставки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите поставку для просмотра деталей", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик добавления товара в поставку
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvSupplyDetails.Rows.Add();
            dgvSupplyDetails.CurrentCell = dgvSupplyDetails.Rows[rowIndex].Cells["ProductID"];
            dgvSupplyDetails.BeginEdit(true);
        }

        // Обработчик изменения значения в ячейке
        private void dgvSupplyDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSupplyDetails.Rows[e.RowIndex];

                // Если изменились количество или цена, пересчитываем общую сумму
                if (e.ColumnIndex == dgvSupplyDetails.Columns["Quantity"].Index ||
                    e.ColumnIndex == dgvSupplyDetails.Columns["UnitPrice"].Index)
                {
                    if (row.Cells["Quantity"].Value != null && row.Cells["UnitPrice"].Value != null)
                    {
                        int quantity;
                        decimal unitPrice;

                        if (int.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity) &&
                            decimal.TryParse(row.Cells["UnitPrice"].Value.ToString(), out unitPrice))
                        {
                            decimal total = quantity * unitPrice;
                            row.Cells["TotalPrice"].Value = total;

                            // Пересчитываем общую сумму поставки
                            CalculateTotalAmount();
                        }
                    }
                }
            }
        }

        // Обработчик удаления строки
        private void dgvSupplyDetails_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            // Пересчитываем общую сумму после удаления строки
            CalculateTotalAmount();
        }

        // Пересчет общей суммы поставки
        private void CalculateTotalAmount()
        {
            totalAmount = 0;
            foreach (DataGridViewRow row in dgvSupplyDetails.Rows)
            {
                if (!row.IsNewRow && row.Cells["TotalPrice"].Value != null)
                {
                    decimal totalPrice;
                    if (decimal.TryParse(row.Cells["TotalPrice"].Value.ToString(), out totalPrice))
                    {
                        totalAmount += totalPrice;
                    }
                }
            }
            txtTotalAmount.Text = totalAmount.ToString("C2");
        }
    }
}