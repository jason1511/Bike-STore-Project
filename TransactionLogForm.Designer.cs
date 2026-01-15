namespace Bike_STore_Project
{
    partial class TransactionLogForm
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
            mainMenuControl1 = new MainMenuControl();
            dgvSales = new DataGridView();
            btnRefresh = new Button();
            label1 = new Label();
            txtSearch = new TextBox();
            dgvSaleLines = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvSales).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvSaleLines).BeginInit();
            SuspendLayout();
            // 
            // mainMenuControl1
            // 
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 30);
            mainMenuControl1.TabIndex = 0;
            // 
            // dgvSales
            // 
            dgvSales.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSales.Dock = DockStyle.Bottom;
            dgvSales.Location = new Point(0, 244);
            dgvSales.Name = "dgvSales";
            dgvSales.RowHeadersWidth = 51;
            dgvSales.Size = new Size(800, 206);
            dgvSales.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(215, 43);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(122, 29);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "REFRESH";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 45);
            label1.Name = "label1";
            label1.Size = new Size(53, 20);
            label1.TabIndex = 3;
            label1.Text = "Search";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(71, 45);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(125, 27);
            txtSearch.TabIndex = 4;
            // 
            // dgvSaleLines
            // 
            dgvSaleLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSaleLines.Dock = DockStyle.Bottom;
            dgvSaleLines.Location = new Point(0, 78);
            dgvSaleLines.Name = "dgvSaleLines";
            dgvSaleLines.RowHeadersWidth = 51;
            dgvSaleLines.Size = new Size(800, 166);
            dgvSaleLines.TabIndex = 5;
            // 
            // TransactionLogForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgvSaleLines);
            Controls.Add(txtSearch);
            Controls.Add(label1);
            Controls.Add(btnRefresh);
            Controls.Add(dgvSales);
            Controls.Add(mainMenuControl1);
            Name = "TransactionLogForm";
            Text = "TransactionLogForm";
            ((System.ComponentModel.ISupportInitialize)dgvSales).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvSaleLines).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MainMenuControl mainMenuControl1;
        private DataGridView dgvSales;
        private Button btnRefresh;
        private Label label1;
        private TextBox txtSearch;
        private DataGridView dgvSaleLines;
    }
}