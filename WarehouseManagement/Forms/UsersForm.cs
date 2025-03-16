using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using WarehouseManagement.Models;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Forms
{
    public partial class UsersForm : Form
    {
        private List<User> users = new List<User>();
        private int selectedUserId = 0;

        public UsersForm()
        {
            InitializeComponent();
            LoadUsers();

            // Настраиваем список ролей
            cmbRole.Items.Add("Administrator");
            cmbRole.Items.Add("Manager");
            cmbRole.Items.Add("Warehouse");
            cmbRole.SelectedIndex = 2; // По умолчанию выбираем Warehouse
        }

        // Загрузка списка пользователей
        private void LoadUsers()
        {
            try
            {
                string query = "SELECT * FROM Users ORDER BY Username";
                var dataTable = DatabaseHelper.ExecuteQuery(query);

                users.Clear();

                foreach (DataRow row in dataTable.Rows)
                {
                    users.Add(new User
                    {
                        UserID = Convert.ToInt32(row["UserID"]),
                        Username = row["Username"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString(),
                        Role = row["Role"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }

                RefreshUsersList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обновление списка пользователей в ListView
        private void RefreshUsersList()
        {
            lvUsers.Items.Clear();

            foreach (var user in users)
            {
                ListViewItem item = new ListViewItem(user.Username);
                item.SubItems.Add(user.FullName);
                item.SubItems.Add(user.Role);
                item.SubItems.Add(user.IsActive ? "Да" : "Нет");
                item.Tag = user.UserID;

                lvUsers.Items.Add(item);
            }

            // Очищаем поля формы
            ClearForm();
        }

        // Очистка полей формы
        private void ClearForm()
        {
            txtUsername.Text = string.Empty;
            txtFullName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            chkIsActive.Checked = true;
            cmbRole.SelectedIndex = 2; // Warehouse

            selectedUserId = 0;
            btnSave.Text = "Добавить";
            btnDelete.Enabled = false;
        }

        // Обработчик выбора пользователя в ListView
        private void lvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUsers.SelectedItems.Count > 0)
            {
                selectedUserId = (int)lvUsers.SelectedItems[0].Tag;
                User selectedUser = users.Find(u => u.UserID == selectedUserId);

                if (selectedUser != null)
                {
                    txtUsername.Text = selectedUser.Username;
                    txtFullName.Text = selectedUser.FullName;
                    txtEmail.Text = selectedUser.Email ?? string.Empty;
                    txtPassword.Text = string.Empty; // Не отображаем пароль для безопасности
                    chkIsActive.Checked = selectedUser.IsActive;

                    // Выбираем роль в combobox
                    switch (selectedUser.Role)
                    {
                        case "Administrator":
                            cmbRole.SelectedIndex = 0;
                            break;
                        case "Manager":
                            cmbRole.SelectedIndex = 1;
                            break;
                        case "Warehouse":
                            cmbRole.SelectedIndex = 2;
                            break;
                    }

                    btnSave.Text = "Сохранить";
                    btnDelete.Enabled = selectedUser.UserID != UserSession.UserID; // Нельзя удалить себя
                }
            }
            else
            {
                ClearForm();
            }
        }

        // Обработчик кнопки "Добавить"
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();
            txtUsername.Focus();
        }

        // Обработчик кнопки "Сохранить"
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Проверяем заполнение обязательных полей
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text) ||
                cmbRole.SelectedIndex < 0)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем, что при создании нового пользователя указан пароль
            if (selectedUserId == 0 && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Пожалуйста, укажите пароль для нового пользователя", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (selectedUserId == 0)
                {
                    // Добавляем нового пользователя
                    AddUser();
                }
                else
                {
                    // Обновляем существующего пользователя
                    UpdateUser();
                }

                // Перезагружаем список пользователей
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пользователя: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Добавление нового пользователя
        private void AddUser()
        {
            // Проверяем уникальность имени пользователя
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

            NpgsqlParameter[] checkParameters = {
                new NpgsqlParameter("@Username", txtUsername.Text.Trim())
            };

            int userCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParameters));

            if (userCount > 0)
            {
                throw new Exception("Пользователь с таким именем уже существует");
            }

            // В реальном приложении здесь должно быть хеширование пароля
            // string hashedPassword = UserSession.HashPassword(txtPassword.Text);

            string insertQuery = @"
                INSERT INTO Users (Username, Password, FullName, Email, Role, IsActive)
                VALUES (@Username, @Password, @FullName, @Email, @Role, @IsActive)";

            NpgsqlParameter[] insertParameters = {
                new NpgsqlParameter("@Username", txtUsername.Text.Trim()),
                new NpgsqlParameter("@Password", txtPassword.Text),
                new NpgsqlParameter("@FullName", txtFullName.Text.Trim()),
                new NpgsqlParameter("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text.Trim()),
                new NpgsqlParameter("@Role", cmbRole.SelectedItem.ToString()),
                new NpgsqlParameter("@IsActive", chkIsActive.Checked)
            };

            DatabaseHelper.ExecuteNonQuery(insertQuery, insertParameters);

            MessageBox.Show("Пользователь успешно добавлен", "Успех",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Обновление существующего пользователя
        private void UpdateUser()
        {
            // Проверяем уникальность имени пользователя
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND UserID != @UserID";

            NpgsqlParameter[] checkParameters = {
                new NpgsqlParameter("@Username", txtUsername.Text.Trim()),
                new NpgsqlParameter("@UserID", selectedUserId)
            };

            int userCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParameters));

            if (userCount > 0)
            {
                throw new Exception("Пользователь с таким именем уже существует");
            }

            // Проверяем, не пытается ли пользователь отключить себя
            if (selectedUserId == UserSession.UserID && !chkIsActive.Checked)
            {
                throw new Exception("Вы не можете отключить свою учетную запись");
            }

            // Проверяем, не пытается ли пользователь изменить свою роль на более низкую
            if (selectedUserId == UserSession.UserID &&
                ((UserSession.Role == "Administrator" && cmbRole.SelectedItem.ToString() != "Administrator") ||
                 (UserSession.Role == "Manager" && cmbRole.SelectedItem.ToString() == "Warehouse")))
            {
                throw new Exception("Вы не можете понизить свою роль");
            }

            // Формируем запрос в зависимости от того, меняется ли пароль
            string updateQuery;
            NpgsqlParameter[] updateParameters;

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                // Не меняем пароль
                updateQuery = @"
                    UPDATE Users
                    SET Username = @Username,
                        FullName = @FullName,
                        Email = @Email,
                        Role = @Role,
                        IsActive = @IsActive
                    WHERE UserID = @UserID";

                updateParameters = new NpgsqlParameter[] {
                    new NpgsqlParameter("@UserID", selectedUserId),
                    new NpgsqlParameter("@Username", txtUsername.Text.Trim()),
                    new NpgsqlParameter("@FullName", txtFullName.Text.Trim()),
                    new NpgsqlParameter("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text.Trim()),
                    new NpgsqlParameter("@Role", cmbRole.SelectedItem.ToString()),
                    new NpgsqlParameter("@IsActive", chkIsActive.Checked)
                };
            }
            else
            {
                // Меняем пароль
                // В реальном приложении здесь должно быть хеширование пароля
                // string hashedPassword = UserSession.HashPassword(txtPassword.Text);

                updateQuery = @"
                    UPDATE Users
                    SET Username = @Username,
                        Password = @Password,
                        FullName = @FullName,
                        Email = @Email,
                        Role = @Role,
                        IsActive = @IsActive
                    WHERE UserID = @UserID";

                updateParameters = new NpgsqlParameter[] {
                    new NpgsqlParameter("@UserID", selectedUserId),
                    new NpgsqlParameter("@Username", txtUsername.Text.Trim()),
                    new NpgsqlParameter("@Password", txtPassword.Text),
                    new NpgsqlParameter("@FullName", txtFullName.Text.Trim()),
                    new NpgsqlParameter("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ? DBNull.Value : (object)txtEmail.Text.Trim()),
                    new NpgsqlParameter("@Role", cmbRole.SelectedItem.ToString()),
                    new NpgsqlParameter("@IsActive", chkIsActive.Checked)
                };
            }

            DatabaseHelper.ExecuteNonQuery(updateQuery, updateParameters);

            MessageBox.Show("Пользователь успешно обновлен", "Успех",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Если пользователь обновил себя, обновляем сессию
            if (selectedUserId == UserSession.UserID)
            {
                UserSession.Username = txtUsername.Text.Trim();
                UserSession.FullName = txtFullName.Text.Trim();
                UserSession.Role = cmbRole.SelectedItem.ToString();
            }
        }

        // Обработчик кнопки "Удалить"
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserId == 0)
            {
                return;
            }

            // Проверяем, не пытается ли пользователь удалить себя
            if (selectedUserId == UserSession.UserID)
            {
                MessageBox.Show("Вы не можете удалить свою учетную запись", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Проверяем, есть ли у пользователя связанные записи
                    string checkQuery = @"
                        SELECT
                            (SELECT COUNT(*) FROM Orders WHERE UserID = @UserID) AS OrderCount,
                            (SELECT COUNT(*) FROM Supplies WHERE UserID = @UserID) AS SupplyCount";

                    NpgsqlParameter[] checkParameters = {
                        new NpgsqlParameter("@UserID", selectedUserId)
                    };

                    var dataTable = DatabaseHelper.ExecuteQuery(checkQuery, checkParameters);

                    int orderCount = Convert.ToInt32(dataTable.Rows[0]["OrderCount"]);
                    int supplyCount = Convert.ToInt32(dataTable.Rows[0]["SupplyCount"]);

                    if (orderCount > 0 || supplyCount > 0)
                    {
                        MessageBox.Show("Невозможно удалить пользователя, так как у него есть связанные заказы или поставки",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Удаляем пользователя
                    string deleteQuery = "DELETE FROM Users WHERE UserID = @UserID";

                    DatabaseHelper.ExecuteNonQuery(deleteQuery, checkParameters);

                    MessageBox.Show("Пользователь успешно удален", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Перезагружаем список пользователей
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик кнопки "Отмена"
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            if (lvUsers.SelectedItems.Count > 0)
            {
                lvUsers.SelectedItems[0].Selected = false;
            }
        }

        // Обработчик кнопки "Обновить"
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }
    }

    // Класс для хранения информации о пользователе
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}