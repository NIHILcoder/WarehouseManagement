namespace WarehouseManagement.Forms
{
    partial class WarehouseMapForm
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
            this.btnAutoDistribution = new System.Windows.Forms.Button();
            this.btnShowLowStock = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnFindProduct = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbZones = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelMap = new System.Windows.Forms.Panel();
            this.panelCellInfo = new System.Windows.Forms.Panel();
            this.btnClearCell = new System.Windows.Forms.Button();
            this.btnMoveProduct = new System.Windows.Forms.Button();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCellUsed = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCellCapacity = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCellLocation = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblLowStock = new System.Windows.Forms.Label();
            this.lblFull = new System.Windows.Forms.Label();
            this.lblPartiallyFull = new System.Windows.Forms.Label();
            this.lblEmpty = new System.Windows.Forms.Label();
            this.lblInactive = new System.Windows.Forms.Label();
            this.panelLowStock = new System.Windows.Forms.Panel();
            this.panelFull = new System.Windows.Forms.Panel();
            this.panelPartiallyFull = new System.Windows.Forms.Panel();
            this.panelEmpty = new System.Windows.Forms.Panel();
            this.panelInactive = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panelCellInfo.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(143, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Карта склада";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnAutoDistribution);
            this.panel1.Controls.Add(this.btnShowLowStock);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.btnFindProduct);
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbZones);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 45);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(876, 64);
            this.panel1.TabIndex = 1;
            // 
            // btnAutoDistribution
            // 
            this.btnAutoDistribution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutoDistribution.Location = new System.Drawing.Point(761, 33);
            this.btnAutoDistribution.Name = "btnAutoDistribution";
            this.btnAutoDistribution.Size = new System.Drawing.Size(103, 23);
            this.btnAutoDistribution.TabIndex = 7;
            this.btnAutoDistribution.Text = "Распределение";
            this.btnAutoDistribution.UseVisualStyleBackColor = true;
            this.btnAutoDistribution.Click += new System.EventHandler(this.btnAutoDistribution_Click);
            // 
            // btnShowLowStock
            // 
            this.btnShowLowStock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowLowStock.Location = new System.Drawing.Point(761, 7);
            this.btnShowLowStock.Name = "btnShowLowStock";
            this.btnShowLowStock.Size = new System.Drawing.Size(103, 23);
            this.btnShowLowStock.TabIndex = 6;
            this.btnShowLowStock.Text = "Низкий запас";
            this.btnShowLowStock.UseVisualStyleBackColor = true;
            this.btnShowLowStock.Click += new System.EventHandler(this.btnShowLowStock_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(285, 7);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnFindProduct
            // 
            this.btnFindProduct.Location = new System.Drawing.Point(285, 33);
            this.btnFindProduct.Name = "btnFindProduct";
            this.btnFindProduct.Size = new System.Drawing.Size(75, 23);
            this.btnFindProduct.TabIndex = 4;
            this.btnFindProduct.Text = "Найти";
            this.btnFindProduct.UseVisualStyleBackColor = true;
            this.btnFindProduct.Click += new System.EventHandler(this.btnFindProduct_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(105, 35);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(174, 20);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Поиск по товару:";
            // 
            // cmbZones
            // 
            this.cmbZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZones.FormattingEnabled = true;
            this.cmbZones.Location = new System.Drawing.Point(105, 8);
            this.cmbZones.Name = "cmbZones";
            this.cmbZones.Size = new System.Drawing.Size(174, 21);
            this.cmbZones.TabIndex = 1;
            this.cmbZones.SelectedIndexChanged += new System.EventHandler(this.cmbZones_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Зона:";
            // 
            // panelMap
            // 
            this.panelMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMap.AutoScroll = true;
            this.panelMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMap.Location = new System.Drawing.Point(12, 154);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(566, 395);
            this.panelMap.TabIndex = 2;
            // 
            // panelCellInfo
            // 
            this.panelCellInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCellInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCellInfo.Controls.Add(this.btnClearCell);
            this.panelCellInfo.Controls.Add(this.btnMoveProduct);
            this.panelCellInfo.Controls.Add(this.txtProductName);
            this.panelCellInfo.Controls.Add(this.label6);
            this.panelCellInfo.Controls.Add(this.txtCellUsed);
            this.panelCellInfo.Controls.Add(this.label5);
            this.panelCellInfo.Controls.Add(this.txtCellCapacity);
            this.panelCellInfo.Controls.Add(this.label4);
            this.panelCellInfo.Controls.Add(this.txtCellLocation);
            this.panelCellInfo.Controls.Add(this.label3);
            this.panelCellInfo.Location = new System.Drawing.Point(584, 154);
            this.panelCellInfo.Name = "panelCellInfo";
            this.panelCellInfo.Size = new System.Drawing.Size(304, 395);
            this.panelCellInfo.TabIndex = 3;
            this.panelCellInfo.Visible = false;
            // 
            // btnClearCell
            // 
            this.btnClearCell.Location = new System.Drawing.Point(162, 210);
            this.btnClearCell.Name = "btnClearCell";
            this.btnClearCell.Size = new System.Drawing.Size(126, 23);
            this.btnClearCell.TabIndex = 9;
            this.btnClearCell.Text = "Освободить ячейку";
            this.btnClearCell.UseVisualStyleBackColor = true;
            this.btnClearCell.Click += new System.EventHandler(this.btnClearCell_Click);
            // 
            // btnMoveProduct
            // 
            this.btnMoveProduct.Location = new System.Drawing.Point(10, 210);
            this.btnMoveProduct.Name = "btnMoveProduct";
            this.btnMoveProduct.Size = new System.Drawing.Size(146, 23);
            this.btnMoveProduct.TabIndex = 8;
            this.btnMoveProduct.Text = "Переместить товар";
            this.btnMoveProduct.UseVisualStyleBackColor = true;
            this.btnMoveProduct.Click += new System.EventHandler(this.btnMoveProduct_Click);
            // 
            // txtProductName
            // 
            this.txtProductName.Location = new System.Drawing.Point(10, 164);
            this.txtProductName.Multiline = true;
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.ReadOnly = true;
            this.txtProductName.Size = new System.Drawing.Size(278, 40);
            this.txtProductName.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Товар:";
            // 
            // txtCellUsed
            // 
            this.txtCellUsed.Location = new System.Drawing.Point(10, 117);
            this.txtCellUsed.Name = "txtCellUsed";
            this.txtCellUsed.ReadOnly = true;
            this.txtCellUsed.Size = new System.Drawing.Size(100, 20);
            this.txtCellUsed.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Использовано:";
            // 
            // txtCellCapacity
            // 
            this.txtCellCapacity.Location = new System.Drawing.Point(10, 71);
            this.txtCellCapacity.Name = "txtCellCapacity";
            this.txtCellCapacity.ReadOnly = true;
            this.txtCellCapacity.Size = new System.Drawing.Size(100, 20);
            this.txtCellCapacity.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Вместимость:";
            // 
            // txtCellLocation
            // 
            this.txtCellLocation.Location = new System.Drawing.Point(10, 25);
            this.txtCellLocation.Name = "txtCellLocation";
            this.txtCellLocation.ReadOnly = true;
            this.txtCellLocation.Size = new System.Drawing.Size(100, 20);
            this.txtCellLocation.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ячейка (номер):";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lblLowStock);
            this.panel2.Controls.Add(this.lblFull);
            this.panel2.Controls.Add(this.lblPartiallyFull);
            this.panel2.Controls.Add(this.lblEmpty);
            this.panel2.Controls.Add(this.lblInactive);
            this.panel2.Controls.Add(this.panelLowStock);
            this.panel2.Controls.Add(this.panelFull);
            this.panel2.Controls.Add(this.panelPartiallyFull);
            this.panel2.Controls.Add(this.panelEmpty);
            this.panel2.Controls.Add(this.panelInactive);
            this.panel2.Location = new System.Drawing.Point(12, 115);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(876, 33);
            this.panel2.TabIndex = 4;
            // 
            // lblLowStock
            // 
            this.lblLowStock.AutoSize = true;
            this.lblLowStock.Location = new System.Drawing.Point(714, 9);
            this.lblLowStock.Name = "lblLowStock";
            this.lblLowStock.Size = new System.Drawing.Size(79, 13);
            this.lblLowStock.TabIndex = 9;
            this.lblLowStock.Text = "Низкий запас";
            // 
            // lblFull
            // 
            this.lblFull.AutoSize = true;
            this.lblFull.Location = new System.Drawing.Point(568, 9);
            this.lblFull.Name = "lblFull";
            this.lblFull.Size = new System.Drawing.Size(66, 13);
            this.lblFull.TabIndex = 8;
            this.lblFull.Text = "Заполнено";
            // 
            // lblPartiallyFull
            // 
            this.lblPartiallyFull.AutoSize = true;
            this.lblPartiallyFull.Location = new System.Drawing.Point(380, 9);
            this.lblPartiallyFull.Name = "lblPartiallyFull";
            this.lblPartiallyFull.Size = new System.Drawing.Size(108, 13);
            this.lblPartiallyFull.TabIndex = 7;
            this.lblPartiallyFull.Text = "Частично заполнено";
            // 
            // lblEmpty
            // 
            this.lblEmpty.AutoSize = true;
            this.lblEmpty.Location = new System.Drawing.Point(207, 9);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(93, 13);
            this.lblEmpty.TabIndex = 6;
            this.lblEmpty.Text = "Пустая ячейка";
            // 
            // lblInactive
            // 
            this.lblInactive.AutoSize = true;
            this.lblInactive.Location = new System.Drawing.Point(35, 9);
            this.lblInactive.Name = "lblInactive";
            this.lblInactive.Size = new System.Drawing.Size(92, 13);
            this.lblInactive.TabIndex = 5;
            this.lblInactive.Text = "Неактивная ячейка";
            // 
            // panelLowStock
            // 
            this.panelLowStock.BackColor = System.Drawing.Color.Red;
            this.panelLowStock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLowStock.Location = new System.Drawing.Point(685, 5);
            this.panelLowStock.Name = "panelLowStock";
            this.panelLowStock.Size = new System.Drawing.Size(23, 23);
            this.panelLowStock.TabIndex = 4;
            // 
            // panelFull
            // 
            this.panelFull.BackColor = System.Drawing.Color.LightCoral;
            this.panelFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFull.Location = new System.Drawing.Point(539, 5);
            this.panelFull.Name = "panelFull";
            this.panelFull.Size = new System.Drawing.Size(23, 23);
            this.panelFull.TabIndex = 3;
            // 
            // panelPartiallyFull
            // 
            this.panelPartiallyFull.BackColor = System.Drawing.Color.Khaki;
            this.panelPartiallyFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPartiallyFull.Location = new System.Drawing.Point(351, 5);
            this.panelPartiallyFull.Name = "panelPartiallyFull";
            this.panelPartiallyFull.Size = new System.Drawing.Size(23, 23);
            this.panelPartiallyFull.TabIndex = 2;
            // 
            // panelEmpty
            // 
            this.panelEmpty.BackColor = System.Drawing.Color.White;
            this.panelEmpty.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelEmpty.Location = new System.Drawing.Point(178, 5);
            this.panelEmpty.Name = "panelEmpty";
            this.panelEmpty.Size = new System.Drawing.Size(23, 23);
            this.panelEmpty.TabIndex = 1;
            // 
            // panelInactive
            // 
            this.panelInactive.BackColor = System.Drawing.Color.LightGray;
            this.panelInactive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelInactive.Location = new System.Drawing.Point(10, 5);
            this.panelInactive.Name = "panelInactive";
            this.panelInactive.Size = new System.Drawing.Size(23, 23);
            this.panelInactive.TabIndex = 0;
            // 
            // WarehouseMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 561);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelCellInfo);
            this.Controls.Add(this.panelMap);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitle);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "WarehouseMapForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Карта склада";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelCellInfo.ResumeLayout(false);
            this.panelCellInfo.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbZones;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFindProduct;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelMap;
        private System.Windows.Forms.Panel panelCellInfo;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnShowLowStock;
        private System.Windows.Forms.TextBox txtCellLocation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCellCapacity;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCellUsed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnClearCell;
        private System.Windows.Forms.Button btnMoveProduct;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblInactive;
        private System.Windows.Forms.Panel panelLowStock;
        private System.Windows.Forms.Panel panelFull;
        private System.Windows.Forms.Panel panelPartiallyFull;
        private System.Windows.Forms.Panel panelEmpty;
        private System.Windows.Forms.Panel panelInactive;
        private System.Windows.Forms.Label lblLowStock;
        private System.Windows.Forms.Label lblFull;
        private System.Windows.Forms.Label lblPartiallyFull;
        private System.Windows.Forms.Label lblEmpty;
        private System.Windows.Forms.Button btnAutoDistribution;
    }
}