using System;
using System.Windows.Forms;
using Npgsql;
using WarehouseManagement.Models;
using WarehouseManagement.Utils;

namespace WarehouseManagement.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя и пароль", "Ошибка входа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // В реальном приложении здесь должно быть хеширование пароля
                // string hashedPassword = UserSession.HashPassword(password);

                string query = @"SELECT UserID, Username, FullName, Role 
                               FROM Users 
                               WHERE Username = @Username AND Password = @Password AND IsActive = TRUE";

                NpgsqlParameter[] parameters = {
                    new NpgsqlParameter("@Username", username),
                    new NpgsqlParameter("@Password", password)
                };

                var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];

                    // Сохраняем информацию о пользователе в сессии
                    UserSession.UserID = Convert.ToInt32(row["UserID"]);
                    UserSession.Username = row["Username"].ToString();
                    UserSession.FullName = row["FullName"].ToString();
                    UserSession.Role = row["Role"].ToString();

                    // Открываем главную форму
                    MainForm mainForm = new MainForm();
                    this.Hide();
                    mainForm.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль", "Ошибка входа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}