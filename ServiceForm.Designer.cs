namespace Bike_STore_Project
{
    partial class ServiceForm
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
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            numServiceCost = new NumericUpDown();
            label5 = new Label();
            label6 = new Label();
            txtNotes = new TextBox();
            btnAddService = new Button();
            btnClear = new Button();
            mainMenuControl1 = new MainMenuControl();
            txtBrand = new TextBox();
            txtType = new TextBox();
            txtColor = new TextBox();

            tableLayoutPanel1 = new TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)numServiceCost).BeginInit();
            SuspendLayout();

            // mainMenuControl1
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 31);
            mainMenuControl1.TabIndex = 0;

            // tableLayoutPanel1 (ROOT)
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.TabIndex = 1;

            // 2 columns: label + input
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // rows: Brand, Type, Color, Cost, Notes, Buttons
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F)); // Notes fixed height
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // label1 - Brand
            label1.AutoSize = true;
            label1.Name = "label1";
            label1.Text = "Brand";
            label1.Anchor = AnchorStyles.Left;
            label1.Margin = new Padding(3, 8, 10, 8);

            // label2 - Type
            label2.AutoSize = true;
            label2.Name = "label2";
            label2.Text = "Type";
            label2.Anchor = AnchorStyles.Left;
            label2.Margin = new Padding(3, 8, 10, 8);

            // label3 - Color
            label3.AutoSize = true;
            label3.Name = "label3";
            label3.Text = "Color";
            label3.Anchor = AnchorStyles.Left;
            label3.Margin = new Padding(3, 8, 10, 8);

            // label5 - Service Cost
            label5.AutoSize = true;
            label5.Name = "label5";
            label5.Text = "Service Cost";
            label5.Anchor = AnchorStyles.Left;
            label5.Margin = new Padding(3, 8, 10, 8);

            // label6 - Notes
            label6.AutoSize = true;
            label6.Name = "label6";
            label6.Text = "Notes";
            label6.Anchor = AnchorStyles.Left;
            label6.Margin = new Padding(3, 8, 10, 8);

            // txtBrand
            txtBrand.Name = "txtBrand";
            txtBrand.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtBrand.Margin = new Padding(3, 6, 3, 6);
            txtBrand.TabIndex = 0;

            // txtType
            txtType.Name = "txtType";
            txtType.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtType.Margin = new Padding(3, 6, 3, 6);
            txtType.TabIndex = 1;

            // txtColor
            txtColor.Name = "txtColor";
            txtColor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtColor.Margin = new Padding(3, 6, 3, 6);
            txtColor.TabIndex = 2;

            // numServiceCost
            numServiceCost.Name = "numServiceCost";
            numServiceCost.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numServiceCost.ThousandsSeparator = true;
            numServiceCost.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numServiceCost.Margin = new Padding(3, 6, 3, 6);
            numServiceCost.TabIndex = 3;

            // txtNotes
            txtNotes.Name = "txtNotes";
            txtNotes.CharacterCasing = CharacterCasing.Upper;
            txtNotes.Multiline = true;
            txtNotes.ScrollBars = ScrollBars.Vertical;
            txtNotes.Dock = DockStyle.Fill; // fill the fixed-height row
            txtNotes.Margin = new Padding(3, 6, 3, 6);
            txtNotes.TabIndex = 4;

            // btnAddService
            btnAddService.Name = "btnAddService";
            btnAddService.Text = "ADD SERVICE";
            btnAddService.AutoSize = true;
            btnAddService.Margin = new Padding(6);
            btnAddService.TabIndex = 5;
            btnAddService.UseVisualStyleBackColor = true;

            // btnClear
            btnClear.Name = "btnClear";
            btnClear.Text = "CLEAR";
            btnClear.AutoSize = true;
            btnClear.Margin = new Padding(6);
            btnClear.TabIndex = 6;
            btnClear.UseVisualStyleBackColor = true;

            // buttons panel (inside root table)
            var flpButtons = new FlowLayoutPanel();
            flpButtons.Name = "flpButtons";
            flpButtons.Dock = DockStyle.Fill;
            flpButtons.AutoSize = true;
            flpButtons.WrapContents = false;
            flpButtons.FlowDirection = FlowDirection.RightToLeft;
            flpButtons.Controls.Add(btnClear);
            flpButtons.Controls.Add(btnAddService);

            // add controls to root table
            tableLayoutPanel1.SuspendLayout();

            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(txtBrand, 1, 0);

            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(txtType, 1, 1);

            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(txtColor, 1, 2);

            tableLayoutPanel1.Controls.Add(label5, 0, 3);
            tableLayoutPanel1.Controls.Add(numServiceCost, 1, 3);

            tableLayoutPanel1.Controls.Add(label6, 0, 4);
            tableLayoutPanel1.Controls.Add(txtNotes, 1, 4);

            tableLayoutPanel1.Controls.Add(flpButtons, 0, 5);
            tableLayoutPanel1.SetColumnSpan(flpButtons, 2);

            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();

            // ServiceForm
            AcceptButton = btnAddService;
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = false;
            MinimumSize = new Size(620, 460);

            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainMenuControl1);

            Name = "ServiceForm";
            Text = "Service";

            ((System.ComponentModel.ISupportInitialize)numServiceCost).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Label label1;
        private Label label2;
        private Label label3;
        private NumericUpDown numServiceCost;
        private Label label5;
        private Label label6;
        private TextBox txtNotes;
        private Button btnAddService;
        private Button btnClear;
        private MainMenuControl mainMenuControl1;
        private TextBox txtBrand;
        private TextBox txtType;
        private TextBox txtColor;

        private TableLayoutPanel tableLayoutPanel1;
    }
}
