namespace WarehouseManagement.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImportExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWarehouse = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProducts = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSuppliers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCustomers = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSupplies = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOrders = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuWarehouseMap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAnalytics = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDashboard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReports = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUsers = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblUserInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblNotifications = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnWarehouseMap = new System.Windows.Forms.Button();
            this.btnAnalytics = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnReports = new System.Windows.Forms.Button();
            this.btnOrders = new System.Windows.Forms.Button();
            this.btnSupplies = new System.Windows.Forms.Button();
            this.btnCustomers = new System.Windows.Forms.Button();
            this.btnSuppliers = new System.Windows.Forms.Button();
            this.btnProducts = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnViewNotifications = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuWarehouse,
            this.menuAnalytics,
            this.menuReports,
            this.menuUsers,
            this.menuSettings,
            this.menuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuImportExport,
            this.toolStripSeparator1,
            this.menuExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(48, 20);
            this.menuFile.Text = "Файл";
            // 
            // menuImportExport
            // 
            this.menuImportExport.Name = "menuImportExport";
            this.menuImportExport.Size = new System.Drawing.Size(167, 22);
            this.menuImportExport.Text = "Импорт/Экспорт";
            this.menuImportExport.Click += new System.EventHandler(this.menuImportExport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(167, 22);
            this.menuExit.Text = "Выход";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuWarehouse
            // 
            this.menuWarehouse.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuProducts,
            this.menuSuppliers,
            this.menuCustomers,
            this.toolStripSeparator2,
            this.menuSupplies,
            this.menuOrders,
            this.toolStripSeparator3,
            this.menuWarehouseMap});
            this.menuWarehouse.Name = "menuWarehouse";
            this.menuWarehouse.Size = new System.Drawing.Size(51, 20);
            this.menuWarehouse.Text = "Склад";
            // 
            // menuProducts
            // 
            this.menuProducts.Name = "menuProducts";
            this.menuProducts.Size = new System.Drawing.Size(173, 22);
            this.menuProducts.Text = "Товары";
            this.menuProducts.Click += new System.EventHandler(this.menuProducts_Click);
            // 
            // menuSuppliers
            // 
            this.menuSuppliers.Name = "menuSuppliers";
            this.menuSuppliers.Size = new System.Drawing.Size(173, 22);
            this.menuSuppliers.Text = "Поставщики";
            this.menuSuppliers.Click += new System.EventHandler(this.menuSuppliers_Click);
            // 
            // menuCustomers
            // 
            this.menuCustomers.Name = "menuCustomers";
            this.menuCustomers.Size = new System.Drawing.Size(173, 22);
            this.menuCustomers.Text = "Клиенты";
            this.menuCustomers.Click += new System.EventHandler(this.menuCustomers_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(170, 6);
            // 
            // menuSupplies
            // 
            this.menuSupplies.Name = "menuSupplies";
            this.menuSupplies.Size = new System.Drawing.Size(173, 22);
            this.menuSupplies.Text = "Поставки";
            this.menuSupplies.Click += new System.EventHandler(this.menuSupplies_Click);
            // 
            // menuOrders
            // 
            this.menuOrders.Name = "menuOrders";
            this.menuOrders.Size = new System.Drawing.Size(173, 22);
            this.menuOrders.Text = "Заказы";
            this.menuOrders.Click += new System.EventHandler(this.menuOrders_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(170, 6);
            // 
            // menuWarehouseMap
            // 
            this.menuWarehouseMap.Name = "menuWarehouseMap";
            this.menuWarehouseMap.Size = new System.Drawing.Size(173, 22);
            this.menuWarehouseMap.Text = "Карта склада";
            this.menuWarehouseMap.Click += new System.EventHandler(this.menuWarehouseMap_Click);
            // 
            // menuAnalytics
            // 
            this.menuAnalytics.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDashboard});
            this.menuAnalytics.Name = "menuAnalytics";
            this.menuAnalytics.Size = new System.Drawing.Size(80, 20);
            this.menuAnalytics.Text = "Аналитика";
            // 
            // menuDashboard
            // 
            this.menuDashboard.Name = "menuDashboard";
            this.menuDashboard.Size = new System.Drawing.Size(212, 22);
            this.menuDashboard.Text = "Аналитическая панель";
            this.menuDashboard.Click += new System.EventHandler(this.menuDashboard_Click);
            // 
            // menuReports
            // 
            this.menuReports.Name = "menuReports";
            this.menuReports.Size = new System.Drawing.Size(60, 20);
            this.menuReports.Text = "Отчеты";
            this.menuReports.Click += new System.EventHandler(this.menuReports_Click);
            // 
            // menuUsers
            // 
            this.menuUsers.Name = "menuUsers";
            this.menuUsers.Size = new System.Drawing.Size(97, 20);
            this.menuUsers.Text = "Пользователи";
            this.menuUsers.Click += new System.EventHandler(this.menuUsers_Click);
            // 
            // menuSettings
            // 
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.Size = new System.Drawing.Size(79, 20);
            this.menuSettings.Text = "Настройки";
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(65, 20);
            this.menuHelp.Text = "Справка";
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(149, 22);
            this.menuAbout.Text = "О программе";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUserInfo,
            this.lblNotifications});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(884, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(118, 17);
            this.lblUserInfo.Text = "Информация о пользователе";
            // 
            // lblNotifications
            // 
            this.lblNotifications.ForeColor = System.Drawing.Color.Red;
            this.lblNotifications.Name = "lblNotifications";
            this.lblNotifications.Size = new System.Drawing.Size(87, 17);
            this.lblNotifications.Text = "Уведомления: 0";
            this.lblNotifications.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnWarehouseMap);
            this.panel1.Controls.Add(this.btnAnalytics);
            this.panel1.Controls.Add(this.btnUsers);
            this.panel1.Controls.Add(this.btnReports);
            this.panel1.Controls.Add(this.btnOrders);
            this.panel1.Controls.Add(this.btnSupplies);
            this.panel1.Controls.Add(this.btnCustomers);
            this.panel1.Controls.Add(this.btnSuppliers);
            this.panel1.Controls.Add(this.btnProducts);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Location = new System.Drawing.Point(12, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 490);
            this.panel1.TabIndex = 2;
            // 
            // btnWarehouseMap
            // 
            this.btnWarehouseMap.Location = new System.Drawing.Point(15, 300);
            this.btnWarehouseMap.Name = "btnWarehouseMap";
            this.btnWarehouseMap.Size = new System.Drawing.Size(170, 30);
            this.btnWarehouseMap.TabIndex = 9;
            this.btnWarehouseMap.Text = "Карта склада";
            this.btnWarehouseMap.UseVisualStyleBackColor = true;
            this.btnWarehouseMap.Click += new System.EventHandler(this.btnWarehouseMap_Click);
            // 
            // btnAnalytics
            // 
            this.btnAnalytics.Location = new System.Drawing.Point(15, 390);
            this.btnAnalytics.Name = "btnAnalytics";
            this.btnAnalytics.Size = new System.Drawing.Size(170, 30);
            this.btnAnalytics.TabIndex = 8;
            this.btnAnalytics.Text = "Аналитика";
            this.btnAnalytics.UseVisualStyleBackColor = true;
            this.btnAnalytics.Click += new System.EventHandler(this.btnAnalytics_Click);
            // 
            // btnUsers
            // 
            this.btnUsers.Location = new System.Drawing.Point(15, 435);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(170, 30);
            this.btnUsers.TabIndex = 7;
            this.btnUsers.Text = "Пользователи";
            this.btnUsers.UseVisualStyleBackColor = true;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            // 
            // btnReports
            // 
            this.btnReports.Location = new System.Drawing.Point(15, 345);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(170, 30);
            this.btnReports.TabIndex = 6;
            this.btnReports.Text = "Отчеты";
            this.btnReports.UseVisualStyleBackColor = true;
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // btnOrders
            // 
            this.btnOrders.Location = new System.Drawing.Point(15, 255);
            this.btnOrders.Name = "btnOrders";
            this.btnOrders.Size = new System.Drawing.Size(170, 30);
            this.btnOrders.TabIndex = 5;
            this.btnOrders.Text = "Заказы";
            this.btnOrders.UseVisualStyleBackColor = true;
            this.btnOrders.Click += new System.EventHandler(this.btnOrders_Click);
            // 
            // btnSupplies
            // 
            this.btnSupplies.Location = new System.Drawing.Point(15, 210);
            this.btnSupplies.Name = "btnSupplies";
            this.btnSupplies.Size = new System.Drawing.Size(170, 30);
            this.btnSupplies.TabIndex = 4;
            this.btnSupplies.Text = "Поставки";
            this.btnSupplies.UseVisualStyleBackColor = true;
            this.btnSupplies.Click += new System.EventHandler(this.btnSupplies_Click);
            // 
            // btnCustomers
            // 
            this.btnCustomers.Location = new System.Drawing.Point(15, 165);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(170, 30);
            this.btnCustomers.TabIndex = 3;
            this.btnCustomers.Text = "Клиенты";
            this.btnCustomers.UseVisualStyleBackColor = true;
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            // 
            // btnSuppliers
            // 
            this.btnSuppliers.Location = new System.Drawing.Point(15, 120);
            this.btnSuppliers.Name = "btnSuppliers";
            this.btnSuppliers.Size = new System.Drawing.Size(170, 30);
            this.btnSuppliers.TabIndex = 2;
            this.btnSuppliers.Text = "Поставщики";
            this.btnSuppliers.UseVisualStyleBackColor = true;
            this.btnSuppliers.Click += new System.EventHandler(this.btnSuppliers_Click);
            // 
            // btnProducts
            // 
            this.btnProducts.Location = new System.Drawing.Point(15, 75);
            this.btnProducts.Name = "btnProducts";
            this.btnProducts.Size = new System.Drawing.Size(170, 30);
            this.btnProducts.TabIndex = 1;
            this.btnProducts.Text = "Товары";
            this.btnProducts.UseVisualStyleBackColor = true;
            this.btnProducts.Click += new System.EventHandler(this.btnProducts_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitle.Location = new System.Drawing.Point(11, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(149, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Главное меню";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnViewNotifications);
            this.panel2.Controls.Add(this.btnLogout);
            this.panel2.Location = new System.Drawing.Point(227, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(645, 490);
            this.panel2.TabIndex = 3;
            // 
            // btnViewNotifications
            // 
            this.btnViewNotifications.ForeColor = System.Drawing.Color.Red;
            this.btnViewNotifications.Location = new System.Drawing.Point(451, 3);
            this.btnViewNotifications.Name = "btnViewNotifications";
            this.btnViewNotifications.Size = new System.Drawing.Size(120, 23);
            this.btnViewNotifications.TabIndex = 1;
            this.btnViewNotifications.Text = "Просмотр уведомлений";
            this.btnViewNotifications.UseVisualStyleBackColor = true;
            this.btnViewNotifications.Visible = false;
            this.btnViewNotifications.Click += new System.EventHandler(this.btnViewNotifications_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(577, 3);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(63, 23);
            this.btnLogout.TabIndex = 0;
            this.btnLogout.Text = "Выход";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Система управления складом";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuImportExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuWarehouse;
        private System.Windows.Forms.ToolStripMenuItem menuProducts;
        private System.Windows.Forms.ToolStripMenuItem menuSuppliers;
        private System.Windows.Forms.ToolStripMenuItem menuCustomers;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuSupplies;
        private System.Windows.Forms.ToolStripMenuItem menuOrders;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuWarehouseMap;
        private System.Windows.Forms.ToolStripMenuItem menuAnalytics;
        private System.Windows.Forms.ToolStripMenuItem menuDashboard;
        private System.Windows.Forms.ToolStripMenuItem menuReports;
        private System.Windows.Forms.ToolStripMenuItem menuUsers;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblUserInfo;
        private System.Windows.Forms.ToolStripStatusLabel lblNotifications;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnWarehouseMap;
        private System.Windows.Forms.Button btnAnalytics;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnReports;
        private System.Windows.Forms.Button btnOrders;
        private System.Windows.Forms.Button btnSupplies;
        private System.Windows.Forms.Button btnCustomers;
        private System.Windows.Forms.Button btnSuppliers;
        private System.Windows.Forms.Button btnProducts;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnViewNotifications;
    }
}