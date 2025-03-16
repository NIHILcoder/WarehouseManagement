using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WarehouseManagement.Models;

namespace WarehouseManagement.Forms
{
    public partial class MainForm : Form
    {
        // Список уведомлений для отображения
        private List<string> notifications = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            ConfigureUIBasedOnUserRole();
            LoadNotifications();
        }

        // Настройка интерфейса в зависимости от роли пользователя
        private void ConfigureUIBasedOnUserRole()
        {
            // Отображаем имя и роль пользователя
            lblUserInfo.Text = $"Пользователь: {UserSession.FullName} ({UserSession.Role})";

            // Управление доступом к функциям в зависимости от роли

            // Модуль управления пользователями доступен только администраторам
            menuUsers.Visible = UserSession.IsAdmin;
            btnUsers.Visible = UserSession.IsAdmin;

            // Отчеты и статистика доступны менеджерам и администраторам
            menuReports.Visible = UserSession.IsManager;
            btnReports.Visible = UserSession.IsManager;

            // Настройки доступны только администраторам
            menuSettings.Visible = UserSession.IsAdmin;

            // Управление аналитикой доступно менеджерам и администраторам
            menuAnalytics.Visible = UserSession.IsManager;
            btnAnalytics.Visible = UserSession.IsManager;
        }

        // Загрузка уведомлений о низком уровне запасов
        private void LoadNotifications()
        {
            try
            {
                // Получаем список уведомлений о низком уровне запасов
                string query = @"SELECT n.NotificationID, p.Name, n.Message, n.CreatedAt 
                               FROM Notifications n
                               JOIN Products p ON n.ProductID = p.ProductID
                               WHERE n.IsRead = FALSE
                               ORDER BY n.CreatedAt DESC";

                var dataTable = Utils.DatabaseHelper.ExecuteQuery(query);

                if (dataTable.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in dataTable.Rows)
                    {
                        string notificationText = $"{row["Name"]} - {row["Message"]} ({row["CreatedAt"]})";
                        notifications.Add(notificationText);
                    }

                    // Если есть уведомления, показываем иконку в трее
                    UpdateNotificationStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке уведомлений: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обновление статуса уведомлений
        private void UpdateNotificationStatus()
        {
            if (notifications.Count > 0)
            {
                lblNotifications.Text = $"Уведомления: {notifications.Count}";
                lblNotifications.Visible = true;
                btnViewNotifications.Visible = true;
            }
            else
            {
                lblNotifications.Visible = false;
                btnViewNotifications.Visible = false;
            }
        }

        // Обработчики событий для меню и кнопок

        private void btnProducts_Click(object sender, EventArgs e)
        {
            OpenForm(new ProductsForm());
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            OpenForm(new SuppliersForm());
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            OpenForm(new CustomersForm());
        }

        private void btnSupplies_Click(object sender, EventArgs e)
        {
            OpenForm(new SuppliesForm());
        }

        private void btnOrders_Click(object sender, EventArgs e)
        {
            OpenForm(new OrdersForm());
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            OpenForm(new ReportsForm());
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            OpenForm(new UsersForm());
        }

        private void btnAnalytics_Click(object sender, EventArgs e)
        {
            OpenForm(new DashboardForm());
        }

        private void btnWarehouseMap_Click(object sender, EventArgs e)
        {
            OpenForm(new WarehouseMapForm());
        }

        private void btnViewNotifications_Click(object sender, EventArgs e)
        {
            // Показываем список уведомлений
            if (notifications.Count > 0)
            {
                string allNotifications = string.Join(Environment.NewLine, notifications);
                MessageBox.Show(allNotifications, "Уведомления о низком уровне запасов",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Помечаем уведомления как прочитанные
                try
                {
                    string query = "UPDATE Notifications SET IsRead = TRUE WHERE IsRead = FALSE";
                    Utils.DatabaseHelper.ExecuteNonQuery(query);

                    // Очищаем список
                    notifications.Clear();
                    UpdateNotificationStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении статуса уведомлений: " + ex.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Выход из системы
            UserSession.Clear();
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        // Вспомогательный метод для открытия форм
        private void OpenForm(Form form)
        {
            form.ShowDialog();
        }

        // Обработчики меню
        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuProducts_Click(object sender, EventArgs e)
        {
            btnProducts_Click(sender, e);
        }

        private void menuSuppliers_Click(object sender, EventArgs e)
        {
            btnSuppliers_Click(sender, e);
        }

        private void menuCustomers_Click(object sender, EventArgs e)
        {
            btnCustomers_Click(sender, e);
        }

        private void menuSupplies_Click(object sender, EventArgs e)
        {
            btnSupplies_Click(sender, e);
        }

        private void menuOrders_Click(object sender, EventArgs e)
        {
            btnOrders_Click(sender, e);
        }

        private void menuReports_Click(object sender, EventArgs e)
        {
            btnReports_Click(sender, e);
        }

        private void menuUsers_Click(object sender, EventArgs e)
        {
            btnUsers_Click(sender, e);
        }

        private void menuWarehouseMap_Click(object sender, EventArgs e)
        {
            btnWarehouseMap_Click(sender, e);
        }

        private void menuDashboard_Click(object sender, EventArgs e)
        {
            btnAnalytics_Click(sender, e);
        }

        private void menuImportExport_Click(object sender, EventArgs e)
        {
            // Открываем форму для импорта/экспорта данных
            MessageBox.Show("Функционал импорта/экспорта данных находится в разработке", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Система управления складом\nВерсия 1.0\n© WarehouseTech, 2023", "О программе",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}