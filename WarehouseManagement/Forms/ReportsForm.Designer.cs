namespace WarehouseManagement.Forms
{
    partial class ReportsForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbABC = new System.Windows.Forms.RadioButton();
            this.rbPopular = new System.Windows.Forms.RadioButton();
            this.rbSupplies = new System.Windows.Forms.RadioButton();
            this.rbOrders = new System.Windows.Forms.RadioButton();
            this.rbInventory = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.btnView = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.lblReportTitle = new System.Windows.Forms.Label();
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(83, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Отчеты";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.rbABC);
            this.panel1.Controls.Add(this.rbPopular);
            this.panel1.Controls.Add(this.rbSupplies);
            this.panel1.Controls.Add(this.rbOrders);
            this.panel1.Controls.Add(this.rbInventory);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dtpEndDate);
            this.panel1.Controls.Add(this.lblEndDate);
            this.panel1.Controls.Add(this.dtpStartDate);
            this.panel1.Controls.Add(this.lblStartDate);
            this.panel1.Controls.Add(this.btnView);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Location = new System.Drawing.Point(12, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(860, 119);
            this.panel1.TabIndex = 1;
            // 
            // rbABC
            // 
            this.rbABC.AutoSize = true;
            this.rbABC.Location = new System.Drawing.Point(123, 82);
            this.rbABC.Name = "rbABC";
            this.rbABC.Size = new System.Drawing.Size(84, 17);
            this.rbABC.TabIndex = 12;
            this.rbABC.Text = "ABC-анализ";
            this.rbABC.UseVisualStyleBackColor = true;
            this.rbABC.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // rbPopular
            // 
            this.rbPopular.AutoSize = true;
            this.rbPopular.Location = new System.Drawing.Point(123, 59);
            this.rbPopular.Name = "rbPopular";
            this.rbPopular.Size = new System.Drawing.Size(123, 17);
            this.rbPopular.TabIndex = 11;
            this.rbPopular.Text = "Популярные товары";
            this.rbPopular.UseVisualStyleBackColor = true;
            this.rbPopular.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // rbSupplies
            // 
            this.rbSupplies.AutoSize = true;
            this.rbSupplies.Location = new System.Drawing.Point(123, 36);
            this.rbSupplies.Name = "rbSupplies";
            this.rbSupplies.Size = new System.Drawing.Size(73, 17);
            this.rbSupplies.TabIndex = 10;
            this.rbSupplies.Text = "Поставки";
            this.rbSupplies.UseVisualStyleBackColor = true;
            this.rbSupplies.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // rbOrders
            // 
            this.rbOrders.AutoSize = true;
            this.rbOrders.Location = new System.Drawing.Point(123, 13);
            this.rbOrders.Name = "rbOrders";
            this.rbOrders.Size = new System.Drawing.Size(64, 17);
            this.rbOrders.TabIndex = 9;
            this.rbOrders.Text = "Заказы";
            this.rbOrders.UseVisualStyleBackColor = true;
            this.rbOrders.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // rbInventory
            // 
            this.rbInventory.AutoSize = true;
            this.rbInventory.Checked = true;
            this.rbInventory.Location = new System.Drawing.Point(9, 36);
            this.rbInventory.Name = "rbInventory";
            this.rbInventory.Size = new System.Drawing.Size(69, 17);
            this.rbInventory.TabIndex = 8;
            this.rbInventory.TabStop = true;
            this.rbInventory.Text = "Остатки";
            this.rbInventory.UseVisualStyleBackColor = true;
            this.rbInventory.CheckedChanged += new System.EventHandler(this.ReportTypeChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Тип отчета:";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Location = new System.Drawing.Point(424, 33);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(145, 20);
            this.dtpEndDate.TabIndex = 6;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(421, 13);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(92, 13);
            this.lblEndDate.TabIndex = 5;
            this.lblEndDate.Text = "Дата окончания:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(271, 33);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(145, 20);
            this.dtpStartDate.TabIndex = 4;
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(268, 13);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(74, 13);
            this.lblStartDate.TabIndex = 3;
            this.lblStartDate.Text = "Дата начала:";
            // 
            // btnView
            // 
            this.btnView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnView.Location = new System.Drawing.Point(595, 33);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(80, 23);
            this.btnView.TabIndex = 2;
            this.btnView.Text = "Просмотр";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(681, 33);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(80, 23);
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "Экспорт";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.Location = new System.Drawing.Point(767, 33);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(80, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "Печать";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lblReportTitle
            // 
            this.lblReportTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReportTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblReportTitle.Location = new System.Drawing.Point(12, 170);
            this.lblReportTitle.Name = "lblReportTitle";
            this.lblReportTitle.Size = new System.Drawing.Size(860, 22);
            this.lblReportTitle.TabIndex = 2;
            this.lblReportTitle.Text = "Название отчета";
            this.lblReportTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvReport
            // 
            this.dgvReport.AllowUserToAddRows = false;
            this.dgvReport.AllowUserToDeleteRows = false;
            this.dgvReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReport.Location = new System.Drawing.Point(12, 195);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.ReadOnly = true;
            this.dgvReport.Size = new System.Drawing.Size(860, 354);
            this.dgvReport.TabIndex = 3;
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.dgvReport);
            this.Controls.Add(this.lblReportTitle);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitle);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ReportsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Отчеты";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.RadioButton rbInventory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbPopular;
        private System.Windows.Forms.RadioButton rbSupplies;
        private System.Windows.Forms.RadioButton rbOrders;
        private System.Windows.Forms.RadioButton rbABC;
        private System.Windows.Forms.Label lblReportTitle;
        private System.Windows.Forms.DataGridView dgvReport;
    }
}