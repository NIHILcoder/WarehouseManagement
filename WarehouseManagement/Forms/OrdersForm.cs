using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using WarehouseManagement.Models;

namespace WarehouseManagement.Forms
{
    public partial class OrdersForm : Form
    {
        private List<Order> orders = new List<Order>();
        private int selectedOrderId = 0;
        private List<OrderDetail> currentOrderDetails = new List<OrderDetail>();
        private decimal totalAmount = 0;

        public OrdersForm()
        {
            InitializeComponent();
            LoadOrders();
            LoadCustomers();
            ConfigureUIBasedOnUserRole();
            SetupDetailsGrid();

            // Устанавливаем "Все статусы" в фильтре
            cmbStatusFilter.SelectedIndex = 0;
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
            gbOrderDetails.Enabled = canEdit;
            panelOrderItems.Enabled = canEdit;
        }

        // Загрузка списка заказов
        private void LoadOrders()
        {
            try
            {
                orders = Order.GetAllOrders();
                BindOrdersToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Привязка списка заказов к DataGridView
        private void BindOrdersToGrid()
        {
            dgvOrders.DataSource = null;
            dgvOrders.DataSource = orders;

            // Настраиваем отображение столбцов
            if (dgvOrders.Columns.Contains("OrderID"))
                dgvOrders.Columns["OrderID"].HeaderText = "ID";
            if (dgvOrders.Columns.Contains("CustomerName"))
                dgvOrders.Columns["CustomerName"].HeaderText = "Клиент";
            if (dgvOrders.Columns.Contains("OrderNumber"))
                dgvOrders.Columns["OrderNumber"].HeaderText = "Номер заказа";
            if (dgvOrders.Columns.Contains("OrderDate"))
            {
                dgvOrders.Columns["OrderDate"].HeaderText = "Дата заказа";
                dgvOrders.Columns["OrderDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            }
            if (dgvOrders.Columns.Contains("TotalAmount"))
            {
                dgvOrders.Columns["TotalAmount"].HeaderText = "Общая сумма";
                dgvOrders.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
            }
            if (dgvOrders.Columns.Contains("Status"))
                dgvOrders.Columns["Status"].HeaderText = "Статус";
            if (dgvOrders.Columns.Contains("UserName"))
                dgvOrders.Columns["UserName"].HeaderText = "Менеджер";

            // Скрываем ненужные столбцы
            if (dgvOrders.Columns.Contains("CustomerID"))
                dgvOrders.Columns["CustomerID"].Visible = false;
            if (dgvOrders.Columns.Contains("UserID"))
                dgvOrders.Columns["UserID"].Visible = false;
            if (dgvOrders.Columns.Contains("CreatedAt"))
                dgvOrders.Columns["CreatedAt"].Visible = false;
            if (dgvOrders.Columns.Contains("Details"))
                dgvOrders.Columns["Details"].Visible = false;
        }

        // Загрузка списка клиентов
        private void LoadCustomers()
        {
            try
            {
                var customers = Order.GetAllCustomers();
                cmbCustomer.DisplayMember = "Value";
                cmbCustomer.ValueMember = "Key";
                cmbCustomer.DataSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Настройка таблицы деталей заказа
        private void SetupDetailsGrid()
        {
            // Создаем столбцы для деталей заказа
            dgvOrderDetails.Columns.Clear();

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

            DataGridViewTextBoxColumn serialColumn = new DataGridViewTextBoxColumn();
            serialColumn.HeaderText = "Серийный номер";
            serialColumn.Name = "SerialNumber";

            dgvOrderDetails.Columns.AddRange(new DataGridViewColumn[] {
                productColumn, quantityColumn, priceColumn, totalColumn, serialColumn
            });

            // Добавляем возможность удаления строк
            dgvOrderDetails.AllowUserToDeleteRows = true;
        }

        // Получение списка товаров для выбора
        private List<KeyValuePair<int, string>> GetProducts()
        {
            List<KeyValuePair<int, string>> products = new List<KeyValuePair<int, string>>();

            // Загружаем список товаров из базы данных
            string query = @"
                SELECT p.ProductID, p.Name, i.Quantity, i.ReservedQuantity 
                FROM Products p 
                JOIN Inventory i ON p.ProductID = i.ProductID
                WHERE (i.Quantity - i.ReservedQuantity) > 0
                ORDER BY p.Name";

            var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                int availableQty = Convert.ToInt32(row["Quantity"]) - Convert.ToInt32(row["ReservedQuantity"]);
                products.Add(new KeyValuePair<int, string>(
                    Convert.ToInt32(row["ProductID"]),
                    $"{row["Name"]} (доступно: {availableQty})"
                ));
            }

            return products;
        }

        // Обработчик выбора заказа в таблице
        private void dgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count > 0)
            {
                selectedOrderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
                // Включаем кнопки редактирования и удаления если у пользователя есть права
                btnEdit.Enabled = UserSession.IsManager;
                btnDelete.Enabled = UserSession.IsManager;
            }
            else
            {
                selectedOrderId = 0;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        // Обработчик нажатия кнопки добавления заказа
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Очищаем форму и готовим для создания нового заказа
            ClearForm();
            selectedOrderId = 0;
            gbOrderDetails.Enabled = true;
            panelOrderItems.Enabled = true;
            btnSave.Text = "Добавить";
            tabControl1.SelectedTab = tabDetails;

            // Устанавливаем значения по умолчанию
            dtpOrderDate.Value = DateTime.Now;
            cmbStatus.SelectedIndex = 0; // Pending

            // Генерируем номер заказа
            txtOrderNumber.Text = $"ORD-{DateTime.Now.ToString("yyyyMMdd-HHmmss")}";
        }

        // Обработчик нажатия кнопки редактирования заказа
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedOrderId > 0)
            {
                try
                {
                    // Загружаем детали заказа
                    Order order = Order.GetOrderByID(selectedOrderId);
                    if (order != null)
                    {
                        // Заполняем форму данными
                        for (int i = 0; i < cmbCustomer.Items.Count; i++)
                        {
                            KeyValuePair<int, string> customer = (KeyValuePair<int, string>)cmbCustomer.Items[i];
                            if (customer.Key == order.CustomerID)
                            {
                                cmbCustomer.SelectedIndex = i;
                                break;
                            }
                        }

                        txtOrderNumber.Text = order.OrderNumber ?? string.Empty;
                        txtTotalAmount.Text = order.TotalAmount.ToString("C2");
                        totalAmount = order.TotalAmount;

                        // Устанавливаем статус
                        for (int i = 0; i < cmbStatus.Items.Count; i++)
                        {
                            if (cmbStatus.Items[i].ToString() == order.Status)
                            {
                                cmbStatus.SelectedIndex = i;
                                break;
                            }
                        }

                        dtpOrderDate.Value = order.OrderDate;

                        // Загружаем детали заказа
                        currentOrderDetails = order.Details;
                        UpdateOrderDetailsGrid();

                        // Активируем редактирование
                        gbOrderDetails.Enabled = true;
                        panelOrderItems.Enabled = true;
                        btnSave.Text = "Сохранить";
                        tabControl1.SelectedTab = tabDetails;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных заказа: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обновление таблицы деталей заказа
        private void UpdateOrderDetailsGrid()
        {
            dgvOrderDetails.Rows.Clear();

            foreach (var detail in currentOrderDetails)
            {
                int rowIndex = dgvOrderDetails.Rows.Add();
                DataGridViewRow row = dgvOrderDetails.Rows[rowIndex];

                row.Cells["ProductID"].Value = detail.ProductID;
                row.Cells["Quantity"].Value = detail.Quantity;
                row.Cells["UnitPrice"].Value = detail.UnitPrice;
                row.Cells["TotalPrice"].Value = detail.TotalPrice;
                row.Cells["SerialNumber"].Value = detail.SerialNumber ?? string.Empty;
            }

            // Обновляем общую сумму
            CalculateTotalAmount();
        }

        // Обработчик нажатия кнопки удаления заказа
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedOrderId > 0)
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный заказ?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = Order.DeleteOrder(selectedOrderId);

                        if (success)
                        {
                            MessageBox.Show("Заказ успешно удален", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadOrders();
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки сохранения заказа
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите клиента", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtOrderNumber.Text))
            {
                MessageBox.Show("Введите номер заказа", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dgvOrderDetails.Rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Собираем данные о заказе
                Order order = new Order
                {
                    OrderID = selectedOrderId,
                    CustomerID = ((KeyValuePair<int, string>)cmbCustomer.SelectedItem).Key,
                    OrderNumber = txtOrderNumber.Text,
                    OrderDate = dtpOrderDate.Value,
                    TotalAmount = totalAmount,
                    Status = cmbStatus.SelectedItem.ToString(),
                    UserID = UserSession.UserID,
                    Details = new List<OrderDetail>()
                };

                // Собираем детали заказа
                foreach (DataGridViewRow row in dgvOrderDetails.Rows)
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

                    order.Details.Add(new OrderDetail
                    {
                        ProductID = productId,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        SerialNumber = row.Cells["SerialNumber"].Value?.ToString()
                    });
                }

                bool success;
                if (selectedOrderId == 0)
                {
                    // Добавляем новый заказ
                    success = order.AddOrder();
                    if (success)
                    {
                        MessageBox.Show("Заказ успешно добавлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Обновляем статус существующего заказа
                    success = order.UpdateStatus(order.Status);
                    if (success)
                    {
                        MessageBox.Show("Статус заказа успешно обновлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadOrders();
                    ClearForm();
                    gbOrderDetails.Enabled = false;
                    panelOrderItems.Enabled = false;
                    tabControl1.SelectedTab = tabList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия кнопки отмены
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            gbOrderDetails.Enabled = false;
            panelOrderItems.Enabled = false;
            tabControl1.SelectedTab = tabList;
        }

        // Очистка формы
        private void ClearForm()
        {
            if (cmbCustomer.Items.Count > 0)
                cmbCustomer.SelectedIndex = 0;
            txtOrderNumber.Text = string.Empty;
            txtTotalAmount.Text = "0";
            totalAmount = 0;
            if (cmbStatus.Items.Count > 0)
                cmbStatus.SelectedIndex = 0;
            dtpOrderDate.Value = DateTime.Now;

            // Очищаем таблицу деталей
            dgvOrderDetails.Rows.Clear();
            currentOrderDetails.Clear();
        }

        // Обработчик нажатия кнопки обновления списка заказов
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        // Обработчик кнопки поиска
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadOrders();
                return;
            }

            try
            {
                // Выполняем поиск по номеру заказа или наименованию клиента
                List<Order> filteredOrders = orders.FindAll(o =>
                    (o.OrderNumber != null && o.OrderNumber.Contains(searchText)) ||
                    (o.CustomerName != null && o.CustomerName.Contains(searchText)));

                // Если выбран фильтр по статусу, применяем его
                if (cmbStatusFilter.SelectedIndex > 0)
                {
                    string statusFilter = cmbStatusFilter.SelectedItem.ToString();
                    filteredOrders = filteredOrders.FindAll(o => o.Status == statusFilter);
                }

                dgvOrders.DataSource = null;
                dgvOrders.DataSource = filteredOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске заказов: {ex.Message}", "Ошибка",
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

        // Обработчик изменения фильтра по статусу
        private void cmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStatusFilter.SelectedIndex == 0)
            {
                // "Все статусы"
                dgvOrders.DataSource = orders;
            }
            else
            {
                string statusFilter = cmbStatusFilter.SelectedItem.ToString();
                List<Order> filteredOrders = orders.FindAll(o => o.Status == statusFilter);
                dgvOrders.DataSource = filteredOrders;
            }

            BindOrdersToGrid();
        }

        // Обработчик просмотра деталей заказа
        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            if (selectedOrderId > 0)
            {
                try
                {
                    Order order = Order.GetOrderByID(selectedOrderId);
                    if (order != null)
                    {
                        string details = $"Заказ №{order.OrderID}\n" +
                            $"Клиент: {order.CustomerName}\n" +
                            $"Номер заказа: {order.OrderNumber}\n" +
                            $"Дата: {order.OrderDate.ToString("dd.MM.yyyy HH:mm")}\n" +
                            $"Статус: {order.Status}\n" +
                            $"Сумма: {order.TotalAmount.ToString("C2")}\n\n" +
                            "Позиции в заказе:\n";

                        foreach (var detail in order.Details)
                        {
                            details += $"- {detail.ProductName}, {detail.Quantity} шт. по {detail.UnitPrice.ToString("C2")}, " +
                                $"всего: {detail.TotalPrice.ToString("C2")}\n";
                        }

                        MessageBox.Show(details, "Детали заказа", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке деталей заказа: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для просмотра деталей", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик добавления товара в заказ
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            int rowIndex = dgvOrderDetails.Rows.Add();
            dgvOrderDetails.CurrentCell = dgvOrderDetails.Rows[rowIndex].Cells["ProductID"];
            dgvOrderDetails.BeginEdit(true);
        }

        // Обработчик изменения значения в ячейке
        private void dgvOrderDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvOrderDetails.Rows[e.RowIndex];

                // Если изменились количество или цена, пересчитываем общую сумму
                if (e.ColumnIndex == dgvOrderDetails.Columns["Quantity"].Index ||
                    e.ColumnIndex == dgvOrderDetails.Columns["UnitPrice"].Index)
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

                            // Пересчитываем общую сумму заказа
                            CalculateTotalAmount();
                        }
                    }
                }
            }
        }

        // Обработчик удаления строки
        private void dgvOrderDetails_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            // Пересчитываем общую сумму после удаления строки
            CalculateTotalAmount();
        }

        // Пересчет общей суммы заказа
        private void CalculateTotalAmount()
        {
            totalAmount = 0;
            foreach (DataGridViewRow row in dgvOrderDetails.Rows)
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