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
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 38);
            label1.Name = "label1";
            label1.Size = new Size(48, 20);
            label1.TabIndex = 0;
            label1.Text = "Brand";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(26, 69);
            label2.Name = "label2";
            label2.Size = new Size(40, 20);
            label2.TabIndex = 3;
            label2.Text = "Type";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(21, 102);
            label3.Name = "label3";
            label3.Size = new Size(45, 20);
            label3.TabIndex = 5;
            label3.Text = "Color";
            // 
            // numQuantity
            // 
            numQuantity.Location = new Point(75, 166);
            numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantity.Name = "numQuantity";
            numQuantity.Size = new Size(150, 27);
            numQuantity.TabIndex = 6;
            numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numPrice
            // 
            numPrice.DecimalPlaces = 2;
            numPrice.Location = new Point(75, 212);
            numPrice.Maximum = new decimal(new int[] { 1410065408, 2, 0, 0 });
            numPrice.Name = "numPrice";
            numPrice.Size = new Size(150, 27);
            numPrice.TabIndex = 7;
            numPrice.ThousandsSeparator = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(4, 168);
            label4.Name = "label4";
            label4.Size = new Size(65, 20);
            label4.TabIndex = 8;
            label4.Text = "Quantity";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 214);
            label5.Name = "label5";
            label5.Size = new Size(41, 20);
            label5.TabIndex = 9;
            label5.Text = "Price";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(-3, 256);
            label6.Name = "label6";
            label6.Size = new Size(72, 20);
            label6.TabIndex = 10;
            label6.Text = "Customer";
            // 
            // txtCustomer
            // 
            txtCustomer.CharacterCasing = CharacterCasing.Upper;
            txtCustomer.Location = new Point(75, 253);
            txtCustomer.Name = "txtCustomer";
            txtCustomer.Size = new Size(304, 27);
            txtCustomer.TabIndex = 11;
            // 
            // btnAddSale
            // 
            btnAddSale.Location = new Point(64, 317);
            btnAddSale.Name = "btnAddSale";
            btnAddSale.Size = new Size(94, 29);
            btnAddSale.TabIndex = 12;
            btnAddSale.Text = "ADD SALE";
            btnAddSale.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(209, 317);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(94, 29);
            btnClear.TabIndex = 14;
            btnClear.Text = "CLEAR";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // mainMenuControl1
            // 
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 29);
            mainMenuControl1.TabIndex = 15;
            // 
            // cmbBrand
            // 
            cmbBrand.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBrand.FormattingEnabled = true;
            cmbBrand.Location = new Point(75, 35);
            cmbBrand.Name = "cmbBrand";
            cmbBrand.Size = new Size(151, 28);
            cmbBrand.TabIndex = 16;
            // 
            // cmbType
            // 
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.FormattingEnabled = true;
            cmbType.Location = new Point(75, 66);
            cmbType.Name = "cmbType";
            cmbType.Size = new Size(151, 28);
            cmbType.TabIndex = 17;
            // 
            // cmbColor
            // 
            cmbColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbColor.FormattingEnabled = true;
            cmbColor.Location = new Point(75, 99);
            cmbColor.Name = "cmbColor";
            cmbColor.Size = new Size(151, 28);
            cmbColor.TabIndex = 18;
            // 
            // SalesForm
            // 
            AcceptButton = btnAddSale;
            AutoScaleMode = AutoScaleMode.Inherit;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(800, 450);
            Controls.Add(cmbColor);
            Controls.Add(cmbType);
            Controls.Add(cmbBrand);
            Controls.Add(mainMenuControl1);
            Controls.Add(btnClear);
            Controls.Add(btnAddSale);
            Controls.Add(txtCustomer);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(numPrice);
            Controls.Add(numQuantity);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "SalesForm";
            Text = "SalesForm";
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
    }
}