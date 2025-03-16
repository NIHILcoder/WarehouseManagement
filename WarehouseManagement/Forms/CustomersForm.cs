using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using WarehouseManagement.Models;

namespace WarehouseManagement.Forms
{
    public partial class CustomersForm : Form
    {
        private List<Customer> customers = new List<Customer>();
        private int selectedCustomerId = 0;

        public CustomersForm()
        {
            InitializeComponent();
            LoadCustomers();
            ConfigureUIBasedOnUserRole();
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
            gbCustomerDetails.Enabled = canEdit;
        }

        // Загрузка списка клиентов
        private void LoadCustomers()
        {
            try
            {
                customers = Customer.GetAllCustomers();
                BindCustomersToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Привязка списка клиентов к DataGridView
        private void BindCustomersToGrid()
        {
            dgvCustomers.DataSource = null;
            dgvCustomers.DataSource = customers;

            // Настраиваем отображение столбцов
            dgvCustomers.Columns["CustomerID"].HeaderText = "ID";
            dgvCustomers.Columns["Name"].HeaderText = "Наименование";
            dgvCustomers.Columns["ContactPerson"].HeaderText = "Контактное лицо";
            dgvCustomers.Columns["Email"].HeaderText = "Email";
            dgvCustomers.Columns["Phone"].HeaderText = "Телефон";
            dgvCustomers.Columns["Address"].HeaderText = "Адрес";

            // Скрываем ненужные столбцы
            dgvCustomers.Columns["CreatedAt"].Visible = false;
        }

        // Обработчик выбора клиента в таблице
        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["CustomerID"].Value);
                Customer selectedCustomer = customers.Find(c => c.CustomerID == customerId);

                if (selectedCustomer != null)
                {
                    selectedCustomerId = selectedCustomer.CustomerID;

                    // Заполняем форму данными выбранного клиента
                    txtName.Text = selectedCustomer.Name;
                    txtContactPerson.Text = selectedCustomer.ContactPerson ?? string.Empty;
                    txtEmail.Text = selectedCustomer.Email ?? string.Empty;
                    txtPhone.Text = selectedCustomer.Phone ?? string.Empty;
                    txtAddress.Text = selectedCustomer.Address ?? string.Empty;

                    // Включаем кнопки редактирования и удаления
                    btnEdit.Enabled = UserSession.IsManager;
                    btnDelete.Enabled = UserSession.IsManager;
                }
            }
        }

        // Обработчик нажатия кнопки добавления клиента
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            selectedCustomerId = 0;
            gbCustomerDetails.Enabled = true;
            btnSave.Text = "Добавить";
            tabControl1.SelectedTab = tabDetails;
        }

        // Обработчик нажатия кнопки редактирования клиента
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId > 0)
            {
                gbCustomerDetails.Enabled = true;
                btnSave.Text = "Сохранить";
                tabControl1.SelectedTab = tabDetails;
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки удаления клиента
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId > 0)
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранного клиента?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = Customer.DeleteCustomer(selectedCustomerId);

                        if (success)
                        {
                            MessageBox.Show("Клиент успешно удален", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadCustomers();
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении клиента: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки сохранения клиента
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Введите наименование клиента", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Создаем объект клиента
                Customer customer = new Customer
                {
                    CustomerID = selectedCustomerId,
                    Name = txtName.Text,
                    ContactPerson = string.IsNullOrWhiteSpace(txtContactPerson.Text) ? null : txtContactPerson.Text,
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                    Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text,
                    Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text
                };

                bool success;

                if (selectedCustomerId == 0)
                {
                    // Добавляем нового клиента
                    success = customer.AddCustomer();
                    if (success)
                    {
                        MessageBox.Show("Клиент успешно добавлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Обновляем существующего клиента
                    success = customer.UpdateCustomer();
                    if (success)
                    {
                        MessageBox.Show("Клиент успешно обновлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadCustomers();
                    gbCustomerDetails.Enabled = false;
                    tabControl1.SelectedTab = tabList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении клиента: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия кнопки отмены
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            gbCustomerDetails.Enabled = false;
            tabControl1.SelectedTab = tabList;
        }

        // Очистка формы
        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtContactPerson.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtAddress.Text = string.Empty;
        }

        // Обработчик нажатия кнопки обновления списка клиентов
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        // Обработчик кнопки поиска
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadCustomers();
                return;
            }

            try
            {
                customers = Customer.SearchCustomers(searchText);
                BindCustomersToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске клиентов: {ex.Message}", "Ошибка",
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

        // Обработчик кнопки просмотра заказов клиента
        private void btnViewOrders_Click(object sender, EventArgs e)
        {
            if (selectedCustomerId > 0)
            {
                // Здесь можно открыть форму с заказами клиента
                MessageBox.Show("Функционал просмотра заказов клиента находится в разработке", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Выберите клиента для просмотра заказов", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}