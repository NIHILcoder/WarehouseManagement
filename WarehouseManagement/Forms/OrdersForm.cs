using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WarehouseManagement.Models;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Forms
{
    public partial class OrdersForm : Form
    {
        private List<Order> orders = new List<Order>();
        private List<Customer> customers = new List<Customer>();
        private List<Product> products = new List<Product>();
        private Order currentOrder = null;
        private BindingList<OrderDetail> orderDetails = new BindingList<OrderDetail>();
        private int selectedOrderId = 0;
        private int selectedDetailId = 0;

        public OrdersForm()
        {
            InitializeComponent();
            ConfigureUIBasedOnUserRole();
            InitializeForm();
        }

        // Настройка интерфейса в зависимости от роли пользователя
        private void ConfigureUIBasedOnUserRole()
        {
            // Разрешаем редактирование только администраторам и менеджерам
            bool canEdit = UserSession.IsManager;
            btnAdd.Enabled = canEdit;
            btnEdit.Enabled = canEdit;
            btnDelete.Enabled = canEdit;
            btnCancelOrder.Enabled = canEdit;
            btnGenerateInvoice.Enabled = canEdit;

            gbOrderDetails.Enabled = canEdit;
        }

        // Инициализация формы
        private void InitializeForm()
        {
            try
            {
                // Загружаем заказы
                LoadOrders();

                // Загружаем клиентов
                customers = Customer.GetAllCustomers();
                cmbCustomer.DataSource = customers;
                cmbCustomer.DisplayMember = "Name";
                cmbCustomer.ValueMember = "CustomerID";

                // Загружаем товары
                products = Product.GetAllProducts();
                cmbProduct.DataSource = products;
                cmbProduct.DisplayMember = "Name";
                cmbProduct.ValueMember = "ProductID";

                // Настраиваем таблицу деталей заказа
                dgvOrderDetails.AutoGenerateColumns = false;
                dgvOrderDetails.DataSource = orderDetails;

                // Настраиваем статусы заказа
                cmbStatus.Items.AddRange(new string[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" });
                cmbStatus.SelectedIndex = 0;

                // Устанавливаем сегодняшнюю дату
                dtpOrderDate.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации формы: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            dgvOrders.Columns["OrderID"].HeaderText = "ID";
            dgvOrders.Columns["CustomerID"].Visible = false;
            dgvOrders.Columns["CustomerName"].HeaderText = "Клиент";
            dgvOrders.Columns["OrderNumber"].HeaderText = "Номер заказа";
            dgvOrders.Columns["OrderDate"].HeaderText = "Дата заказа";
            dgvOrders.Columns["OrderDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            dgvOrders.Columns["TotalAmount"].HeaderText = "Сумма";
            dgvOrders.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
            dgvOrders.Columns["Status"].HeaderText = "Статус";
            dgvOrders.Columns["UserID"].Visible = false;
            dgvOrders.Columns["UserName"].HeaderText = "Менеджер";
            dgvOrders.Columns["CreatedAt"].Visible = false;
            dgvOrders.Columns["Details"].Visible = false;

            // Выделяем цветом заказы в зависимости от статуса
            foreach (DataGridViewRow row in dgvOrders.Rows)
            {
                string status = row.Cells["Status"].Value.ToString();

                switch (status)
                {
                    case "Pending":
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        break;
                    case "Processing":
                        row.DefaultCellStyle.BackColor = Color.LightBlue;
                        break;
                    case "Shipped":
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        break;
                    case "Delivered":
                        row.DefaultCellStyle.BackColor = Color.PaleGreen;
                        break;
                    case "Cancelled":
                        row.DefaultCellStyle.BackColor = Color.LightPink;
                        break;
                }
            }
        }

        // Обработчик события выбора заказа
        private void dgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count > 0)
            {
                int orderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderID"].Value);
                selectedOrderId = orderId;

                try
                {
                    // Загружаем выбранный заказ
                    currentOrder = Order.GetOrderByID(orderId);

                    if (currentOrder != null)
                    {
                        // Заполняем детали заказа
                        orderDetails.Clear();
                        foreach (var detail in currentOrder.Details)
                        {
                            orderDetails.Add(detail);
                        }

                        // Включаем кнопки
                        btnEdit.Enabled = UserSession.IsManager && currentOrder.Status != "Delivered" && currentOrder.Status != "Cancelled";
                        btnDelete.Enabled = UserSession.IsManager && currentOrder.Status == "Pending";
                        btnCancelOrder.Enabled = UserSession.IsManager && currentOrder.Status != "Delivered" && currentOrder.Status != "Cancelled";
                        btnGenerateInvoice.Enabled = true;

                        // Обновляем сумму заказа
                        UpdateOrderTotal();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке заказа: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                selectedOrderId = 0;
                currentOrder = null;
                orderDetails.Clear();
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnCancelOrder.Enabled = false;
                btnGenerateInvoice.Enabled = false;
            }
        }

        // Обработчик события выбора детали заказа
        private void dgvOrderDetails_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrderDetails.SelectedRows.Count > 0)
            {
                selectedDetailId = dgvOrderDetails.SelectedRows[0].Index;

                // Заполняем поля детали заказа
                var detail = orderDetails[selectedDetailId];

                // Устанавливаем выбранный товар
                for (int i = 0; i < cmbProduct.Items.Count; i++)
                {
                    if (((Product)cmbProduct.Items[i]).ProductID == detail.ProductID)
                    {
                        cmbProduct.SelectedIndex = i;
                        break;
                    }
                }

                txtQuantity.Text = detail.Quantity.ToString();
                txtPrice.Text = detail.UnitPrice.ToString("N2");
                txtSerialNumber.Text = detail.SerialNumber ?? string.Empty;

                // Включаем кнопки
                btnUpdateDetail.Enabled = true;
                btnRemoveDetail.Enabled = true;
            }
            else
            {
                selectedDetailId = -1;
                btnUpdateDetail.Enabled = false;
                btnRemoveDetail.Enabled = false;
            }
        }

        // Обработчик кнопки добавления нового заказа
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            tabControl1.SelectedTab = tabDetails;
            gbOrderDetails.Enabled = true;
            btnSave.Text = "Добавить";
        }

        // Обработчик кнопки редактирования заказа
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedOrderId > 0 && currentOrder != null)
            {
                if (currentOrder.Status == "Delivered" || currentOrder.Status == "Cancelled")
                {
                    MessageBox.Show("Невозможно редактировать доставленный или отмененный заказ", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Заполняем форму данными выбранного заказа
                txtOrderNumber.Text = currentOrder.OrderNumber;
                dtpOrderDate.Value = currentOrder.OrderDate;

                // Выбираем клиента
                for (int i = 0; i < cmbCustomer.Items.Count; i++)
                {
                    if (((Customer)cmbCustomer.Items[i]).CustomerID == currentOrder.CustomerID)
                    {
                        cmbCustomer.SelectedIndex = i;
                        break;
                    }
                }

                // Выбираем статус
                for (int i = 0; i < cmbStatus.Items.Count; i++)
                {
                    if (cmbStatus.Items[i].ToString() == currentOrder.Status)
                    {
                        cmbStatus.SelectedIndex = i;
                        break;
                    }
                }

                // Переходим на вкладку деталей
                tabControl1.SelectedTab = tabDetails;
                gbOrderDetails.Enabled = true;
                btnSave.Text = "Сохранить";
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик кнопки удаления заказа
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedOrderId > 0 && currentOrder != null)
            {
                if (currentOrder.Status != "Pending")
                {
                    MessageBox.Show("Удалить можно только заказы в статусе 'Pending'", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Вы действительно хотите удалить выбранный заказ?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = Order.DeleteOrder(selectedOrderId);

                        if (success)
                        {
                            MessageBox.Show("Заказ успешно удален", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadOrders();
                            orderDetails.Clear();
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

        // Обработчик кнопки отмены заказа
        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (selectedOrderId > 0 && currentOrder != null)
            {
                if (currentOrder.Status == "Delivered" || currentOrder.Status == "Cancelled")
                {
                    MessageBox.Show("Невозможно отменить доставленный или уже отмененный заказ", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Вы действительно хотите отменить выбранный заказ?",
                    "Подтверждение отмены", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = currentOrder.UpdateStatus("Cancelled");

                        if (success)
                        {
                            MessageBox.Show("Заказ успешно отменен", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadOrders();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при отмене заказа: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для отмены", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик кнопки генерации накладной
        private void btnGenerateInvoice_Click(object sender, EventArgs e)
        {
            if (selectedOrderId > 0 && currentOrder != null)
            {
                try
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "PDF файлы (*.pdf)|*.pdf";
                    saveDialog.Title = "Сохранение накладной";
                    saveDialog.FileName = $"Order_{currentOrder.OrderNumber}_{DateTime.Now:yyyyMMdd}.pdf";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        bool success = PdfGenerator.GenerateOrderInvoice(currentOrder, saveDialog.FileName);

                        if (success)
                        {
                            MessageBox.Show("Накладная успешно сохранена", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Открываем файл
                            System.Diagnostics.Process.Start(saveDialog.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при генерации накладной: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для генерации накладной", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик кнопки добавления детали заказа
        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Укажите корректное количество", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Replace(",", "."), out decimal price) || price <= 0)
            {
                MessageBox.Show("Укажите корректную цену", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем выбранный товар
            Product selectedProduct = (Product)cmbProduct.SelectedItem;

            // Проверяем наличие товара в достаточном количестве
            if (quantity > selectedProduct.AvailableQuantity)
            {
                MessageBox.Show($"Недостаточное количество товара. Доступно: {selectedProduct.AvailableQuantity}", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем, есть ли уже такой товар в заказе
            var existingDetail = orderDetails.FirstOrDefault(d => d.ProductID == selectedProduct.ProductID);

            if (existingDetail != null)
            {
                MessageBox.Show("Этот товар уже добавлен в заказ. Используйте редактирование для изменения количества.", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Создаем новую деталь заказа
            OrderDetail newDetail = new OrderDetail
            {
                ProductID = selectedProduct.ProductID,
                ProductName = selectedProduct.Name,
                Quantity = quantity,
                UnitPrice = price,
                SerialNumber = string.IsNullOrWhiteSpace(txtSerialNumber.Text) ? null : txtSerialNumber.Text
            };

            // Добавляем деталь в список
            orderDetails.Add(newDetail);

            // Очищаем поля ввода
            ClearDetailForm();

            // Обновляем общую сумму
            UpdateOrderTotal();
        }

        // Обработчик кнопки обновления детали заказа
        private void btnUpdateDetail_Click(object sender, EventArgs e)
        {
            if (selectedDetailId < 0 || selectedDetailId >= orderDetails.Count)
            {
                MessageBox.Show("Выберите деталь заказа для обновления", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Укажите корректное количество", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text.Replace(",", "."), out decimal price) || price <= 0)
            {
                MessageBox.Show("Укажите корректную цену", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем выбранный товар
            Product selectedProduct = (Product)cmbProduct.SelectedItem;

            // Получаем текущую деталь
            var detail = orderDetails[selectedDetailId];

            // Проверяем наличие товара в достаточном количестве (только если товар изменился или количество увеличилось)
            if (selectedProduct.ProductID != detail.ProductID || quantity > detail.Quantity)
            {
                // Если товар изменился, проверяем доступное количество
                if (selectedProduct.ProductID != detail.ProductID)
                {
                    if (quantity > selectedProduct.AvailableQuantity)
                    {
                        MessageBox.Show($"Недостаточное количество товара. Доступно: {selectedProduct.AvailableQuantity}", "Предупреждение",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Проверяем, есть ли уже такой товар в заказе
                    var existingDetail = orderDetails.FirstOrDefault(d => d.ProductID == selectedProduct.ProductID && d != detail);

                    if (existingDetail != null)
                    {
                        MessageBox.Show("Этот товар уже добавлен в заказ", "Предупреждение",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                // Если количество увеличилось, проверяем доступное количество
                else if (quantity > detail.Quantity)
                {
                    int additionalQuantity = quantity - detail.Quantity;
                    if (additionalQuantity > selectedProduct.AvailableQuantity)
                    {
                        MessageBox.Show($"Недостаточное количество товара. Доступно для добавления: {selectedProduct.AvailableQuantity}", "Предупреждение",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            // Обновляем деталь
            detail.ProductID = selectedProduct.ProductID;
            detail.ProductName = selectedProduct.Name;
            detail.Quantity = quantity;
            detail.UnitPrice = price;
            detail.SerialNumber = string.IsNullOrWhiteSpace(txtSerialNumber.Text) ? null : txtSerialNumber.Text;

            // Обновляем отображение
            dgvOrderDetails.Refresh();

            // Очищаем поля ввода
            ClearDetailForm();

            // Обновляем общую сумму
            UpdateOrderTotal();
        }

        // Обработчик кнопки удаления детали заказа
        private void btnRemoveDetail_Click(object sender, EventArgs e)
        {
            if (selectedDetailId < 0 || selectedDetailId >= orderDetails.Count)
            {
                MessageBox.Show("Выберите деталь заказа для удаления", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Удаляем деталь
            orderDetails.RemoveAt(selectedDetailId);

            // Очищаем поля ввода
            ClearDetailForm();

            // Обновляем общую сумму
            UpdateOrderTotal();
        }

        // Обработчик выбора товара
        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem != null)
            {
                Product selectedProduct = (Product)cmbProduct.SelectedItem;
                txtPrice.Text = selectedProduct.Price.ToString("N2");
            }
        }

        // Обработчик кнопки сохранения заказа
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtOrderNumber.Text))
            {
                MessageBox.Show("Введите номер заказа", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (orderDetails.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну позицию в заказ", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Создаем или обновляем заказ
                Order order = new Order
                {
                    OrderID = selectedOrderId > 0 ? selectedOrderId : 0,
                    CustomerID = ((Customer)cmbCustomer.SelectedItem).CustomerID,
                    OrderNumber = txtOrderNumber.Text,
                    OrderDate = dtpOrderDate.Value,
                    Status = cmbStatus.SelectedItem.ToString(),
                    UserID = UserSession.UserID,
                    TotalAmount = CalculateTotalAmount(),
                    Details = orderDetails.ToList()
                };

                bool success;

                if (selectedOrderId == 0)
                {
                    // Добавляем новый заказ
                    success = order.AddOrder();
                    if (success)
                    {
                        MessageBox.Show("Заказ успешно добавлен", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Обновляем существующий заказ
                    success = order.UpdateStatus(order.Status);
                    if (success)
                    {
                        MessageBox.Show("Заказ успешно обновлен", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadOrders();
                    ClearForm();
                    tabControl1.SelectedTab = tabList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик кнопки отмены редактирования
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            tabControl1.SelectedTab = tabList;
        }

        // Очистка формы
        private void ClearForm()
        {
            txtOrderNumber.Text = string.Empty;
            dtpOrderDate.Value = DateTime.Now;
            cmbCustomer.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
            ClearDetailForm();
            orderDetails.Clear();
            lblTotal.Text = "0.00";
        }

        // Очистка формы детали заказа
        private void ClearDetailForm()
        {
            cmbProduct.SelectedIndex = -1;
            txtQuantity.Text = "1";
            txtPrice.Text = "0.00";
            txtSerialNumber.Text = string.Empty;
            selectedDetailId = -1;
            btnUpdateDetail.Enabled = false;
            btnRemoveDetail.Enabled = false;
        }

        // Расчет общей суммы заказа
        private decimal CalculateTotalAmount()
        {
            decimal total = 0;

            foreach (var detail in orderDetails)
            {
                total += detail.Quantity * detail.UnitPrice;
            }

            return total;
        }

        // Обновление общей суммы заказа
        private void UpdateOrderTotal()
        {
            decimal total = CalculateTotalAmount();
            lblTotal.Text = total.ToString("N2");
        }

        // Обработчик кнопки поиска
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadOrders();
                return;
            }

            string searchText = txtSearch.Text.Trim().ToLower();

            var filteredOrders = orders.Where(o =>
                o.OrderNumber.ToLower().Contains(searchText) ||
                o.CustomerName.ToLower().Contains(searchText) ||
                o.Status.ToLower().Contains(searchText) ||
                o.UserName.ToLower().Contains(searchText)
            ).ToList();

            dgvOrders.DataSource = null;
            dgvOrders.DataSource = filteredOrders;
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

        // Обработчик кнопки обновления
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
            txtSearch.Text = string.Empty;
        }

        // Генерация номера заказа
        private void btnGenerateNumber_Click(object sender, EventArgs e)
        {
            txtOrderNumber.Text = $"ORD-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        }
    }
}