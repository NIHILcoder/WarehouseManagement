using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using WarehouseManagement.Models;

namespace WarehouseManagement.Forms
{
    public partial class SuppliersForm : Form
    {
        private List<Supplier> suppliers = new List<Supplier>();
        private int selectedSupplierId = 0;

        public SuppliersForm()
        {
            InitializeComponent();
            LoadSuppliers();
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
            gbSupplierDetails.Enabled = canEdit;
        }

        // Загрузка списка поставщиков
        private void LoadSuppliers()
        {
            try
            {
                suppliers = Supplier.GetAllSuppliers();
                BindSuppliersToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке поставщиков: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Привязка списка поставщиков к DataGridView
        private void BindSuppliersToGrid()
        {
            dgvSuppliers.DataSource = null;
            dgvSuppliers.DataSource = suppliers;

            // Настраиваем отображение столбцов
            dgvSuppliers.Columns["SupplierID"].HeaderText = "ID";
            dgvSuppliers.Columns["Name"].HeaderText = "Наименование";
            dgvSuppliers.Columns["ContactPerson"].HeaderText = "Контактное лицо";
            dgvSuppliers.Columns["Email"].HeaderText = "Email";
            dgvSuppliers.Columns["Phone"].HeaderText = "Телефон";
            dgvSuppliers.Columns["Address"].HeaderText = "Адрес";

            // Скрываем ненужные столбцы
            dgvSuppliers.Columns["CreatedAt"].Visible = false;
        }

        // Обработчик выбора поставщика в таблице
        private void dgvSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count > 0)
            {
                int supplierId = Convert.ToInt32(dgvSuppliers.SelectedRows[0].Cells["SupplierID"].Value);
                Supplier selectedSupplier = suppliers.Find(s => s.SupplierID == supplierId);

                if (selectedSupplier != null)
                {
                    selectedSupplierId = selectedSupplier.SupplierID;

                    // Заполняем форму данными выбранного поставщика
                    txtName.Text = selectedSupplier.Name;
                    txtContactPerson.Text = selectedSupplier.ContactPerson ?? string.Empty;
                    txtEmail.Text = selectedSupplier.Email ?? string.Empty;
                    txtPhone.Text = selectedSupplier.Phone ?? string.Empty;
                    txtAddress.Text = selectedSupplier.Address ?? string.Empty;

                    // Включаем кнопки редактирования и удаления
                    btnEdit.Enabled = UserSession.IsManager;
                    btnDelete.Enabled = UserSession.IsManager;
                }
            }
        }

        // Обработчик нажатия кнопки добавления поставщика
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            selectedSupplierId = 0;
            gbSupplierDetails.Enabled = true;
            btnSave.Text = "Добавить";
            tabControl1.SelectedTab = tabDetails;
        }

        // Обработчик нажатия кнопки редактирования поставщика
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedSupplierId > 0)
            {
                gbSupplierDetails.Enabled = true;
                btnSave.Text = "Сохранить";
                tabControl1.SelectedTab = tabDetails;
            }
            else
            {
                MessageBox.Show("Выберите поставщика для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки удаления поставщика
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedSupplierId > 0)
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранного поставщика?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = Supplier.DeleteSupplier(selectedSupplierId);

                        if (success)
                        {
                            MessageBox.Show("Поставщик успешно удален", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadSuppliers();
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении поставщика: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите поставщика для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки сохранения поставщика
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Введите наименование поставщика", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Создаем объект поставщика
                Supplier supplier = new Supplier
                {
                    SupplierID = selectedSupplierId,
                    Name = txtName.Text,
                    ContactPerson = string.IsNullOrWhiteSpace(txtContactPerson.Text) ? null : txtContactPerson.Text,
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                    Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text,
                    Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text
                };

                bool success;

                if (selectedSupplierId == 0)
                {
                    // Добавляем нового поставщика
                    success = supplier.AddSupplier();
                    if (success)
                    {
                        MessageBox.Show("Поставщик успешно добавлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Обновляем существующего поставщика
                    success = supplier.UpdateSupplier();
                    if (success)
                    {
                        MessageBox.Show("Поставщик успешно обновлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadSuppliers();
                    gbSupplierDetails.Enabled = false;
                    tabControl1.SelectedTab = tabList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении поставщика: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия кнопки отмены
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            gbSupplierDetails.Enabled = false;
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

        // Обработчик нажатия кнопки обновления списка поставщиков
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        // Обработчик кнопки поиска
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadSuppliers();
                return;
            }

            try
            {
                suppliers = Supplier.SearchSuppliers(searchText);
                BindSuppliersToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске поставщиков: {ex.Message}", "Ошибка",
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

        // Обработчик кнопки просмотра поставок от поставщика
        private void btnViewSupplies_Click(object sender, EventArgs e)
        {
            if (selectedSupplierId > 0)
            {
                // Здесь можно открыть форму с поставками от поставщика
                MessageBox.Show("Функционал просмотра поставок от поставщика находится в разработке", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Выберите поставщика для просмотра поставок", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}