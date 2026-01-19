namespace Bike_STore_Project
{
    partial class ServiceLogForm
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
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            mainMenuControl1 = new MainMenuControl();
            dgvServices = new DataGridView();
            btnRefresh = new Button();
            txtSearch = new TextBox();
            label1 = new Label();

            tableLayoutPanel1 = new TableLayoutPanel();
            tlpSearchRow = new TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)dgvServices).BeginInit();
            SuspendLayout();

            // mainMenuControl1
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 32);
            mainMenuControl1.TabIndex = 0;

            // tableLayoutPanel1 (ROOT)
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.TabIndex = 1;

            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // search row
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // grid row

            // tlpSearchRow
            tlpSearchRow.Dock = DockStyle.Fill;
            tlpSearchRow.Name = "tlpSearchRow";
            tlpSearchRow.TabIndex = 2;

            tlpSearchRow.ColumnCount = 4;
            tlpSearchRow.ColumnStyles.Clear();
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // label
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // textbox
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // button
            tlpSearchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));  // spacer

            tlpSearchRow.RowCount = 1;
            tlpSearchRow.RowStyles.Clear();
            tlpSearchRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // label1
            label1.AutoSize = true;
            label1.Location = new Point(3, 8);
            label1.Margin = new Padding(3, 8, 10, 8);
            label1.Name = "label1";
            label1.Size = new Size(53, 20);
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
            btnRefresh.Size = new Size(97, 29);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "REFRESH";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Anchor = AnchorStyles.Left;

            // Add search controls
            tlpSearchRow.Controls.Add(label1, 0, 0);
            tlpSearchRow.Controls.Add(txtSearch, 1, 0);
            tlpSearchRow.Controls.Add(btnRefresh, 2, 0);

            // dgvServices
            dgvServices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvServices.Dock = DockStyle.Fill; // IMPORTANT: Fill, not Bottom
            dgvServices.Margin = new Padding(0, 6, 0, 0);
            dgvServices.Name = "dgvServices";
            dgvServices.RowHeadersWidth = 51;
            dgvServices.TabIndex = 3;

            // Add to root
            tableLayoutPanel1.Controls.Add(tlpSearchRow, 0, 0);
            tableLayoutPanel1.Controls.Add(dgvServices, 0, 1);

            // ServiceLogForm
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = false;
            MinimumSize = new Size(760, 520);

            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainMenuControl1);

            Name = "ServiceLogForm";
            Text = "Service Log";

            ((System.ComponentModel.ISupportInitialize)dgvServices).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private MainMenuControl mainMenuControl1;
        private DataGridView dgvServices;
        private Button btnRefresh;
        private TextBox txtSearch;
        private Label label1;

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tlpSearchRow;
    }
}
