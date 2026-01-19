namespace Bike_STore_Project
{
    partial class TransactionLogForm
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
            mainMenuControl1 = new MainMenuControl();
            dgvSales = new DataGridView();
            btnRefresh = new Button();
            label1 = new Label();
            txtSearch = new TextBox();
            dgvSaleLines = new DataGridView();
            tableLayoutPanel1 = new TableLayoutPanel();
            tlpSearchRow = new TableLayoutPanel();
            splitContainer1 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)dgvSales).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvSaleLines).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tlpSearchRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuControl1
            // 
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(882, 30);
            mainMenuControl1.TabIndex = 0;
            // 
            // dgvSales
            // 
            dgvSales.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSales.Dock = DockStyle.Fill;
            dgvSales.Location = new Point(0, 0);
            dgvSales.Name = "dgvSales";
            dgvSales.RowHeadersWidth = 51;
            dgvSales.Size = new Size(852, 120);
            dgvSales.TabIndex = 0;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Left;
            btnRefresh.Location = new Point(717, 35);
            btnRefresh.Margin = new Padding(3, 6, 3, 6);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(122, 29);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "REFRESH";
            btnRefresh.UseVisualStyleBackColor = true;
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
            txtSearch.Size = new Size(635, 27);
            txtSearch.TabIndex = 0;
            // 
            // dgvSaleLines
            // 
            dgvSaleLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSaleLines.Dock = DockStyle.Fill;
            dgvSaleLines.Location = new Point(0, 0);
            dgvSaleLines.Name = "dgvSaleLines";
            dgvSaleLines.RowHeadersWidth = 51;
            dgvSaleLines.Size = new Size(852, 223);
            dgvSaleLines.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tlpSearchRow, 0, 0);
            tableLayoutPanel1.Controls.Add(splitContainer1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 30);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(12);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(882, 483);
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
            tlpSearchRow.Size = new Size(852, 100);
            tlpSearchRow.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(15, 121);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dgvSaleLines);
            splitContainer1.Panel1MinSize = 120;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dgvSales);
            splitContainer1.Panel2MinSize = 120;
            splitContainer1.Size = new Size(852, 347);
            splitContainer1.SplitterDistance = 223;
            splitContainer1.TabIndex = 3;
            // 
            // TransactionLogForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(882, 513);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainMenuControl1);
            MinimumSize = new Size(900, 560);
            Name = "TransactionLogForm";
            Text = "Transaction Log";
            ((System.ComponentModel.ISupportInitialize)dgvSales).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvSaleLines).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tlpSearchRow.ResumeLayout(false);
            tlpSearchRow.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private MainMenuControl mainMenuControl1;
        private DataGridView dgvSales;
        private Button btnRefresh;
        private Label label1;
        private TextBox txtSearch;
        private DataGridView dgvSaleLines;

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tlpSearchRow;
        private SplitContainer splitContainer1;
    }
}
