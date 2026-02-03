namespace Bike_STore_Project
{
    partial class InventoryForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new Label();
            txtSearch = new TextBox();
            btnRefresh = new Button();
            dgvProducts = new DataGridView();
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            mainMenuControl1 = new MainMenuControl();
            btnReceiveStock = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tlpSearchRow = new TableLayoutPanel();
            flpActions = new FlowLayoutPanel();
            btnLogout = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tlpSearchRow.SuspendLayout();
            flpActions.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 40);
            label1.Margin = new Padding(3, 8, 10, 8);
            label1.Name = "label1";
            label1.Size = new Size(53, 20);
            label1.TabIndex = 0;
            label1.Text = "Search";
            // 
            // txtSearch
            // 
            txtSearch.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.CharacterCasing = CharacterCasing.Upper;
            txtSearch.Location = new Point(69, 36);
            txtSearch.Margin = new Padding(3, 6, 10, 6);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(757, 27);
            txtSearch.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Left;
            btnRefresh.Location = new Point(839, 32);
            btnRefresh.Margin = new Padding(3, 6, 3, 6);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(118, 36);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.Location = new Point(12, 172);
            dgvProducts.Margin = new Padding(0, 6, 0, 0);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.RowHeadersWidth = 51;
            dgvProducts.Size = new Size(976, 338);
            dgvProducts.TabIndex = 8;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(0, 0);
            btnAdd.Margin = new Padding(0, 0, 8, 0);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(118, 36);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "ADD";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(126, 0);
            btnEdit.Margin = new Padding(0, 0, 8, 0);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(118, 36);
            btnEdit.TabIndex = 5;
            btnEdit.Text = "EDIT";
            btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(252, 0);
            btnDelete.Margin = new Padding(0, 0, 8, 0);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(118, 36);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "DELETE";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // mainMenuControl1
            // 
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Margin = new Padding(5);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(1000, 40);
            mainMenuControl1.TabIndex = 0;
            // 
            // btnReceiveStock
            // 
            btnReceiveStock.Location = new Point(378, 0);
            btnReceiveStock.Margin = new Padding(0, 0, 8, 0);
            btnReceiveStock.Name = "btnReceiveStock";
            btnReceiveStock.Size = new Size(149, 36);
            btnReceiveStock.TabIndex = 7;
            btnReceiveStock.Text = "RECEIVE STOCK";
            btnReceiveStock.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tlpSearchRow, 0, 0);
            tableLayoutPanel1.Controls.Add(flpActions, 0, 1);
            tableLayoutPanel1.Controls.Add(dgvProducts, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 40);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(12);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1000, 522);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // tlpSearchRow
            // 
            tlpSearchRow.ColumnCount = 4;
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle());
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle());
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tlpSearchRow.Controls.Add(label1, 0, 0);
            tlpSearchRow.Controls.Add(txtSearch, 1, 0);
            tlpSearchRow.Controls.Add(btnRefresh, 2, 0);
            tlpSearchRow.Dock = DockStyle.Fill;
            tlpSearchRow.Location = new Point(15, 15);
            tlpSearchRow.Name = "tlpSearchRow";
            tlpSearchRow.RowCount = 1;
            tlpSearchRow.RowStyles.Add(new RowStyle());
            tlpSearchRow.Size = new Size(970, 100);
            tlpSearchRow.TabIndex = 2;
            // 
            // flpActions
            // 
            flpActions.AutoSize = true;
            flpActions.Controls.Add(btnAdd);
            flpActions.Controls.Add(btnEdit);
            flpActions.Controls.Add(btnDelete);
            flpActions.Controls.Add(btnReceiveStock);
            flpActions.Controls.Add(btnLogout);
            flpActions.Dock = DockStyle.Fill;
            flpActions.Location = new Point(12, 124);
            flpActions.Margin = new Padding(0, 6, 0, 6);
            flpActions.Name = "flpActions";
            flpActions.Size = new Size(976, 36);
            flpActions.TabIndex = 3;
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(538, 3);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(94, 29);
            btnLogout.TabIndex = 8;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            // 
            // InventoryForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1000, 562);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainMenuControl1);
            MinimumSize = new Size(900, 520);
            Name = "InventoryForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Inventory";
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tlpSearchRow.ResumeLayout(false);
            tlpSearchRow.PerformLayout();
            flpActions.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private TextBox txtSearch;
        private Button btnRefresh;
        private DataGridView dgvProducts;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private MainMenuControl mainMenuControl1;
        private Button btnReceiveStock;

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tlpSearchRow;
        private FlowLayoutPanel flpActions;
        private Button btnLogout;
    }
}
