namespace Bike_STore_Project
{
    partial class ProductEditForm
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
            txtBrand = new TextBox();
            Brand = new Label();
            txtType = new TextBox();
            Type = new Label();
            numQuantity = new NumericUpDown();
            label1 = new Label();
            numPrice = new NumericUpDown();
            Price = new Label();
            btnOk = new Button();
            btnCancel = new Button();
            txtColor = new TextBox();
            label2 = new Label();
            mainMenuControl1 = new MainMenuControl();
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).BeginInit();
            SuspendLayout();
            // 
            // txtBrand
            // 
            txtBrand.CharacterCasing = CharacterCasing.Upper;
            txtBrand.Location = new Point(77, 40);
            txtBrand.Name = "txtBrand";
            txtBrand.Size = new Size(125, 27);
            txtBrand.TabIndex = 0;
            // 
            // Brand
            // 
            Brand.AutoSize = true;
            Brand.Location = new Point(23, 47);
            Brand.Name = "Brand";
            Brand.Size = new Size(48, 20);
            Brand.TabIndex = 1;
            Brand.Text = "Brand";
            // 
            // txtType
            // 
            txtType.CharacterCasing = CharacterCasing.Upper;
            txtType.Location = new Point(77, 86);
            txtType.Name = "txtType";
            txtType.Size = new Size(125, 27);
            txtType.TabIndex = 2;
            // 
            // Type
            // 
            Type.AutoSize = true;
            Type.Location = new Point(31, 86);
            Type.Name = "Type";
            Type.Size = new Size(40, 20);
            Type.TabIndex = 3;
            Type.Text = "Type";
            // 
            // numQuantity
            // 
            numQuantity.Location = new Point(519, 41);
            numQuantity.Name = "numQuantity";
            numQuantity.Size = new Size(150, 27);
            numQuantity.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(448, 40);
            label1.Name = "label1";
            label1.Size = new Size(65, 20);
            label1.TabIndex = 5;
            label1.Text = "Quantity";
            // 
            // numPrice
            // 
            numPrice.DecimalPlaces = 2;
            numPrice.Location = new Point(519, 87);
            numPrice.Maximum = new decimal(new int[] { 1410065408, 2, 0, 0 });
            numPrice.Name = "numPrice";
            numPrice.Size = new Size(150, 27);
            numPrice.TabIndex = 6;
            numPrice.ThousandsSeparator = true;
            // 
            // Price
            // 
            Price.AutoSize = true;
            Price.Location = new Point(472, 89);
            Price.Name = "Price";
            Price.Size = new Size(41, 20);
            Price.TabIndex = 7;
            Price.Text = "Price";
            // 
            // btnOk
            // 
            btnOk.Location = new Point(135, 160);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(94, 29);
            btnOk.TabIndex = 8;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(448, 160);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(94, 29);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtColor
            // 
            txtColor.CharacterCasing = CharacterCasing.Upper;
            txtColor.Location = new Point(77, 127);
            txtColor.Name = "txtColor";
            txtColor.Size = new Size(125, 27);
            txtColor.TabIndex = 10;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(26, 126);
            label2.Name = "label2";
            label2.Size = new Size(45, 20);
            label2.TabIndex = 11;
            label2.Text = "Color";
            // 
            // mainMenuControl1
            // 
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 34);
            mainMenuControl1.TabIndex = 12;
            // 
            // ProductEditForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(mainMenuControl1);
            Controls.Add(label2);
            Controls.Add(txtColor);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(Price);
            Controls.Add(numPrice);
            Controls.Add(label1);
            Controls.Add(numQuantity);
            Controls.Add(Type);
            Controls.Add(txtType);
            Controls.Add(Brand);
            Controls.Add(txtBrand);
            Name = "ProductEditForm";
            Text = "ProductEditForm";
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPrice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtBrand;
        private Label Brand;
        private TextBox txtType;
        private Label Type;
        private NumericUpDown numQuantity;
        private Label label1;
        private NumericUpDown numPrice;
        private Label Price;
        private Button btnOk;
        private Button btnCancel;
        private TextBox txtColor;
        private Label label2;
        private MainMenuControl mainMenuControl1;
    }
}