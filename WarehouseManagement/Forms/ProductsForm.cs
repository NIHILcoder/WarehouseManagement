using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WarehouseManagement.Models;

namespace WarehouseManagement.Forms
{
    public partial class ProductsForm : Form
    {
        private List<Product> products;
        private string selectedImagePath = null;
        private int selectedProductId = 0;

        public ProductsForm()
        {
            InitializeComponent();
            LoadProducts();
            LoadCategories();
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
            gbProductDetails.Enabled = canEdit;
        }

        // Загрузка списка товаров
        private void LoadProducts()
        {
            try
            {
                products = Product.GetAllProducts();
                BindProductsToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Привязка списка товаров к DataGridView
        private void BindProductsToGrid()
        {
            dgvProducts.DataSource = null;
            dgvProducts.DataSource = products;

            // Настраиваем отображение столбцов
            dgvProducts.Columns["ProductID"].HeaderText = "ID";
            dgvProducts.Columns["Name"].HeaderText = "Наименование";
            dgvProducts.Columns["CategoryName"].HeaderText = "Категория";
            dgvProducts.Columns["Price"].HeaderText = "Цена";
            dgvProducts.Columns["Quantity"].HeaderText = "Количество";
            dgvProducts.Columns["ReservedQuantity"].HeaderText = "Зарезервировано";
            dgvProducts.Columns["AvailableQuantity"].HeaderText = "Доступно";
            dgvProducts.Columns["Location"].HeaderText = "Расположение";
            dgvProducts.Columns["SerialNumber"].HeaderText = "Серийный номер";

            // Скрываем ненужные столбцы
            dgvProducts.Columns["Description"].Visible = false;
            dgvProducts.Columns["CategoryID"].Visible = false;
            dgvProducts.Columns["MinimumQuantity"].Visible = false;
            dgvProducts.Columns["ImagePath"].Visible = false;
            dgvProducts.Columns["ExpiryDate"].Visible = false;
            dgvProducts.Columns["CreatedAt"].Visible = false;
            dgvProducts.Columns["UpdatedAt"].Visible = false;

            // Форматирование ячеек с особыми условиями
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                // Выделяем красным товары с количеством ниже минимального
                int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                int minQuantity = Convert.ToInt32(row.Cells["MinimumQuantity"].Value);

                if (quantity <= minQuantity)
                {
                    row.Cells["Quantity"].Style.BackColor = Color.LightPink;
                    row.Cells["Quantity"].Style.ForeColor = Color.DarkRed;
                }
            }
        }

        // Загрузка списка категорий
        private void LoadCategories()
        {
            try
            {
                List<KeyValuePair<int, string>> categories = Product.GetAllCategories();

                // Добавляем элемент "Выберите категорию"
                cmbCategory.Items.Add(new KeyValuePair<int, string>(0, "-- Выберите категорию --"));

                foreach (var category in categories)
                {
                    cmbCategory.Items.Add(category);
                }

                cmbCategory.DisplayMember = "Value";
                cmbCategory.ValueMember = "Key";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик выбора товара в таблице
        private void dgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["ProductID"].Value);
                Product selectedProduct = products.Find(p => p.ProductID == productId);

                if (selectedProduct != null)
                {
                    selectedProductId = selectedProduct.ProductID;

                    // Заполняем форму данными выбранного товара
                    txtName.Text = selectedProduct.Name;
                    txtDescription.Text = selectedProduct.Description;
                    txtPrice.Text = selectedProduct.Price.ToString();
                    txtQuantity.Text = selectedProduct.Quantity.ToString();
                    txtMinQuantity.Text = selectedProduct.MinimumQuantity.ToString();
                    txtSerialNumber.Text = selectedProduct.SerialNumber;
                    txtLocation.Text = selectedProduct.Location;
                    dtpExpiryDate.Value = selectedProduct.ExpiryDate ?? DateTime.Now.AddYears(1);
                    chkHasExpiry.Checked = selectedProduct.ExpiryDate.HasValue;

                    // Выбираем категорию в выпадающем списке
                    for (int i = 0; i < cmbCategory.Items.Count; i++)
                    {
                        var item = (KeyValuePair<int, string>)cmbCategory.Items[i];
                        if (item.Key == selectedProduct.CategoryID)
                        {
                            cmbCategory.SelectedIndex = i;
                            break;
                        }
                    }

                    // Загружаем изображение, если есть
                    selectedImagePath = selectedProduct.ImagePath;
                    if (!string.IsNullOrEmpty(selectedImagePath) && File.Exists(selectedImagePath))
                    {
                        try
                        {
                            pbProductImage.Image = Image.FromFile(selectedImagePath);
                        }
                        catch
                        {
                            pbProductImage.Image = null;
                        }
                    }
                    else
                    {
                        pbProductImage.Image = null;
                    }

                    // Включаем кнопки редактирования и удаления
                    btnEdit.Enabled = UserSession.IsManager;
                    btnDelete.Enabled = UserSession.IsManager;
                }
            }
        }

