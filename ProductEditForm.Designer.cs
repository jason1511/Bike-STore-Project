namespace Bike_STore_Project
{
    partial class ProductEditForm
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
            txtBrand = new TextBox();
            Brand = new Label();
            txtType = new TextBox();
            Type = new Label();
            numQuantity = new NumericUpDown();
            lblQuantity = new Label();
            numPrice = new NumericUpDown();
            lblPrice = new Label();
            btnOk = new Button();
            btnCancel = new Button();
            txtColor = new TextBox();
            label2 = new Label();
            mainMenuControl1 = new MainMenuControl();
            tableLayoutPanel1 = new TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).BeginInit();
            SuspendLayout();

            // -------------------------
            // mainMenuControl1
            // -------------------------
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Margin = new Padding(4);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 34);
            mainMenuControl1.TabIndex = 0;

            // -------------------------
            // tableLayoutPanel1 (ROOT LAYOUT)
            // -------------------------
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Padding = new Padding(12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.TabIndex = 1;

            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // labels left
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));  // inputs left
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // labels right
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));  // inputs right

            tableLayoutPanel1.RowCount = 4; // Brand/Qty, Type/Price, Color, Buttons
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // -------------------------
            // Labels
            // -------------------------
            Brand.AutoSize = true;
            Brand.Name = "Brand";
            Brand.Text = "Brand";
            Brand.Anchor = AnchorStyles.Left;
            Brand.Margin = new Padding(3, 8, 8, 8);

            Type.AutoSize = true;
            Type.Name = "Type";
            Type.Text = "Type";
            Type.Anchor = AnchorStyles.Left;
            Type.Margin = new Padding(3, 8, 8, 8);

            label2.AutoSize = true;
            label2.Name = "label2";
            label2.Text = "Color";
            label2.Anchor = AnchorStyles.Left;
            label2.Margin = new Padding(3, 8, 8, 8);

            lblQuantity.AutoSize = true;
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Text = "Quantity";
            lblQuantity.Anchor = AnchorStyles.Left;
            lblQuantity.Margin = new Padding(12, 8, 8, 8);

            lblPrice.AutoSize = true;
            lblPrice.Name = "lblPrice";
            lblPrice.Text = "Price";
            lblPrice.Anchor = AnchorStyles.Left;
            lblPrice.Margin = new Padding(12, 8, 8, 8);

            // -------------------------
            // Inputs
            // -------------------------
            txtBrand.CharacterCasing = CharacterCasing.Upper;
            txtBrand.Name = "txtBrand";
            txtBrand.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtBrand.Margin = new Padding(3, 6, 3, 6);
            txtBrand.TabIndex = 0;

            txtType.CharacterCasing = CharacterCasing.Upper;
            txtType.Name = "txtType";
            txtType.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtType.Margin = new Padding(3, 6, 3, 6);
            txtType.TabIndex = 1;

            txtColor.CharacterCasing = CharacterCasing.Upper;
            txtColor.Name = "txtColor";
            txtColor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtColor.Margin = new Padding(3, 6, 3, 6);
            txtColor.TabIndex = 2;

            numQuantity.Name = "numQuantity";
            numQuantity.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            numQuantity.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numQuantity.Margin = new Padding(3, 6, 3, 6);
            numQuantity.TabIndex = 3;

            numPrice.Name = "numPrice";
            numPrice.DecimalPlaces = 2;
            numPrice.Maximum = new decimal(new int[] { 1410065408, 2, 0, 0 });
            numPrice.ThousandsSeparator = true;
            numPrice.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numPrice.Margin = new Padding(3, 6, 3, 6);
            numPrice.TabIndex = 4;

            // -------------------------
            // Buttons
            // -------------------------
            btnOk.Name = "btnOk";
            btnOk.Text = "OK";
            btnOk.AutoSize = true;
            btnOk.Margin = new Padding(6);
            btnOk.TabIndex = 5;
            btnOk.UseVisualStyleBackColor = true;

            btnCancel.Name = "btnCancel";
            btnCancel.Text = "Cancel";
            btnCancel.AutoSize = true;
            btnCancel.Margin = new Padding(6);
            btnCancel.TabIndex = 6;
            btnCancel.UseVisualStyleBackColor = true;

            // Put buttons in a right-aligned flow panel (still only ONE root layout panel)
            var flpButtons = new FlowLayoutPanel();
            flpButtons.Name = "flpButtons";
            flpButtons.Dock = DockStyle.Fill;
            flpButtons.AutoSize = true;
            flpButtons.WrapContents = false;
            flpButtons.FlowDirection = FlowDirection.RightToLeft;
            flpButtons.Controls.Add(btnCancel);
            flpButtons.Controls.Add(btnOk);

            // -------------------------
            // Add controls to tableLayoutPanel1 (col, row)
            // -------------------------
            tableLayoutPanel1.SuspendLayout();

            // Row 0: Brand + Quantity
            tableLayoutPanel1.Controls.Add(Brand, 0, 0);
            tableLayoutPanel1.Controls.Add(txtBrand, 1, 0);
            tableLayoutPanel1.Controls.Add(lblQuantity, 2, 0);
            tableLayoutPanel1.Controls.Add(numQuantity, 3, 0);

            // Row 1: Type + Price
            tableLayoutPanel1.Controls.Add(Type, 0, 1);
            tableLayoutPanel1.Controls.Add(txtType, 1, 1);
            tableLayoutPanel1.Controls.Add(lblPrice, 2, 1);
            tableLayoutPanel1.Controls.Add(numPrice, 3, 1);

            // Row 2: Color (span across the rest)
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(txtColor, 1, 2);
            tableLayoutPanel1.SetColumnSpan(txtColor, 3);

            // Row 3: Buttons (span all)
            tableLayoutPanel1.Controls.Add(flpButtons, 0, 3);
            tableLayoutPanel1.SetColumnSpan(flpButtons, 4);

            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();

            // -------------------------
            // ProductEditForm
            // -------------------------
            AutoScaleMode = AutoScaleMode.Dpi; // better fullscreen + Windows scaling
            AutoSize = false;                  // IMPORTANT: don't autosize the form itself
            MinimumSize = new Size(720, 320);

            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainMenuControl1);

            Name = "ProductEditForm";
            Text = "Edit Product";

            // Good WinForms dialog behavior
            AcceptButton = btnOk;
            CancelButton = btnCancel;

            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TextBox txtBrand;
        private Label Brand;
        private TextBox txtType;
        private Label Type;
        private NumericUpDown numQuantity;
        private Label lblQuantity;
        private NumericUpDown numPrice;
        private Label lblPrice;
        private Button btnOk;
        private Button btnCancel;
        private TextBox txtColor;
        private Label label2;
        private MainMenuControl mainMenuControl1;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
