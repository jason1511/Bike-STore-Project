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

            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            SuspendLayout();

            // mainMenuControl1
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Margin = new Padding(5);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(1000, 40);
            mainMenuControl1.TabIndex = 0;

            // tableLayoutPanel1 (ROOT)
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.TabIndex = 1;

            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));         // search
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));         // actions
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));    // grid

            // tlpSearchRow
            tlpSearchRow.Dock = DockStyle.Fill;
            tlpSearchRow.Name = "tlpSearchRow";
            tlpSearchRow.TabIndex = 2;
            tlpSearchRow.ColumnCount = 4;
            tlpSearchRow.ColumnStyles.Clear();
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // label
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // textbox
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // refresh
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));  // spacer
            tlpSearchRow.RowCount = 1;
            tlpSearchRow.RowStyles.Clear();
            tlpSearchRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // label1
            label1.AutoSize = true;
            label1.Margin = new Padding(3, 8, 10, 8);
            label1.Name = "label1";
            label1.Size = new Size(64, 25);
            label1.TabIndex = 0;
            label1.Text = "Search";
            label1.Anchor = AnchorStyles.Left;

            // txtSearch
            txtSearch.CharacterCasing = CharacterCasing.Upper;
            txtSearch.Margin = new Padding(3, 6, 10, 6);
            txtSearch.Name = "txtSearch";
            txtSearch.TabIndex = 1;
            txtSearch.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            // btnRefresh
            btnRefresh.Margin = new Padding(3, 6, 3, 6);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(118, 36);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Anchor = AnchorStyles.Left;

            // Search row add
            tlpSearchRow.Controls.Add(label1, 0, 0);
            tlpSearchRow.Controls.Add(txtSearch, 1, 0);
            tlpSearchRow.Controls.Add(btnRefresh, 2, 0);

            // flpActions
            flpActions.Dock = DockStyle.Fill;
            flpActions.AutoSize = true;
            flpActions.WrapContents = true;
            flpActions.FlowDirection = FlowDirection.LeftToRight;
            flpActions.Name = "flpActions";
            flpActions.TabIndex = 3;
            flpActions.Margin = new Padding(0, 6, 0, 6);

            // btnAdd
            btnAdd.Margin = new Padding(0, 0, 8, 0);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(118, 36);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "ADD";
            btnAdd.UseVisualStyleBackColor = true;

            // btnEdit
            btnEdit.Margin = new Padding(0, 0, 8, 0);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(118, 36);
            btnEdit.TabIndex = 5;
            btnEdit.Text = "EDIT";
            btnEdit.UseVisualStyleBackColor = true;

            // btnDelete
            btnDelete.Margin = new Padding(0, 0, 8, 0);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(118, 36);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "DELETE";
            btnDelete.UseVisualStyleBackColor = true;

            // btnReceiveStock
            btnReceiveStock.Margin = new Padding(0, 0, 8, 0);
            btnReceiveStock.Name = "btnReceiveStock";
            btnReceiveStock.Size = new Size(149, 36);
            btnReceiveStock.TabIndex = 7;
            btnReceiveStock.Text = "RECEIVE STOCK";
            btnReceiveStock.UseVisualStyleBackColor = true;

            flpActions.Controls.Add(btnAdd);
            flpActions.Controls.Add(btnEdit);
            flpActions.Controls.Add(btnDelete);
            flpActions.Controls.Add(btnReceiveStock);

            // dgvProducts
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Dock = DockStyle.Fill; // IMPORTANT: Fill, not Bottom
            dgvProducts.Margin = new Padding(0, 6, 0, 0);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.RowHeadersWidth = 51;
            dgvProducts.TabIndex = 8;

            // Add to root table
            tableLayoutPanel1.Controls.Add(tlpSearchRow, 0, 0);
            tableLayoutPanel1.Controls.Add(flpActions, 0, 1);
            tableLayoutPanel1.Controls.Add(dgvProducts, 0, 2);

            // InventoryForm
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = false;
            MinimumSize = new Size(900, 520);

            ClientSize = new Size(1000, 562);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainMenuControl1);

            Name = "InventoryForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Inventory";

            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
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
    }
}