        // Обработчик нажатия кнопки добавления товара
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            selectedProductId = 0;
            gbProductDetails.Enabled = true;
            btnSave.Text = "Добавить";
            tabControl1.SelectedTab = tabDetails;
        }

        // Обработчик нажатия кнопки редактирования товара
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedProductId > 0)
            {
                gbProductDetails.Enabled = true;
                btnSave.Text = "Сохранить";
                tabControl1.SelectedTab = tabDetails;
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки удаления товара
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductId > 0)
            {
                DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранный товар?",
                    "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool success = Product.DeleteProduct(selectedProductId);

                        if (success)
                        {
                            MessageBox.Show("Товар успешно удален", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadProducts();
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("Невозможно удалить товар, так как он используется в заказах или поставках",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки сохранения товара
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Введите наименование товара", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену товара", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtMinQuantity.Text, out int minQuantity) || minQuantity < 0)
            {
                MessageBox.Show("Введите корректное минимальное количество", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Создаем объект товара
                Product product = new Product
                {
                    ProductID = selectedProductId,
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    CategoryID = ((KeyValuePair<int, string>)cmbCategory.SelectedItem).Key,
                    Price = price,
                    MinimumQuantity = minQuantity,
                    ImagePath = selectedImagePath,
                    SerialNumber = txtSerialNumber.Text,
                    ExpiryDate = chkHasExpiry.Checked ? dtpExpiryDate.Value : (DateTime?)null,
                    Quantity = quantity,
                    ReservedQuantity = 0,
                    Location = txtLocation.Text
                };

                bool success;

                if (selectedProductId == 0)
                {
                    // Добавляем новый товар
                    success = product.AddProduct();
                    if (success)
                    {
                        MessageBox.Show("Товар успешно добавлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Обновляем существующий товар
                    success = product.UpdateProduct();
                    if (success)
                    {
                        MessageBox.Show("Товар успешно обновлен", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (success)
                {
                    LoadProducts();
                    gbProductDetails.Enabled = false;
                    tabControl1.SelectedTab = tabList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик нажатия кнопки отмены
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            gbProductDetails.Enabled = false;
            tabControl1.SelectedTab = tabList;
        }

        // Обработчик нажатия кнопки выбора изображения
        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Все файлы|*.*";
                openFileDialog.Title = "Выберите изображение товара";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedImagePath = openFileDialog.FileName;
                        pbProductImage.Image = Image.FromFile(selectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        selectedImagePath = null;
                        pbProductImage.Image = null;
                    }
                }
            }
        }

        // Обработчик нажатия кнопки удаления изображения
        private void btnRemoveImage_Click(object sender, EventArgs e)
        {
            selectedImagePath = null;
            pbProductImage.Image = null;
        }

        // Обработчик изменения состояния флажка наличия срока годности
        private void chkHasExpiry_CheckedChanged(object sender, EventArgs e)
        {
            dtpExpiryDate.Enabled = chkHasExpiry.Checked;
        }

        // Очистка формы
        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtPrice.Text = "0";
            txtQuantity.Text = "0";
            txtMinQuantity.Text = "10";
            txtSerialNumber.Text = string.Empty;
            txtLocation.Text = string.Empty;
            cmbCategory.SelectedIndex = 0;
            dtpExpiryDate.Value = DateTime.Now.AddYears(1);
            chkHasExpiry.Checked = false;
            selectedImagePath = null;
            pbProductImage.Image = null;
        }

        // Обработчик нажатия кнопки обновления списка товаров
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        // Обработчик кнопки поиска
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadProducts();
                return;
            }

            List<Product> filteredProducts = products.FindAll(p =>
                p.Name.ToLower().Contains(searchText) ||
                (p.Description != null && p.Description.ToLower().Contains(searchText)) ||
                (p.SerialNumber != null && p.SerialNumber.ToLower().Contains(searchText)) ||
                (p.CategoryName != null && p.CategoryName.ToLower().Contains(searchText)) ||
                (p.Location != null && p.Location.ToLower().Contains(searchText))
            );

            dgvProducts.DataSource = null;
            dgvProducts.DataSource = filteredProducts;
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

        // Обработчик выбора фильтра товаров с низким запасом
        private void chkLowStock_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLowStock.Checked)
            {
                try
                {
                    List<Product> lowStockProducts = Product.GetLowStockProducts();
                    dgvProducts.DataSource = null;
                    dgvProducts.DataSource = lowStockProducts;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке товаров с низким запасом: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chkLowStock.Checked = false;
                }
            }
            else
            {
                LoadProducts();
            }
        }
    }
}