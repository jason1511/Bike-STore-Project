namespace Bike_STore_Project
{
    partial class SalesForm
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            numQuantity = new NumericUpDown();
            numPrice = new NumericUpDown();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            txtCustomer = new TextBox();
            btnAddSale = new Button();
            btnClear = new Button();
            mainMenuControl1 = new MainMenuControl();
            cmbBrand = new ComboBox();
            cmbType = new ComboBox();
            cmbColor = new ComboBox();
            tableLayoutPanel1 = new TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).BeginInit();
            SuspendLayout();

            // mainMenuControl1
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 29);
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

            // 7 rows: Brand, Type, Color, Qty, Price, Customer, Buttons
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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

            // label4 - Quantity
            label4.AutoSize = true;
            label4.Name = "label4";
            label4.Text = "Quantity";
            label4.Anchor = AnchorStyles.Left;
            label4.Margin = new Padding(3, 8, 10, 8);

            // label5 - Price
            label5.AutoSize = true;
            label5.Name = "label5";
            label5.Text = "Price";
            label5.Anchor = AnchorStyles.Left;
            label5.Margin = new Padding(3, 8, 10, 8);

            // label6 - Customer
            label6.AutoSize = true;
            label6.Name = "label6";
            label6.Text = "Customer";
            label6.Anchor = AnchorStyles.Left;
            label6.Margin = new Padding(3, 8, 10, 8);

            // cmbBrand
            cmbBrand.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBrand.FormattingEnabled = true;
            cmbBrand.Name = "cmbBrand";
            cmbBrand.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbBrand.Margin = new Padding(3, 6, 3, 6);
            cmbBrand.TabIndex = 0;

            // cmbType
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.FormattingEnabled = true;
            cmbType.Name = "cmbType";
            cmbType.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbType.Margin = new Padding(3, 6, 3, 6);
            cmbType.TabIndex = 1;

            // cmbColor
            cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbColor.FormattingEnabled = true;
            cmbColor.Name = "cmbColor";
            cmbColor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            cmbColor.Margin = new Padding(3, 6, 3, 6);
            cmbColor.TabIndex = 2;

            // numQuantity
            numQuantity.Name = "numQuantity";
            numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantity.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numQuantity.Margin = new Padding(3, 6, 3, 6);
            numQuantity.TabIndex = 3;

            // numPrice
            numPrice.Name = "numPrice";
            numPrice.DecimalPlaces = 2;
            numPrice.Maximum = new decimal(new int[] { 1410065408, 2, 0, 0 });
            numPrice.ThousandsSeparator = true;
            numPrice.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numPrice.Margin = new Padding(3, 6, 3, 6);
            numPrice.TabIndex = 4;

            // txtCustomer
            txtCustomer.CharacterCasing = CharacterCasing.Upper;
            txtCustomer.Name = "txtCustomer";
            txtCustomer.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCustomer.Margin = new Padding(3, 6, 3, 6);
            txtCustomer.TabIndex = 5;

            // btnAddSale
            btnAddSale.Name = "btnAddSale";
            btnAddSale.Text = "ADD SALE";
            btnAddSale.AutoSize = true;
            btnAddSale.Margin = new Padding(6);
            btnAddSale.TabIndex = 6;
            btnAddSale.UseVisualStyleBackColor = true;

            // btnClear
            btnClear.Name = "btnClear";
            btnClear.Text = "CLEAR";
            btnClear.AutoSize = true;
            btnClear.Margin = new Padding(6);
            btnClear.TabIndex = 7;
            btnClear.UseVisualStyleBackColor = true;

            // buttons row panel (inside root table)
            var flpButtons = new FlowLayoutPanel();
            flpButtons.Name = "flpButtons";
            flpButtons.Dock = DockStyle.Fill;
            flpButtons.AutoSize = true;
            flpButtons.WrapContents = false;
            flpButtons.FlowDirection = FlowDirection.RightToLeft;
            flpButtons.Controls.Add(btnClear);
            flpButtons.Controls.Add(btnAddSale);

            // add controls to table (col, row)
            tableLayoutPanel1.SuspendLayout();

            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(cmbBrand, 1, 0);

            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(cmbType, 1, 1);

            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(cmbColor, 1, 2);

            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(numQuantity, 1, 3);

            tableLayoutPanel1.Controls.Add(label5, 0, 4);
            tableLayoutPanel1.Controls.Add(numPrice, 1, 4);

            tableLayoutPanel1.Controls.Add(label6, 0, 5);
            tableLayoutPanel1.Controls.Add(txtCustomer, 1, 5);

            tableLayoutPanel1.Controls.Add(flpButtons, 0, 6);
            tableLayoutPanel1.SetColumnSpan(flpButtons, 2);

            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();

            // SalesForm
            AcceptButton = btnAddSale;
            AutoScaleMode = AutoScaleMode.Dpi;  // better fullscreen scaling
            AutoSize = false;                   // IMPORTANT: don't autosize the form itself
            MinimumSize = new Size(560, 420);

            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainMenuControl1);

            Name = "SalesForm";
            Text = "Sales";

            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private NumericUpDown numQuantity;
        private NumericUpDown numPrice;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox txtCustomer;
        private Button btnAddSale;
        private Button btnClear;
        private MainMenuControl mainMenuControl1;
        private ComboBox cmbBrand;
        private ComboBox cmbType;
        private ComboBox cmbColor;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
